using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Input;
using WpfApp11.Model;

namespace WPFSQLL.ViewModel
{
    public class EditViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private UserModel _currentUser = new UserModel();
        private ObservableCollection<UserModel> _users;
        private UserModel _selectedUser;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ObservableCollection<UserModel> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        public string SelectedNickname
        {
            get => _currentUser.Username;
            set
            {
                if (_currentUser.Username != value)
                {
                    _currentUser.Username = value;
                    OnPropertyChanged(nameof(SelectedNickname));
                }
            }
        }

        public string SelectedPassword
        {
            get => _currentUser.Password;
            set
            {
                if (_currentUser.Password != value)
                {
                    _currentUser.Password = value;
                    OnPropertyChanged(nameof(SelectedPassword));
                }
            }
        }
        public UserModel SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));

                // Установка ID выбранного пользователя для обновления
                if (_selectedUser != null)
                {
                    _currentUser.UserId = _selectedUser.UserId;
                }
            }
        }

        public List<UserModel> GetUsers()
        {
            string con = "Data Source=HOME-PC\\MSSQLSERVER01;Initial Catalog=User;Integrated Security=True;";
            List<UserModel> users = new List<UserModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(con))
                {
                    connection.Open();

                    string query = "SELECT UserId, Username, Password FROM [dbo].[User1]";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserModel user = new UserModel
                                {
                                    UserId = Convert.ToInt32(reader["UserId"]),
                                    Username = reader["Username"].ToString(),
                                    Password = reader["Password"].ToString()
                                };

                                users.Add(user);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок, например, логирование или отображение сообщения
                Console.WriteLine($"Ошибка при загрузке данных из БД: {ex.Message}");
            }

            return users;
        }

        public void UpdateUser(int id, string nickname, string password)
        {
            string con = "Data Source=HOME-PC\\MSSQLSERVER01;Initial Catalog=User;Integrated Security=True;";
            using (SqlConnection connection = new SqlConnection(con))
            {
                connection.Open();

                string query = "UPDATE User1 SET Username = @Username, Password = @Password WHERE UserId = @UserId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", id);
                    command.Parameters.AddWithValue("@Username", nickname);
                    command.Parameters.AddWithValue("@Password", password);

                    command.ExecuteNonQuery();
                }
            }
        }

        public ICommand EditCommand { get; private set; }

        public EditViewModel()
        {
            
            EditCommand = new RelayCommand(EditUser);
            Users = new ObservableCollection<UserModel>(GetUsers());
        }

        public void EditUser(object parameter)
        {
            if (!string.IsNullOrEmpty(_currentUser.Username) && !string.IsNullOrEmpty(_currentUser.Password))
            {
                UpdateUser(_currentUser.UserId, _currentUser.Username, _currentUser.Password);
                MessageBox.Show("Данные пользователя обновлены!");

                // Обновление списка пользователей после изменения
                Users = new ObservableCollection<UserModel>(GetUsers());
            }
            else
            {
                MessageBox.Show("Не все данные заполнены!");
            }
        }
    }
}

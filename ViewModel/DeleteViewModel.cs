using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using WpfApp11.Model;

namespace WpfApp11.ViewModel
{
    public class DeleteViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<UserModel> _users;
        private UserModel _selectedUser;
        private int _inputUserId;
        private string _newUsername;
        private string _newPassword;


        public ObservableCollection<UserModel> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        public UserModel SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;
                    OnPropertyChanged(nameof(SelectedUser));
                }
            }
        }

        public int InputUserId
        {
            get { return _inputUserId; }
            set
            {
                _inputUserId = value;
                OnPropertyChanged(nameof(InputUserId));
            }
        }

        public string NewUsername
        {
            get { return _newUsername; }
            set
            {
                _newUsername = value;
                OnPropertyChanged(nameof(NewUsername));
            }
        }

        public string NewPassword
        {
            get { return _newPassword; }
            set
            {
                _newPassword = value;
                OnPropertyChanged(nameof(NewPassword));
            }
        }

        public ICommand DeleteCommand { get; private set; }


        public DeleteViewModel()
        {
            Users = new ObservableCollection<UserModel>();
            DeleteCommand = new RelayCommand(Delete);
            LoadUsersFromDatabase();
            SelectedUser = new UserModel(); // Проверьте, есть ли у вас конструктор по умолчанию
        }

        
        private void LoadUsersFromDatabase()
        {
            try
            {
                string connectionString = "Data Source=HOME-PC\\MSSQLSERVER01;Initial Catalog=User;Integrated Security=True;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT UserId, Username, Password FROM User1";
                    SqlCommand command = new SqlCommand(query, connection);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Users.Add(new UserModel
                            {
                                UserId = Convert.ToInt32(reader["UserId"]),
                                Username = reader["Username"].ToString(),
                                Password = reader["Password"].ToString(),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных из БД: {ex.Message}");
            }
        }

        private void Delete(object parameter)
        {
            try
            {
                string connectionString = "Data Source=HOME-PC\\MSSQLSERVER01;Initial Catalog=User;Integrated Security=True;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string deleteQuery = "DELETE FROM User1 WHERE UserId = @UserId";
                    SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);
                    deleteCommand.Parameters.AddWithValue("@UserId", SelectedUser.UserId);

                    int rowsAffected = deleteCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Пользователь успешно удален!");
                        Users.Remove(SelectedUser); // Удаление из коллекции после удаления из БД
                        SelectedUser = null; // Сброс выбранного пользователя после удаления
                    }
                    else
                    {
                        MessageBox.Show("Не удалось удалить пользователя.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении пользователя: {ex.Message}");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


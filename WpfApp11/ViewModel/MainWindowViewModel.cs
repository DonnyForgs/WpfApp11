using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using WpfApp11.Model;
using WpfApp11.View;
using System.Data;
using WPFSQLL.View;


namespace WpfApp11.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private UserModel _user;
        private UserModel _selectedUser;
        public ObservableCollection<UserModel> Users { get; set; }
        public ICommand SignUpCommand {  get; set; }
        public ICommand LoginCommand {  get; set; }
        public ICommand SigmaCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand SigmaRegisterCommand { get; set; }
        public ICommand ShowUsersCommand { get; set; }
        public ICommand LightThemeCommand { get; set; }
        public ICommand DarkThemeCommand { get; set; }
        public ICommand SettingsCommand { get; set; }

        public ICommand DeleteCommand { get; set; }
        public UserModel User // Доступ к объекту UserModel
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged(nameof(User));
            }
        }

        public UserModel SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
            }
        }
        public MainWindowViewModel()
        {
            User = new UserModel();
            EditCommand = new RelayCommand(_Edit);
            SignUpCommand = new RelayCommand(SignUp, CanSignUp);
            LoginCommand = new RelayCommand(Login, CanLogin);
            SigmaCommand = new RelayCommand(SigmaUp);
            SigmaRegisterCommand = new RelayCommand(SigmaRegister);
            ShowUsersCommand = new RelayCommand(param => LoadUsers(), param => true);
            LightThemeCommand = new RelayCommand(LoadLightTheme);
            DarkThemeCommand = new RelayCommand(LoadDarkTheme);
            SettingsCommand = new RelayCommand(Settings);
            DeleteCommand = new RelayCommand(Delete);
            LoadUsers();
        }

        private void _Edit(object parameter)
        {
            Edit editwin = new Edit();
            editwin.Show();
        }

        private void Delete(object parameter)
        {
            Delete delete = new Delete();
            delete.Show();
        }
        private bool CanSignUp(object parameter)
        {
            // Верните true или false в зависимости от того, может ли пользователь зарегистрироваться
            return true;
        }

        private void SignUp(object parameter)
        {
            // Открываем окно регистрации
            Register registerWindow = new Register();
            registerWindow.Show();
        }

        private bool CanLogin(object parameter)
        {
            // Верните true или false в зависимости от того, может ли пользователь войти
            return true;
        }

        private void Settings(object parameter)
        {
            Settings settings = new Settings();
            settings.Show();
        }
        private void LoadUsers()
        {
            try
            {
                Users = new ObservableCollection<UserModel>();

                string connectionString = "Data Source=HOME-PC\\MSSQLSERVER01;Initial Catalog=User;Integrated Security=True;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT UserId, Username, Password FROM User1";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Users.Add(new UserModel
                        {
                            UserId = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            Password = reader.GetString(2)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}");
            }
        }
        public void LoadLightTheme(object parameter)
        {
            // Установка светлой темы
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("Themes/Light.xaml", UriKind.Relative) });
        }

        public void LoadDarkTheme(object parameter)
        {
            // Установка темной темы
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/Themes/Dark.xaml", UriKind.Relative) });
        }

        private void Login(object parameter)
        {
            // Открываем главное окно
            Login mainWindow = new Login();
            mainWindow.Show();

            // Закрываем текущее окно
            Application.Current.MainWindow.Close();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SigmaUp(object parameter)
        {
            try
            {
                string connectionString = "Data Source=HOME-PC\\MSSQLSERVER01;Initial Catalog=User;Integrated Security=True;";
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string loginQuary = "SELECT COUNT(*) FROM User1 WHERE Username = @Username AND Password = @Password";
                        SqlCommand loginCommand = new SqlCommand(loginQuary, connection);
                        loginCommand.Parameters.AddWithValue("@Username", User.Username);
                        loginCommand.Parameters.AddWithValue("@Password", User.Password);

                        int userCount = (int)loginCommand.ExecuteScalar();

                        if (userCount > 0)
                        {
                            MessageBox.Show("Вход успешно выполнен!");
                            Data DataWindow = new Data();
                            DataWindow.Show();
                            Application.Current.Windows[0].Close();
                        }
                        else
                        {
                            MessageBox.Show("Ошибка входа. Проверьте логин и пароль.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении входа: {ex.Message}");
            }

        }
        private void SigmaRegister(object parameter) 
        {
            string connectionString = "Data Source=HOME-PC\\MSSQLSERVER01;Initial Catalog=User;Integrated Security=True;";
            string username = User.Username;
            string password = User.Password;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string checkUserQuery = $"SELECT COUNT(*) FROM User1 WHERE Username = @Username";
                    SqlCommand checkUserCommand = new SqlCommand(checkUserQuery, connection);
                    checkUserCommand.Parameters.AddWithValue("@Username", User.Username);
                    int existingUserCount = (int)checkUserCommand.ExecuteScalar();

                    if (existingUserCount > 0)
                    {
                        MessageBox.Show("Пользователь с таким именем уже существует!", "Ошибка", MessageBoxButton.OK);
                        return;
                    }

                    string insertQuery = "INSERT INTO User1 (Username, Password) VALUES (@Username, @Password)";
                    SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                    insertCommand.Parameters.AddWithValue("@Username", User.Username);
                    insertCommand.Parameters.AddWithValue("@Password", User.Password);
                    insertCommand.ExecuteNonQuery();

                    MessageBox.Show("Готово! Пользователь зарегистрирован.");
                    Login LoginWindow = new Login();
                    LoginWindow.Show();
                    Application.Current.Windows[0].Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK);
            }


        }
    }
}

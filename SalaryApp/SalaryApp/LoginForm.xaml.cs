using MySql.Data.MySqlClient;
using System;
using System.Data.Common;
using System.Windows;

namespace SalaryApp
{
    /// <summary>
    /// Логика взаимодействия для LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {

        public int id = 0;
        public string login;
        public string fullName;
        public string grade;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void AuthBtn_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                FindUser(conn);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }

        private void FindUser(MySqlConnection conn)
        {

            string loginUser = LoginField.Text;
            string passUser = PassField.Password;

            MySqlCommand command = new MySqlCommand("SELECT * FROM `users` WHERE Login = '" + loginUser + "' AND Password = '" + passUser + "'", conn);

            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    id = reader.GetInt32(0);
                    login = reader.GetString(1);
                    fullName = reader.GetString(3);

                    bool IsExecutor = reader.GetString(4) == "Исполнитель" ? true : false;

                    if (IsExecutor)
                    {
                        grade = reader.GetString(5);

                        Executor exForm = new Executor
                        {
                            id = id,
                            login = login,
                            grade = grade
                        };
                        exForm.Show();
                        Close();
                    }
                    else
                    {

                        Manager manForm = new Manager
                        {
                            id = id,
                            login = login,
                            fullName = fullName
                        };
                        manForm.Show();
                        Close();
                    }
                }
                else
                {
                    ErrorLabel.Visibility = Visibility.Visible;
                    ErrorLabel.Content = "Введён неправильный логин или пароль!";
                }
            }
        }
    }
}

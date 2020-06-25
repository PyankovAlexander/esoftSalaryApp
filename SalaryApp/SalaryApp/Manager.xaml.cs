using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;


namespace SalaryApp
{
    /// <summary>
    /// Логика взаимодействия для Manager.xaml
    /// </summary>
    public partial class Manager : Window
    {

        public int id = 0;
        public string login;
        public string fullName;
        DataTable dt;
        Dictionary<int, string> executors;

        public Manager()
        {
            InitializeComponent();
            Loaded += Manager_Loaded;
        }

        private void Manager_Loaded(object sender, RoutedEventArgs e)
        {
            LoginLabel.Content = "Ваш логин: " + login;
            FullNameLabel.Content = "ФИО Менеджера: " + fullName;

            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                dbHandler db = new dbHandler();
                executors = db.GetExecutors(conn, id);
                GetTasks(conn);

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

        private void GetTasks(MySqlConnection conn)
        {
            if (executors.Count != 0)
            {

                MySqlDataAdapter adapter = null;
                dt = new DataTable("tasks");

                foreach (KeyValuePair<int, string> keyValue in executors)
                {
                    MySqlCommand command = new MySqlCommand("SELECT * FROM `tasks` WHERE Performer = '" + keyValue.Key + "' AND Deleted = 0", conn);
                    command.ExecuteNonQuery();
                    adapter = new MySqlDataAdapter(command);
                    adapter.Fill(dt);
                }

                TasksDG.ItemsSource = dt.DefaultView;
                adapter.Update(dt);
            }
        }

        private void CoeffBtn_Click(object sender, RoutedEventArgs e)
        {
            Coefficients coefForm = new Coefficients
            {
                id = id,
                login = login,
                fullName = fullName
            };
            coefForm.Show();
            Close();
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {

            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                var taskID = dt.Rows[TasksDG.SelectedIndex][0];
                dt.Rows.RemoveAt(TasksDG.SelectedIndex);
                MySqlCommand command = new MySqlCommand("UPDATE `tasks` SET Deleted = 1 WHERE id = '" + taskID + "'", conn);
                command.ExecuteNonQuery();

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

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            AddTask addForm = new AddTask
            {
                id = id,
                login = login,
                fullName = fullName
            };
            addForm.Show();
            Close();
        }

        private void CalculateBtn_Click(object sender, RoutedEventArgs e)
        {
            Payroll payForm = new Payroll
            {
                id = id,
                login = login,
                fullName = fullName
            };
            payForm.Show();
            Close();
        }

        private void UserBtn_Click(object sender, RoutedEventArgs e)
        {
            LoginForm lf = new LoginForm();
            lf.Show();
            Close();
        }
    }
}

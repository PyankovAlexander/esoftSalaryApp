using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;

namespace SalaryApp
{
    /// <summary>
    /// Логика взаимодействия для Executor.xaml
    /// </summary>
    public partial class Executor : Window
    {
        public int id = 0;
        public string login;
        public string grade;
        DataTable dt;

        public Executor()
        {
            InitializeComponent();
            Loaded += Executor_Loaded;
        }

        private void Executor_Loaded(object sender, RoutedEventArgs e)
        {

            LoginLabel.Content = "Ваш логин: " + login;
            GradeLabel.Content = "Должность: " + grade;

            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
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

            MySqlCommand command = new MySqlCommand("SELECT * FROM `tasks` WHERE Performer = '" + id + "'", conn);
            command.ExecuteNonQuery();

            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            dt = new DataTable("tasks");
            adapter.Fill(dt);
            TasksDG.ItemsSource = dt.DefaultView;
            adapter.Update(dt);

        }

        private void StatusBtn_Click(object sender, RoutedEventArgs e)
        {
            var taskID = dt.Rows[TasksDG.SelectedIndex][0];
            if (dt.Rows[TasksDG.SelectedIndex][4].Equals("Запланирована"))
            {
                dt.Rows[TasksDG.SelectedIndex][4] = "Выполняется";
                dt.Rows[TasksDG.SelectedIndex][6] = DateTime.Now;
            }
            else if (dt.Rows[TasksDG.SelectedIndex][4].Equals("Выполняется"))
            {
                MessageBoxResult result = MessageBox.Show("Задача выполнена?", "My App", MessageBoxButton.YesNoCancel);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        dt.Rows[TasksDG.SelectedIndex][4] = "Завершена";
                        dt.Rows[TasksDG.SelectedIndex][7] = DateTime.Now;
                        break;
                    case MessageBoxResult.No:
                        dt.Rows[TasksDG.SelectedIndex][4] = "Отменена";
                        break;
                }
            }

            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {

                MySqlCommand command = new MySqlCommand("UPDATE `tasks` SET Status = @Status, StartTime = @ST, EndTime = @ET WHERE id = '" + taskID + "'", conn);
                command.Parameters.Add("@Status", MySqlDbType.Enum).Value = dt.Rows[TasksDG.SelectedIndex][4];
                command.Parameters.Add("@ST", MySqlDbType.DateTime).Value = dt.Rows[TasksDG.SelectedIndex][6];
                command.Parameters.Add("@ET", MySqlDbType.DateTime).Value = dt.Rows[TasksDG.SelectedIndex][7];
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

        private void UserBtn_Click(object sender, RoutedEventArgs e)
        {
            LoginForm lf = new LoginForm();
            lf.Show();
            Close();
        }
    }
}

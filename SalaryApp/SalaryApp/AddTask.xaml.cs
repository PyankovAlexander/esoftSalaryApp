using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SalaryApp
{
    /// <summary>
    /// Логика взаимодействия для AddTask.xaml
    /// </summary>
    public partial class AddTask : Window
    {
        public int id = 0;
        public string login;
        public string fullName;

        List<string> typeList;
        Dictionary<int, string> executors;
        int idPerf;

        public AddTask()
        {
            InitializeComponent();
            Loaded += AddTask_Loaded;
        }

        private void AddTask_Loaded(object sender, RoutedEventArgs e)
        {

            typeList = new List<string>() { "Анализ и проектирование", "Установка оборудования", "Техническое обслуживание и сопровождение" };
            TypeCB.ItemsSource = typeList;
            StatusTB.Text = "Запланирована";

            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                List<string> perfList = new List<string>();
                dbHandler db = new dbHandler();
                executors = db.GetExecutors(conn, id);
                foreach (KeyValuePair<int, string> keyValue in executors)
                {
                    perfList.Add(keyValue.Value);
                }
                PerfCB.ItemsSource = perfList;
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
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                MySqlCommand command = new MySqlCommand("INSERT tasks (Performer, Name, Description, Complexity, TypeWork, NeedTime, StartTime) VALUES(@perf, @name, @desk, @comp, @type, @ntime, @stime)", conn);
                foreach (KeyValuePair<int, string> keyValue in executors)
                {
                    if (keyValue.Value.Equals(PerfCB.SelectedItem))
                    {
                        idPerf = keyValue.Key;
                        break;
                    }
                }

                command.Parameters.Add("@perf", MySqlDbType.Int32).Value = idPerf;
                command.Parameters.Add("@name", MySqlDbType.String).Value = NameField.Text;
                command.Parameters.Add("@desk", MySqlDbType.String).Value = DescTB.Text;
                command.Parameters.Add("@comp", MySqlDbType.Int32).Value = Convert.ToInt32(ComplexityField.Text);
                command.Parameters.Add("@type", MySqlDbType.String).Value = TypeCB.SelectedItem;
                var year = NeedData.SelectedDate.Value.Year;
                var month = NeedData.SelectedDate.Value.Month;
                var day = NeedData.SelectedDate.Value.Day;
                var time = new string[3];
                time = NeedTime.Text.Split(':');
                DateTime dt = new DateTime(year, month, day, Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), Convert.ToInt32(time[2])); //чекнуть
                command.Parameters.Add("@ntime", MySqlDbType.DateTime).Value = dt;
                command.Parameters.Add("@stime", MySqlDbType.DateTime).Value = DateTime.Now;
                command.ExecuteNonQuery();

                MessageBox.Show("Новое задание успешно добавлено!");
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

        private void BackBtn_Click(object sender, RoutedEventArgs e)
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
}

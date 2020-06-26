using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Windows;
using System.Windows.Data;

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
        ObservableCollection<TaskTable> tasksList;
        ICollectionView Itemlist;

        public Executor()
        {
            InitializeComponent();
            Loaded += Executor_Loaded;
        }

        private void Executor_Loaded(object sender, RoutedEventArgs e)
        {

            LoginLabel.Content = "Ваш логин: " + login;
            GradeLabel.Content = "Должность: " + grade;

            var statusList = new List<string>() { "Любой статус", "Запланирована", "Выполняется", "Завершена", "Отменена" };
            StatusCB.ItemsSource = statusList;

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

        class TaskTable
        {

            public TaskTable(string Name, string Status, string Manager)
            {
                this.Name = Name;
                this.Status = Status;
                this.Manager = Manager;
            }

            public string Name { get; set; }
            public string Status { get; set; }
            public string Manager { get; set; }
        }

        private void GetTasks(MySqlConnection conn)
        {

            MySqlCommand command = new MySqlCommand("SELECT Performer, Name, Status FROM `tasks` WHERE Performer = '" + id + "'", conn);
            command.ExecuteNonQuery();

            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            tasksList = new ObservableCollection<TaskTable>();
            dt = new DataTable("tasks");
            dbHandler db = new dbHandler();
            adapter.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                var name = Convert.ToString(row[1]);
                var status = Convert.ToString(row[2]);
                var managerId = GetManager(Convert.ToInt32(row[0]));
                var manager = db.GetUser(managerId);

                tasksList.Add(new TaskTable(name, status, manager));
            }


            TasksDG.ItemsSource = tasksList;
            adapter.Update(dt);

        }

        private int GetManager(int id)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                MySqlCommand command = new MySqlCommand("SELECT Manager FROM `relationship` WHERE Performer = '" + id + "'", conn);
                command.ExecuteNonQuery();
                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetInt32(0);
                    }
                }
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
            return 0;
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

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StatusCB_DropDownClosed(object sender, EventArgs e)
        {
            if (StatusCB.Text.Equals("Любой статус"))
            {
                TasksDG.ItemsSource = tasksList;
            }
            else
            {
                var _itemSourceList = new CollectionViewSource() { Source = tasksList };
                Itemlist = _itemSourceList.View;

                GroupFilter gf = new GroupFilter();

                var statusFilter = new Predicate<object>(item => ((TaskTable)item).Status.Equals(StatusCB.Text));
                gf.AddFilter(statusFilter);

                Itemlist.Filter = gf.Filter;
                TasksDG.ItemsSource = Itemlist;
            }
        }
    }
}

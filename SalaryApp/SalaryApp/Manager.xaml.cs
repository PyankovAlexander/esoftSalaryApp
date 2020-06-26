using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Data;

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
        ObservableCollection<TaskTable> tasksList;
        ICollectionView Itemlist;

        public Manager()
        {
            InitializeComponent();
            Loaded += Manager_Loaded;
        }

        private void Manager_Loaded(object sender, RoutedEventArgs e)
        {
            LoginLabel.Content = "Ваш логин: " + login;
            FullNameLabel.Content = "ФИО Менеджера: " + fullName;

            var statusList = new List<string>() { "Любой статус", "Запланирована", "Выполняется", "Завершена", "Отменена" };
            StatusCB.ItemsSource = statusList;


            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                dbHandler db = new dbHandler();
                executors = db.GetExecutors(conn, id);
                ExecutorsCB.Items.Add("Все исполнители");
                foreach (KeyValuePair<int, string> keyValue in executors)
                {
                    ExecutorsCB.Items.Add(keyValue.Value);
                }
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

            public TaskTable(string Name, string Status, string Executor)
            {
                this.Name = Name;
                this.Status = Status;
                this.Executor = Executor;
            }

            public string Name { get; set; }
            public string Status { get; set; }
            public string Executor { get; set; }
        }

        private void GetTasks(MySqlConnection conn)
        {
            if (executors.Count != 0)
            {

                MySqlDataAdapter adapter = null;

                tasksList = new ObservableCollection<TaskTable>();

                dt = new DataTable("tasks");
                dbHandler db = new dbHandler();

                foreach (KeyValuePair<int, string> keyValue in executors)
                {
                    MySqlCommand command = new MySqlCommand("SELECT id, Performer, Name, Status FROM `tasks` WHERE Performer = '" + keyValue.Key + "' AND Deleted = 0", conn);
                    command.ExecuteNonQuery();
                    adapter = new MySqlDataAdapter(command);
                    adapter.Fill(dt);
                }

                foreach (DataRow row in dt.Rows)
                {
                    var name = Convert.ToString(row[2]);
                    var status = Convert.ToString(row[3]);
                    var executor = db.GetUser(Convert.ToInt32(row[1]));

                    tasksList.Add(new TaskTable(name, status, executor));
                }

                TasksDG.ItemsSource = tasksList;
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

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            /*var selectedIndex = TasksDG.SelectedIndex;
            var taskID = dt.Rows[selectedIndex][0];
            AddTask addForm = new AddTask
            {
                id = id,
                login = login,
                fullName = fullName,
                taskID = Convert.ToInt32(taskID)
            };
            addForm.Show();
            Close(); */
        }

        private void ExecutorsCB_DropDownClosed(object sender, EventArgs e)
        {
            if (ExecutorsCB.Text.Equals("Все исполнители") && StatusCB.Text.Equals("Любой статус"))
            {
                TasksDG.ItemsSource = tasksList;
            }
            else
            {
                var _itemSourceList = new CollectionViewSource() { Source = tasksList };
                Itemlist = _itemSourceList.View;

                GroupFilter gf = new GroupFilter();

                if (!ExecutorsCB.Text.Equals("Все исполнители")) { 
                    var nameFilter = new Predicate<object>(item => ((TaskTable)item).Executor.Equals(ExecutorsCB.Text));
                    gf.AddFilter(nameFilter);
                }
                if (!StatusCB.Text.Equals("Любой статус"))
                {
                    var statusFilter = new Predicate<object>(item => ((TaskTable)item).Status.Equals(StatusCB.Text));
                    gf.AddFilter(statusFilter);
                }        

                Itemlist.Filter = gf.Filter;
                TasksDG.ItemsSource = Itemlist;
            }
        }

        private void StatusCB_DropDownClosed(object sender, EventArgs e)
        {
            if (ExecutorsCB.Text.Equals("Все исполнители") && StatusCB.Text.Equals("Любой статус"))
            {
                TasksDG.ItemsSource = tasksList;
            }
            else
            {
                var _itemSourceList = new CollectionViewSource() { Source = tasksList };
                Itemlist = _itemSourceList.View;

                GroupFilter gf = new GroupFilter();

                if (!ExecutorsCB.Text.Equals("Все исполнители"))
                {
                    var nameFilter = new Predicate<object>(item => ((TaskTable)item).Executor.Equals(ExecutorsCB.Text));
                    gf.AddFilter(nameFilter);
                }
                if (!StatusCB.Text.Equals("Любой статус"))
                {
                    var statusFilter = new Predicate<object>(item => ((TaskTable)item).Status.Equals(StatusCB.Text));
                    gf.AddFilter(statusFilter);
                }

                Itemlist.Filter = gf.Filter;
                TasksDG.ItemsSource = Itemlist;
            }
        }
    }
}

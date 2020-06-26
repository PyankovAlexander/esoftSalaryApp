using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace SalaryApp
{
    /// <summary>
    /// Логика взаимодействия для ExList.xaml
    /// </summary>
    public partial class ExList : Window
    {

        public int id = 0;
        public string login;
        public string grade;
        ObservableCollection<ExecutorTable> exList;

        public ExList()
        {
            InitializeComponent();
            Loaded += ExList_Loaded;
        }

        private void ExList_Loaded(object sender, RoutedEventArgs e)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                exList = new ObservableCollection<ExecutorTable>();
                GetExList(conn);
                ExListDG.ItemsSource = exList;
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

        private void GetExList(MySqlConnection conn)
        {
            DataTable dt = new DataTable("ex");
            dbHandler db = new dbHandler();

            MySqlCommand command = new MySqlCommand("SELECT id, FullName, Grade FROM `users` WHERE TypeUser = 'Исполнитель'", conn);
            command.ExecuteNonQuery();
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            adapter.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                var fullName = Convert.ToString(row[1]);
                var grade = Convert.ToString(row[2]);
                var managerId = db.GetManager(Convert.ToInt32(row[0]));
                var manager = db.GetUser(managerId);
                exList.Add(new ExecutorTable(fullName, grade, manager));
            }
        }

        class ExecutorTable
        {
            public ExecutorTable(string Executor, string Grade, string Manager)
            {
                this.Executor = Executor;
                this.Grade = Grade;
                this.Manager = Manager;
            }

            public string Executor { get; set; }
            public string Grade { get; set; }
            public string Manager { get; set; }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Executor exForm = new Executor
            {
                id = id,
                login = login,
                grade = grade
            };
            exForm.Show();
            Close();
        }
    }
}

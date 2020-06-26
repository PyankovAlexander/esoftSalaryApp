using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace SalaryApp
{
    /// <summary>
    /// Логика взаимодействия для Payroll.xaml
    /// </summary>
    public partial class Payroll : Window
    {
        public int id = 0;
        List<double> coeffList;
        public string login;
        public string fullName;
        Dictionary<int, string> executors;
        DataTable dt;
        ObservableCollection<SalaryTable> salaryList;


        public Payroll()
        {
            InitializeComponent();
        }

        private void SalaryBtn_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                dbHandler db = new dbHandler();
                coeffList = db.GetCoefficients(conn, id);
                executors = db.GetExecutors(conn, id);
                GetSalary(conn);

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

        class SalaryTable
        {
            public SalaryTable(string Executor, double Salary)
            {
                this.Executor = Executor;
                this.Salary = Salary;
            }
            public string Executor { get; set; }
            public double Salary { get; set; }
        }

        private void GetSalary(MySqlConnection conn)
        {
            if (executors.Count != 0)
            {

                MySqlDataAdapter adapter = null;

                salaryList = new ObservableCollection<SalaryTable>();

                foreach (KeyValuePair<int, string> keyValue in executors)
                {

                    dt = new DataTable("tasks");

                    MySqlCommand command = new MySqlCommand("SELECT Complexity, TypeWork, NeedTime, EndTime FROM `tasks` WHERE Performer = '" + keyValue.Key + "' AND Deleted = 0 AND Status = 'Завершена'", conn);
                    command.ExecuteNonQuery();

                    adapter = new MySqlDataAdapter(command);
                    adapter.Fill(dt);

                    double salary = 0;


                    double typeCoeff;

                    foreach (DataRow row in dt.Rows)
                    {
                        dbHandler db = new dbHandler();

                        var grade = 0.0;
                        var gradeString = db.GetGrade(conn, keyValue.Key);
                        if (gradeString.Equals("junior"))
                        {
                            grade = coeffList[0];
                        }
                        else if (gradeString.Equals("middle"))
                        {
                            grade = coeffList[1];
                        }
                        else
                            grade = coeffList[2];

                        var comp = Convert.ToDouble(row[0]);
                        var type = row[1].ToString();
                        if (type.Equals("Анализ и проектирование"))
                            typeCoeff = coeffList[3];
                        else if (type.Equals("Установка оборудования"))
                            typeCoeff = coeffList[4];
                        else
                            typeCoeff = coeffList[5];
                        var timeSpan = Convert.ToDateTime(row[2]) - Convert.ToDateTime(row[3]);
                        var time = (timeSpan.Days * 24 + timeSpan.Hours) * 60 + timeSpan.Minutes;
                        var DR = coeffList[6];
                        var TR = coeffList[7];
                        var CE = coeffList[8];

                        salary = (typeCoeff + comp * DR + time * TR) * CE + salary;
                        salary = grade + salary;
                        salaryList.Add(new SalaryTable(keyValue.Value, salary));

                    }

                    SalaryDG.ItemsSource = salaryList;

                }

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

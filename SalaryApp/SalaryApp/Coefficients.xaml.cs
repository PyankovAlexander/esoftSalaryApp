using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SalaryApp
{
    /// <summary>
    /// Логика взаимодействия для Coefficients.xaml
    /// </summary>
    public partial class Coefficients : Window
    {

        public int id = 0;
        List<double> coeffList;
        public string login;
        public string fullName;

        public Coefficients()
        {
            InitializeComponent();
            Loaded += Coefficients_Loaded;
        }

        private void Coefficients_Loaded(object sender, RoutedEventArgs e)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                dbHandler db = new dbHandler();
                coeffList = db.GetCoefficients(conn, id);
                Jun.Text = Convert.ToString(coeffList[0]);
                Mid.Text = Convert.ToString(coeffList[1]);
                Sen.Text = Convert.ToString(coeffList[2]);
                Ana.Text = Convert.ToString(coeffList[3]);
                Dep.Text = Convert.ToString(coeffList[4]);
                Sup.Text = Convert.ToString(coeffList[5]);
                DR.Text = Convert.ToString(coeffList[6]);
                TR.Text = Convert.ToString(coeffList[7]);
                CE.Text = Convert.ToString(coeffList[8]);
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

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                SetCoefficients(conn);
                MessageBox.Show("Данные успешно обновлены!");

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

        private void SetCoefficients(MySqlConnection conn)
        {
            MySqlCommand command = new MySqlCommand("UPDATE `coefficients` " +
                "SET Junior = @Jun, Middle = @Mid, Senior = @Sen, " +
                "Analysis = @Anal, Deployment = @Dep, Support = @Sup, " +
                "DifficultyRatio = @DR, TimeRatio = @TR, CashEquivalent = @CE WHERE Manager = '" + id + "'", conn);
            command.Parameters.Add("@Jun", MySqlDbType.Double).Value = Convert.ToDouble(Jun.Text);
            command.Parameters.Add("@Mid", MySqlDbType.Double).Value = Convert.ToDouble(Mid.Text);
            command.Parameters.Add("@Sen", MySqlDbType.Double).Value = Convert.ToDouble(Sen.Text);
            command.Parameters.Add("@Anal", MySqlDbType.Double).Value = Convert.ToDouble(Ana.Text);
            command.Parameters.Add("@Dep", MySqlDbType.Double).Value = Convert.ToDouble(Dep.Text);
            command.Parameters.Add("@Sup", MySqlDbType.Double).Value = Convert.ToDouble(Sup.Text);
            command.Parameters.Add("@DR", MySqlDbType.Double).Value = Convert.ToDouble(DR.Text);
            command.Parameters.Add("@TR", MySqlDbType.Double).Value = Convert.ToDouble(TR.Text);
            command.Parameters.Add("@CE", MySqlDbType.Double).Value = Convert.ToDouble(CE.Text);
            command.ExecuteNonQuery();
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

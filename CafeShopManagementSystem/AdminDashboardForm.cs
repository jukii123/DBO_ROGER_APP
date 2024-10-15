using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace CafeShopManagementSystem
{
    public partial class AdminDashboardForm : UserControl
    {
        static string conn = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;

        public AdminDashboardForm()
        {
            InitializeComponent();
            LoadDashboardData(); // Cargar datos después de inicializar
        }

        private void LoadDashboardData()
        {
            displayTotalCashier();
            displayTotalCustomers();
            displayTotalIncome();
            displayTodaysIncome();
        }

        public void displayTotalCashier()
        {
            using (SqlConnection connect = new SqlConnection(conn))
            {
                if (connect.State == ConnectionState.Closed)
                {
                    try
                    {
                        connect.Open();

                        string selectData = "SELECT COUNT(id) FROM users WHERE role = @role AND status = @status";

                        using (SqlCommand cmd = new SqlCommand(selectData, connect))
                        {
                            cmd.Parameters.AddWithValue("@role", "Cashier");
                            cmd.Parameters.AddWithValue("@status", "Active");

                            SqlDataReader reader = cmd.ExecuteReader();

                            if (reader.Read())
                            {
                                int count = Convert.ToInt32(reader[0]);
                                dashboard_TC.Text = count.ToString();
                            }

                            reader.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed connection: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public void displayTotalCustomers()
        {
            using (SqlConnection connect = new SqlConnection(conn))
            {
                if (connect.State == ConnectionState.Closed)
                {
                    try
                    {
                        connect.Open();

                        string selectData = "SELECT COUNT(id) FROM customers";

                        using (SqlCommand cmd = new SqlCommand(selectData, connect))
                        {
                            SqlDataReader reader = cmd.ExecuteReader();

                            if (reader.Read())
                            {
                                int count = Convert.ToInt32(reader[0]);
                                dashboard_TCust.Text = count.ToString();
                            }

                            reader.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed connection: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public void displayTodaysIncome()
        {
            using (SqlConnection connect = new SqlConnection(conn))
            {
                if (connect.State == ConnectionState.Closed)
                {
                    try
                    {
                        connect.Open();

                        string selectData = "SELECT SUM(total_price) FROM customers WHERE DATE = @date";

                        using (SqlCommand cmd = new SqlCommand(selectData, connect))
                        {
                            DateTime today = DateTime.Today;
                            string getToday = today.ToString("yyyy-MM-dd");

                            cmd.Parameters.AddWithValue("@date", getToday);

                            SqlDataReader reader = cmd.ExecuteReader();

                            if (reader.Read())
                            {
                                object value = reader[0];

                                if (value != DBNull.Value)
                                {
                                    float count = Convert.ToSingle(reader[0]);
                                    dashboard_TI.Text = "$" + count.ToString("0.00");
                                }
                            }

                            reader.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed connection: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public void displayTotalIncome()
        {
            using (SqlConnection connect = new SqlConnection(conn))
            {
                if (connect.State == ConnectionState.Closed)
                {
                    try
                    {
                        connect.Open();

                        string selectData = "SELECT SUM(total_price) FROM customers";

                        using (SqlCommand cmd = new SqlCommand(selectData, connect))
                        {
                            SqlDataReader reader = cmd.ExecuteReader();

                            if (reader.Read())
                            {
                                float count = Convert.ToSingle(reader[0]);
                                dashboard_TIn.Text = "$" + count.ToString("0.00");
                            }

                            reader.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed connection: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}

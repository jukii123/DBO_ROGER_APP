using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

using System.Security.Cryptography;
using System.Text;



namespace CafeShopManagementSystem
{
    public partial class Form1 : Form
    {
        static string conn = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;
        SqlConnection connect = new SqlConnection(conn);

        public Form1()
        {
            InitializeComponent();
        }

        private void close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void login_registerBtn_Click(object sender, EventArgs e)
        {
            RegisterForm regForm = new RegisterForm();
            regForm.Show();

            this.Hide();
        }

        private void login_showPass_CheckedChanged(object sender, EventArgs e)
        {
            login_password.PasswordChar = login_showPass.Checked ? '\0' : '*';
        }

        public bool emptyFields()
        {
            if(login_username.Text == "" || login_password.Text == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }





        private void login_btn_Click(object sender, EventArgs e)
        {
            if (emptyFields())
            {
                MessageBox.Show("All fields are required to be filled.", "Error Message", MessageBoxButtons.OK);
            }
            else
            {
                if (connect.State == ConnectionState.Closed)
                {
                    try
                    {
                        connect.Open();

                        // Primero obtenemos el hash almacenado de la contraseña del usuario
                        string selectHash = "SELECT password, role, status FROM users WHERE username = @usern";

                        using (SqlCommand cmd = new SqlCommand(selectHash, connect))
                        {
                            cmd.Parameters.AddWithValue("@usern", login_username.Text.Trim());

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string storedHash = reader["password"].ToString();
                                    string userRole = reader["role"].ToString();
                                    string status = reader["status"].ToString();

                                    // Verificamos que el estado sea "Activo"
                                    if (status == "Active")
                                    {
                                        // Comparamos la contraseña ingresada con el hash almacenado
                                        if (SimpleHashHelper.VerifyPassword(login_password.Text.Trim(), storedHash))
                                        {
                                            MessageBox.Show("Login successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                            if (userRole == "Admin")
                                            {
                                                AdminMainForm adminForm = new AdminMainForm();
                                                adminForm.Show();
                                                this.Hide();
                                            }
                                            else if (userRole == "Cashier")
                                            {
                                                CashierMainForm cashierForm = new CashierMainForm();
                                                cashierForm.Show();
                                                this.Hide();
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Incorrect password.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Account not active. Await admin approval.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Username not found.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Connection failed: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connect.Close();
                    }
                }
            }
        }









    }
}

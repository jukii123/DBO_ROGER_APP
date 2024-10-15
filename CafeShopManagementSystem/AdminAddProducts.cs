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
using System.IO;
using System.Configuration;

namespace CafeShopManagementSystem
{
    public partial class AdminAddProducts : UserControl
    {
        static string conn = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;
        SqlConnection connect = new SqlConnection(conn);

        public AdminAddProducts()
        {
            InitializeComponent();
            displayData();

            // Agregar elementos al ComboBox al inicializar el control
            Ordenar_por.Items.Add("ID");
            Ordenar_por.Items.Add("Nombre");
            Ordenar_por.Items.Add("Precio");

            // Seleccionar un valor predeterminado
            Ordenar_por.SelectedIndex = 0;  // Selecciona "ID" como predeterminado
        }


        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }
            displayData();
        }

        public bool emptyFields()
        {
            return adminAddProducts_id.Text == "" || adminAddProducts_name.Text == ""
                || adminAddProducts_type.SelectedIndex == -1 || adminAddProducts_stock.Text == ""
                || adminAddProducts_price.Text == "" || adminAddProducts_status.SelectedIndex == -1;
        }

        public void displayData(string ordenarPor = "ID")
        {
            AdminAddProductsData prodData = new AdminAddProductsData();
            List<AdminAddProductsData> listData = prodData.productsListData();

            // Aplicar el ordenamiento según el criterio seleccionado
            switch (ordenarPor)
            {
                case "Nombre":
                    listData = QuickSort(listData, "Nombre");
                    break;
                case "Precio":
                    listData = QuickSort(listData, "Precio");
                    break;
                case "ID":
                default:
                    listData = QuickSort(listData, "ID");
                    break;
            }

            // Mostrar los datos ordenados en el DataGridView
            DataGridView1.DataSource = listData;
        }

        public List<AdminAddProductsData> QuickSort(List<AdminAddProductsData> products, string criterio)
        {
            if (products.Count <= 1)
                return products;

            var pivot = products[products.Count / 2];
            List<AdminAddProductsData> left;
            List<AdminAddProductsData> right;

            switch (criterio)
            {
                case "Nombre":
                    left = products.Where(x => string.Compare(x.ProductName, pivot.ProductName, StringComparison.OrdinalIgnoreCase) < 0).ToList();
                    right = products.Where(x => string.Compare(x.ProductName, pivot.ProductName, StringComparison.OrdinalIgnoreCase) > 0).ToList();
                    break;
                case "Precio":
                    left = products.Where(x => Convert.ToDecimal(x.Price) < Convert.ToDecimal(pivot.Price)).ToList();
                    right = products.Where(x => Convert.ToDecimal(x.Price) > Convert.ToDecimal(pivot.Price)).ToList();
                    break;
                case "ID":
                default:
                    left = products.Where(x => string.Compare(x.ProductID, pivot.ProductID, StringComparison.OrdinalIgnoreCase) < 0).ToList();
                    right = products.Where(x => string.Compare(x.ProductID, pivot.ProductID, StringComparison.OrdinalIgnoreCase) > 0).ToList();
                    break;
            }

            return QuickSort(left, criterio).Concat(new List<AdminAddProductsData> { pivot }).Concat(QuickSort(right, criterio)).ToList();
        }








        public List<AdminAddProductsData> QuickSort(List<AdminAddProductsData> products)
        {
            if (products.Count <= 1)
                return products;

            var pivot = products[products.Count / 2];
            var left = products.Where(x => string.Compare(x.ProductName, pivot.ProductName, StringComparison.OrdinalIgnoreCase) < 0).ToList();
            var right = products.Where(x => string.Compare(x.ProductName, pivot.ProductName, StringComparison.OrdinalIgnoreCase) > 0).ToList();

            return QuickSort(left).Concat(new List<AdminAddProductsData> { pivot }).Concat(QuickSort(right)).ToList();
        }





        private void adminAddProducts_addBtn_Click(object sender, EventArgs e)
        {
            if (emptyFields())
            {
                MessageBox.Show("All fields are required to be filled.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();

                    // Check if the product ID exists
                    string selectProdID = "SELECT * FROM products WHERE prod_id = @prodID";
                    using (SqlCommand selectPID = new SqlCommand(selectProdID, connect))
                    {
                        selectPID.Parameters.AddWithValue("@prodID", adminAddProducts_id.Text.Trim());
                        SqlDataAdapter adapter = new SqlDataAdapter(selectPID);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count >= 1)
                        {
                            MessageBox.Show("Product ID: " + adminAddProducts_id.Text.Trim() + " is already taken", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            // Insert product data
                            string insertData = "INSERT INTO products (prod_id, prod_name, prod_type, prod_stock, prod_price, prod_status, prod_image, date_insert) " +
                                                "VALUES(@prodID, @prodName, @prodType, @prodStock, @prodPrice, @prodStatus, @prodImage, @dateInsert)";
                            DateTime today = DateTime.Today;

                            string path = Path.Combine(@"C:\Users\Lisq8\source\repos\CafeShopManagementSystem\CafeShopManagementSystem\Product_Directory\", adminAddProducts_id.Text.Trim() + ".jpg");
                            string directoryPath = Path.GetDirectoryName(path);

                            if (!Directory.Exists(directoryPath))
                            {
                                Directory.CreateDirectory(directoryPath);
                            }

                            File.Copy(adminAddProducts_imageView.ImageLocation, path, true);

                            using (SqlCommand cmd = new SqlCommand(insertData, connect))
                            {
                                cmd.Parameters.AddWithValue("@prodID", adminAddProducts_id.Text.Trim());
                                cmd.Parameters.AddWithValue("@prodName", adminAddProducts_name.Text.Trim());
                                cmd.Parameters.AddWithValue("@prodType", adminAddProducts_type.Text.Trim());
                                cmd.Parameters.AddWithValue("@prodStock", adminAddProducts_stock.Text.Trim());
                                cmd.Parameters.AddWithValue("@prodPrice", adminAddProducts_price.Text.Trim());
                                cmd.Parameters.AddWithValue("@prodStatus", adminAddProducts_status.Text.Trim());
                                cmd.Parameters.AddWithValue("@prodImage", path);
                                cmd.Parameters.AddWithValue("@dateInsert", today);

                                cmd.ExecuteNonQuery();
                                clearFields();
                                MessageBox.Show("Added successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                displayData();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Connection failed: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        private void adminAddProducts_importBtn_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog
                {
                    Filter = "Image Files (*.jpg; *.png)|*.jpg;*.png"
                };
                string imagePath = "";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    imagePath = dialog.FileName;
                    adminAddProducts_imageView.ImageLocation = imagePath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void clearFields()
        {
            adminAddProducts_id.Text = "";
            adminAddProducts_name.Text = "";
            adminAddProducts_type.SelectedIndex = -1;
            adminAddProducts_stock.Text = "";
            adminAddProducts_price.Text = "";
            adminAddProducts_status.SelectedIndex = -1;
            adminAddProducts_imageView.Image = null;
        }

        private void adminAddProducts_clearBtn_Click(object sender, EventArgs e)
        {
            clearFields();
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow row = DataGridView1.Rows[e.RowIndex];
                adminAddProducts_id.Text = row.Cells[1].Value.ToString();
                adminAddProducts_name.Text = row.Cells[2].Value.ToString();
                adminAddProducts_type.Text = row.Cells[3].Value.ToString();
                adminAddProducts_stock.Text = row.Cells[4].Value.ToString();
                adminAddProducts_price.Text = row.Cells[5].Value.ToString();
                adminAddProducts_status.Text = row.Cells[6].Value.ToString();

                string imagepath = row.Cells[7].Value.ToString();
                try
                {
                    if (imagepath != null)
                    {
                        adminAddProducts_imageView.Image = Image.FromFile(imagepath);
                    }
                    else
                    {
                        adminAddProducts_imageView.Image = null;
                    }
                }
                catch
                {
                    adminAddProducts_imageView.Image = null; // Opcional: Reiniciar la imagen si hay un error
                }
            }
        }


        private void adminAddProducts_updateBtn_Click(object sender, EventArgs e)
        {
            if (connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();

                    // Si deseas evitar el mensaje de campos vacíos, puedes comentar esta parte:
                    /*
                    if (emptyFields())
                    {
                        MessageBox.Show("All fields are required to be filled.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    */

                    // Verificar si el ID del producto existe
                    string selectProdID = "SELECT * FROM products WHERE prod_id = @prodID";
                    using (SqlCommand selectPID = new SqlCommand(selectProdID, connect))
                    {
                        selectPID.Parameters.AddWithValue("@prodID", adminAddProducts_id.Text.Trim());
                        SqlDataAdapter adapter = new SqlDataAdapter(selectPID);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            MessageBox.Show("Product ID: " + adminAddProducts_id.Text.Trim() + " does not exist.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            // Actualizar los datos del producto
                            string updateData = "UPDATE products SET prod_name = @prodName, prod_type = @prodType, prod_stock = @prodStock, " +
                                                "prod_price = @prodPrice, prod_status = @prodStatus, prod_image = @prodImage WHERE prod_id = @prodID";

                            string path = Path.Combine(@"C:\Users\Lisq8\source\repos\CafeShopManagementSystem\CafeShopManagementSystem\Product_Directory\", adminAddProducts_id.Text.Trim() + ".jpg");
                            string directoryPath = Path.GetDirectoryName(path);

                            if (!Directory.Exists(directoryPath))
                            {
                                Directory.CreateDirectory(directoryPath);
                            }

                            // Aquí se debe manejar la carga de la nueva imagen si es necesario
                            if (!string.IsNullOrEmpty(adminAddProducts_imageView.ImageLocation))
                            {
                                File.Copy(adminAddProducts_imageView.ImageLocation, path, true);
                            }

                            using (SqlCommand cmd = new SqlCommand(updateData, connect))
                            {
                                cmd.Parameters.AddWithValue("@prodID", adminAddProducts_id.Text.Trim());
                                cmd.Parameters.AddWithValue("@prodName", adminAddProducts_name.Text.Trim());
                                cmd.Parameters.AddWithValue("@prodType", adminAddProducts_type.Text.Trim());
                                cmd.Parameters.AddWithValue("@prodStock", adminAddProducts_stock.Text.Trim());
                                cmd.Parameters.AddWithValue("@prodPrice", adminAddProducts_price.Text.Trim());
                                cmd.Parameters.AddWithValue("@prodStatus", adminAddProducts_status.Text.Trim());
                                cmd.Parameters.AddWithValue("@prodImage", path);

                                cmd.ExecuteNonQuery();
                                clearFields();
                                MessageBox.Show("Updated successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                displayData();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Connection failed: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }


        private void adminAddProducts_deleteBtn_Click(object sender, EventArgs e)
        {
            if (emptyFields())
            {
                MessageBox.Show("All fields are required.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult check = MessageBox.Show("Are you sure you want to delete Product ID: " + adminAddProducts_id.Text.Trim() + "?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (check == DialogResult.Yes)
            {
                if (connect.State != ConnectionState.Open)
                {
                    try
                    {
                        connect.Open();

                        string updateData = "UPDATE products SET date_delete = @dateDelete WHERE prod_id = @prodID";
                        DateTime today = DateTime.Today;

                        using (SqlCommand deleteD = new SqlCommand(updateData, connect))
                        {
                            deleteD.Parameters.AddWithValue("@dateDelete", today);
                            deleteD.Parameters.AddWithValue("@prodID", adminAddProducts_id.Text.Trim());

                            deleteD.ExecuteNonQuery();
                            clearFields();
                            MessageBox.Show("Deleted successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            displayData();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Connection failed: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connect.Close();
                    }
                }
            }
        }

        private void loadCsvToDbBtn_Click(object sender, EventArgs e)
        {
            
        }


        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                Title = "Select a CSV file"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Lee todas las líneas del archivo CSV
                    string[] lines = File.ReadAllLines(openFileDialog.FileName);

                    // Verifica si la conexión está cerrada
                    if (connect.State == ConnectionState.Closed)
                    {
                        connect.Open();
                    }

                    // Itera sobre las líneas del CSV, omitiendo la primera (encabezado)
                    foreach (string line in lines.Skip(1))
                    {
                        var values = line.Split(',');

                        // Asegúrate de que haya suficientes valores para insertar
                        if (values.Length >= 7) // Cambiado a 7 por el nuevo campo
                        {
                            // Debugging line
                            Console.WriteLine($"Inserting: {string.Join(", ", values)}");

                            // Prepara la instrucción SQL para la inserción
                            string insertData = "INSERT INTO products (prod_id, prod_name, prod_type, prod_stock, prod_price, prod_status, prod_image, date_insert) " +
                                                "VALUES(@prodID, @prodName, @prodType, @prodStock, @prodPrice, @prodStatus, @prodImage, @dateInsert)";

                            using (SqlCommand cmd = new SqlCommand(insertData, connect))
                            {
                                cmd.Parameters.AddWithValue("@prodID", values[0]);
                                cmd.Parameters.AddWithValue("@prodName", values[1]);
                                cmd.Parameters.AddWithValue("@prodType", values[2]);
                                cmd.Parameters.AddWithValue("@prodStock", values[3]);
                                cmd.Parameters.AddWithValue("@prodPrice", values[4]);
                                cmd.Parameters.AddWithValue("@prodStatus", values[5]);
                                cmd.Parameters.AddWithValue("@prodImage", values[6]); // Asegúrate de agregar esto
                                cmd.Parameters.AddWithValue("@dateInsert", DateTime.Now);

                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Mostrar un mensaje si la línea no tiene suficientes valores
                            Console.WriteLine($"Skipping line due to insufficient values: {line}");
                        }
                    }

                    MessageBox.Show("CSV data imported successfully.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    displayData(); // Asegúrate de que este método esté bien implementado
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading CSV: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Cierra la conexión si está abierta
                    if (connect.State == ConnectionState.Open)
                    {
                        connect.Close();
                    }
                }
            }
        }


        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Ordenar_quick_sort_Click(object sender, EventArgs e)
        {
            //displayData(sortByProductName: true);
        }

        private void Ordenar_por_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Obtener el criterio seleccionado en el ComboBox
            string criterio = Ordenar_por.SelectedItem.ToString();

            // Llamar a displayData con el criterio de ordenación
            displayData(criterio);

            

        }

    }

}

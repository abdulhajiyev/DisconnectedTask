using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace DisconnectedTask
{
    public partial class MainWindow
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DbConnect"].ConnectionString;

        public MainWindow()
        {
            InitializeComponent();

            LoadOrderIdToCombo();

            CBoxOrderIDs.SelectedIndex = 0;
        }

        private void LoadOrderIdToCombo()
        {
            CBoxOrderIDs.Items.Clear();
            try
            {
                SqlConnection sqlConnection;
                using (sqlConnection = new SqlConnection(_connectionString))
                {


                    sqlConnection.Open();

                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("SELECT * FROM Orders", sqlConnection);
                    DataTable dt = new DataTable();
                    sqlDataAdapter.Fill(dt);
                    DataTableReader dtr = dt.CreateDataReader();

                    while (dtr.Read())

                    {
                        CBoxOrderIDs.Items.Add(dtr["OrderID"].ToString());

                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void LoadCustomerProduct()
        {
            try
            {
                SqlConnection sqlConnection;
                using (sqlConnection = new SqlConnection(_connectionString))
                {
                    var cmdString = $@"SELECT Customers.CompanyName,Products.ProductName FROM Orders JOIN [Order Details] ON Orders.OrderID = [Order Details].OrderID JOIN Products ON Products.ProductID = [Order Details].ProductID JOIN Customers ON Customers.CustomerID = Orders.CustomerID WHERE Orders.OrderID = {CBoxOrderIDs.SelectedItem}";

                    SqlCommand sqlCommand = new SqlCommand(cmdString, sqlConnection);
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    DataTable dataTable = new DataTable(CBoxOrderIDs.SelectedItem.ToString());

                    sqlDataAdapter.Fill(dataTable);

                    MainDataGrid.ItemsSource = dataTable.DefaultView;
                    MWindow.Title = $"OrderID: {CBoxOrderIDs.SelectedItem}";
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        private void ShowDetailedInfo()
        {
            try
            {
                DataRowView row = (DataRowView)MainDataGrid.SelectedItem;
                SqlConnection sqlConnection;
                using (sqlConnection = new SqlConnection(_connectionString))
                {
                    var cmd = @"SELECT Customers.CompanyName,Products.ProductName, [Order Details].Quantity, [Order Details].UnitPrice, [Order Details].Discount, Orders.OrderID, (Quantity * [Order Details].UnitPrice) AS 'Total Price' FROM Orders JOIN [Order Details] ON Orders.OrderID = [Order Details].OrderID JOIN Products ON Products.ProductID = [Order Details].ProductID JOIN Customers ON Customers.CustomerID = Orders.CustomerID WHERE Products.ProductName = @pName";

                    sqlConnection.Open();

                    SqlDataAdapter sda = new SqlDataAdapter(cmd, sqlConnection);
                    sda.SelectCommand.Parameters.Add("@pName", SqlDbType.NVarChar).Value = row["ProductName"];

                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    OrderDetails orderDetails = new OrderDetails(dt);
                    orderDetails.ShowDialog();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        private void CBoxOrderIDs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadCustomerProduct();
        }

        private void BtnOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            ShowDetailedInfo();
        }

        private void MainDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnOrderDetails.IsEnabled = MainDataGrid.CurrentCell.Item.ToString() != "{NewItemPlaceholder}";
        }
    }
}

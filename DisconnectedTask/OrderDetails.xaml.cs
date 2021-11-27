using System.Data;

namespace DisconnectedTask
{
    /// <summary>
    /// Interaction logic for OrderDetails.xaml
    /// </summary>
    public partial class OrderDetails
    {
        public OrderDetails()
        {
            InitializeComponent();

        }

        public OrderDetails(DataTable dt)
        {
            InitializeComponent();

            CompTxt.Text = dt.Rows[0][0].ToString();
            ProdTxt.Text = dt.Rows[0][1].ToString();
            QuanTxt.Text = dt.Rows[0][2].ToString();
            PriceTxt.Text = dt.Rows[0][3].ToString();
            DisTxt.Text = dt.Rows[0][4].ToString();
            OrdIdTxt.Text = dt.Rows[0][5].ToString();
            TotPrcTxt.Text = dt.Rows[0][6].ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Payroll_System
{
    public partial class DetailsForm : Form
    {
        public static DetailsForm instance;
        private string EmployeeID;
        private decimal EmployeeDailyRate;
        readonly SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|Database1.mdf; Integrated Security=False");
        SqlDataReader dataReader;

        public object MessaageBox { get; private set; }

        public static DetailsForm getInstance()
        {
            if (instance == null || instance.IsDisposed)
                instance = new DetailsForm();
            else
                instance.BringToFront();
            return instance;
        }

        public DetailsForm()
        {
            InitializeComponent();
        }

        private void DetailsForm_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'database1DataSet.employee' table. You can move, or remove it, as needed.
            this.employeeTableAdapter.Fill(this.database1DataSet.employee);
            empNumber.Text = null;
        }

        private void empNumber_TextChanged(object sender, EventArgs e)
        {
            string empID = empNumber.Text;
            fillListView(empID);
        }

        private void fillListView(string empID)
        {
            if(empID != null)
            {
                try
                {
                    con.Open();
                    string query = $"SELECT * FROM employee WHERE id = '{empID}'";
                    SqlCommand cmd = new SqlCommand(query, con);
                    dataReader = cmd.ExecuteReader();
                    if (dataReader.Read())
                    {
                        empDetailsListView.Items.Clear();

                        this.EmployeeID = dataReader["id"].ToString();
                        this.EmployeeDailyRate = decimal.Parse(dataReader["dailyRate"].ToString());

                        ListViewItem item1 = new ListViewItem("Employee Number");
                        item1.SubItems.Add(EmployeeID+" (computer generated)");
                        ListViewItem item2 = new ListViewItem("Employee Name");
                        string empName = $"{dataReader["lastName"]}, " +
                            $"{dataReader["firstName"]} " +
                            $"{dataReader["middleName"]}";

                        item2.SubItems.Add(empName.ToUpper());
                        ListViewItem item3 = new ListViewItem("Date of Birth ");
                        item3.SubItems.Add(DateTime.Parse(dataReader["birthDate"].ToString()).ToString("MMMM dd, yyyy"));
                        ListViewItem item4 = new ListViewItem("Daily Rate");
                        item4.SubItems.Add(dataReader["dailyRate"].ToString());
                        ListViewItem item5 = new ListViewItem("Working Days");
                        item5.SubItems.Add(dataReader["workingDays"].ToString());
                        empDetailsListView.Items.Add(item1);
                        empDetailsListView.Items.Add(item2);
                        empDetailsListView.Items.Add(item3);
                        empDetailsListView.Items.Add(item4);
                        empDetailsListView.Items.Add(item5);
                    }
                    con.Close();
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message.ToString());
                }
            }
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            double Days = (endingDate.Value - startingDate.Value).TotalDays;
            if (Days <= 0) {
                MessageBox.Show("Invalid Date");
            }
            else if(Days > 3)
            {
                decimal singlePay = (decimal.Parse(Days.ToString()) - 3) * this.EmployeeDailyRate;
                decimal doublePay = this.EmployeeDailyRate * 2 * 3;
                decimal totalPay = singlePay + doublePay;
                labelTHPay.Text = "₱ " + totalPay.ToString();
            }
            else
            {
                decimal totalPay = decimal.Parse(Days.ToString()) * this.EmployeeDailyRate;
                labelTHPay.Text = "₱ " + totalPay.ToString();
            }
            
            labelEmpNumber.Text = this.EmployeeID.ToString();
            labelStartingDate.Text = startingDate.Value.ToString("MMMM dd, yyyy");
            labelEndingDate.Text = endingDate.Value.ToString("MMMM dd, yyyy");   
        }
    }
}

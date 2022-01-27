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
    public partial class EditForm : System.Windows.Forms.Form
    {
        readonly SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|Database1.mdf; Integrated Security=False");
        SqlDataReader dataReader;

        public static EditForm instance;
        private readonly Action loadEmployee;
        private string selectedEmpID;


        public static EditForm getInstance(Action action, string selectedEmpID)
        {
            if (instance == null || instance.IsDisposed)
                instance = new EditForm(action, selectedEmpID);
            else
                instance.BringToFront();
            return instance;
        }

        private void getEmployee()
        {
            try
            {
                con.Open();
                string query = $"SELECT * FROM employee WHERE id = '{this.selectedEmpID}'";
                SqlCommand cmd = new SqlCommand(query, con);
                dataReader = cmd.ExecuteReader();
                if (dataReader.Read())
                {
                    firstName.Text = dataReader["firstName"].ToString();
                    middleName.Text = dataReader["middleName"].ToString();
                    lastName.Text = dataReader["lastName"].ToString();
                    birthDate.Value = DateTime.Parse(dataReader["birthDate"].ToString());
                    dailyRate.Text = dataReader["dailyRate"].ToString();
                    workingDays.Text = dataReader["workingDays"].ToString();
                }
                con.Close();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }

        }
        public EditForm(Action action, string selectedEmpID)
        {
            this.loadEmployee = action;
            this.selectedEmpID = selectedEmpID;
            InitializeComponent();
            getEmployee();
        }

        private void CloseForm()
        {
            loadEmployee();
            this.Dispose();
        }

        private bool IsFormValid()
        {
            if (string.IsNullOrWhiteSpace(firstName.Text) ||
                string.IsNullOrWhiteSpace(lastName.Text) ||
                string.IsNullOrWhiteSpace(birthDate.Text) ||
                string.IsNullOrWhiteSpace(dailyRate.Text) ||
                string.IsNullOrWhiteSpace(workingDays.Text) &&
                (workingDays.Text != "MWF" || workingDays.Text != "TTH"))
            {
                return false;
            }
            return true;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to update this?", "Update", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (IsFormValid())
                {
                    try
                    {
                        con.Open();
                        string query = "UPDATE employee SET firstName=@firstName, middleName=@middleName, " +
                            $"lastName=@lastName, birthDate=@birthDate, dailyRate=@dailyRate, workingDays=@workingDays WHERE id='{selectedEmpID}'";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@firstName", firstName.Text);
                        cmd.Parameters.AddWithValue("@middleName", middleName.Text);
                        cmd.Parameters.AddWithValue("@lastName", lastName.Text);
                        cmd.Parameters.AddWithValue("@birthDate", birthDate.Value);
                        cmd.Parameters.AddWithValue("@dailyRate", decimal.Parse(dailyRate.Text));
                        cmd.Parameters.AddWithValue("@workingDays", workingDays.Text);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        MessageBox.Show($"Employee no. {selectedEmpID} has been updated!");
                        CloseForm();
                    }
                    catch(Exception ex)
                    {
                        con.Close();
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
                else
                {
                    MessageBox.Show("Please check all required fields");
                }
            }
        }
    }
}

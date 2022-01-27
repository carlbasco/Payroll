using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Payroll_System
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        readonly SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|Database1.mdf; Integrated Security=False");
        SqlDataReader dataReader;

        private string selectedListItemID;

        public MainForm()
        {
            InitializeComponent();
        }

        private void ButtonHide()
        {
            btnDelete.Hide();
            btnEdit.Hide();
        }

        private void ButtonShow()
        {
            btnDelete.Show();
            btnEdit.Show();
        }

        public void LoadEmployee()
        {
            try
            {
                empListView.Items.Clear();
                con.Open();
                string query = "SELECT * FROM employee";
                SqlCommand cmd = new SqlCommand(query, con);
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    string fName = dataReader["firstName"].ToString();
                    string mName = dataReader["middleName"].ToString();
                    string lName = dataReader["lastName"].ToString();
                    string birthDate = DateTime.Parse(dataReader["birthDate"].ToString()).ToString("MMMM dd, yyyy");
                    string id = dataReader["id"].ToString();
                    string emp_Name_Dob = $"{lName.ToUpper()}, {fName.ToUpper()} {mName.ToUpper()} \n\n{birthDate}";

                    ListViewItem item = new ListViewItem(emp_Name_Dob);
                    item.SubItems.Add(id);
                    empListView.Items.Add(item);
                }
                con.Close();
            }
            catch (Exception e)
            {
                con.Close();
                MessageBox.Show(e.Message.ToString());
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadEmployee();
        }

        private void NewForm_Click(object sender, EventArgs e)
        {
            search.Text = "";
            Form formEmployee = NewForm.getInstance(this.LoadEmployee);
            formEmployee.Show();
        }

        private void SearchQuery(string word)
        {
            try
            {
                empListView.Items.Clear();
                con.Open();
                string query = $"SELECT * FROM employee WHERE firstName LIKE '%{word}%' OR lastName LIKE '%{word}%' OR middleName LIKE '%{word}%' OR id LIKE '%{word}%'";
                SqlCommand cmd = new SqlCommand(query, con);
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    string fName = dataReader["firstName"].ToString();
                    string mName = dataReader["middleName"].ToString();
                    string lName = dataReader["lastName"].ToString();
                    string birthDate = DateTime.Parse(dataReader["birthDate"].ToString()).ToString("MMMM dd, yyyy");
                    string id = dataReader["id"].ToString();
                    string emp_Name_Dob = $"{lName.ToUpper()}, {fName.ToUpper()} {mName.ToUpper()} \n\n{birthDate}";

                    ListViewItem item = new ListViewItem(emp_Name_Dob);
                    item.SubItems.Add(id);
                    empListView.Items.Add(item);
                }
                con.Close();

            }
            catch (Exception e)
            {
                con.Close();
                MessageBox.Show(e.Message.ToString());
            }

        }

        private void Search_TextChanged(object sender, EventArgs e)
        {
            if (search.Text != "")
            {
                SearchQuery(search.Text);
            }
            else
            {
                LoadEmployee();
            }
        }

        private void setSelectedListViewItemID()
        {
            if (empListView.SelectedItems.Count > 0)
            {
                ListViewItem item = empListView.SelectedItems[0];
                selectedListItemID = item.SubItems[1].Text;
            }
            else
            {
                selectedListItemID = null;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            search.Text = "";
            setSelectedListViewItemID();
            if (selectedListItemID != null && selectedListItemID != "")
            {
                Form editEmployeeForm = EditForm.getInstance(LoadEmployee, selectedListItemID);
                editEmployeeForm.Show();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            search.Text = "";
            setSelectedListViewItemID();
            try
            {
                if (selectedListItemID != null && selectedListItemID != "")
                {
                    if (MessageBox.Show("Are you sure you want to delete this?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        con.Open();
                        string query = $"DELETE FROM employee WHERE id = '{selectedListItemID}'";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        LoadEmployee();
                    }
                }
                
            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCompute_Click(object sender, EventArgs e)
        {
            Form detailsForm = DetailsForm.getInstance();
            detailsForm.Show();
        }
    }
}

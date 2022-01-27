using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Payroll_System
{
    public partial class NewForm : System.Windows.Forms.Form
    {
        readonly SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|Database1.mdf; Integrated Security=False");
        public static NewForm instance;
        private readonly Action loadEmployee;

        public NewForm(Action action)
        {
            this.loadEmployee = action;
            InitializeComponent();
        }

        public static NewForm getInstance(Action action)
        {
            if (instance == null || instance.IsDisposed)
                instance = new NewForm(action);
            else
                instance.BringToFront();
            return instance;
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

        private string GenerateID()
        {
            string lName = lastName.Text.Substring(lastName.TextLength - 3).ToUpper();
            Random generator = new Random();
            string randomNumber = generator.Next(0, 100000).ToString("D5");
            string bDate = birthDate.Value.ToString("ddMMMyyyy").ToUpper();
            return $"{lName}-{randomNumber}-{bDate}";
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (IsFormValid())
            {
                try
                {
                    con.Open();
                    string query = "INSERT INTO employee(id, firstName, middleName, lastName, birthDate, dailyRate, workingDays) values " +
                        "(@id, @firstName, @middleName, @lastName, @birthDate, @dailyRate, @workingDays)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", GenerateID());
                    cmd.Parameters.AddWithValue("@firstName", firstName.Text);
                    cmd.Parameters.AddWithValue("@middleName", middleName.Text);
                    cmd.Parameters.AddWithValue("@lastName", lastName.Text);
                    cmd.Parameters.AddWithValue("@birthDate", birthDate.Value);
                    cmd.Parameters.AddWithValue("@dailyRate", decimal.Parse(dailyRate.Text));
                    cmd.Parameters.AddWithValue("@workingDays", workingDays.Text);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("New Employee has been added!");
                    CloseForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            else
            {
                MessageBox.Show("Please check all required fields");
            }

        }

        private void CloseForm()
        {
            loadEmployee();
            this.Dispose();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void DailyRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void workingDays_Enter(object sender, EventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            if (!combo.DroppedDown)
            {
                if (combo.PointToClient(Cursor.Position).X <
                    combo.ClientRectangle.Width - SystemInformation.VerticalScrollBarWidth)
                {
                    combo.DroppedDown = true;
                    Cursor = Cursors.Arrow;
                }
            }
        }
    }
}

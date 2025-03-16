using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace StudentManagement
{
    public partial class Enrollment : Form
    {
        public Enrollment()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(@"Data Source=LAPTOP-6RI8HTNK;Initial Catalog=StudentDB;Integrated Security=True;");
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            // Automatically formats the date using default behavior
            dateTimePicker1.CustomFormat = "dd/MM/yyyy";
        }

        private void dateTimePicker1_KeyDown(object sender, KeyEventArgs e)
        {
            // Optionally clear the custom format if backspace is pressed
            if (e.KeyCode == Keys.Back)
            {
                dateTimePicker1.CustomFormat = "";  // Clears the format if Backspace is pressed
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate user inputs to prevent empty or invalid fields
                if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text))
                {
                    MessageBox.Show("Please fill all fields.");
                    return;
                }

                using (SqlConnection con = GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("Insert into Enroll (eid, studentname, course, enrolldate) values(@Eid, @StudentName, @Course, @EnrollDate)", con))
                    {
                        cmd.Parameters.Add("@Eid", SqlDbType.Int).Value = int.Parse(textBox1.Text);
                        cmd.Parameters.AddWithValue("@StudentName", textBox2.Text);
                        cmd.Parameters.AddWithValue("@Course", textBox3.Text);
                        cmd.Parameters.Add("@EnrollDate", SqlDbType.DateTime).Value = dateTimePicker1.Value;

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Record Saved Successfully!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("Select * from Enroll", con))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM Enroll WHERE eid=@Eid", con))
                    {
                        cmd.Parameters.Add("@Eid", SqlDbType.Int).Value = int.Parse(textBox1.Text);  // Explicit parameter type
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Record Deleted Successfully!");
                }

                // Refresh the DataGridView to reflect the changes
                btnAdd_Click(sender, e);  // This will reload the data and update the DataGridView

                // Reset all the text boxes and date picker after deletion
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                dateTimePicker1.Value = DateTime.Now;  // Set date picker to current date
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Enrollment_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'studentDBDataSet2.Enroll' table. You can move, or remove it, as needed.
            this.enrollTableAdapter.Fill(this.studentDBDataSet2.Enroll);

            try
            {
                using (SqlConnection con = GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Enroll", con))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Close the Enrollment form when the label5 is clicked
        private void label6_Click(object sender, EventArgs e)
        {
            this.Close(); // Close only the current form (Enrollment form)
        }
    }
}

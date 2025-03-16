using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace StudentManagement
{
    public partial class Student : Form
    {
        public Student()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        // Centralized method for opening the database connection
        private SqlConnection GetConnection()
        {
            return new SqlConnection(@"Data Source=LAPTOP-6RI8HTNK;Initial Catalog=StudentDB;Integrated Security=True;");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate StudentId before updating
                if (string.IsNullOrWhiteSpace(textBox1.Text) || !int.TryParse(textBox1.Text, out int studentId))
                {
                    MessageBox.Show("Please enter a valid StudentId.");
                    return;
                }

                using (SqlConnection con = GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("UPDATE Student SET studentname=@studentname, phone=@phone, email=@email WHERE studentid=@studentid", con))
                    {
                        cmd.Parameters.AddWithValue("@StudentId", studentId);
                        cmd.Parameters.AddWithValue("@StudentName", textBox2.Text);
                        cmd.Parameters.AddWithValue("@Phone", textBox3.Text);
                        cmd.Parameters.AddWithValue("@Email", textBox4.Text);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Record Updated Successfully!");
                            LoadStudentData(); // Refresh the DataGridView after saving
                            ResetTextBoxes();  // Clear textboxes after saving
                        }
                        else
                        {
                            MessageBox.Show("Student Saved Successfully.");
                        }
                    }
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
                // Validate StudentId before adding
                if (string.IsNullOrWhiteSpace(textBox1.Text) || !int.TryParse(textBox1.Text, out int studentId))
                {
                    MessageBox.Show("Please enter a valid StudentId.");
                    return;
                }

                using (SqlConnection con = GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("Insert into Student (studentid, studentname, phone, email) values(@studentid, @studentname, @phone, @email)", con))
                    {
                        cmd.Parameters.AddWithValue("@StudentId", studentId);
                        cmd.Parameters.AddWithValue("@StudentName", textBox2.Text);
                        cmd.Parameters.AddWithValue("@Phone", textBox3.Text);
                        cmd.Parameters.AddWithValue("@Email", textBox4.Text);

                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("New Record Added Successfully!");
                    LoadStudentData(); // Refresh the DataGridView after adding
                    ResetTextBoxes();  // Clear textboxes after adding
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
                // Validate StudentId before attempting to delete
                if (string.IsNullOrWhiteSpace(textBox1.Text) || !int.TryParse(textBox1.Text, out int studentId))
                {
                    MessageBox.Show("Please enter a valid StudentId to delete.");
                    return;
                }

                using (SqlConnection con = GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM Student WHERE studentid=@studentid", con))
                    {
                        cmd.Parameters.AddWithValue("@StudentId", studentId);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Record Deleted Successfully!");

                    // After deletion, reset the textboxes and refresh the DataGridView
                    ResetTextBoxes();  // Clear textboxes after deletion
                    LoadStudentData(); // Refresh the DataGridView after deleting
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Load data into the DataGridView
        private void Student_Load(object sender, EventArgs e)
        {
            LoadStudentData();
        }

        // Method to refresh DataGridView after each operation
        private void LoadStudentData()
        {
            try
            {
                using (SqlConnection con = GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Student", con))
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

        // Reset textboxes after operation
        private void ResetTextBoxes()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }

        // Close the application when label6 is clicked
        private void label6_Click(object sender, EventArgs e)
        {
            this.Close(); // Close only the current form (Student form)
        }

        // Populate the textboxes when a row is clicked in the DataGridView
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if a valid row is clicked
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Set the textboxes to the values of the selected row
                textBox1.Text = row.Cells["studentid"].Value.ToString();
                textBox2.Text = row.Cells["studentname"].Value.ToString();
                textBox3.Text = row.Cells["phone"].Value.ToString();
                textBox4.Text = row.Cells["email"].Value.ToString();
            }
        }
    }
}

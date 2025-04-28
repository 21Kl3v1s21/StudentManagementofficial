using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text.RegularExpressions;
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

        // Validate email format
        private bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }

        // Client-side validation before updating
        private bool ValidateInputsForUpdate()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || !int.TryParse(textBox1.Text, out int studentId))
            {
                MessageBox.Show("Please enter a valid StudentId.");
                SetTextBoxBorderColor(textBox1, false);  // Set red border for invalid input
                return false;
            }
            SetTextBoxBorderColor(textBox1, true);  // Reset border for valid input

            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Please enter a student name.");
                SetTextBoxBorderColor(textBox2, false);  // Set red border for invalid input
                return false;
            }
            SetTextBoxBorderColor(textBox2, true);  // Reset border for valid input

            if (string.IsNullOrWhiteSpace(textBox3.Text) || string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Please fill in all fields.");
                SetTextBoxBorderColor(textBox3, false);  // Set red border for invalid input
                SetTextBoxBorderColor(textBox4, false);
                return false;
            }
            SetTextBoxBorderColor(textBox3, true);  // Reset border for valid input
            SetTextBoxBorderColor(textBox4, true);

            if (!IsValidEmail(textBox4.Text))
            {
                MessageBox.Show("Please enter a valid email.");
                SetTextBoxBorderColor(textBox4, false);  // Set red border for invalid input
                return false;
            }
            SetTextBoxBorderColor(textBox4, true);  // Reset border for valid input

            return true;
        }

        // Method to add student
        private bool ValidateInputsForAdd()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || !int.TryParse(textBox1.Text, out int studentId))
            {
                MessageBox.Show("Please enter a valid StudentId.");
                SetTextBoxBorderColor(textBox1, false);  // Set red border for invalid input
                return false;
            }
            SetTextBoxBorderColor(textBox1, true);  // Reset border for valid input

            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Please enter a student name.");
                SetTextBoxBorderColor(textBox2, false);  // Set red border for invalid input
                return false;
            }
            SetTextBoxBorderColor(textBox2, true);  // Reset border for valid input

            if (string.IsNullOrWhiteSpace(textBox3.Text) || string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Please fill in all fields.");
                SetTextBoxBorderColor(textBox3, false);  // Set red border for invalid input
                SetTextBoxBorderColor(textBox4, false);
                return false;
            }
            SetTextBoxBorderColor(textBox3, true);  // Reset border for valid input
            SetTextBoxBorderColor(textBox4, true);

            if (!IsValidEmail(textBox4.Text))
            {
                MessageBox.Show("Please enter a valid email.");
                SetTextBoxBorderColor(textBox4, false);  // Set red border for invalid input
                return false;
            }
            SetTextBoxBorderColor(textBox4, true);  // Reset border for valid input

            if (StudentExists(int.Parse(textBox1.Text)))
            {
                MessageBox.Show("A student with this ID already exists.");
                SetTextBoxBorderColor(textBox1, false);  // Set red border for invalid input
                return false;
            }
            return true;
        }

        // Method to set text box border color based on validation result
        private void SetTextBoxBorderColor(TextBox textBox, bool isValid)
        {
            if (isValid)
            {
                textBox.BorderStyle = BorderStyle.FixedSingle;  // Default border style
                textBox.BackColor = Color.White;  // Reset background to white
            }
            else
            {
                textBox.BorderStyle = BorderStyle.Fixed3D;  // Red border for invalid input
                textBox.BackColor = Color.LightCoral;  // Change background color to light red
            }
        }

        // Save button event handler
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInputsForUpdate())
                return; // Exit if validation fails

            try
            {
                int studentId = int.Parse(textBox1.Text);

                // Check if the student exists before updating
                if (!StudentExists(studentId))
                {
                    MessageBox.Show("Student with this ID doesn't exist.");
                    return; // Exit if the student doesn't exist
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
                            LoadStudentData(); // Refresh DataGridView after saving
                            ResetTextBoxes();  // Clear textboxes after saving
                        }
                        else
                        {
                            MessageBox.Show("Failed to update student.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Method to add student
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputsForAdd())
                return; // Exit if validation fails

            try
            {
                using (SqlConnection con = GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Student (studentid, studentname, phone, email) VALUES(@studentid, @studentname, @phone, @email)", con))
                    {
                        cmd.Parameters.AddWithValue("@StudentId", textBox1.Text);
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

        // Method to check if the student exists by ID
        private bool StudentExists(int studentId)
        {
            using (SqlConnection con = GetConnection())
            {
                con.Open();
                string query = "SELECT COUNT(*) FROM Student WHERE studentid=@studentid";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        // Delete Student data
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate StudentId before attempting to delete
                if (string.IsNullOrWhiteSpace(textBox1.Text) || !int.TryParse(textBox1.Text, out int studentId))
                {
                    MessageBox.Show("Please enter a valid StudentId to delete.");
                    SetTextBoxBorderColor(textBox1, false);  // Set red border for invalid input
                    return;
                }
                SetTextBoxBorderColor(textBox1, true);  // Reset border to normal if input is valid

                // Check if the student exists
                if (!StudentExists(studentId))
                {
                    MessageBox.Show("No student found with this ID.");
                    return;  // Exit if no student is found with the given ID
                }

                using (SqlConnection con = GetConnection())
                {
                    con.Open();
                    // Delete the student record based on the provided StudentId
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM Student WHERE studentid=@studentid", con))
                    {
                        cmd.Parameters.AddWithValue("@StudentId", studentId);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Record Deleted Successfully!");

                    // After deletion, reset the textboxes and refresh the DataGridView
                    ResetTextBoxes();  // Clear the input fields
                    LoadStudentData(); // Refresh DataGridView to reflect the updated data
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

        // Refresh DataGridView
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

        // Reset textboxes after each operation
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

        // Populate textboxes when a row is clicked in DataGridView
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textBox1.Text = row.Cells["studentid"].Value.ToString();
                textBox2.Text = row.Cells["studentname"].Value.ToString();
                textBox3.Text = row.Cells["phone"].Value.ToString();
                textBox4.Text = row.Cells["email"].Value.ToString();
            }
        }
    }
}

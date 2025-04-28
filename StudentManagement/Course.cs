using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace StudentManagement
{
    public partial class Course : Form
    {
        public Course()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        // Centralized method for opening the database connection
        private SqlConnection GetConnection()
        {
            return new SqlConnection(@"Data Source=LAPTOP-6RI8HTNK;Initial Catalog=StudentDB;Integrated Security=True;");
        }

        // Set the border color of textboxes (for invalid/valid input)
        private void SetTextBoxBorderColor(TextBox textBox, bool isValid)
        {
            if (isValid)
            {
                textBox.BorderStyle = BorderStyle.FixedSingle;
                textBox.BackColor = Color.White;
            }
            else
            {
                textBox.BorderStyle = BorderStyle.Fixed3D;
                textBox.BackColor = Color.LightCoral;
            }
        }

        // Validate input before updating course
        private bool ValidateInputsForUpdate()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || !int.TryParse(textBox1.Text, out int courseId))
            {
                MessageBox.Show("Please enter a valid CourseId.");
                SetTextBoxBorderColor(textBox1, false);  // Set red border for invalid input
                return false;
            }
            SetTextBoxBorderColor(textBox1, true);

            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Please enter a course name.");
                SetTextBoxBorderColor(textBox2, false);  // Set red border for invalid input
                return false;
            }
            SetTextBoxBorderColor(textBox2, true);

            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Please enter a valid Duration.");
                SetTextBoxBorderColor(textBox3, false);  // Set red border for invalid input
                return false;
            }
            SetTextBoxBorderColor(textBox3, true);

            return true;
        }

        // Validate input before adding a course
        private bool ValidateInputsForAdd()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || !int.TryParse(textBox1.Text, out int courseId))
            {
                MessageBox.Show("Please enter a valid CourseId.");
                SetTextBoxBorderColor(textBox1, false);  // Set red border for invalid input
                return false;
            }
            SetTextBoxBorderColor(textBox1, true);

            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Please enter a course name.");
                SetTextBoxBorderColor(textBox2, false);  // Set red border for invalid input
                return false;
            }
            SetTextBoxBorderColor(textBox2, true);

            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Please enter a valid Duration.");
                SetTextBoxBorderColor(textBox3, false);  // Set red border for invalid input
                return false;
            }
            SetTextBoxBorderColor(textBox3, true);

            if (CourseExists(int.Parse(textBox1.Text)))
            {
                MessageBox.Show("A course with this ID already exists.");
                SetTextBoxBorderColor(textBox1, false);  // Set red border for invalid input
                return false;
            }

            return true;
        }

        // Method to check if the course already exists by CourseId
        private bool CourseExists(int courseId)
        {
            try
            {
                using (SqlConnection con = GetConnection())
                {
                    con.Open();
                    string query = "SELECT COUNT(*) FROM Course WHERE CourseId=@CourseId";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@CourseId", courseId);
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0; // Returns true if the course exists
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking course existence: " + ex.Message);
                return false;
            }
        }

        // Save Course data (Update existing record)
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInputsForUpdate())
                return; // Exit if validation fails

            try
            {
                int courseId = int.Parse(textBox1.Text);

                // Debugging: Output the CourseId and parameters to verify their correctness
                MessageBox.Show($"Updating course with ID: {courseId}\nCourse: {textBox2.Text}\nDuration: {textBox3.Text}");

                // Check if the course exists
                if (!CourseExists(courseId))
                {
                    MessageBox.Show("Course with this ID does not exist. Please check the CourseId.");
                    return; // Exit if the course doesn't exist
                }

                using (SqlConnection con = GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("UPDATE Course SET Course=@Course, Duration=@Duration WHERE CourseId=@CourseId", con))
                    {
                        cmd.Parameters.AddWithValue("@CourseId", courseId);
                        cmd.Parameters.AddWithValue("@Course", textBox2.Text);
                        cmd.Parameters.AddWithValue("@Duration", textBox3.Text);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Check if any rows were affected
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Record Updated Successfully!");
                            LoadCourseData(); // Refresh the DataGridView after saving
                            ResetTextBoxes();  // Clear textboxes after saving
                        }
                        else
                        {
                            MessageBox.Show("No records were updated. Please check the CourseId.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Add Course data (Insert new record)
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputsForAdd())
                return; // Exit if validation fails

            try
            {
                using (SqlConnection con = GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Course (CourseId, Course, Duration) VALUES (@CourseId, @Course, @Duration)", con))
                    {
                        cmd.Parameters.AddWithValue("@CourseId", textBox1.Text);
                        cmd.Parameters.AddWithValue("@Course", textBox2.Text);
                        cmd.Parameters.AddWithValue("@Duration", textBox3.Text);

                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("New Course Added Successfully!");
                    LoadCourseData(); // Refresh the DataGridView after adding
                    ResetTextBoxes();  // Clear textboxes after adding
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Delete Course data
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate CourseId before attempting to delete
                if (string.IsNullOrWhiteSpace(textBox1.Text) || !int.TryParse(textBox1.Text, out int courseId))
                {
                    MessageBox.Show("Please enter a valid CourseId to delete.");
                    SetTextBoxBorderColor(textBox1, false);  // Set red border for invalid input
                    return;
                }
                SetTextBoxBorderColor(textBox1, true);  // Reset border to normal if input is valid

                using (SqlConnection con = GetConnection())
                {
                    con.Open();
                    // Delete the record based on the provided CourseId
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM Course WHERE CourseId=@CourseId", con))
                    {
                        cmd.Parameters.AddWithValue("@CourseId", courseId);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Record Deleted Successfully!");

                    // After deletion, reset the textboxes and refresh the DataGridView
                    ResetTextBoxes();
                    LoadCourseData(); // Refresh the DataGridView after deletion
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        // Load Course data into DataGridView on form load
        private void Course_Load(object sender, EventArgs e)
        {
            LoadCourseData();
        }

        // Centralized method to load Course data into DataGridView
        private void LoadCourseData()
        {
            try
            {
                using (SqlConnection con = GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Course", con))
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
        }

        // Close the application when label5 is clicked
        private void label5_Click(object sender, EventArgs e)
        {
            this.Close(); // Close only the current form (Course form)
        }
    }
}

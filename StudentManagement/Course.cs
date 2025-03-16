using System;
using System.Data;
using System.Data.SqlClient;
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

        // Save Course data (Update existing record)
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate CourseId and Duration before proceeding
                if (string.IsNullOrWhiteSpace(textBox1.Text) || !int.TryParse(textBox1.Text, out int courseId))
                {
                    MessageBox.Show("Please enter a valid CourseId.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(textBox3.Text))
                {
                    MessageBox.Show("Please enter a valid Duration.");
                    return;
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

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Record Updated Successfully!");
                            LoadCourseData(); // Refresh the DataGridView after saving
                        }
                        else
                        {
                            MessageBox.Show("Course Saved Successfully.");
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
            try
            {
                // Validate CourseId and Duration before proceeding
                if (string.IsNullOrWhiteSpace(textBox1.Text) || !int.TryParse(textBox1.Text, out int courseId))
                {
                    MessageBox.Show("Please enter a valid CourseId.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(textBox3.Text))
                {
                    MessageBox.Show("Please enter a valid Duration.");
                    return;
                }

                using (SqlConnection con = GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Course (CourseId, Course, Duration) VALUES (@CourseId, @Course, @Duration)", con))
                    {
                        cmd.Parameters.AddWithValue("@CourseId", courseId);
                        cmd.Parameters.AddWithValue("@Course", textBox2.Text);
                        cmd.Parameters.AddWithValue("@Duration", textBox3.Text);

                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("New Course Added Successfully!");
                    LoadCourseData(); // Refresh the DataGridView after adding
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
                    return;
                }

                using (SqlConnection con = GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM Course WHERE CourseId=@CourseId", con))
                    {
                        cmd.Parameters.AddWithValue("@CourseId", courseId);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Record Deleted Successfully!");

                    // After deletion, reset the textboxes and refresh the DataGridView
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox3.Clear();

                    LoadCourseData(); // Refresh the DataGridView after deleting
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

        // Close the application when label5 is clicked
        private void label5_Click(object sender, EventArgs e)
        {
            this.Close(); // Close only the current form (Course form)
        }
    }
}

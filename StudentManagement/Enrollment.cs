using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
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

        // Validate inputs before saving or updating the enrollment
        private bool ValidateInputs()
        {
            // Validate Eid (ensure it's a number)
            if (string.IsNullOrWhiteSpace(textBox1.Text) || !int.TryParse(textBox1.Text, out int eid))
            {
                MessageBox.Show("Please enter a valid Enrollment ID.");
                SetTextBoxBorderColor(textBox1, false);
                return false;
            }
            SetTextBoxBorderColor(textBox1, true);

            // Validate StudentName
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Please enter a valid Student Name.");
                SetTextBoxBorderColor(textBox2, false);
                return false;
            }
            SetTextBoxBorderColor(textBox2, true);

            // Validate Course
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Please enter a valid Course.");
                SetTextBoxBorderColor(textBox3, false);
                return false;
            }
            SetTextBoxBorderColor(textBox3, true);

            // Validate Enrollment Date (ensure it's selected)
            if (dateTimePicker1.Value == null || dateTimePicker1.Value == DateTime.MinValue)
            {
                MessageBox.Show("Please select a valid Enrollment Date.");
                return false;
            }

            return true;
        }

        // Check if the Enrollment ID already exists
        private bool EnrollmentExists(int eid)
        {
            using (SqlConnection con = GetConnection())
            {
                con.Open();
                string query = "SELECT COUNT(*) FROM Enroll WHERE eid=@Eid";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Eid", eid);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0; // Returns true if the enrollment exists
                }
            }
        }

        // Save or Update Enrollment data
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInputs())
                    return; // Exit if validation fails

                int eid = int.Parse(textBox1.Text);

                // Check if the Enrollment exists
                if (!EnrollmentExists(eid))
                {
                    MessageBox.Show("Enrollment with this ID doesn't exist. Please check the Enrollment ID.");
                    return;
                }

                using (SqlConnection con = GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("UPDATE Enroll SET studentname=@StudentName, course=@Course, enrolldate=@EnrollDate WHERE eid=@Eid", con))
                    {
                        cmd.Parameters.AddWithValue("@Eid", eid);
                        cmd.Parameters.AddWithValue("@StudentName", textBox2.Text);
                        cmd.Parameters.AddWithValue("@Course", textBox3.Text);
                        cmd.Parameters.AddWithValue("@EnrollDate", dateTimePicker1.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Record Updated Successfully!");
                            LoadEnrollmentData(); // Refresh the DataGridView after saving
                            ResetTextBoxes();  // Clear textboxes after saving
                        }
                        else
                        {
                            MessageBox.Show("Failed to update the record. Please try again.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Add Enrollment data (Insert new record)
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs before adding a new record
                if (!ValidateInputs())
                    return; // Exit if validation fails

                using (SqlConnection con = GetConnection())
                {
                    con.Open();

                    // Prepare the INSERT query to add new data
                    string query = "INSERT INTO Enroll (eid, studentname, course, enrolldate) " +
                                   "VALUES (@Eid, @StudentName, @Course, @EnrollDate)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Add parameters to the query
                        cmd.Parameters.AddWithValue("@Eid", int.Parse(textBox1.Text)); // Enrollment ID
                        cmd.Parameters.AddWithValue("@StudentName", textBox2.Text); // Student Name
                        cmd.Parameters.AddWithValue("@Course", textBox3.Text); // Course Name
                        cmd.Parameters.AddWithValue("@EnrollDate", dateTimePicker1.Value); // Enrollment Date

                        // Execute the query
                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Check if the insert was successful
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("New Enrollment Added Successfully!");
                            LoadEnrollmentData(); // Refresh the DataGridView after adding the new record
                            ResetTextBoxes();  // Clear textboxes after adding
                        }
                        else
                        {
                            MessageBox.Show("Failed to add the new record. Please try again.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Delete Enrollment data
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate Eid before attempting to delete
                if (string.IsNullOrWhiteSpace(textBox1.Text) || !int.TryParse(textBox1.Text, out int eid))
                {
                    MessageBox.Show("Please enter a valid Enrollment ID to delete.");
                    SetTextBoxBorderColor(textBox1, false);  // Set red border if the input is invalid
                    return;
                }
                SetTextBoxBorderColor(textBox1, true);  // Reset border to normal if the input is valid

                // Check if the Enrollment exists before attempting to delete
                if (!EnrollmentExists(eid))
                {
                    MessageBox.Show("Enrollment with this ID doesn't exist. Please check the Enrollment ID.");
                    return;
                }

                using (SqlConnection con = GetConnection())
                {
                    con.Open();
                    // Delete the record based on the provided Enrollment ID (Eid)
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM Enroll WHERE eid=@Eid", con))
                    {
                        cmd.Parameters.Add("@Eid", SqlDbType.Int).Value = eid;
                        cmd.ExecuteNonQuery();  // Execute the deletion query
                    }
                    MessageBox.Show("Record Deleted Successfully!");

                    // Immediately refresh the DataGridView to show the updated data
                    LoadEnrollmentData(); // This will reload the data and update the DataGridView
                    ResetTextBoxes();  // Clear the form fields after deletion
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }



        // Load Enrollment data into DataGridView on form load
        private void Enrollment_Load(object sender, EventArgs e)
        {
            LoadEnrollmentData();
        }

        // Centralized method to load Enrollment data into DataGridView
        private void LoadEnrollmentData()
        {
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

        // Reset textboxes and date picker after each operation
        private void ResetTextBoxes()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            dateTimePicker1.Value = DateTime.Now;  // Set date picker to current date
        }

        // Close the Enrollment form when the label5 is clicked
        private void label6_Click(object sender, EventArgs e)
        {
            this.Close(); // Close only the current form (Enrollment form)
        }
    }
}

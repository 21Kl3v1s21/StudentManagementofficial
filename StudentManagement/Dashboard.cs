using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace StudentManagement
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(@"Data Source=LAPTOP-6RI8HTNK;Initial Catalog=StudentDB;Integrated Security=True;");
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            // Call displayCounts() to load the data when the form loads
            displayCounts();
        }

        private void displayCounts()
        {
            try
            {
                using (SqlConnection con = GetConnection())
                {
                    con.Open();

                    // Query to get the count of students
                    SqlCommand cmdStudent = new SqlCommand("SELECT COUNT(*) FROM Student", con);
                    int studentCount = (int)cmdStudent.ExecuteScalar();  // Execute scalar to get the result

                    // Query to get the count of courses
                    SqlCommand cmdCourse = new SqlCommand("SELECT COUNT(*) FROM Course", con);
                    int courseCount = (int)cmdCourse.ExecuteScalar();

                    // Query to get the count of enrollments
                    SqlCommand cmdEnrollment = new SqlCommand("SELECT COUNT(*) FROM Enroll", con);
                    int enrollmentCount = (int)cmdEnrollment.ExecuteScalar();
                   
                    label7.Text = $" {studentCount}";     
                    label3.Text = $"{courseCount}";       
                    label8.Text = $" {enrollmentCount}"; 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void label9_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}

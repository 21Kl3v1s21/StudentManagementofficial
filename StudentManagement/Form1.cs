using System;
using System.Windows.Forms;

namespace StudentManagement
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Open the Student form
            Student st = new Student();
            st.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Course ce = new Course();
            ce.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Enrollment et = new Enrollment();
            et.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Dashboard dd = new Dashboard();
            dd.Show();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            // Close the entire application when the exit label is clicked
            Application.Exit();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // If Ctrl key is pressed, hide the current Student form if it's open
            if (e.Control && e.KeyCode == Keys.ControlKey)
            {
                foreach (Form form in Application.OpenForms)
                {
                    if (form is Student)
                    {
                        form.Hide();  // Hide the Student form
                        return; // Exit the method
                    }
                }
            }
        }

        private void logoutBtn_Click(object sender, EventArgs e)
        {
            // Show a confirmation message when the user tries to log out
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to log out?", "Log Out Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // If the user clicks Yes, proceed with logging out
            if (dialogResult == DialogResult.Yes)
            {
                this.Hide(); // Hide the current form
                LoginForm loginForm = new LoginForm();
                loginForm.Show(); // Show the login form
            }
        }
    }
}

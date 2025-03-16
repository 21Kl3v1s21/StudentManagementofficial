using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace StudentManagement
{
    public partial class LoginForm : Form
    {
        SqlConnection connect = new SqlConnection(@"Data Source=LAPTOP-6RI8HTNK;Initial Catalog=StudentDB;Integrated Security=True;");

        public LoginForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            password.PasswordChar = '*'; // Mask password field characters
            this.KeyPreview = true; // Ensure form captures key presses
            this.KeyDown += new KeyEventHandler(LoginForm_KeyDown); // Attach KeyDown event

            // Set the AcceptButton property to the login button so Enter triggers it.
            this.AcceptButton = loginBtn;

            // Attach KeyDown events for username and password fields
            username.KeyDown += username_KeyDown;
            password.KeyDown += password_KeyDown;

            // Set the font size and padding for textboxes
            SetTextBoxAppearance(username);
            SetTextBoxAppearance(password);
        }

        // Method to adjust font size and padding of textboxes
        private void SetTextBoxAppearance(TextBox textBox)
        {
            textBox.Font = new System.Drawing.Font("Arial", 12); // Set the font size
            textBox.Padding = new Padding(10); // Add padding inside the textbox
        }

        // Close the application when the label is clicked
        private void label2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Detect when Ctrl key is pressed and close the application
        private void LoginForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if Ctrl key is pressed
            if (e.Control && e.KeyCode == Keys.ControlKey) // Ensure it's Ctrl key
            {
                Application.Exit(); // Exit the application when Ctrl is pressed
            }
        }

        // Toggle password visibility when checkbox is checked/unchecked
        private void showPass_CheckedChanged(object sender, EventArgs e)
        {
            password.PasswordChar = showPass.Checked ? '\0' : '*'; // Show/hide password
        }

        // Handle Login Button click event
        private void loginBtn_Click(object sender, EventArgs e)
        {
            if (username.Text == "" || password.Text == "")
            {
                MessageBox.Show("Please fill in the blanks!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    connect.Open();

                    // Select user data from the database for matching username and password
                    string selectData = "SELECT * FROM users WHERE username = @username AND password=@password";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        cmd.Parameters.AddWithValue("@username", username.Text.Trim());
                        cmd.Parameters.AddWithValue("@password", password.Text.Trim());

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count >= 1)
                        {
                            MessageBox.Show("Login Successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Create an instance of Form1 (your main form)
                            Form1 form1 = new Form1();

                            // Show Form1
                            form1.Show();

                            // Hide the LoginForm
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Wrong Username or Password. Please try again!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to connect to Database" + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        // Move focus from username to password on Shift + Down Arrow key press
        private void username_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if Shift and Down Arrow are pressed
            if (e.KeyCode == Keys.Down && e.Shift)
            {
                password.Focus();  // Move focus to Password field
            }
        }

        // Move focus from password to login button on Shift + Down Arrow key press
        private void password_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if Shift and Down Arrow are pressed
            if (e.KeyCode == Keys.Down && e.Shift)
            {
                loginBtn.Focus();  // Move focus to Login Button
            }

            // Shift + Up Arrow to move focus back to Username field
            if (e.KeyCode == Keys.Up && e.Shift)
            {
                username.Focus();  // Move focus back to Username field
            }
        }

        private void username_TextChanged(object sender, EventArgs e)
        {
            // You can leave this empty or handle text changes here if necessary.
        }
    }
}

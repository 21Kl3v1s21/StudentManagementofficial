using System;
using System.Windows.Forms;

namespace StudentManagement
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            // Make sure the timer is started after form is loaded
            timer1.Start();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            // Ensuring the timer starts when the form is loaded
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Increase the width of panel2 gradually
            panel2.Width += 6;

            // Stop the timer when the panel width exceeds 80
            if (panel2.Width >= 528)
            {
                timer1.Stop();

                LoginForm lForm = new LoginForm();
                lForm.Show();
                this.Hide();
            }
        }
    }
}

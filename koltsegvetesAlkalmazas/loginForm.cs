using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using System.Configuration;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;


namespace koltsegvetesAlkalmazas
{
    public partial class loginForm : Form
    {
        /*string connStr = ConfigurationManager
        .ConnectionStrings["MySqlConnection"]
        .ConnectionString;*/

        private readonly string connStr = ConfigurationManager
            .ConnectionStrings["MySqlConnection"]
            .ConnectionString;

        public loginForm()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            using (var regForm = new registerForm())
            {
                regForm.ShowDialog();
            }
            this.Show();
        }
    }
}

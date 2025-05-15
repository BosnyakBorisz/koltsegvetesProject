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
using Mysqlx.Crud;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace koltsegvetesAlkalmazas
{
    public partial class registerForm : Form
    {
        private readonly string connStr = ConfigurationManager
            .ConnectionStrings["MySqlConnection"]
            .ConnectionString;
        public registerForm()
        {
            InitializeComponent();


        }

        private string ComputeSha256Hash(string raw)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(raw));
                var sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
        }

        private void btnRegister_Click_1(object sender, EventArgs e)
        {

        }

        private void chkShowPassReg2_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void chkShowPassReg1_CheckedChanged_1(object sender, EventArgs e)
        {

        }
    }
}

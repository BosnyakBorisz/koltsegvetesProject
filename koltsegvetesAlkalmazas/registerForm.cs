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


            // 2) Eseménykezelők (ha nem a Designerben kötötted)
            //chkShowPassReg1.CheckedChanged += chkShowPassReg1_CheckedChanged;
            //chkShowPassReg2.CheckedChanged += chkShowPassReg2_CheckedChanged;
            //linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            //btnRegister.Click += btnRegister_Click;
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
            string email = txtEmail.Text.Trim();
            string user = txtRegUser.Text.Trim();
            string p1 = txtRegPass1.Text;
            string p2 = txtRegPass2.Text;

            if (string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(user) ||
                string.IsNullOrEmpty(p1) ||
                string.IsNullOrEmpty(p2))
            {
                MessageBox.Show("Minden mező kitöltése kötelező!",
                                "Hiányzó adat",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }
            if (p1 != p2)
            {
                MessageBox.Show("A két jelszó nem egyezik!",
                                "Hiba",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            string hash = ComputeSha256Hash(p1);

            try
            {
                using (var conn = new MySqlConnection(connStr))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText =
                      "INSERT INTO users (email, username, password_hash) " +
                      "VALUES (@e,@u,@p)";

                    cmd.Parameters.AddWithValue("@e", email);
                    cmd.Parameters.AddWithValue("@u", user);
                    cmd.Parameters.AddWithValue("@p", hash);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Sikeres regisztráció!",
                                    "OK",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    this.Close();
                    //var tranzF = new tranzakciokForm();
                    //tranzF.FormClosed += (s, args) => this.Close();
                    //tranzF.Show();
                    //this.Hide();
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                MessageBox.Show("Ez az email vagy felhasználónév már foglalt.",
                                "Duplikált érték",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba az adatbázis-művelet során:\n" + ex.Message,
                                "Adatbázis hiba",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void chkShowPassReg1_CheckedChanged_1(object sender, EventArgs e)
        {
            bool show = chkShowPassReg1.Checked;
            txtRegPass1.UseSystemPasswordChar = !show;
        }

        private void chkShowPassReg2_CheckedChanged_1(object sender, EventArgs e)
        {
            bool show = chkShowPassReg2.Checked;
            txtRegPass2.UseSystemPasswordChar = !show;
        }
    }
}

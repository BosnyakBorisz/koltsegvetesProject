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
            //chkShowPassword.CheckedChanged += chkShowPassword_CheckedChanged;
            //linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            //btnLogin.Click += btnLogin_Click;

            txtPass.UseSystemPasswordChar = true;
            chkShowPassword.CheckedChanged += chkShowPassword_CheckedChanged;
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
            this.Hide();
            using (var regForm = new registerForm())
            {
                regForm.ShowDialog();
            }
            this.Show();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUser.Text.Trim();
            string pass = txtPass.Text;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Add meg a felhasználónevet és a jelszót!",
                                "Hiányzó adat",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            string hash = ComputeSha256Hash(pass);

            try
            {
                using (var conn = new MySqlConnection(connStr))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText =
                      "SELECT COUNT(*) FROM users WHERE username=@u AND password_hash=@p";
                    cmd.Parameters.AddWithValue("@u", user);
                    cmd.Parameters.AddWithValue("@p", hash);

                    long count = (long)cmd.ExecuteScalar();
                    if (count == 1)
                    {
                        MessageBox.Show("Sikeres bejelentkezés!",
                                        "OK",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);

                        //ha sikeres a bejelntkezes megnyitja a tranzakciokFormot
                        //var tranzF = new tranzakciokForm();
                        //tranzF.FormClosed += (s, args) => this.Close();
                        //tranzF.Show();
                        //this.Hide();
                        cmd.CommandText = "SELECT id FROM users WHERE username=@u AND password_hash=@p";
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            int userId = Convert.ToInt32(result);
                            var tranzF = new tranzakciokForm(userId);
                            tranzF.FormClosed += (s, args) => this.Close();
                            tranzF.Show();
                            this.Hide();
                        }
                        // TODO: itt indítsd a főablakot, pl.:
                        // var main = new MainForm();
                        // main.Show();
                        // this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Hibás felhasználónév vagy jelszó.",
                                        "Hiba",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba az adatbázishoz való csatlakozás során:\n"
                                + ex.Message,
                                "Adatbázis hiba",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }


        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPass.UseSystemPasswordChar = !chkShowPassword.Checked;
        }

   
    }
}

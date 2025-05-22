using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;

namespace koltsegvetesAlkalmazas
{
    public partial class tranzakciokForm : Form
    {
        private readonly int _userId;
        private readonly string _connStr;
        public tranzakciokForm(int userId)
        {
            InitializeComponent();

            // Connection string beolvasása
            _connStr = ConfigurationManager
                .ConnectionStrings["MySqlConnection"]
                .ConnectionString;

            // Gombok stílusai
            foreach (var btn in new[] { button1, button2, button3, button4, btnAdd, btnDelete })
            {
                btn.UseVisualStyleBackColor = false;
                btn.BackColor = Color.FromArgb(0, 192, 192);
                RoundButtonCorners(btn, 20);
            }

            _userId = userId;

            // Események
            //btnAdd.Click += btnAdd_Click;
            btnDelete.Click += btnDelete_Click;

            // ComboBox-ok feltöltése
            comboCurrency.Items.AddRange(new object[] { "HUF", "EUR", "USD" });
            comboCurrency.SelectedIndex = 0;
            comboCategory.Items.AddRange(new object[] {
                "Élelmiszer", "Közlekedés", "Szórakozás", "Rezsi", "Egyéb"
            });
            comboCategory.SelectedIndex = 0;

            // Első betöltés
            LoadTransactions();
            UpdateSummary();
        }

        private void RoundButtonCorners(Button btn, int radius)
        {
            var bounds = new Rectangle(0, 0, btn.Width, btn.Height);
            var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, radius, radius, 180, 90);
            path.AddArc(bounds.Right - radius, bounds.Y, radius, radius, 270, 90);
            path.AddArc(bounds.Right - radius, bounds.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            btn.Region = new Region(path);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string currency = comboCurrency.Text;
            decimal amount = numAmount.Value;
            string type = rbIncome.Checked ? "income" : "expense";
            string category = comboCategory.Text;
            string note = txtNote.Text;

            // INSERT tranzakció
            using (var conn = new MySqlConnection(_connStr))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = @"
                    INSERT INTO transactions
                      (user_id, currency, amount, type, category, note)
                    VALUES
                      (@uid, @c, @a, @t, @cat, @n)";
                cmd.Parameters.AddWithValue("@uid", _userId);
                cmd.Parameters.AddWithValue("@c", currency);
                cmd.Parameters.AddWithValue("@a", amount);
                cmd.Parameters.AddWithValue("@t", type);
                cmd.Parameters.AddWithValue("@cat", category);
                cmd.Parameters.AddWithValue("@n", note);
                cmd.ExecuteNonQuery();
            }

            LoadTransactions();
            UpdateSummary();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //if (listBoxTransactions.SelectedItem == null) return;
            // Ha egyszerű String-et tároltunk, nem yankkoljuk ID-t – 
            // törléshez át kell térni TxItem class-ra, de most skip
            //MessageBox.Show("Törlés még nincs implementálva.");

            comboCurrency.SelectedIndex = -1;

            numAmount.Value = 0;

            rbIncome.Checked = true;
            rbExpense.Checked = false;

            comboCategory.SelectedIndex = -1;

            txtNote.Clear();

            dtpDate.Value = DateTime.Today;

            listBoxTransactions.ClearSelected();
        }

        private void LoadTransactions()
        {
            listBoxTransactions.Items.Clear();

            using (var conn = new MySqlConnection(_connStr))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = @"
                    SELECT type, category, amount
                      FROM transactions
                     WHERE user_id = @uid
                  ORDER BY created_at DESC";
                cmd.Parameters.AddWithValue("@uid", _userId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string type = reader.GetString("type");
                        string category = reader.GetString("category");
                        decimal amount = reader.GetDecimal("amount");
                        // ListBoxba csak category, típus és összeg
                        listBoxTransactions.Items.Add(
                            $"{category,-12} {type,-7} {amount,8:0.00}"
                        );
                    }
                }
            }
        }

        private void UpdateSummary()
        {
            DateTime now = DateTime.Now;
            DateTime start = new DateTime(now.Year, now.Month, 1);
            DateTime end = start.AddMonths(1);

            decimal income = 0m, expense = 0m;
            using (var conn = new MySqlConnection(_connStr))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                // Bevétel összege
                cmd.CommandText = @"
                    SELECT IFNULL(SUM(amount),0)
                      FROM transactions
                     WHERE user_id=@uid
                       AND type='income'
                       AND created_at>=@start
                       AND created_at<@end";
                cmd.Parameters.AddWithValue("@uid", _userId);
                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@end", end);
                income = Convert.ToDecimal(cmd.ExecuteScalar());

                // Kiadás összege
                cmd.CommandText = @"
                    SELECT IFNULL(SUM(amount),0)
                      FROM transactions
                     WHERE user_id=@uid
                       AND type='expense'
                       AND created_at>=@start
                       AND created_at<@end";
                expense = Convert.ToDecimal(cmd.ExecuteScalar());
            }

            lblIncome.Text = income.ToString("0.00");
            lblExpense.Text = expense.ToString("0.00");
            lblBalance.Text = (income - expense).ToString("0.00");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //this.Hide();
            //using (var regForm = new naptarForm())
            //{
            //    regForm.ShowDialog();
            //}
            //this.Close();

            var naptar = new naptarForm();
            naptar.FormClosed += (s, args) => this.Close(); // ha bezárod a naptárt, zárja le ezt is
            naptar.Show();
            this.Hide();
        }

        private void btnSzerkesztes_Click(object sender, EventArgs e)
        {

        }
    }
}

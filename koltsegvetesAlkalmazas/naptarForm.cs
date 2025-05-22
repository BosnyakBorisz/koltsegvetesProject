using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace koltsegvetesAlkalmazas
{
    public partial class naptarForm : Form
    {
        private readonly int _userId;
        private readonly string _connStr;
        public naptarForm()
        {
            InitializeComponent();

            // _userId = userId;
            //button1.Click += button1_Click;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var regForm = new tranzakciokForm(_userId))
            {
                regForm.ShowDialog();
            }
            //this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }


    }
}

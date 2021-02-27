using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ospedale_Covid
{
    public partial class EffettiCollaterali : Form
    {

        Database db;
        int rowIndex = 0;
        public EffettiCollaterali()
        {
            InitializeComponent();
            db = new Database();
        }

        private void EffettiCollaterali_Load(object sender, EventArgs e)
        {
            db.DataSource("EffettiCollaterali", dataGridView1);
            db.caricaInComboBox(dataGridView1, comboBox1);
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Trim() != "")
            {
                string comandosql = string.Format(@"SELECT * FROM {0} WHERE {1} LIKE '{2}' COLLATE NOCASE", "EffettiCollaterali", comboBox1.Text, textBox2.Text);
                db.aggiungi(comandosql, dataGridView1);
            }
            else
            {
                db.DataSource("EffettiCollaterali", dataGridView1);
            }
        }
    }
}

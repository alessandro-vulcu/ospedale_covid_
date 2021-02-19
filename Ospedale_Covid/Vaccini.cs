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
    public partial class Vaccini : Form
    {
        Database db;
        int rowIndex;
        public Vaccini()
        {
            InitializeComponent();
            db = new Database();
        }

        private void Vaccini_Load(object sender, EventArgs e)
        {
            db.DataSource("vacciniCovid", dataGridView1);
            db.DataSource("vaccinazioni", dataGridView2);
        }
    }
}

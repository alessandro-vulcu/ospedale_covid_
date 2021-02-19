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
    public partial class InformazioniStruttura : Form
    {
        Database db;
        string idStruttura;
        int rowIndex;
        public InformazioniStruttura(string idStruttura)
        {
            InitializeComponent();
            db = new Database();
            this.idStruttura = idStruttura;
        }
        private void InformazioniStruttura_Load(object sender, EventArgs e)
        {
            db.DataSourceWhere("orariStrutture", idStruttura, "idStruttura", dataGridView1);
        }
        private void iconButton1_Click(object sender, EventArgs e)
        {
            string giorni = string.Format("{0} - {1}", comboBox1.Text, comboBox2.Text);
            string ora = string.Format("{0} - {1}", textBox2.Text, textBox3.Text);
            string comando = string.Format("INSERT INTO orariStrutture VALUES (\"{0}\", \"{1}\", \"{2}\", \"{3}\")", db.generateID(), idStruttura, giorni, ora);
            db.esegui(comando);
            db.DataSource("orariStrutture", dataGridView1);
        }

        
    }
}

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
    public partial class OrariStudio : Form
    {
        Database db;
        string idPersonale;
        public OrariStudio(string idPersonale, string nome, string cognome)
        {
            InitializeComponent();
            db = new Database();
            this.idPersonale = idPersonale;
            comboBox3.DataSource = db.daColonnaALista("pazienti", "codiceFiscale");
            label5.Text = string.Format("{0} {1}", nome, cognome);
        }

        private void OrariStudio_Load(object sender, EventArgs e)
        {
            db.DataSourceWhere("orariPersonale", idPersonale, "idPersonale", dataGridView1);
            db.DataSourceWhere("mediciPaziente", idPersonale, "idPersonale", dataGridView2);
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            string giorni = string.Format("{0} - {1}", comboBox1.Text, comboBox2.Text);
            string ora = string.Format("{0} - {1}", textBox2.Text, textBox3.Text);
            string comando = string.Format("INSERT INTO orariPersonale VALUES (\"{0}\", \"{1}\", \"{2}\")", idPersonale, giorni, ora);
            db.esegui(comando);
            db.DataSource("orariPersonale", dataGridView1);
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            string comando = string.Format("INSERT INTO mediciPaziente VALUES (\"{0}\", \"{1}\")", idPersonale, comboBox3.Text); ;
            db.esegui(comando);
            db.DataSourceWhere("mediciPaziente", idPersonale, "idPersonale", dataGridView2);
        }
    }
}

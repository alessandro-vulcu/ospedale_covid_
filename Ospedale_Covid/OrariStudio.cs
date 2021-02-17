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
    public partial class InormazioniMedico : Form
    {
        Database db;
        string idPersonale;
        int rowIndex;
        public InormazioniMedico(string idPersonale, string nome, string cognome)
        {
            InitializeComponent();
            db = new Database();
            this.idPersonale = idPersonale;
            comboBox3.DataSource = db.daColonnaALista("pazienti", "codiceFiscale");
            comboBox4.DataSource = db.daColonnaALista("strutture", "idStruttura");
            label5.Text = string.Format("{0} {1}", nome, cognome);
        }

        private void OrariStudio_Load(object sender, EventArgs e)
        {
            db.DataSourceWhere("orariPersonale", idPersonale, "idPersonale", dataGridView1);
            db.DataSourceWhere("mediciPaziente", idPersonale, "idPersonale", dataGridView2);
            writeInLabel();
        }
        public void writeInLabel()
        {
            string comando = string.Format("SELECT idPersonale FROM personaleStrutture WHERE idPersonale = '{0}'", idPersonale);
            
            if(db.getData(comando) != "")
            {
                label7.Text = db.getData(string.Format("SELECT idStruttura FROM personaleStrutture WHERE idPersonale = '{0}'", idPersonale));
            }
            else
            {
                label7.Text = "Nessuna";
            }
        } 

        private void iconButton1_Click(object sender, EventArgs e)
        {
            string giorni = string.Format("{0} - {1}", comboBox1.Text, comboBox2.Text);
            string ora = string.Format("{0} - {1}", textBox2.Text, textBox3.Text);
            string comando = string.Format("INSERT INTO orariPersonale VALUES (\"{0}\", \"{1}\", \"{2}\", \"{3}\")", db.generateID(),idPersonale, giorni, ora);
            db.esegui(comando);
            db.DataSource("orariPersonale", dataGridView1);
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            try
            {
                string comando = string.Format("INSERT INTO mediciPaziente VALUES (\"{0}\", \"{1}\")", idPersonale, comboBox3.Text); ;
                db.esegui(comando);
                db.DataSourceWhere("mediciPaziente", idPersonale, "idPersonale", dataGridView2);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string comando = string.Format("INSERT INTO personaleStrutture VALUES (\"{0}\", \"{1}\")", idPersonale, comboBox4.Text);
                db.esegui(comando);
                writeInLabel();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string comando = string.Format("DELETE FROM personaleStrutture WHERE  idPersonale = \"{0}\"", idPersonale);
            db.esegui(comando);
            writeInLabel();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex != -1)
            {
                this.dataGridView2.Rows[e.RowIndex].Selected = true;
                this.rowIndex = e.RowIndex;
                this.dataGridView2.CurrentCell = this.dataGridView2.Rows[e.RowIndex].Cells[1];
                this.contextMenuStrip1.Show(this.dataGridView2, e.Location);
                contextMenuStrip1.Show(Cursor.Position);
            }
        }

        private void eliminaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.dataGridView2.Rows[this.rowIndex].IsNewRow)
            {
                string comando = string.Format("DELETE FROM mediciPaziente WHERE idPersonale = '{0}' AND idPaziente = '{1}'", dataGridView2.Rows[this.rowIndex].Cells[0].Value.ToString(), dataGridView2.Rows[this.rowIndex].Cells[1].Value.ToString());
                db.esegui(comando);
                db.DataSource("mediciPaziente", dataGridView2);
            }
        }

        private void eliminaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!this.dataGridView1.Rows[this.rowIndex].IsNewRow)
            {
                string comando = string.Format("DELETE FROM orariPersonale WHERE idOrario = '{0}'", dataGridView1.Rows[this.rowIndex].Cells[0].Value.ToString());
                db.esegui(comando);
                db.DataSource("orariPersonale", dataGridView1);
            }
        }

        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex != -1)
            {
                this.dataGridView1.Rows[e.RowIndex].Selected = true;
                this.rowIndex = e.RowIndex;
                this.dataGridView1.CurrentCell = this.dataGridView1.Rows[e.RowIndex].Cells[1];
                this.contextMenuStrip2.Show(this.dataGridView1, e.Location);
                contextMenuStrip2.Show(Cursor.Position);
            }
        }
    }
}

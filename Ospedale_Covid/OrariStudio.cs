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
            label5.Text = string.Format("Medico: {0} {1}", nome, cognome);
            db.DataSource("turni", dataGridView3);
            dateTimePicker1.ShowUpDown = true;
            dateTimePicker2.ShowUpDown = true;
        }

        private void OrariStudio_Load(object sender, EventArgs e)
        {
            db.DataSourceWhere("orariPersonale", idPersonale, "idPersonale", dataGridView1);
            //db.DataSourceWhere("mediciPaziente", idPersonale, "idPersonale", dataGridView2);
            joinTraTabelle();
            writeInLabel();
            bloccaMediciBase(selectTipo());
            controllaSeOperatoreCovid();
        }
        public void joinTraTabelle()
        {
            string comando = string.Format("SELECT nome, cognome, pazienti.codiceFiscale FROM pazienti INNER JOIN mediciPaziente ON pazienti.codiceFiscale = mediciPaziente.idPaziente WHERE idPersonale = '{0}'", idPersonale);
            db.DataSourceComando(comando, dataGridView2);
        }
        public bool selectTipo()
        {
            string comando = string.Format("SELECT tipo FROM personale WHERE idPersonale = '{0}'", idPersonale);
            if (db.getData(comando) == "Medico")
                return true;
            else
                return false;
        }
        public void bloccaMediciBase(bool tipo)
        {
            if (!tipo)
            {
                comboBox3.Enabled = false;
                iconButton2.Enabled = false;
            }
        }
        public void writeInLabel()
        {
            string comando = string.Format("SELECT idPersonale FROM personaleStrutture WHERE idPersonale = '{0}'", idPersonale);
            string a = db.getData(comando);
            if (a != null)
            {
                textBox1.Text = string.Format("Il medico corrente è assegnato alla struttura \n" + db.getData(string.Format("SELECT idStruttura FROM personaleStrutture WHERE idPersonale = '{0}'", idPersonale)));
            }
            else
            {
                textBox1.Text = "Nessuna struttura assegnata";
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
                joinTraTabelle();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            

            if (controllaSovrapposizioneTurni())
            {
                
                try
                {
                    string comando = string.Format("INSERT INTO turni VALUES (\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\")", db.generateID(), db.getData(string.Format("SELECT idoperatoreCovid FROM operatoreCovid WHERE idPersonale = '{0}'", idPersonale)), dateTimePicker3.Text, dateTimePicker1.Text, dateTimePicker2.Text);
                    db.esegui(comando);
                    db.DataSource("turni", dataGridView3);
                    writeInLabel();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            
        }

        public bool controllaOreTurni()
        {
            DateTime dateTime1 = Convert.ToDateTime(string.Format("{0} {1}", dateTimePicker3.Text, dateTimePicker1.Text));
            DateTime dateTime2 = Convert.ToDateTime(string.Format("{0} {1}", dateTimePicker3.Text, dateTimePicker2.Text));

            string comando1 = string.Format("SELECT giornoInizio FROM orariStrutture WHERE idStruttura = '{0}'", comboBox4.Text);
            string comando2 = string.Format("SELECT giornoFine FROM orariStrutture WHERE idStruttura = '{0}'", comboBox4.Text);

            string giornoInizio = db.getData(comando1);
            string giornoFine = db.getData(comando2);


            return false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string comando = string.Format("DELETE FROM turni WHERE id = \"{0}\"", idPersonale);
            db.esegui(comando);
            db.DataSource("turni", dataGridView3);
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
                string comando = string.Format("DELETE FROM mediciPaziente WHERE idPersonale = '{0}' AND idPaziente = '{1}'", idPersonale, dataGridView2.Rows[this.rowIndex].Cells[2].Value.ToString());
                db.esegui(comando);
                joinTraTabelle();
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

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }
        public void controllaSeOperatoreCovid()
        {
            string comando = string.Format("SELECT idPersonale FROM operatoreCovid WHERE idPersonale = '{0}'", idPersonale);
            string id = db.getData(comando);
            if(id != null)
            {
                checkBox1.Checked = true;
                comboBox4.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                groupBox1.Enabled = false;
                groupBox2.Enabled = false;
            }
            else
            {
                checkBox1.Checked = false;
                comboBox4.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = false;
                groupBox1.Enabled = true;
                groupBox2.Enabled = true;
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
            {
                try
                {
                    string comando = string.Format("INSERT INTO operatoreCovid VALUES('{0}', '{1}')", db.generateID(), idPersonale);
                    db.esegui(comando);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
            }
            else
            {
                try
                {
                    string comando = string.Format("DELETE FROM operatoreCovid WHERE idPersonale = '{0}'", idPersonale);
                    db.esegui(comando);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            controllaSeOperatoreCovid();
        }

        private void dataGridView3_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex != -1)
            {
                this.dataGridView3.Rows[e.RowIndex].Selected = true;
                this.rowIndex = e.RowIndex;
                this.dataGridView3.CurrentCell = this.dataGridView3.Rows[e.RowIndex].Cells[1];
                this.contextMenuStrip3.Show(this.dataGridView3, e.Location);
                contextMenuStrip3.Show(Cursor.Position);
            }
        }

        private void eliminaToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (!this.dataGridView3.Rows[this.rowIndex].IsNewRow)
            {
                string comando = string.Format("DELETE FROM turni WHERE idTurno = '{0}'", dataGridView3.Rows[this.rowIndex].Cells[0].Value.ToString());
                db.esegui(comando);
                db.DataSource("turni", dataGridView3);
            }
        }
        public bool controllaSovrapposizioneTurni()
        {
            if(db.getData(string.Format("SELECT dataTurno FROM turni WHERE idOperatoreCovid = '{0}' AND dataTurno = '{1}'", db.getData(string.Format("SELECT idoperatoreCovid FROM operatoreCovid WHERE idPersonale = '{0}'", idPersonale)), dateTimePicker3.Text)) != null)
            {
                MessageBox.Show("ERRORE: questo operatore ha già un turno assegnato per questa data");
                return false;
            }
            return true;
        }
        
    }
}

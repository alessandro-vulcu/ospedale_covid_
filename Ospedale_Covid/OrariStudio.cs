using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
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
        string currentPK;
        string idoperatoreCovid;
        
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
            db.DataSource("ore", dataGridView4);
            dateTimePicker1.ShowUpDown = true;
            dateTimePicker2.ShowUpDown = true;
            qualeStudio();

            button1.Enabled = true;
            button2.Enabled = true;
        }

        public void qualeStudio()
        {
            string comando = string.Format("SELECT idStudio FROM studioPersonale WHERE idPersonale = '{0}'", idPersonale);
            if(comando != null)
            {
                textBox4.Text = "Assegnato a studio " + db.getData(comando);
            }
            else
            {
                textBox4.Text = "Nessuno studio medico assegnato";
            }
        }

        private void OrariStudio_Load(object sender, EventArgs e)
        {
            db.DataSourceWhere("orariPersonale", idPersonale, "idPersonale", dataGridView1);
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

        public void aggiungiOre()
        {
            DateTime dm1 = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, dateTimePicker1.Value.Hour, dateTimePicker1.Value.Minute, dateTimePicker1.Value.Second);
            DateTime dm2 = new DateTime(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month, dateTimePicker2.Value.Day, dateTimePicker2.Value.Hour, dateTimePicker2.Value.Minute, dateTimePicker2.Value.Second);

            TimeSpan diff = dm2.Subtract(dm1);

            string addOre = string.Format("INSERT INTO ore VALUES('{0}', '{1}','{2}','{3}')", db.generateID(), db.getData(string.Format("SELECT idoperatoreCovid FROM operatoreCovid WHERE idPersonale = '{0}'", idPersonale)), dateTimePicker3.Text, diff.TotalHours);
            db.esegui(addOre);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            

            if (controllaSovrapposizioneTurni())
            {
                
                try
                {
                    aggiungiOre();
                    string comando = string.Format("INSERT INTO turni VALUES (\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\")", db.generateID(), db.getData(string.Format("SELECT idoperatoreCovid FROM operatoreCovid WHERE idPersonale = '{0}'", idPersonale)), comboBox4.Text, dateTimePicker3.Text, dateTimePicker1.Text, dateTimePicker2.Text);
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
                radioButton2.Checked = true;
                comboBox4.Enabled = true;

                groupBox1.Enabled = false;
                groupBox2.Enabled = false;
                groupBox3.Enabled = true;
                groupBox4.Enabled = true;
            }
            else
            {
                radioButton1.Checked = true;
                comboBox4.Enabled = false;
                
                groupBox1.Enabled = true;
                groupBox2.Enabled = true;
                groupBox3.Enabled = false;
                groupBox4.Enabled = false;
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
            
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

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView4_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            currentPK = Convert.ToString(db.getData(string.Format(@"SELECT idOre FROM ore WHERE idOre = '{0}'", dataGridView4.SelectedRows[0].Cells[0].Value.ToString())));
            idoperatoreCovid = Convert.ToString(db.getData(string.Format(@"SELECT idoperatoreCovid FROM ore WHERE idOre = '{0}'", dataGridView4.SelectedRows[0].Cells[0].Value.ToString())));
            dateTimePicker4.Value = DateTime.ParseExact(db.getData(string.Format(@"SELECT giorno FROM ore WHERE idOre = '{0}'", dataGridView4.SelectedRows[0].Cells[0].Value.ToString())), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            numericUpDown1.Value = db.getDataInt(string.Format(@"SELECT ora FROM ore WHERE idOre = '{0}'", dataGridView4.SelectedRows[0].Cells[0].Value.ToString()));
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            //try
            //{
            //    string comando1 = string.Format("UPDATE ore SET nome = \"{0}\", cognome = \"{1}\", luogoNascita = \"{2}\", datanascita = \"{3}\", telefono = \"{4}\", email = \"{5}\" WHERE codiceFiscale = \"{6}\"", txtNome.Text, txtCognome.Text, txtNascita.Text, txtData.Text, txtTelefono.Text, txtEmail.Text, txtCF.Text);
            //    db.esegui(comando1);
            //    foreach (Control txt in groupBox4.Controls.Cast<Control>().OrderBy(c => c.TabIndex))
            //    {
            //        if (txt is TextBox)
            //        {
            //            txt.Text = "";
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            //db.DataSource("pazienti", dataGridView1);
            //button2.Enabled = false;
            //button1.Enabled = true;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

            if (radioButton2.Checked == true)
            {
                try
                {
                    string comando = string.Format("INSERT INTO operatoreCovid VALUES('{0}', '{1}')", db.generateID(), idPersonale);
                    db.esegui(comando);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            controllaSeOperatoreCovid();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
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
        public bool controllaMedicoDiBase()
        {
            if(db.getDataInt(string.Format("SELECT COUNT(idPaziente) FROM mediciPaziente WHERE idPersonale = '{0}'", idPersonale)) != 0)
            {
                MessageBox.Show("Il medico non può lavorare in una struttura perchè già impegnato come medico di base");
                return false;
            }
            return true;
        }
    }
}

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
    public partial class Prenotazioni : Form
    {
        Database db;
        
        int rowIndex;
        string currentPK;
        public Prenotazioni()
        {
            InitializeComponent();
            db = new Database();
        }

        private void Prenotazioni_Load(object sender, EventArgs e)
        {
            dateTimePicker2.ShowUpDown = true;
            db.DataSource("Prenotazioni", dataGridView1);
            db.caricaInComboBox(dataGridView1, comboBox1);
            dateTimePicker2.CustomFormat = "hh:mm";

            comboBox2.DataSource = db.daColonnaALista("pazienti", "codiceFiscale");
            comboBox3.DataSource = db.daColonnaALista("strutture", "idStruttura");
        }
        public string oraFine()
        {
            string oraFine = "";
            if (radioButton1.Checked)
            {
                oraFine = dateTimePicker2.Value.AddHours(1).ToString("HH:mm");
            }
            else if(radioButton2.Checked)
            {
                oraFine = dateTimePicker2.Value.AddHours(2).ToString("HH:mm");
            }
            return oraFine;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (controllaSomministrazioniMassime())
            {
                try
                {
                    string comando = string.Format("INSERT INTO Prenotazioni VALUES(\"{0}\",\"{1}\",\"{2}\",\"{3}\", \"{4}\", \"{5}\")", db.generateID(), comboBox2.Text, comboBox3.Text, dateTimePicker1.Text, dateTimePicker2.Value.ToString("HH:mm"), oraFine());
                    db.esegui(comando);
                    db.DataSource("Prenotazioni", dataGridView1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public bool controllaSomministrazioniMassime()
        {
            string comando = string.Format("SELECT COUNT(idStruttura) AS CountIdStruttura FROM Prenotazioni WHERE idStruttura = '{0}'", comboBox3.Text);
            int n = db.getDataInt(comando);
            
            if(n < db.getDataInt(string.Format("SELECT quantitaSomministrazioni FROM strutture WHERE idStruttura = '{0}'", comboBox3.Text)))
            {
                return true;
            }
            else
            {
                MessageBox.Show("La struttura è satura, selezionarne un'altra");
                return false;
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Trim() != "")
            {
                string comandosql = string.Format(@"SELECT * FROM {0} WHERE {1} LIKE '{2}' COLLATE NOCASE", "Prenotazioni", comboBox1.Text, textBox2.Text);
                db.aggiungi(comandosql, dataGridView1);
            }
            else
            {
                db.DataSource("Prenotazioni", dataGridView1);
            }
        }

        private void confermaVaccinazioneToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void confermaVaccinazioneToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Conferma_Vaccino cv = new Conferma_Vaccino(dataGridView1.Rows[this.rowIndex].Cells[1].Value.ToString(), dataGridView1.Rows[this.rowIndex].Cells[2].Value.ToString(), dataGridView1.Rows[this.rowIndex].Cells[0].Value.ToString(), dataGridView1);
            cv.ShowDialog();
        }

        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex != -1)
            {
                this.dataGridView1.Rows[e.RowIndex].Selected = true;
                this.rowIndex = e.RowIndex;
                this.dataGridView1.CurrentCell = this.dataGridView1.Rows[e.RowIndex].Cells[1];
                this.contextMenuStrip1.Show(this.dataGridView1, e.Location);
                contextMenuStrip1.Show(Cursor.Position);
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void eliminaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.dataGridView1.Rows[this.rowIndex].IsNewRow)
            {
                db.esegui(string.Format("DELETE FROM Prenotazioni WHERE idPrenotazione = '{0}'", dataGridView1.Rows[this.rowIndex].Cells[0].Value.ToString()));
                db.DataSource("Prenotazioni", dataGridView1);
            }
        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = true;

            currentPK = Convert.ToString(db.getData(string.Format(@"SELECT idPrenotazione FROM Prenotazioni WHERE idPrenotazione = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())));
            comboBox2.Text = Convert.ToString(db.getData(string.Format(@"SELECT idPaziente FROM Prenotazioni WHERE idPrenotazione = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())));
            comboBox3.Text = Convert.ToString(db.getData(string.Format(@"SELECT idStruttura FROM Prenotazioni WHERE idPrenotazione = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())));
            dateTimePicker1.Value = DateTime.ParseExact(db.getData(string.Format(@"SELECT giorno FROM Prenotazioni WHERE idPrenotazione = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            dateTimePicker2.Value = DateTime.ParseExact(db.getData(string.Format(@"SELECT ora FROM Prenotazioni WHERE idPrenotazione = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())), "HH:mm", CultureInfo.InvariantCulture);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!db.CheckTextBox(panel1))
            {
                try
                {
                    string comando1 = string.Format("UPDATE Prenotazioni SET idPaziente = \"{0}\", idStruttura = \"{1}\", giorno = \"{2}\", ora = \"{3}\", oraFine = \"{4}\"", comboBox2.Text, comboBox3.Text, dateTimePicker1.Text, dateTimePicker2.Value.ToString("HH:mm"), oraFine());
                    db.esegui(comando1);
                    foreach (Control txt in panel1.Controls.Cast<Control>().OrderBy(c => c.TabIndex))
                    {
                        if (txt is TextBox)
                        {
                            txt.Text = "";
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            db.DataSource("Prenotazioni", dataGridView1);
            button2.Enabled = false;
            button1.Enabled = true;
        }
    }
}

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
    public partial class Vaccini : Form
    {
        Database db;
        int rowIndex;
        string currentPK;
        public Vaccini()
        {
            InitializeComponent();
            db = new Database();
            button3.Enabled = false;
            button4.Enabled = false;
        }

        private void Vaccini_Load(object sender, EventArgs e)
        {
            db.DataSource("vacciniCovid", dataGridView1);
            db.DataSource("vaccinazioni", dataGridView2);
            //comboStruttura.DataSource = db.daColonnaALista("strutture", "idStruttura");
            db.caricaInComboBox(dataGridView1, comboBox2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!db.CheckTextBox(panel1))
            {
                try
                {
                    string comando1 = string.Format("INSERT INTO vacciniCovid VALUES(\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\", \"{6}\")", db.generateID(), txtCasaFarmaceutica.Text,  txtNome.Text, txtLotto.Text, dataProduzione.Text, dataScadenza.Text, txtBugiardino.Text);
                    db.esegui(comando1);
                    foreach (Control txt in panel1.Controls.Cast<Control>().OrderBy(c => c.TabIndex))
                        if (txt is TextBox)
                        {
                            txt.Text = "";
                        }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            db.DataSource("vacciniCovid", dataGridView1);
        }

        private void txtLotto_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            if (textBox6.Text.Trim() != "")
            {
                string comandosql = string.Format(@"SELECT * FROM {0} WHERE {1} LIKE '{2}' COLLATE NOCASE", "vacciniCovid", comboBox2.Text, textBox6.Text);
                db.aggiungi(comandosql, dataGridView1);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!db.CheckTextBox(panel3))
            {
                try
                {
                    string comando1 = string.Format("INSERT INTO vaccinazioni VALUES(\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\", \"{6}\")", db.generateID(), txtMalattia.Text, txtTipo.Text, txtCasaFarma.Text, dataProduzione.Text, dataScadenza.Text, txtLotto.Text);
                    db.esegui(comando1);
                    foreach (Control txt in panel3.Controls.Cast<Control>().OrderBy(c => c.TabIndex))
                        if (txt is TextBox)
                        {
                            txt.Text = "";
                        }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            db.DataSource("vaccinazioni", dataGridView2);
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

        private void eliminaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.dataGridView1.Rows[this.rowIndex].IsNewRow)
            {
                db.esegui(string.Format("DELETE FROM vacciniCovid WHERE idVaccinoCovid = '{0}'", dataGridView1.Rows[this.rowIndex].Cells[0].Value.ToString()));
                db.DataSource("vacciniCovid", dataGridView1);
                button1.Enabled = true;
                button3.Enabled = false;
            }
        }

        private void eliminaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!this.dataGridView2.Rows[this.rowIndex].IsNewRow)
            {
                db.esegui(string.Format("DELETE FROM vaccinazioni WHERE idVaccino = '{0}'", dataGridView2.Rows[this.rowIndex].Cells[0].Value.ToString()));
                db.DataSource("vaccinazioni", dataGridView2);
                button2.Enabled = true;
                button4.Enabled = false;
            }
        }

        private void dataGridView2_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex != -1)
            {
                this.dataGridView2.Rows[e.RowIndex].Selected = true;
                this.rowIndex = e.RowIndex;
                this.dataGridView2.CurrentCell = this.dataGridView2.Rows[e.RowIndex].Cells[1];
                this.contextMenuStrip2.Show(this.dataGridView2, e.Location);
                contextMenuStrip2.Show(Cursor.Position);
            }
        }

        private void dataGridView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            button2.Enabled = false;
            button4.Enabled = true;

            currentPK = Convert.ToString(db.getData(string.Format(@"SELECT idVaccino FROM vaccinazioni WHERE idVaccino = '{0}'", dataGridView2.SelectedRows[0].Cells[0].Value.ToString())));
            txtTipo.Text = Convert.ToString(db.getData(string.Format(@"SELECT tipo FROM vaccinazioni WHERE idVaccino = '{0}'", dataGridView2.SelectedRows[0].Cells[0].Value.ToString())));
            txtMalattia.Text = Convert.ToString(db.getData(string.Format(@"SELECT malattiaCurata FROM vaccinazioni WHERE idVaccino = '{0}'", dataGridView2.SelectedRows[0].Cells[0].Value.ToString())));
            txtCasaFarma.Text = Convert.ToString(db.getData(string.Format(@"SELECT casaFarmaceutica FROM vaccinazioni WHERE idVaccino = '{0}'", dataGridView2.SelectedRows[0].Cells[0].Value.ToString())));
            dataScadenza2.Value = DateTime.ParseExact(db.getData(string.Format(@"SELECT dataScadenza FROM vaccinazioni WHERE idVaccino = '{0}'", dataGridView2.SelectedRows[0].Cells[0].Value.ToString())), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            dataProduzione2.Value = DateTime.ParseExact(db.getData(string.Format(@"SELECT dataProduzione FROM vaccinazioni WHERE idVaccino = '{0}'", dataGridView2.SelectedRows[0].Cells[0].Value.ToString())), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            txtLotto2.Text = db.getData(string.Format(@"SELECT lotto FROM vaccinazioni WHERE idVaccino = '{0}'", dataGridView2.SelectedRows[0].Cells[0].Value.ToString()));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!db.CheckTextBox(panel3))
            {
                try
                {
                    string comando1 = string.Format("UPDATE vaccinazioni SET idVaccino = \"{0}\", malattiaCurata = \"{1}\", tipo = \"{2}\", casaFarmaceutica = \"{3}\", dataScadenza = \"{4}\", dataProduzione = \"{5}\", lotto = \"{6}\" WHERE idVaccino = \"{7}\"",currentPK, txtMalattia.Text, txtTipo.Text, txtCasaFarma.Text, dataScadenza2.Text, dataProduzione2.Text, txtLotto2.Text, currentPK);
                    db.esegui(comando1);
                    foreach (Control txt in panel3.Controls.Cast<Control>().OrderBy(c => c.TabIndex))
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
                db.DataSource("vaccinazioni", dataGridView2);
                button2.Enabled = true;
                button4.Enabled = false;
                currentPK = "";
            }
        }
            

        private void button3_Click(object sender, EventArgs e)
        {
            if (!db.CheckTextBox(panel1))
            {
                try
                {
                    string comando1 = string.Format("UPDATE vacciniCovid SET nomeVaccino = \"{0}\", casaFarmaceutica = \"{1}\", lotto = \"{2}\", dataProduzione = \"{3}\", dataScadenza = \"{4}\", bugiardino = \"{5}\" WHERE idVaccinoCovid = \"{6}\"", txtNome.Text, txtCasaFarmaceutica.Text, txtLotto.Text, dataProduzione.Text, dataScadenza.Text,txtBugiardino.Text, currentPK);
                    db.esegui(comando1);
                    foreach (Control txt in panel1.Controls.Cast<Control>().OrderBy(c => c.TabIndex))
                    {
                        if (txt is TextBox || txt is ComboBox)
                        {
                            txt.Text = "";
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                db.DataSource("vacciniCovid", dataGridView1);
                button1.Enabled = true;
                button3.Enabled = false;
                currentPK = "";
            }
        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            button1.Enabled = false;
            button3.Enabled = true;

            currentPK = Convert.ToString(db.getData(string.Format(@"SELECT idVaccinoCovid FROM vacciniCovid WHERE idVaccinoCovid = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())));
            txtNome.Text = Convert.ToString(db.getData(string.Format(@"SELECT nomeVaccino FROM vacciniCovid WHERE idVaccinoCovid = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())));
            txtCasaFarmaceutica.Text = Convert.ToString(db.getData(string.Format(@"SELECT casaFarmaceutica FROM vacciniCovid WHERE idVaccinoCovid = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())));
            txtLotto.Text = db.getData(string.Format(@"SELECT lotto FROM vacciniCovid WHERE idVaccinoCovid = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString()));
            dataScadenza.Value = DateTime.ParseExact(db.getData(string.Format(@"SELECT dataScadenza FROM vacciniCovid WHERE idVaccinoCovid = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            dataProduzione.Value = DateTime.ParseExact(db.getData(string.Format(@"SELECT dataProduzione FROM vacciniCovid WHERE idVaccinoCovid = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            txtBugiardino.Text = Convert.ToString(db.getData(string.Format(@"SELECT bugiardino FROM vacciniCovid WHERE idVaccinoCovid = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())));

        }

        private void inviaAStrutturaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Spedizione sp = new Spedizione(dataGridView1.Rows[this.rowIndex].Cells[0].Value.ToString());
            sp.ShowDialog();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

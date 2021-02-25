using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using Ospedale_Covid.Gestionale;

namespace Ospedale_Covid
{
    public partial class Accertamento_telefonico : Form
    {
        Database db;
        int rowIndex;
        string currentPK;
        public Accertamento_telefonico()
        {
            InitializeComponent();
            db = new Database();
            button1.Enabled = true;
            button2.Enabled = false;
        }

        private void Accertamento_telefonico_Load(object sender, EventArgs e)
        {

            comboBox2.DataSource = db.daColonnaALista("pazientiVaccinati", "DISTINCT idPaziente");
            comboBox3.DataSource = db.daColonnaALista("turni", "DISTINCT idoperatoreCovid");

            comboBox4.DataSource = db.daColonnaALista("EffettiCollaterali", "effetto_collaterale");

            db.fillCombo("SELECT * FROM vacciniCovid", comboBox5, "casaFarmaceutica", "idVaccinoCovid");

            db.DataSource("accertamentoTelefonico", dataGridView1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ComboboxItem c = (ComboboxItem)comboBox5.SelectedItem;
            string idv = c.Value.ToString();

            string comando = string.Format("INSERT INTO accertamentoTelefonico VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", db.generateID(), comboBox2.Text, comboBox3.Text, idv, dateTimePicker1.Text, dateTimePicker2.Value.ToString("HH:mm"), comboBox4.Text);
            db.esegui(comando);
            controllaSintomoEsistente(idv);
            db.DataSource("accertamentoTelefonico", dataGridView1);
        }
        public void controllaSintomoEsistente(string idv)
        {
            
            int n = Convert.ToInt32(db.getDataInt(string.Format("SELECT COUNT(effetto_collaterale) FROM EffettiCollaterali WHERE effetto_collaterale = '{0}' AND idVaccinoCovid = '{1}'", comboBox4.Text, idv)));
            if (n == 0)
            {
                string inserisciNuovoEffetto = string.Format("INSERT INTO EffettiCollaterali VALUES('{0}','{1}','{2}','{3}','{4}')", db.generateID(), idv, dateTimePicker1.Text, comboBox4.Text, 1);
                db.esegui(inserisciNuovoEffetto);
                comboBox4.DataSource = db.daColonnaALista("EffettiCollaterali", "effetto_collaterale");
            }
            else
            {
                string aggiornaQuantità = string.Format("UPDATE EffettiCollaterali SET quanti = (quanti + 1) WHERE effetto_collaterale = '{0}' AND idVaccinoCovid = '{1}'", comboBox4.Text, idv);
                db.esegui(aggiornaQuantità);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!db.CheckTextBox(panel1))
            {
                try
                {
                    string comando1 = string.Format("UPDATE accertamentoTelefonico SET idAccertamento = \"{0}\", idPaziente = \"{1}\", idOperatore = \"{2}\", idVaccinoCovid = \"{3}\", dataAccertamento = \"{4}\", oraAccertamento = \"{5}\", sintomi = \"{6}\" WHERE idAccertamento = \"{9}\"", currentPK, comboBox2.Text, comboBox3.Text, comboBox5.Text, dateTimePicker1.Value, dateTimePicker2.Value.ToString("HH:mm"), comboBox4.Text, currentPK);
                    db.esegui(comando1);
                    foreach (Control txt in panel1.Controls.Cast<Control>().OrderBy(c => c.TabIndex))
                    {
                        if (txt is TextBox)
                        {
                            txt.Text = "";
                        }
                        if (txt is NumericUpDown)
                        {
                            txt.Text = "0";
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                db.DataSource("strutture", dataGridView1);
                button1.Enabled = true;
                button2.Enabled = false;
                currentPK = "";
            }
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

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = true;

            currentPK = Convert.ToString(db.getData(string.Format(@"SELECT idAccertamento FROM accertamentoTelefonico WHERE idAccertamento = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())));
            comboBox2.Text = Convert.ToString(db.getData(string.Format(@"SELECT idPaziente FROM accertamentoTelefonico WHERE idAccertamento = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())));
            comboBox3.Text = Convert.ToString(db.getData(string.Format(@"SELECT idOperatore FROM accertamentoTelefonico WHERE idAccertamento = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())));
            comboBox5.Text = Convert.ToString(db.getData(string.Format(@"SELECT idVaccinoCovid FROM accertamentoTelefonico WHERE idAccertamento = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())));
            dateTimePicker1.Text = Convert.ToString(db.getData(string.Format(@"SELECT dataAccertamento FROM accertamentoTelefonico WHERE idAccertamento = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())));
            dateTimePicker2.Text = Convert.ToString(db.getData(string.Format(@"SELECT oraAccertamento FROM accertamentoTelefonico WHERE idAccertamento = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())));
            comboBox4.Text = Convert.ToString(db.getData(string.Format(@"SELECT sintomi FROM accertamentoTelefonico WHERE idAccertamento = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())));

        }

        private void eliminaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.dataGridView1.Rows[this.rowIndex].IsNewRow)
            {
                db.esegui(string.Format("DELETE FROM accertamentoTelefonico WHERE idAccertamento = '{0}'", dataGridView1.Rows[this.rowIndex].Cells[0].Value.ToString()));
                db.DataSource("accertamentoTelefonico", dataGridView1);
                button1.Enabled = true;
                button2.Enabled = false;
            }
            
        }
    }
}

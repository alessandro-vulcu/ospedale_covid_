using Ospedale_Covid.Gestionale;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ospedale_Covid
{
    public partial class Informazioni_pazienti : Form
    {
        string idPaziente;
        Database db = new Database();
        int rowIndex;
        public Informazioni_pazienti(string idPaziente)
        {
            InitializeComponent();
            this.idPaziente = idPaziente;
        }

        private void Informazioni_pazienti_Load(object sender, EventArgs e)
        {
            selectResidenza();
            scriviInLabel();
            fillComboVaccini();
            db.DataSourceComando(string.Format(@"SELECT * FROM pazientiVaccinazioni WHERE idPaziente = '{0}'", idPaziente), dataGridView1);
            
        }
        public void scriviInLabel()
        {
            string medicoAssegnato = db.getData(string.Format("SELECT idPersonale FROM mediciPaziente WHERE idPaziente = '{0}'", idPaziente));
            if (medicoAssegnato != null)
                label10.Text = string.Format("Paziente {0} assegnato al medico {1}", idPaziente, medicoAssegnato);
            else
                label10.Text = string.Format("Paziente {0} non assegnato a un medico", idPaziente);
        }
        public void selectResidenza()
        {
            string comando = string.Format(@"SELECT via,cap,provincia,regione,stato FROM residenza WHERE idPaziente = '{0}'", idPaziente);
            object[] residenza = db.getRiga(comando);

            textBox1.Text = residenza[0].ToString();
            textBox2.Text = residenza[1].ToString();
            textBox3.Text = residenza[2].ToString();
            textBox4.Text = residenza[3].ToString();
            textBox5.Text = residenza[4].ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ComboboxItem c = (ComboboxItem)comboBox1.SelectedItem;
                string idv = c.Value.ToString();

                string comando = string.Format("INSERT INTO pazientiVaccinazioni VALUES('{0}', '{1}', '{2}', '{3}', '{4}')", idPaziente, idv, dateTimePicker1.Text, dateTimePicker2.Text, textBox8.Text);
                db.esegui(comando);
                db.DataSource("pazientiVaccinazioni", dataGridView1);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void eliminaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.dataGridView1.Rows[this.rowIndex].IsNewRow)
            {
                db.esegui(string.Format("DELETE FROM pazientiVaccinazioni WHERE idPaziente = '{0}' AND idVaccino = '{1}'", idPaziente, dataGridView1.Rows[this.rowIndex].Cells[1].Value.ToString()));
                db.DataSource("pazientiVaccinazioni", dataGridView1);
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
        private void fillComboVaccini()
        {
            string cmd = "SELECT * FROM vaccinazioni";
            comboBox1.DisplayMember = "Text";
            comboBox1.ValueMember = "Value";
            using (SQLiteConnection connessione = new SQLiteConnection(@"Data Source=ospedale_covidDB.db; foreign keys=True"))
            {
                connessione.Open();
                using (SQLiteCommand comando = new SQLiteCommand(cmd, connessione))
                {
                    SQLiteDataReader dr = comando.ExecuteReader();
                    while (dr.Read())
                    {
                        comboBox1.Items.Add(new ComboboxItem(dr["tipo"].ToString(), dr["idVaccino"].ToString()));
                    }
                    dr.Close();
                }
                connessione.Close();
            }
        }
    }
}

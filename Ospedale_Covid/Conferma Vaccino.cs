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
    public partial class Conferma_Vaccino : Form
    {
        string idPaziente;
        string idStruttura;
        string idPrenotazione;
        Database db;
        DataGridView dgw;
        public Conferma_Vaccino(string idPaziente, string idStruttura, string idPrenotazione, DataGridView dgw)
        {
            InitializeComponent();
            db = new Database();
            fillCombo("SELECT * FROM vacciniCovid", comboBox1, "casaFarmaceutica", "idVaccinoCovid");
            comboBox2.DataSource = db.daColonnaALista("operatoreCovid", "idoperatoreCovid");
            this.idPaziente = idPaziente;
            this.idStruttura = idStruttura;
            this.idPrenotazione = idPrenotazione;
            this.dgw = dgw;

            sbloccaDateTime();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (toglidosevaccino())
            {
                try
                {
                    if(checkBox1.Checked == true)
                    {
                        string comando1 = string.Format("INSERT INTO Prenotazioni VALUES('{0}', '{1}','{2}','{3}','{4}','{5}')", db.generateID(), idPaziente, idStruttura, dateTimePicker2.Text, dateTimePicker3.Value.ToString("HH:mm"), dateTimePicker4.Value.ToString("HH:mm"));
                        db.esegui(comando1);
                    }
                    ComboboxItem c = (ComboboxItem)comboBox1.SelectedItem;
                    string idv = c.Value.ToString();

                    string comando = string.Format("INSERT INTO pazientiVaccinati VALUES('{0}', '{1}', '{2}', '{3}', '{4}')", comboBox1.Text, idPaziente, dateTimePicker1.Value.ToString(), idStruttura, comboBox2.Text);
                    db.esegui(comando);
                    toglidosevaccino();
                    eliminaPrenotazione();
                    MessageBox.Show("Operazione completata");
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            
        }

        public bool toglidosevaccino()
        {
            if(db.getDataInt(string.Format("SELECT quantitaVaccini FROM strutture WHERE idStruttura = '{0}'", idStruttura)) != 0)
            {
                string comando = string.Format("UPDATE strutture SET quantitaVaccini = (quantitaVaccini - 1) WHERE idStruttura = '{0}'", idStruttura);
                db.esegui(comando);
                return true;
            }
            else
            {
                MessageBox.Show(string.Format("Mancano vaccini alla sede {0}", idStruttura));
                return false;
            }
            
        }
        public void eliminaPrenotazione()
        {
            string comando = string.Format("DELETE FROM Prenotazioni WHERE idPrenotazione = '{0}'", idPrenotazione);
            db.esegui(comando);
        }

        public void fillCombo(string comandosql, ComboBox qualecombo, string nomeDaVisualizzare, string nomeID)
        {
            
            comboBox1.DisplayMember = "Text";
            comboBox1.ValueMember = "Value";
            using (SQLiteConnection connessione = new SQLiteConnection(@"Data Source=ospedale_covidDB.db; foreign keys=True"))
            {
                connessione.Open();
                using (SQLiteCommand comando = new SQLiteCommand(comandosql, connessione))
                {
                    SQLiteDataReader dr = comando.ExecuteReader();
                    while (dr.Read())
                    {
                        qualecombo.Items.Add(new ComboboxItem(dr[nomeDaVisualizzare].ToString(), dr[nomeID].ToString()));
                    }
                    dr.Close();
                }
                connessione.Close();
            }
        }

        private void Conferma_Vaccino_FormClosing(object sender, FormClosingEventArgs e)
        {
            db.DataSource("Prenotazioni", dgw);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            sbloccaDateTime();
        }
        public void sbloccaDateTime()
        {
            if (checkBox1.Checked == true)
            {
                dateTimePicker2.Enabled = true;
                dateTimePicker3.Enabled = true;
                dateTimePicker4.Enabled = true;
            }
            else
            {
                dateTimePicker2.Enabled = false;
                dateTimePicker3.Enabled = false;
                dateTimePicker4.Enabled = false;

            }
        }
    }
}

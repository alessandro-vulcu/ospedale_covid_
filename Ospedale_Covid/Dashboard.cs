using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ospedale_Covid
{
    public partial class Dashboard : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );

        Database db = new Database();
        public Dashboard()
        {
            InitializeComponent();
            comboBox1.DataSource = db.daColonnaALista("strutture", "idStruttura");
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        public void saturazioneStrutture()
        {
            string comando = string.Format("SELECT COUNT(idPrenotazione) FROM Prenotazioni WHERE giorno = '{0}' AND idStruttura = '{1}'", DateTime.Now.ToString("dd/MM/yyyy"), comboBox1.Text);
            double n = db.getDataInt(comando);
            double percentuale1 = (n / 200) * 100;
            textBox5.Text = percentuale1+"% oggi";

        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            saturazioneStrutture();
            vacciniEffettuati();
            selezionaEffetticollateraliFrequenti();
        }
        public void vacciniEffettuati()
        {
            string comando = string.Format("SELECT COUNT(idVaccinoCovid) FROM pazientiVaccinati WHERE idStruttura = '{0}'", comboBox1.Text);
            textBox1.Text = db.getDataInt(comando) + " dosi";
        }
        public void selezionaEffetticollateraliFrequenti()
        {
            string comando = string.Format("SELECT vacciniCovid.nomeVaccino, vacciniCovid.casaFarmaceutica, EffettiCollaterali.effetto_collaterale, EffettiCollaterali.quanti FROM EffettiCollaterali INNER JOIN vacciniCovid ON EffettiCollaterali.idVaccinoCovid = vacciniCovid.idVaccinoCovid ORDER BY quanti DESC");
            db.DataSourceComando(comando, dataGridView1);
            //dataGridView1.Sort(dataGridView1.Columns["quanti"], ListSortDirection.Ascending);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            saturazioneStrutture();
            vacciniEffettuati();
            selezionaEffetticollateraliFrequenti();
        }
    }
}

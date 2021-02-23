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
    
    public partial class Spedizione : Form
    {

        string idVaccinoCovid;
        Database db = new Database();
        public Spedizione(string idVaccinoCovid)
        {
            InitializeComponent();

            this.idVaccinoCovid = idVaccinoCovid;
            comboBox1.DataSource = db.daColonnaALista("strutture", "idStruttura");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime dataArrivo = dateTimePicker1.Value.AddDays(5);

            if (!db.CheckTextBox(panel1))
            {
                string comando1 = string.Format("INSERT INTO spedizioniVaccino VALUES(\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\")", db.generateID(), comboBox1.Text, idVaccinoCovid, dateTimePicker1.Text, dataArrivo.ToShortDateString(), numericUpDown1.Value);
                string comando2 = string.Format("UPDATE strutture SET quantitaVaccini = (quantitaVaccini + '{0}') WHERE idStruttura = '{1}'", numericUpDown1.Value, comboBox1.Text);
                db.esegui(comando1);
                db.esegui(comando2);
            }
        }
    }
}

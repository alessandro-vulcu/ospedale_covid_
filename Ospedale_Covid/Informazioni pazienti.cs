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
    public partial class Informazioni_pazienti : Form
    {
        string idPaziente;
        Database db = new Database();
        public Informazioni_pazienti(string idPaziente)
        {
            InitializeComponent();
            this.idPaziente = idPaziente;
        }

        private void Informazioni_pazienti_Load(object sender, EventArgs e)
        {
            selectResidenza();
            db.DataSourceComando(string.Format(@"SELECT * FROM pazientiVaccinazioni WHERE idPaziente = '{0}'", idPaziente), dataGridView1);
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
    }
}

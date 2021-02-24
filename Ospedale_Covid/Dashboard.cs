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
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            string comando = string.Format("SELECT COUNT(idPrenotazione) FROM Prenotazioni WHERE giorno = '{0}'", DateTime.Now.ToString("dd/MM/yyyy"));
            
            
            DateTime Giornosettimana1 = DateTime.Now.AddDays(-3);
            DateTime Giornosettimana2 = DateTime.Now.AddDays(4);
            string comando2 = string.Format("SELECT COUNT(idPrenotazione) FROM Prenotazioni WHERE giorno > '{0}' AND giorno < '{1}'", Giornosettimana1, Giornosettimana2);

            DateTime date = DateTime.Today;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            string comando3 = string.Format("SELECT COUNT(idPrenotazione) FROM Prenotazioni WHERE giorno > '{0}' AND giorno < '{1}'", firstDayOfMonth, lastDayOfMonth);

            textBox1.Text = Convert.ToString(db.getDataInt(comando));
            textBox2.Text = Convert.ToString(db.getDataInt(comando2));
            textBox3.Text = Convert.ToString(db.getDataInt(comando3));
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
    }
}

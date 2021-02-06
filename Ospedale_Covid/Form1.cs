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

namespace Ospedale_Covid
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        private void esegui(string comandosql)
        {
            string stringaConnessione = @"Data Source=ospedale_covidDB.db";
            using (SQLiteConnection connessione = new SQLiteConnection(stringaConnessione))
            {
                connessione.Open();
                using (SQLiteCommand comando = new SQLiteCommand(comandosql, connessione))
                {
                    comando.ExecuteNonQuery();
                }
                connessione.Close();
            }
        }
        public void DataSource(string nometabella, DataGridView tabComuni)
        {
            using (SQLiteConnection connessione = new SQLiteConnection(@"Data Source=ospedale_covidDB.db"))
            {
                connessione.Open();
                using (SQLiteCommand comando = new SQLiteCommand(String.Format("select * from {0}", nometabella), connessione))
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter(comando);
                    DataSet ds = new DataSet("tabelle");

                    da.Fill(ds, "tabella");
                    tabComuni.DataSource = ds.Tables["tabella"];
                    tabComuni.Refresh();
                }
                connessione.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataSource("pazienti", tabella1);
        }

        private void btnPersonale_Click(object sender, EventArgs e)
        {

        }
    }
}

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
using FontAwesome.Sharp;

namespace Ospedale_Covid
{
    public partial class Pazienti : Form
    {
        public Pazienti()
        {
            InitializeComponent();

        }

        private void Pazienti_Load(object sender, EventArgs e)
        {
            DataSource("Pazienti", dataGridView1);
        }
        //INSERT INTO pazienti (nome, cognome, luogoNascita, dataNascita, codiceFiscale, telefono, email) VALUES ("Alessandro", "Vulcu", "Camposampiero", '2002-03-21', "1", "3341687061", "alexvulcu21@gmail.com")
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
    }
}

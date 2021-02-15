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
    class Database
    {
        
        public Database()
        {
            
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
        public void aggiungi(string comandosql, DataGridView dataGridView1)
        {
            dataGridView1.Columns.Clear();
            string stringaConnessione = @"Data Source=ospedale_covidDB.db";

            using (SQLiteConnection connessione = new SQLiteConnection(stringaConnessione))
            {
                connessione.Open();

                using (SQLiteCommand comando = new SQLiteCommand(comandosql, connessione))
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter(comando);
                    DataSet ds = new DataSet("tabelle");
                    da.Fill(ds, "tabella");
                    dataGridView1.DataSource = ds.Tables["tabella"];
                    dataGridView1.Refresh();
                }
                connessione.Close();
            }
        }
        public string getData(string comandosql)
        {
            string var;
            string stringaConnessione = @"Data Source=ospedale_covidDB.db";
            using (SQLiteConnection connessione = new SQLiteConnection(stringaConnessione))
            {
                connessione.Open();
                using (SQLiteCommand comando = new SQLiteCommand(comandosql, connessione))
                {
                    comando.ExecuteNonQuery();
                    var = (string)comando.ExecuteScalar();
                }
                connessione.Close();
            }
            return var;
        }
        public void esegui(string comandosql)
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
        public void dropRow(string nometabella, string ID, string nomeID)
        {
            string stringaConnessione = @"Data Source=ospedale_covidDB.db";
            using (SQLiteConnection connessione = new SQLiteConnection(stringaConnessione))
            {
                connessione.Open();

                using (var transaction = connessione.BeginTransaction())
                {
                    using (SQLiteCommand comando = new SQLiteCommand(connessione))
                    {
                        comando.CommandText = String.Format("DELETE FROM {0} WHERE {1} = '{2}'", nometabella, nomeID, ID);
                        
                        comando.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                connessione.Close();
            }
        }
    }
}

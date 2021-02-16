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
        public void DataSourceWhere(string nometabella, string id, string nomeid, DataGridView tabComuni)
        {
            using (SQLiteConnection connessione = new SQLiteConnection(@"Data Source=ospedale_covidDB.db"))
            {
                connessione.Open();
                using (SQLiteCommand comando = new SQLiteCommand(String.Format("SELECT * FROM {0} WHERE {1} = '{2}'", nometabella, nomeid, id), connessione))
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
            string stringaConnessione = @"Data Source=ospedale_covidDB.db;";
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
        public void caricaInComboBox(DataGridView dataGridView1, ComboBox comboBox1)
        {
            List<string> columnHead = new List<string>();
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                columnHead.Add(dataGridView1.Columns[i].HeaderText);
            }
            comboBox1.DataSource = columnHead;
        }
        public string generateID()
        {
            StringBuilder builder = new StringBuilder();
            Enumerable
               .Range(65, 26)
                .Select(e => ((char)e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(e => Guid.NewGuid())
                .Take(11)
                .ToList().ForEach(e => builder.Append(e));
            string id = builder.ToString().ToUpper();
            return id;
        }
        public List<string> daColonnaALista(string nometabella, string colonna)
        {
            List<string> IDs = new List<string>();
            string stringaConnessione = @"Data Source=ospedale_covidDB.db;";
            using (SQLiteConnection connessione = new SQLiteConnection(stringaConnessione))
            {
                connessione.Open();

                using (var transaction = connessione.BeginTransaction())
                {
                    using (SQLiteCommand comando = new SQLiteCommand(connessione))
                    {
                        comando.CommandText = string.Format("SELECT {0} FROM {1}", colonna, nometabella);

                        SQLiteDataReader reader = comando.ExecuteReader();
                        
                        while (reader.Read())
                        {
                            IDs.Add(reader.GetString(0));
                        }
                    }
                    transaction.Commit();
                }
                connessione.Close();
            }
            return IDs;
        } 
    }
}

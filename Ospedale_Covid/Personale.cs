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
    public partial class Personale : Form
    {
        public Personale()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!CheckTextBox(panel1))
            {
                string comando1 = string.Format("INSERT INTO pazienti(nome, cognome, luogoNascita, dataNascita, codiceFiscale, telefono, email) VALUES(\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\", \"{6}\")", txtNome.Text, txtCognome.Text, txtNascita.Text, txtData.Text, txtCF.Text, txtTelefono.Text, txtEmail.Text);
                esegui(comando1);
            }
            DataSource("pazienti", dataGridView1);
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
        private string generateID()
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

        private bool CheckTextBox(Panel panelTextBox)
        {
            foreach (Control txt in panelTextBox.Controls.Cast<Control>().OrderBy(c => c.TabIndex))
            {
                if (txt is TextBox && txt.Text == "")
                {
                    MessageBox.Show("Controlla tutti i campi", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;
                }
            }
            return false;
        }
        private void caricaInComboBox()
        {
            List<string> columnHead = new List<string>();
            for(int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                columnHead.Add(dataGridView1.Columns[i].HeaderText);
            }
            comboTipo.Items.Add(columnHead);
        }
        private void Personale_Load(object sender, EventArgs e)
        {
            DataSource("Personale", dataGridView1);
            caricaInComboBox();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }
    }
}

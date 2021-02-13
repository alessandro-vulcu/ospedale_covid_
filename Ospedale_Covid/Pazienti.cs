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
            caricaInComboBox();
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

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //generateID(txtNome.Text, txtCognome.Text);
            if (!CheckTextBox(panel1))
            {
                string comando1 = string.Format("INSERT INTO pazienti(nome, cognome, luogoNascita, dataNascita, codiceFiscale, telefono, email) VALUES(\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\", \"{6}\")", txtNome.Text, txtCognome.Text, txtNascita.Text, txtData.Text, txtCF.Text, txtTelefono.Text, txtEmail.Text);
                string comando2 = string.Format("INSERT INTO residenza(idr, idpaziente, via, cap, provincia, regione, stato) VALUES(\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\", \"{6}\")", generateID(), txtCF.Text, txtVia.Text, txtCAP.Text, txtProvincia.Text, txtRegione.Text, txtStato.Text);
                esegui(comando1);
                esegui(comando2);
            }
            DataSource("pazienti", dataGridView1);
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
            foreach(Control txt in panelTextBox.Controls.Cast<Control>().OrderBy(c => c.TabIndex))
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
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                columnHead.Add(dataGridView1.Columns[i].HeaderText);
            }
            comboBox1.DataSource = columnHead;
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            if(iconButton3.Text == "Pazienti")
            {
                DataSource("pazienti", dataGridView1);
                iconButton3.Text = "Residenza";
            }
            else
            {
                DataSource("residenza", dataGridView1);
                iconButton3.Text = "Pazienti";
            }
            
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            string comando = string.Format(@"SELECT * FROM pazienti WHERE {0} = '{1}'", comboBox1.Text, textBox1.Text);
            aggiungi(comando);
        }
        private void aggiungi(string comandosql)
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
    }
}

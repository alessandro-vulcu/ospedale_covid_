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
using System.Globalization;

namespace Ospedale_Covid
{
    public partial class Personale : Form
    {
        Database db;
        string currentPK;
        public Personale()
        {
            InitializeComponent();
            db = new Database();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!CheckTextBox(panel1))
            {
                string comando1 = string.Format("INSERT INTO personale(idPersonale, nome, cognome, luogoNascita, dataNascita, codiceFiscale, telefono, mail, specializzazione, tipo) VALUES(\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\", \"{6}\", \"{7}\", \"{8}\", \"{9}\")", db.generateID(), txtNome.Text, txtCognome.Text, txtNascita.Text, txtData.Text, txtCF.Text, txtTelefono.Text, txtEmail.Text, txtSpec.Text, comboTipo.Text);
                esegui(comando1);
            }
            DataSource("personale", dataGridView1);
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
            db.caricaInComboBox(dataGridView1, comboBox1);
            caricaInComboBox();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Trim() != "")
            {
                string comandosql = string.Format(@"SELECT * FROM {0} WHERE {1} LIKE '{2}' COLLATE NOCASE", "personale", comboBox1.Text, txtSpec.Text);
                db.aggiungi(comandosql, dataGridView1);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!CheckTextBox(panel1))
            {
                try
                {
                    string comando1 = string.Format("UPDATE personale SET nome = \"{0}\", cognome = \"{1}\", luogoNascita = \"{2}\", datanascita = \"{3}\", codiceFiscale = \"{4}\", telefono = \"{5}\", mail = \"{6}\", specializzazione = \"{7}\", tipo = \"{8}\" WHERE idPersonale = \"{9}\"", txtNome.Text, txtCognome.Text, txtNascita.Text, txtData.Text, txtCF.Text, txtTelefono.Text, txtEmail.Text, txtSpec.Text, comboTipo.Text, currentPK);
                    db.esegui(comando1);
                    foreach (Control txt in panel1.Controls.Cast<Control>().OrderBy(c => c.TabIndex))
                    {
                        if (txt is TextBox)
                        {
                            txt.Text = "";
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            db.DataSource("personale", dataGridView1);
            button2.Enabled = false;
            button1.Enabled = true;
            txtCF.Enabled = true;
            currentPK = "";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            button2.Enabled = true;
            button1.Enabled = false;
            currentPK = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();

            txtNome.Text = Convert.ToString(db.getData(string.Format(@"SELECT nome FROM personale WHERE idPersonale = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())));
            txtCognome.Text = Convert.ToString(db.getData(string.Format(@"SELECT cognome FROM personale WHERE idPersonale = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())));
            txtNascita.Text = Convert.ToString(db.getData(string.Format(@"SELECT luogoNascita FROM personale WHERE idPersonale = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())));
            txtData.Value = DateTime.ParseExact(db.getData(string.Format(@"SELECT dataNascita FROM personale WHERE idPersonale = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            txtCF.Text = Convert.ToString(db.getData(string.Format(@"SELECT codiceFiscale FROM personale WHERE idPersonale = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())));
            txtTelefono.Text = Convert.ToString(db.getData(string.Format(@"SELECT telefono FROM personale WHERE idPersonale = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())));
            txtEmail.Text = Convert.ToString(db.getData(string.Format(@"SELECT mail FROM personale WHERE idPersonale = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())));
            txtSpec.Text = Convert.ToString(db.getData(string.Format(@"SELECT specializzazione FROM personale WHERE idPersonale = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())));
            comboTipo.Text = Convert.ToString(db.getData(string.Format(@"SELECT tipo FROM personale WHERE idPersonale = '{0}'", dataGridView1.SelectedRows[0].Cells[0].Value.ToString())));
            
        }
    }
}

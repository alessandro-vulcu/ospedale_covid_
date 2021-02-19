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
using System.Globalization;
using FontAwesome.Sharp;

namespace Ospedale_Covid
{
    public partial class Pazienti : Form
    {
        Database db;
        int rowIndex;
        string qualeDatabase = "pazienti";
        public Pazienti()
        {
            InitializeComponent();
            db = new Database();
        }

        private void Pazienti_Load(object sender, EventArgs e)
        {
            db.DataSource("Pazienti", dataGridView1);
            db.caricaInComboBox(dataGridView1, comboBox1);
            button2.Enabled = false;
        }
        //INSERT INTO pazienti (nome, cognome, luogoNascita, dataNascita, codiceFiscale, telefono, email) VALUES ("Alessandro", "Vulcu", "Camposampiero", '2002-03-21', "1", "3341687061", "alexvulcu21@gmail.com")
        
        private bool checkDoublePKs(string CF)
        {
            string comandosql = String.Format(@"SELECT codiceFiscale FROM pazienti WHERE codiceFiscale = '{0}'", CF);
            string str = "";

            string stringaConnessione = @"Data Source=ospedale_covidDB.db";
            using (SQLiteConnection connessione = new SQLiteConnection(stringaConnessione))
            {
                connessione.Open();
                using (SQLiteCommand comando = new SQLiteCommand(comandosql, connessione))
                {
                    comando.ExecuteNonQuery();
                    str = (string)comando.ExecuteScalar();
                }
                connessione.Close();
            }
            if (str != CF)
                return true;
            else
            {
                MessageBox.Show("Utente con lo stesso Codice Fiscale già esistente", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
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
                if (checkDoublePKs(txtCF.Text))
                {
                    try
                    {
                        string comando1 = string.Format("INSERT INTO pazienti(nome, cognome, luogoNascita, dataNascita, codiceFiscale, telefono, email) VALUES(\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\", \"{6}\")", txtNome.Text, txtCognome.Text, txtNascita.Text, txtData.Text, txtCF.Text, txtTelefono.Text, txtEmail.Text);
                        string comando2 = string.Format("INSERT INTO residenza(idr, idpaziente, via, cap, provincia, regione, stato) VALUES(\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\", \"{6}\")", db.generateID(), txtCF.Text, txtVia.Text, txtCAP.Text, txtProvincia.Text, txtRegione.Text, txtStato.Text);
                        db.esegui(comando1);
                        db.esegui(comando2);
                        foreach (Control txt in panel1.Controls.Cast<Control>().OrderBy(c => c.TabIndex))
                            if (txt is TextBox)
                            {
                                txt.Text = "";
                            }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            db.DataSource("pazienti", dataGridView1);
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
                txt.Text.Trim();
            }
            return false;
        }


        private void iconButton1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text.Trim() != "")
            {
                string comandosql = string.Format(@"SELECT * FROM {0} WHERE {1} LIKE '{2}' COLLATE NOCASE", qualeDatabase,comboBox1.Text, textBox1.Text);
                db.aggiungi(comandosql, dataGridView1);
            }
        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int PK = 0;
            if (dataGridView1.Columns[4].HeaderText == "codiceFiscale")
                PK = 4;
            else
                PK = 1;
            button2.Enabled = true;
            button1.Enabled = false;
            txtCF.Enabled = false;
            
            txtNome.Text = Convert.ToString(db.getData(string.Format(@"SELECT nome FROM pazienti WHERE codiceFiscale = '{0}'", dataGridView1.SelectedRows[0].Cells[PK].Value.ToString())));
            txtCognome.Text = Convert.ToString(db.getData(string.Format(@"SELECT cognome FROM pazienti WHERE codiceFiscale = '{0}'", dataGridView1.SelectedRows[0].Cells[PK].Value.ToString())));
            txtNascita.Text = Convert.ToString(db.getData(string.Format(@"SELECT luogoNascita FROM pazienti WHERE codiceFiscale = '{0}'", dataGridView1.SelectedRows[0].Cells[PK].Value.ToString())));
            txtData.Value = DateTime.ParseExact(db.getData(string.Format(@"SELECT dataNascita FROM pazienti WHERE codiceFiscale = '{0}'", dataGridView1.SelectedRows[0].Cells[PK].Value.ToString())), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            txtCF.Text = Convert.ToString(db.getData(string.Format(@"SELECT codiceFiscale FROM pazienti WHERE codiceFiscale = '{0}'", dataGridView1.SelectedRows[0].Cells[PK].Value.ToString())));
            txtTelefono.Text = Convert.ToString(db.getData(string.Format(@"SELECT telefono FROM pazienti WHERE codiceFiscale = '{0}'", dataGridView1.SelectedRows[0].Cells[PK].Value.ToString())));
            txtEmail.Text = Convert.ToString(db.getData(string.Format(@"SELECT email FROM pazienti WHERE codiceFiscale = '{0}'", dataGridView1.SelectedRows[0].Cells[PK].Value.ToString())));

            txtVia.Text = Convert.ToString(db.getData(string.Format(@"SELECT via FROM residenza WHERE idpaziente = '{0}'", dataGridView1.SelectedRows[0].Cells[PK].Value.ToString())));
            txtCAP.Text = Convert.ToString(db.getData(string.Format(@"SELECT cap FROM residenza WHERE idpaziente = '{0}'", dataGridView1.SelectedRows[0].Cells[PK].Value.ToString())));
            txtProvincia.Text = Convert.ToString(db.getData(string.Format(@"SELECT provincia FROM residenza WHERE idpaziente = '{0}'", dataGridView1.SelectedRows[0].Cells[PK].Value.ToString())));
            txtRegione.Text = Convert.ToString(db.getData(string.Format(@"SELECT regione FROM residenza WHERE idpaziente = '{0}'", dataGridView1.SelectedRows[0].Cells[PK].Value.ToString())));
            txtStato.Text = Convert.ToString(db.getData(string.Format(@"SELECT stato FROM residenza WHERE idpaziente = '{0}'", dataGridView1.SelectedRows[0].Cells[PK].Value.ToString())));

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!CheckTextBox(panel1))
            {
                try
                {
                    string comando1 = string.Format("UPDATE pazienti SET nome = \"{0}\", cognome = \"{1}\", luogoNascita = \"{2}\", datanascita = \"{3}\", telefono = \"{4}\", email = \"{5}\" WHERE codiceFiscale = \"{6}\"", txtNome.Text, txtCognome.Text, txtNascita.Text, txtData.Text, txtTelefono.Text, txtEmail.Text, txtCF.Text);
                    string comando2 = string.Format("UPDATE residenza SET via = \"{0}\", cap = \"{1}\", provincia = \"{2}\", regione = \"{3}\", stato = \"{4}\"  WHERE idpaziente = \"{5}\"", txtVia.Text, txtCAP.Text, txtProvincia.Text, txtRegione.Text, txtStato.Text, txtCF.Text);
                    db.esegui(comando1);
                    db.esegui(comando2);
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
            db.DataSource("pazienti", dataGridView1);
            button2.Enabled = false;
            button1.Enabled = true;
            txtCF.Enabled = true;
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex != -1)
            {
                this.dataGridView1.Rows[e.RowIndex].Selected = true;
                this.rowIndex = e.RowIndex;
                this.dataGridView1.CurrentCell = this.dataGridView1.Rows[e.RowIndex].Cells[1];
                this.contextMenuStrip1.Show(this.dataGridView1, e.Location);
                contextMenuStrip1.Show(Cursor.Position);
            }
        }

        private void eliminaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.dataGridView1.Rows[this.rowIndex].IsNewRow)
            {
                db.dropRow("pazienti", dataGridView1.Rows[this.rowIndex].Cells[4].Value.ToString(), "codiceFiscale");
                db.DataSource("pazienti", dataGridView1);
            }

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void espandiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Informazioni_pazienti informazioni_Pazienti = new Informazioni_pazienti(dataGridView1.Rows[this.rowIndex].Cells[4].Value.ToString());
            informazioni_Pazienti.ShowDialog();
        }
    }
}

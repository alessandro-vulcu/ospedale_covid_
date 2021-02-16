﻿using System;
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
    public partial class Strutture : Form
    {
        Database db;
        int rowIndex;
        public Strutture()
        {
            InitializeComponent();
            db = new Database();
        }

        private void Strutture_Load(object sender, EventArgs e)
        {
            db.DataSource("strutture", dataGridView1);
            db.caricaInComboBox(dataGridView1, comboBox1);
            button2.Enabled = false;
            comboRes.DataSource = db.daColonnaALista("personale", "idPersonale");
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
                txt.Text.Trim();
            }
            return false;
        }
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
        private void button1_Click(object sender, EventArgs e)
        {
            if (!CheckTextBox(panel1))
            {
                try
                {
                    string comando1 = string.Format("INSERT INTO strutture VALUES(\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\", \"{6}\", \"{7}\")", db.generateID(), comboRes.Text, txtNome.Text, txtIndirizzo.Text,txtMail.Text, txtTelefono.Text, txtMax.Text, txtDisponibilità.Text);
                    db.esegui(comando1);
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
            db.DataSource("strutture", dataGridView1);
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ospedale_Covid
{
    public partial class InformazioniStruttura : Form
    {
        Database db;
        string idStruttura;
        int rowIndex;
        public InformazioniStruttura(string idStruttura)
        {
            InitializeComponent();
            db = new Database();
            this.idStruttura = idStruttura;
        }
        public void joinTraTabelle()
        {
            string comando = string.Format("SELECT nome, cognome, codiceFiscale, personale.idPersonale FROM personale INNER JOIN personaleStrutture ON personale.idPersonale = personaleStrutture.idPersonale WHERE idStruttura = '{0}'", idStruttura);
            db.DataSourceComando(comando, dataGridView2);
        }
        private void InformazioniStruttura_Load(object sender, EventArgs e)
        {
            db.DataSourceWhere("orariStrutture", idStruttura, "idStruttura", dataGridView1);
            joinTraTabelle();
        }
        private void iconButton1_Click(object sender, EventArgs e)
        {
            string giorni = string.Format("{0} - {1}", comboBox1.Text, comboBox2.Text);
            string ora = string.Format("{0} - {1}", textBox2.Text, textBox3.Text);
            string comando = string.Format("INSERT INTO orariStrutture VALUES (\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\")", db.generateID(), idStruttura, comboBox1.Text, comboBox2.Text, textBox2.Text, textBox3.Text);
            db.esegui(comando);
            db.DataSource("orariStrutture", dataGridView1);
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            
        }

        private void eliminaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.dataGridView2.Rows[this.rowIndex].IsNewRow)
            {
                db.esegui(string.Format("DELETE FROM personaleStrutture WHERE idPersonale = '{0}' AND idStruttura = '{1}'", dataGridView2.Rows[this.rowIndex].Cells[3].Value.ToString(), idStruttura));
                joinTraTabelle();
            }
        }

        private void dataGridView2_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex != -1)
            {
                this.dataGridView2.Rows[e.RowIndex].Selected = true;
                this.rowIndex = e.RowIndex;
                this.dataGridView2.CurrentCell = this.dataGridView2.Rows[e.RowIndex].Cells[1];
                this.contextMenuStrip1.Show(this.dataGridView2, e.Location);
                contextMenuStrip1.Show(Cursor.Position);
            }
        }
    }
}

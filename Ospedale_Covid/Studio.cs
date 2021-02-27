using System;
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
    public partial class Studio : Form
    {
        Database db;
        int rowIndex;
        string currentPK;
        public Studio()
        {
            InitializeComponent();
            db = new Database();

            comboBox1.DataSource = db.daColonnaALista("personale", "idPersonale");
            db.DataSource("studioPersonale", dataGridView1);

            button1.Enabled = true;
            button2.Enabled = false;
        }

        private void iconButton6_Click(object sender, EventArgs e)
        {
            if (!db.CheckTextBox(panel1) && controlladoppi())
            {
                string comando1 = string.Format("INSERT INTO studioPersonale(idStudio, idPersonale, nomestudio, sedestudi) VALUES(\"{0}\", \"{1}\", \"{2}\", \"{3}\")", db.generateID(), comboBox1.Text, textBox1.Text, textBox2.Text);
                db.esegui(comando1);
            }
            db.DataSource("studioPersonale", dataGridView1);
        }
        public bool controlladoppi()
        {
            string comando = string.Format("SELECT COUNT(idStudio) FROM studioPersonale WHERE idPersonale = '{0}'", comboBox1.Text);
            if(db.getDataInt(comando) != 0)
            {
                MessageBox.Show("ERRORE: personale già assegnato a uno studio");
                return false;
            }
            return true;
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
                db.esegui(string.Format("DELETE FROM studioPersonale WHERE idStudio = '{0}'", dataGridView1.Rows[this.rowIndex].Cells[0].Value.ToString()));
                db.DataSource("studioPersonale", dataGridView1);
                button1.Enabled = true;
                button2.Enabled = false;
            }
        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = true;
            currentPK = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();

            comboBox1.Text = Convert.ToString(db.getData(string.Format(@"SELECT idPersonale FROM studioPersonale WHERE idStudio = '{0}'", currentPK)));
            textBox1.Text = Convert.ToString(db.getData(string.Format(@"SELECT nomestudio FROM studioPersonale WHERE idStudio = '{0}'", currentPK)));
            textBox2.Text = Convert.ToString(db.getData(string.Format(@"SELECT sedeStudi FROM studioPersonale WHERE idStudio = '{0}'", currentPK)));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!db.CheckTextBox(panel1) && controlladoppi())
            {
                try
                {
                    string comando1 = string.Format("UPDATE studioPersonale SET idPersonale = \"{0}\", nomestudio = \"{1}\", sedeStudi = \"{2}\" WHERE idPersonale = \"{3}\"", comboBox1.Text, textBox1.Text, textBox2.Text, currentPK);
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
            db.DataSource("studioPersonale", dataGridView1);
            button2.Enabled = false;
            button1.Enabled = true;
            currentPK = "";
        }
    }
}

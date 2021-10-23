using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Lab1
{
    public partial class MainForm : Form
    {
        Data data;
        string oldTextBoxExp = string.Empty;
        string currentFile;

        public MainForm()
        {
            InitializeComponent();
            data = new Data(dataGridView);
            this.Text = "My Exel";

        }

        internal Data Data
        {
            get => default(Data);
            set { }
        }

        private void dataGridView_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Value.ToString()))
            {
                data.ChangeData(e.Value.ToString(), e.RowIndex, e.ColumnIndex);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                data.SaveToFile(saveFileDialog1.FileName);
                currentFile = saveFileDialog1.FileName;
                this.Text = currentFile + "-MyExel";
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                data = new Data(dataGridView);
                data.OpenFile(openFileDialog1.FileName);
                data.FillData(Mode.Value);
                currentFile = openFileDialog1.FileName;
                this.Text = currentFile + "-MyExel";
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Savchuk Sofia K24 V6");
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        { 
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedCells.Count == 1)
            {
                var selecetedCell = dataGridView.SelectedCells[0];
                expressionTextBox.Text = Data.cells[selecetedCell.RowIndex][selecetedCell.ColumnIndex].exp;
                oldTextBoxExp = expressionTextBox.Text;
            }
        }

        private void dataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            

        }

        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            expressionTextBox.Clear();
            if (Data.cells[e.RowIndex][e.ColumnIndex].exp != null)
            {
                if (!String.IsNullOrEmpty(Data.cells[e.RowIndex][e.ColumnIndex].error))
                {
                    dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Data.cells[e.RowIndex][e.ColumnIndex].error;
                }
                else
                {
                    dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Data.cells[e.RowIndex][e.ColumnIndex].Value.ToString();
                }
            }
            
        }

        private void dataGridView_CellBeginEdit(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Data.cells[e.RowIndex][e.ColumnIndex].exp;
        }

        private void expressionTextBox_Enter(object sender, EventArgs e)
        {
           
        }

        private void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox tb = (TextBox)e.Control;
            tb.TextChanged += Tb_TextChanged;
        }

        private void Tb_TextChanged(object sender, EventArgs e) 
        {
            expressionTextBox.Text = ((TextBox)sender).Text;
            oldTextBoxExp = expressionTextBox.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            data.AddRow();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            data.RemoveRow();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            data.AddColumn();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            data.RemoveColumn();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Your`e going to close program","ATTENTION",
            MessageBoxButtons.OKCancel,
            MessageBoxIcon.Information,
            MessageBoxDefaultButton.Button1,
            MessageBoxOptions.DefaultDesktopOnly);
            if (result == DialogResult.OK)
            {
                e.Cancel = true;
            }
        }

        private void MainForm_Load_1(object sender, EventArgs e)
        {
            data.FillData(Mode.Value);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedCells.Count == 1)
            {
                var selectedCell = dataGridView.SelectedCells[0];
                if (expressionTextBox.Text == string.Empty)
                {
                    Data.cells[selectedCell.RowIndex][selectedCell.ColumnIndex].exp = null;
                    Data.cells[selectedCell.RowIndex][selectedCell.ColumnIndex].val = 0;
                    dataGridView.Rows[selectedCell.RowIndex].Cells[selectedCell.ColumnIndex].Value = "";
                }
                else
                {
                    data.ChangeData(expressionTextBox.Text, selectedCell.RowIndex, selectedCell.ColumnIndex);
                    if (!string.IsNullOrEmpty(Data.cells[selectedCell.RowIndex][selectedCell.ColumnIndex].error))
                    {
                        dataGridView.Rows[selectedCell.RowIndex].Cells[selectedCell.ColumnIndex].Value = Data.cells[selectedCell.RowIndex][selectedCell.ColumnIndex].error;
                    }
                    else
                    {
                        dataGridView.Rows[selectedCell.RowIndex].Cells[selectedCell.ColumnIndex].Value = Data.cells[selectedCell.RowIndex][selectedCell.ColumnIndex].val.ToString();
                    }
                }

            }
        }
    }
}

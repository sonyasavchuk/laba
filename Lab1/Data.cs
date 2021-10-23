using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Lab1
{
    enum Mode{EXPRESSION,Value };
    class Data
    {
        private int columnCount = 100;
        private int rowCount = 100;
        Parser parser = new Parser();
        DataGridView dataGridView;

        public static List<List<MyCell>> cells = new List<List<MyCell>>();

        public Data(DataGridView _dataGridView)
        {
            dataGridView = _dataGridView;
            cells.Clear();
            for (int i =0; i<rowCount; i++)
            {
                cells.Add(new List<MyCell>());
                for (int j = 0; j < columnCount; j++)
                {
                    cells[i].Add(new MyCell() { rowNum = i + 1, columnLetter = Convert.ToChar('A' + j) });
                }
            }

        }

        public MyCell MyCell
        {
            get => default(MyCell);
            set{ }
        }

        public Parser Parser
        {
            get => default(Parser);
            set { }
        }

        public void AddRow()
        {
            cells.Add(new List<MyCell>());
            for (int i=0; i < columnCount; i++)
            {
                cells[cells.Count - 1].Add(new MyCell() { rowNum =rowCount+ 1,columnLetter = Convert.ToChar('A'+i)});
            }
            dataGridView.Rows.Add(1);
            dataGridView.Rows[dataGridView.Rows.Count - 1].HeaderCell.Value = (dataGridView.Rows.Count).ToString();
            rowCount++;
        }
        
        public void AddColumn()
        {
            if (columnCount <= 25)
            {
                for (int i =0; i < rowCount; i++)
                {
                    cells[i].Add(new MyCell() { rowNum = i + 1 ,columnLetter=Convert.ToChar('A'+columnCount)});

                }
                dataGridView.Columns.Add(((char)('A'+columnCount)).ToString(),((char)('A'+columnCount)).ToString());
                columnCount += 1;
                dataGridView.Refresh();
            }
            else
            {
                columnCount++;
                DataGridViewColumn column = new DataGridViewColumn();
                DataGridViewCell cell = new DataGridViewTextBoxCell();
                column.CellTemplate = cell;
                int k = dataGridView.ColumnCount - 1;
                string n = dataGridView.Columns[k].Name;
                GridNumeration num = new GridNumeration();
                column.Name = num.toSys(columnCount - 1);
                dataGridView.Columns.Add(column);
                dataGridView.Refresh();
            }
        }

        public void RemoveRow()
        {
            dataGridView.Rows.RemoveAt(rowCount - 1);
            cells.RemoveAt(rowCount - 1);

            for(int i =0; i < rowCount-1; i++)
            {
                for (int j=0; j < columnCount; j++)
                {
                    if (cells[i][j].Depends.Where(a => a.rowNum == rowCount).Count() != 0)
                    {
                        ChangeData(cells[i][j].exp, i, j);
                    }
                }
            }
            rowCount -= 1;
        }

        public void RemoveColumn()
        {
            dataGridView.Columns.RemoveAt(columnCount - 1);
            for (int i=0; i < rowCount; i++)
            {
                cells[i].RemoveAt(columnCount - 1);
            }
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount-1; j++)
                {
                    if (cells[i][j].Depends.Where(a => a.columnLetter == 'A' + columnCount - 1).Count() != 0)
                    {
                        ChangeData(cells[i][j].exp, i, j);
                    }
                }
            }
            columnCount -= 1;
        }

        public void ChangeData(string expr,int row, int column)
        {
            try
            {
                cells[row][column].exp = expr;
                cells[row][column].val = parser.Start(expr, cells[row][column]);

                RecalcDependsCell(cells[row][column]);
            }
            catch 
            {
                cells[row][column].error = Convert.ToString(parser.err);
                dataGridView.Rows[row].Cells[column].Value = cells[row][column].error;
            }
        }
        void RecalcDependsCell(MyCell cell)
        {
            for (int i =0; i < rowCount; i++)
            {
                for(int j=0; j < columnCount; j++)
                {
                    if (cells[i][j].exp != null)
                    {
                        for (int k = 0; k < cells[i][j].Depends.Count; k++)
                        {
                            if (cells[i][j].Depends[k].rowNum==cell.rowNum 
                                && cells[i][j].Depends[k].columnLetter == cell.columnLetter)
                            {
                                cells[i][j].val = parser.Start(cells[i][j].exp, cells[i][j]);
                                cells[i][j].error = null;
                                dataGridView.Rows[i].Cells[j].Value = cells[i][j].Value.ToString();
                                RecalcDependsCell(cells[i][j]);

                            }
                        }
                    }
                }
            }
        }

        public void FillData (Mode mode)
        {
            dataGridView.Rows.Clear();
            dataGridView.Columns.Clear();
            for (char i = 'A'; i < 'A' + columnCount; i++)
            {
                dataGridView.Columns.Add(i.ToString(), i.ToString());
            }
            dataGridView.Rows.Add(rowCount);

            for (int i =0; i < rowCount; i++)
            {
                dataGridView.Rows[i].HeaderCell.Value = (i + 1).ToString();
                for (int j= 0; j< columnCount; j++)
                {
                    if (cells[i][j].exp != null)
                    {
                        if (cells[i][j].error != null)
                        {
                            dataGridView.Rows[i].Cells[j].Value = cells[i][j].error.ToString();
                        }
                        else
                        {
                            dataGridView.Rows[i].Cells[j].Value = mode == 
                                Mode.EXPRESSION ? cells[i][j].exp.ToString() : cells[i][j].Value.ToString();

                        }
                    }
                }
            }
        }

        public void SaveToFile(string path)
        {
            StreamWriter stream = new StreamWriter(path);
            stream.WriteLine(rowCount);
            stream.WriteLine(columnCount);
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    if (cells[i][j].exp != null)
                    {
                        stream.WriteLine(i);
                        stream.WriteLine(j);
                        stream.WriteLine(cells[i][j].exp);
                        stream.WriteLine(cells[i][j].val);
                        if (cells[i][j].error == null)
                        {
                            stream.WriteLine();
                        }
                        else
                        {
                            stream.WriteLine(cells[i][j].error);
                        }
                    }
                }
            }
            stream.Close();
        }

        public void OpenFile(string path)
        {
            StreamReader stream = new StreamReader(path);
            DataGridView fileDataGridView = new DataGridView();
            rowCount = Convert.ToInt32(stream.ReadLine());
            columnCount = Convert.ToInt32(stream.ReadLine());
            fileDataGridView.ColumnCount = columnCount;
            fileDataGridView.RowCount = rowCount;
            while (!stream.EndOfStream)
            {
                int i = Convert.ToInt32(stream.ReadLine());
                int j = Convert.ToInt32(stream.ReadLine());
                cells[i][j].exp = stream.ReadLine();
                cells[i][j].val = Convert.ToDouble(stream.ReadLine());
                string error = stream.ReadLine();
                if (!string.IsNullOrEmpty(error))
                {
                    cells[i][j].error = error;
                }
            }
            stream.Close();
        }
    }
}

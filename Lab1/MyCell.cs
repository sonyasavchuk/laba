using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab1
{
    class MyCell : DataGridViewTextBoxCell
    {
        public double val { get; set; } 
        public string name { get;} 
        public string exp { get; set; }
        public string error { get; set; } = null;
        public int rowNum;
        public int columnLetter;
        public List<MyCell> Depends { get; set; } = new List<MyCell>();
        public GridNumeration GridNumeration
        {
            get => default(GridNumeration);
            set { }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab1
{
    class Parser
    {
        enum Types { NONE, DELIMITER, VARIABLE, NUMBER } //лексеми
        public enum Errors { NOERR,SYNTAX,UNBALPARENTS,NOEXP,DIVBYZERO}
        public Errors err;
        string exp; //рядок виразу
        int expIndex;//поточний індекс у виразі
        string token; //поточна лексема
        Types tokType;//тип лексеми
        public string error = "";
        private double[] vars = new double[26];//адже 26 літер в алфавіті
        MyCell currentCell;
        int currentColumn, currentRow;
        public Parser(){
            for (int i =0; i<vars.Length;i++)
            {
                vars[i] = 0;
            }
        }
        public double Start(string expStr, MyCell cell)
        {
            currentCell = cell;
            currentColumn = cell.ColumnIndex - 65;
            currentRow = cell.RowIndex - 1;
            currentCell.Depends.Clear();
            double result;
            exp = expStr;
            expIndex = 0;
            try
            {
                GetToken();
                if (token == "")
                {
                    err = Errors.SYNTAX;
                    return 0;
                }
                Assigment(out result);
                if (token != "")
                {
                    err = Errors.SYNTAX;
                }
                if (CheckRecur(currentCell))
                {
                    err = Errors.SYNTAX;
                }
                return result;
            }
            catch
            {
                err = Errors.SYNTAX;
                return 0;
            }
        }
        void GetToken() //отримаємо наступну лексему
        {
            tokType = Types.NONE;
            token = "";
            while (expIndex < exp.Length && Char.IsWhiteSpace(exp[expIndex])) ++expIndex;//пропускаємо пробіли
            if (expIndex == exp.Length) return; //кінець виразу
            if (isDelimiter(exp[expIndex]))
            {
                token += exp[expIndex];
                expIndex++;
                tokType = Types.DELIMITER;
            }
            else if (Char.IsLetter(exp[expIndex]))
            {
                while (!isDelimiter(exp[expIndex]))
                {
                    token += exp[expIndex];
                    expIndex++;
                    if (expIndex >= exp.Length) break;
                }
                tokType = Types.VARIABLE;
            }
            else if (Char.IsDigit(exp[expIndex]))
            {
                while (!isDelimiter(exp[expIndex]))
                {
                    token += exp[expIndex];
                    expIndex++;
                    if (expIndex >= exp.Length) break;
                }
                tokType = Types.NUMBER;
            }
        }
        bool isDelimiter(char c)
        {
            if (("+-*/%^=()|".IndexOf(c))!=-1) return true;
            return false;
        }
        void Atom(out double result)
        {
            switch (tokType)
            {
                case Types.NUMBER:
                    try
                    {
                        result = double.Parse(token);
                    }
                    catch (FormatException)
                    {
                        result = 0.0;

                    }
                    GetToken();
                    return;
                case Types.VARIABLE:
                    result = FindVar(token);
                    GetToken();
                    return;
                default:
                    result = 0.0;
                    break;
            }
        }
        double FindVar(string str)
        {
            if (str.Contains("max{") && str[str.Length - 1] == '}'){
                string t = null;double x, y;
                for(int i=4; i<str.Length-1; i++)
                {
                    t += str[i];
                }
                try
                {
                    string[] s = t.Split(',');
                    x = Convert.ToDouble(s[0]);
                    y = Convert.ToDouble(s[1]);
                }
                catch
                {
                    err = Errors.SYNTAX;
                    x = y = 0.0;
                }
                if (x > y) return x;
                return y;
            }
            if (str.Contains("min{"))
            {
                string t = null; double x, y;
                for (int i = 4; i < str.Length - 1; i++)
                {
                    t += str[i];
                }
                try
                {
                    string[] s = t.Split(',');
                    x = Convert.ToDouble(s[0]);
                    y = Convert.ToDouble(s[1]);
                }
                catch
                {
                    err = Errors.SYNTAX;
                    x = y = 0.0;
                }
                if (x > y) return y;
                return x;
            }
            err = Errors.SYNTAX;
            return 0.0;
        }
        void PutBack()
        {
            for (int i = 0; i < token.Length; i++) expIndex--;
        }
        void Assigment(out double result)
        {
            int varIndex;
            Types tempTokType;
            string tempToken;
            if (tokType == Types.VARIABLE)
            {
                tempToken = String.Copy(token);
                tempTokType = tokType;
                varIndex = Char.ToUpper(token[0]) - 'A';
                GetToken();
                if (token != "=")
                {
                    PutBack();
                    token = String.Copy(tempToken);
                    tokType = tempTokType;
                }
                else
                {
                    GetToken();
                    PlusMin(out result);
                    vars[varIndex] = result;
                    return;
                }
            }
            PlusMin(out result);
        }
        void PlusMin(out double result)
        {
            string op;
            double tempResult;
            MultDiv(out result);
            while ((op=token)=="+" || op == "-")
            {
                GetToken();
                MultDiv(out tempResult);
                switch (op)
                {
                    case ("+"):
                        result += tempResult;
                        break;
                    case ("-"):
                        result -= tempResult;
                        break;
                }
            }

        }
        void MultDiv(out double result)
        {
            string op;
            double tempResult = 0.0;
            Exponentiating(out result);
            while ((op=token)=="*" || op=="/" || op=="%" || op == "|")
            {
                GetToken();
                Exponentiating(out tempResult);
                if (op == "*")
                {
                    result *= tempResult;
                    break;
                }
                if (tempResult == 0.0)
                {
                    err = Errors.DIVBYZERO;
                    MessageBox.Show("Ділення на 0 неможливе");
                    result = 0.0;
                }
                switch (op)
                {
                    case "/":
                        result /= tempResult;
                        break;
                    case "%":
                        result = (int)result % (int)tempResult;
                        break;
                    case "|":
                        result = (int)result / (int)tempResult;
                        break;
                }

            }
        }
        void Exponentiating(out double result)
        {
            double tempResult;
            Brackets(out result);
            if (token == "^")
            {
                GetToken();
                Exponentiating(out tempResult);
                result = Math.Pow(result, tempResult);
            }
        }
        void Brackets (out double result)
        {
            if (token == "(")
            {
                GetToken();
                PlusMin(out result);
                if (token != ")")
                {
                    err = Errors.UNBALPARENTS;
                }
                GetToken();
            }
            else Atom(out result);
        }
        bool CheckRecur(MyCell cell)
        {
            foreach(var i in cell.Depends)
            {
                if (i == currentCell) 
                    return true;
                if (CheckRecur(i) == true)
                    return true;
            }
            return false;
        }
    }
}

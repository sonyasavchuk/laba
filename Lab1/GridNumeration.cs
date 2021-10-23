using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    class GridNumeration
    {
        public string toSys(int i)
        {
            int k = 0;
            int[] Arr = new int[1000];
            while (i > 25)
            {
                Arr[k] = i / 26 - 1;
                k++;
                i %= 26;
            }
            Arr[k] = i;
            string res = "";
            for (int j = 0; j <= k; j++){
                res += ((char)('A' + Arr[j])).ToString();
            }
            return res;
        }
        public int FromSys(string ColumnHeader)
        {
            char[] chArray = ColumnHeader.ToCharArray();
            int len = chArray.Length;
            int res = 0;
            for (int i = len-2; i>=0; i--)
            {
                res += ((int)chArray[i] - 'A') * Convert.ToInt32(Math.Pow(26, len - i - 1));
            }
            res += ((int)chArray[len - 1] - (int)'A');
            return res;
        }
    }
}

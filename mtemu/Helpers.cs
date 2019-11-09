using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mtemu
{
    class Helpers
    {
        public static string ClearBinary(string str, ref int pos)
        {
            string newStr = "";
            for (int i = 0; i < str.Length; ++i) {
                if (str[i] == '0' || str[i] == '1') {
                    newStr += str[i];
                }
                else if (i < pos) {
                    pos--;
                }
            }
            return newStr;
        }

        public static int BinaryToInt(string str)
        {
            int res = 0;
            for (int i = 0; i < str.Length; ++i) {
                if (str[i] == '0' || str[i] == '1') {
                    res <<= 1;
                    res += str[i] - '0';
                }
            }
            return res;
        }

        public static string IntToBinary(int num, int minLen)
        {
            string res = "";
            while (num > 0) {
                res = (num % 2) + res;
                num /= 2;
            }
            while (res.Length < minLen) {
                res = "0" + res;
            }
            return res;
        }
    }
}

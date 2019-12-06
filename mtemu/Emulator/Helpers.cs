using System;
using System.Reflection;
using System.Windows.Forms;

namespace mtemu
{
    public static class Helpers
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
        public static string ClearHex(string str, ref int pos)
        {
            string newStr = "";
            for (int i = 0; i < str.Length; ++i) {
                if ('a' <= str[i] && str[i] <= 'f') {
                    newStr += (char) ('A' + (str[i] - 'a'));
                }
                else if ('0' <= str[i] && str[i] <= '9' || 'A' <= str[i] && str[i] <= 'F') {
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

        public static int HexToInt(string str)
        {
            int res = 0;
            for (int i = 0; i < str.Length; ++i) {
                if ('a' <= str[i] && str[i] <= 'f') {
                    res <<= 4;
                    res += str[i] - 'a' + 10;
                }
                if ('A' <= str[i] && str[i] <= 'F') {
                    res <<= 4;
                    res += str[i] - 'A' + 10;
                }
                else if ('0' <= str[i] && str[i] <= '9') {
                    res <<= 4;
                    res += str[i] - '0';
                }
            }
            return res;
        }

        public static string IntToBinary(int num, int minLen, int groupSize = -1)
        {
            string res = "";

            while (num != 0) {
                res += (num & 1).ToString();
                num >>= 1;
            }

            while (res.Length < minLen)
                res += "0";

            if (groupSize != -1) {
                int groupsCount = (res.Length - 1) / groupSize;
                int offset = groupSize;
                while (groupsCount > 0) {
                    res = res.Insert(offset, " ");
                    offset += 1 + groupSize;
                    groupsCount--;
                }
            }

            char[] arr = res.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        public static int GetBit(int value, int number)
        {
            return (value >> number) & 1;
        }

        public static int GetBitMask(int number)
        {
            return 1 << number;
        }

        public static bool IsBitSet(int value, int number)
        {
            return GetBit(value, number) != 0;
        }

        public static int Mask(int value, int size = -1)
        {
            if (size == -1) {
                size = Command.WORD_SIZE;
            }
            return value & ((1 << size) - 1);
        }

        public static void DoubleBuffered(this Control control, bool enable)
        {
            var doubleBufferPropertyInfo = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            doubleBufferPropertyInfo.SetValue(control, enable, null);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mtemu
{
    partial class Command
    {
        public bool isOffset;
        private int number_;
        private int[] words_;

        public Command(string[] strWords)
        {
            if (strWords.Length != length_) {
                throw new ArgumentException("Count of words must be equal 10!");
            }
            words_ = new int[length_];
            for (int i = 0; i < length_; ++i) {
                words_[i] = Helpers.BinaryToInt(strWords[i]) % 16;
            }
        }

        public Command(int[] words)
        {
            if (words.Length != length_) {
                throw new ArgumentException("Count of words must be equal 10!");
            }
            words_ = new int[length_];
            for (int i = 0; i < length_; ++i) {
                words_[i] = words[i] % 16;
            }
        }

        public Command(Command other)
        {
            isOffset = other.isOffset;
            number_ = other.number_;
            words_ = new int[length_];
            Array.Copy(other.words_, words_, length_);
        }

        public void SetNumber(int num)
        {
            number_ = num;
        }

        public int GetNumber()
        {
            return number_;
        }

        private string[] GetItem_(WordType type)
        {
            return items_[type][GetSelIndex(type)];
        }

        public string GetName()
        {
            if (GetCommandType() == CommandType.Offset) {
                return $"SET OFFSET = 0x{GetNextAdr():X3}"; 
            }

            string res = "";

            switch (GetCommandType()) {
            case CommandType.MtCommand:
                if (GetRawValue(WordType.I68) == 1) {
                    // Without saving
                    res += "Y=F=";
                }
                else {
                    string to = GetItem_(WordType.I68)[2] + "=";
                    to = to.Replace("F/2=", "F/2;F=");
                    to = to.Replace("2F=", "2F;F=");
                    to = to.Replace(";", "; ");
                    res += to;
                }

                string command = GetItem_(WordType.I35)[2];
                command = command.Replace("R", GetItem_(WordType.I02)[2]);
                command = command.Replace("S", GetItem_(WordType.I02)[3]);
                command = command.Replace("C0", GetItem_(WordType.I35)[3]);
                res += command;

                res = res.Replace("A", GetRawValue(WordType.A).ToString());
                res = res.Replace("B", GetRawValue(WordType.B).ToString());
                res = res.Replace("D", GetRawValue(WordType.D).ToString());
                res = res.Replace("+0", "");
                res = res.Replace("-0", "");
                res = res.Replace("-1+1", "");

                res += "; M1=" + (GetFlag(FlagType.M1) ? "1" : "0");
                res += "; M0=" + (GetFlag(FlagType.M0) ? "1" : "0");
                break;
            case CommandType.MemoryPointer:
                res += $"Memory Poiter = 0x{((GetRawValue(WordType.A) << 4) + GetRawValue(WordType.B)):X2}";
                res += "; NewPtr = ";
                if (GetRawValue(WordType.Inc) == 1) {
                    res += "OldPtr + 1";
                }
                else if (GetRawValue(WordType.Inc) == 2) {
                    res += "OldPtr - 1";
                }
                else {
                    res += "OldPtr";
                }
                break;
            case CommandType.DevicePointer:
                res += "Device Poiter = " + ((GetRawValue(WordType.A) << 4) + GetRawValue(WordType.B));
                break;
            case CommandType.LoadSmallCommand:
            case CommandType.LoadCommand:
                switch (GetRawValue(WordType.I35)) {
                case 12:
                    if (GetRawValue(WordType.PS) == 0) {
                        res += $"Memory(Ptr) = РОН({ GetRawValue(WordType.B) })";
                    }
                    else {
                        res += $"Memory(Ptr) = РОН({ GetRawValue(WordType.A) }) << 4 + РОН({ GetRawValue(WordType.B) })";
                    }
                    break;
                case 13:
                    if (GetRawValue(WordType.PS) == 0) {
                        res += $"РОН({ GetRawValue(WordType.B) }) = LOW(Memory(Ptr))";
                    }
                    else {
                        res += $"РОН({ GetRawValue(WordType.A) }) = HIGH(Memory(Ptr))";
                        res += $"; РОН({ GetRawValue(WordType.B) }) = LOW(Memory(Ptr))";
                    }
                    break;
                case 14:
                case 15:
                    // TODO: Maybe to do device registers
                    break;
                }
                break;
            default:
                res += String.Join(" ", words_);
                break;
            }

            res = $"0x{number_:X3}; " + res;

            res += "; " + GetItem_(WordType.CA)[2];
            JumpType jt = GetJumpType();
            if (jt == JumpType.JNZ || jt == JumpType.JMP || jt == JumpType.CLNZ
                || jt == JumpType.CALL || jt == JumpType.JZ || jt == JumpType.JF3
                || jt == JumpType.JOVR || jt == JumpType.JC4) {
                res += " " + $"0x{GetNextAdr():X3}";
            }
            return res;
        }

        public CommandType GetCommandType()
        {
            if (isOffset) {
                return CommandType.Offset;
            }
            else if (GetRawValue(WordType.I35) <= 10) {
                return CommandType.MtCommand;
            }
            else if (GetRawValue(WordType.I35) == 11) {
                if (GetRawValue(WordType.PT) <= 7) {
                    return CommandType.MemoryPointer;
                }
                else {
                    return CommandType.DevicePointer;
                }
            }
            else if (12 <= GetRawValue(WordType.I35) && GetRawValue(WordType.I35) <= 15) {
                if (GetRawValue(WordType.PS) == 0) {
                    return CommandType.LoadSmallCommand;
                }
                else {
                    return CommandType.LoadCommand;
                }
            }
            return CommandType.Unknown;
        }

        public int this[int i] {
            get { return words_[i]; }
            set { words_[i] = value % 16; }
        }

        public string GetLabel(int textIndex)
        {
            return labels_[GetCommandType()][textIndex];
        }

        public int GetRawValue(WordType type)
        {
            int textIndex = GetTextIndexByType(type);
            if (textIndex == -1) {
                return -1;
            }
            return words_[textIndex];
        }

        public int GetSelIndex(WordType type)
        {
            int raw = GetRawValue(type);
            if (raw == -1) {
                return -1;
            }

            if (type == WordType.I02 || type == WordType.I68) {
                return raw % 8;
            }
            else if (type == WordType.PT) {
                if (raw == 8) {
                    return 3;
                }
                else if (raw < 3) {
                    return raw;
                }
                else {
                    return -1;
                }
            }
            else if (type == WordType.Device) {
                if (raw < 2) {
                    return raw;
                }
                else {
                    return -1;
                }
            }
            else if (type == WordType.PS) {
                if (raw < 2) {
                    return raw;
                }
                else {
                    return -1;
                }
            }
            else {
                return raw;
            }
        }

        public void SetValue(WordType type, int selIndex)
        {
            int textIndex = GetTextIndexByType(type);
            if (textIndex == -1) {
                return;
            }

            if (type == WordType.I02 || type == WordType.I68) {
                int oldHigh = words_[textIndex] / 8;
                words_[textIndex] = oldHigh * 8 + selIndex % 8;
            }
            else if (type == WordType.PT) {
                if (selIndex == 3) {
                    words_[textIndex] = 8;
                }
                else {
                    words_[textIndex] = selIndex % 16;
                }
            }
            else {
                words_[textIndex] = selIndex % 16;
            }
        }

        public bool GetFlag(FlagType flagIndex)
        {
            int textIndex = GetTextIndexByFlag(flagIndex);
            if (textIndex == -1) {
                return false;
            }
            return words_[textIndex] / 8 > 0;
        }

        public void SetFlag(FlagType flagIndex, bool value)
        {
            int textIndex = GetTextIndexByFlag(flagIndex);
            if (textIndex == -1) {
                return;
            }

            int oldLow = words_[textIndex] % 8;
            words_[textIndex] = (value ? 8 : 0) + oldLow;
        }

        public JumpType GetJumpType()
        {
            return (JumpType) GetRawValue(WordType.CA);
        }

        public int GetNextAdr()
        {
            return (GetRawValue(WordType.ArHigh) << 8)
                + (GetRawValue(WordType.ArMid) << 4)
                + GetRawValue(WordType.ArLow);
        }
    }
}

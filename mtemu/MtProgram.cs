using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mtemu
{
    enum CommandType : byte
    {
        MtCommand,
        MemoryPointer,
        DevicePointer,
        LoadCommand,
        LoadSmallCommand,
        Unknown = 255,
    }

    enum JumpType : byte
    {
        JNZ,
        JMP,
        JNXT,
        JADR,
        CLNZ,
        CALL,
        RET,
        JSP,
        JSNZ,
        PUSH,
        POP,
        JSNC4,
        JZ,
        JF3,
        JOVR,
        JC4,
    }

    enum WordType : byte
    {
        ArHigh,
        ArMid,
        ArLow,
        CA,
        I68,
        I02,
        I35,
        A,
        B,
        D,
        PT,
        PS,
        Device,
        Unknown = 255,
    }

    enum FlagType : byte
    {
        M0,
        M1,
        Unknown = 255,
    }

    class MtCommand
    {
        private static int length_ = 10;

        // Numbers of text boxes
        private static Dictionary<WordType, int> wordIndexes_ =
            new Dictionary<WordType, int>
        {
            { WordType.ArHigh, 0 },
            { WordType.ArMid, 1 },
            { WordType.ArLow, 2 },
            { WordType.CA, 3 },
            { WordType.I68, 4 },
            { WordType.I02, 5 },
            { WordType.I35, 6 },
            { WordType.A, 7 },
            { WordType.B, 8 },
            { WordType.D, 9 },
            { WordType.PT, 5 },
            { WordType.PS, 5 },
            { WordType.Device, 7 },
        };

        private static Dictionary<CommandType, string[]> labels_ =
            new Dictionary<CommandType, string[]>
        {
            {
                CommandType.MtCommand, new string[] {
                    "AR",
                    "CA",
                    "M1|I6-I8",
                    "M0|I0-I2",
                    "C0|I3-I5",
                    "A",
                    "B",
                    "D",
                }
            },
            {
                CommandType.MemoryPointer, new string[] {
                    "AR",
                    "CA",
                    "",
                    "PT|Inc",
                    "F",
                    "RA",
                    "RB",
                    "",
                }
            },
            {
                CommandType.DevicePointer, new string[] {
                    "AR",
                    "CA",
                    "",
                    "PT",
                    "F",
                    "RA",
                    "RB",
                    "",
                }
            },
            {
                CommandType.LoadCommand, new string[] {
                    "AR",
                    "CA",
                    "",
                    "PS",
                    "F",
                    "RA",
                    "RB",
                    "",
                }
            },
            {
                CommandType.LoadSmallCommand, new string[] {
                    "AR",
                    "CA",
                    "",
                    "PS",
                    "F",
                    "",
                    "RB",
                    "",
                }
            },
        };

        private static Dictionary<WordType, string[][]> items_ =
            new Dictionary<WordType, string[][]>
        {
            {
                WordType.CA, new string[][] {
                    new string[] {"","0000","JNZ"},
                    new string[] {"","0001","JMP"},
                    new string[] {"","0010","JNXT"},
                    new string[] {"","0011","JADR"},
                    new string[] {"","0100","CLNZ"},
                    new string[] {"","0101","CALL"},
                    new string[] {"","0110","RET"},
                    new string[] {"","0111","JSP"},
                    new string[] {"","1000","JSNZ"},
                    new string[] {"","1001","PUSH"},
                    new string[] {"","1010","POP"},
                    new string[] {"","1011","JSNC4"},
                    new string[] {"","1100","JZ"},
                    new string[] {"","1101","JF3"},
                    new string[] {"","1110","JOVR"},
                    new string[] {"","1111","JC4"},
                }
            },
            {
                WordType.I68, new string[][] {
                    new string[] {"","000","PQ=F"},
                    new string[] {"","001","Нет загрузки"},
                    new string[] {"","010","РОН(B)=F"},
                    new string[] {"","011","РОН(B)=F"},
                    new string[] {"","100","PQ=Q/2,РОН(B)=F/2"},
                    new string[] {"","101","РОН(B)=F/2"},
                    new string[] {"","110","PQ=2Q,РОН(B)=2F"},
                    new string[] {"","111","РОН(B)=2F"},
                }
            },
            {
                WordType.I02, new string[][] {
                    new string[] {"","000","РОН(A)","PQ"},
                    new string[] {"","001","РОН(A)","РОН(B)"},
                    new string[] {"","010","0","PQ"},
                    new string[] {"","011","0","РОН(B)"},
                    new string[] {"","100","0","РОН(A)"},
                    new string[] {"","101","D","РОН(A)"},
                    new string[] {"","110","D","PQ"},
                    new string[] {"","111","D","0"},
                }
            },
            {
                WordType.I35, new string[][] {
                    new string[] {"","0000","R+S+C0","0"},
                    new string[] {"","0001","S-R-1+C0","0"},
                    new string[] {"","0010","R-S-1+C0","0"},
                    new string[] {"","0011","R∨S","-"},
                    new string[] {"","0100","R∧S","-"},
                    new string[] {"","0101","¬R∧S","-"},
                    new string[] {"","0110","R⊕S","-"},
                    new string[] {"","0111","¬(R⊕S)","-"},
                    new string[] {"","1000","R+S+C0","1"},
                    new string[] {"","1001","S-R-1+C0","1"},
                    new string[] {"","1010","R-S-1+C0","1"},
                    new string[] {"","1011","","-"},
                    new string[] {"","1100","","-"},
                    new string[] {"","1101","","-"},
                    new string[] {"","1110","","-"},
                    new string[] {"","1111","","-"},
                }
            },
            {
                WordType.PT, new string[][] {
                    new string[] {"","0000","Память; P=P"},
                    new string[] {"","0001","Память; P=P+1"},
                    new string[] {"","0010","Память; P=P-1"},
                    new string[] {"","1000","Регистр устр."},
                }
            },
            {
                WordType.PS, new string[][] {
                    new string[] {"","0000","4 бита"},
                    new string[] {"","0001","8 бит"},
                }
            },
            {
                WordType.Device, new string[][] {
                    new string[] {"","0000","GPIO0"},
                    new string[] {"","0001","GPIO1"},
                }
            },
        };

        private int[] words_;

        public MtCommand(string[] strWords)
        {
            if (strWords.Length != length_) {
                throw new ArgumentException("Count of words must be equal 10!");
            }
            words_ = new int[length_];
            for (int i = 0; i < length_; ++i) {
                words_[i] = Helpers.BinaryToInt(strWords[i]);
            }
        }

        public MtCommand(MtCommand other)
        {
            words_ = new int[length_];
            Array.Copy(other.words_, words_, length_);
        }

        private string[] GetItem_(WordType type)
        {
            return items_[type][GetSelIndex_(type)];
        }

        public string GetName(int index)
        {
            string res = "#";
            string number = index.ToString();
            for (int i = 0; i < 4 - number.Length; ++i) {
                res += "0";
            }
            res += number + "; ";

            switch (GetCommandType()) {
            case CommandType.MtCommand:
                if (GetRawValue_(WordType.I68) == 1) {
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

                res = res.Replace("A", GetRawValue_(WordType.A).ToString());
                res = res.Replace("B", GetRawValue_(WordType.B).ToString());
                res = res.Replace("D", GetRawValue_(WordType.D).ToString());
                res = res.Replace("+0", "");
                res = res.Replace("-0", "");
                res = res.Replace("-1+1", "");

                res += "; M1=" + (GetFlag(FlagType.M1) ? "1" : "0");
                res += "; M0=" + (GetFlag(FlagType.M0) ? "1" : "0");
                break;
            default:
                res += String.Join(" ", words_);
                break;
            }
            res += "; " + GetItem_(WordType.CA)[2];
            JumpType jt = GetJumpType();
            if (jt == JumpType.JNZ || jt == JumpType.JMP || jt == JumpType.CLNZ
                || jt == JumpType.CALL || jt == JumpType.JZ || jt == JumpType.JF3
                || jt == JumpType.JOVR || jt == JumpType.JC4) {
                res += " " + GetNext();
            }
            return res;
        }

        public CommandType GetCommandType()
        {
            if (GetRawValue_(WordType.I35) <= 10) {
                return CommandType.MtCommand;
            }
            else if (GetRawValue_(WordType.I35) == 11) {
                if (GetRawValue_(WordType.PT) <= 7) {
                    return CommandType.MemoryPointer;
                }
                else {
                    return CommandType.DevicePointer;
                }
            }
            else if (12 <= GetRawValue_(WordType.I35) && GetRawValue_(WordType.I35) <= 15) {
                return CommandType.LoadCommand;
            }
            else {
                return CommandType.Unknown;
            }
        }

        public int this[int i] {
            get { return words_[i]; }
            set { words_[i] = value; }
        }

        public static int GetTextIndexByType(WordType type)
        {
            if (wordIndexes_.ContainsKey(type)) {
                return wordIndexes_[type];
            }
            return -1;
        }

        public static int GetTextIndexByFlag(FlagType type)
        {
            switch (type) {
            case FlagType.M0:
                return wordIndexes_[WordType.I02];
            case FlagType.M1:
                return wordIndexes_[WordType.I68];
            }
            return -1;
        }

        public static string[][] GetItems(WordType listIndex)
        {
            return items_[listIndex];
        }

        public string GetLabel(int textIndex)
        {
            return labels_[GetCommandType()][textIndex];
        }

        private int GetRawValue_(WordType type)
        {
            int textIndex = GetTextIndexByType(type);
            if (textIndex == -1) {
                return -1;
            }
            return words_[textIndex];
        }

        private int GetSelIndex_(WordType type)
        {
            int raw = GetRawValue_(type);
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
                else if (raw < 3){
                    return raw;
                }
                else {
                    return -1;
                }
            }
            else if (type == WordType.Device) {
                return raw % 2;
            }
            else if (type == WordType.PS) {
                return raw % 2;
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
                words_[textIndex] = oldHigh * 8 + selIndex;
            }
            else if (type == WordType.PT) {
                if (selIndex == 3) {
                    words_[textIndex] = 8;
                }
                else {
                    words_[textIndex] = selIndex;
                }
            }
            else {
                words_[textIndex] = selIndex;
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
            return (JumpType) GetRawValue_(WordType.CA);
        }

        public int GetNext()
        {
            return (GetRawValue_(WordType.ArHigh) << 8)
                + (GetRawValue_(WordType.ArMid) << 4)
                + GetRawValue_(WordType.ArLow);
        }
    }

    class MtProgram
    {
        private List<MtCommand> commands_ = new List<MtCommand>();

        public MtProgram()
        { }

        public void AddCommand(MtCommand command)
        {
            commands_.Add(command);
        }

        public void RemoveCommand(int index)
        {
            commands_.RemoveAt(index);
        }

        public int Count()
        {
            return commands_.Count();
        }
        public MtCommand this[int i] {
            get { return commands_[i]; }
            set { commands_[i] = value; }
        }
    }
}

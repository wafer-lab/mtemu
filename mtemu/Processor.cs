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
        C0,
        Unknown = 255,
    }

    class Command
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

        public Command(Command other)
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
            default:
                res += String.Join(" ", words_);
                break;
            }
            res += "; " + GetItem_(WordType.CA)[2];
            JumpType jt = GetJumpType();
            if (jt == JumpType.JNZ || jt == JumpType.JMP || jt == JumpType.CLNZ
                || jt == JumpType.CALL || jt == JumpType.JZ || jt == JumpType.JF3
                || jt == JumpType.JOVR || jt == JumpType.JC4) {
                res += " " + GetNextAdr();
            }
            return res;
        }

        public CommandType GetCommandType()
        {
            if (GetRawValue(WordType.I35) <= 10) {
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
                return CommandType.LoadCommand;
            }
            else {
                return CommandType.Unknown;
            }
        }

        public int this[int i] {
            get { return words_[i]; }
            set { words_[i] = value % 16; }
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
            case FlagType.C0:
                return wordIndexes_[WordType.I35];
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

        public int GetRawValue(WordType type)
        {
            int textIndex = GetTextIndexByType(type);
            if (textIndex == -1) {
                return -1;
            }
            return words_[textIndex];
        }

        private int GetSelIndex_(WordType type)
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

    class Processor
    {
        private static int stackSize_ = 4;
        private static int regSize_ = 16;
        private static int maxAutoCount_ = 1 << 14;

        private int prevPC_;
        private int pc_;
        private List<Command> commands_ = new List<Command>();

        private int sp_;
        private int[] stack_ = new int[stackSize_];

        private int regQ_;
        private int[] regCommon_ = new int[regSize_];

        private int f_;

        private bool z_;
        private bool f3_;
        private bool c4_;
        private bool ovr_;
        private bool g_;
        private bool p_;

        public static int GetStackSize()
        {
            return stackSize_;
        }

        public static int GetRegSize()
        {
            return regSize_;
        }

        public Processor()
        {
            Reset();
        }

        public void AddCommand(Command command)
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
        public Command this[int i] {
            get { return commands_[i]; }
            set { commands_[i] = value; }
        }

        public void Reset()
        {
            prevPC_ = -1;
            pc_ = 0;
            sp_ = 0;
            regQ_ = 0;
            f_ = 0;
            z_ = false;
            f3_ = false;
            c4_ = false;
            ovr_ = false;
            g_ = false;
            p_ = false;
        }

        private Command Current_()
        {
            return commands_[pc_];
        }

        private bool Jump_()
        {
            prevPC_ = pc_;
            switch (Current_().GetJumpType()) {
            case JumpType.JNZ:
                if (!z_) {
                    pc_ = Current_().GetNextAdr();
                    return true;
                }
                break;
            case JumpType.JMP:
                pc_ = Current_().GetNextAdr();
                return true;
            case JumpType.JADR:
                pc_ = 0; // TODO: Check this shit: Current_().GetRawValue(WordType.D)
                return true;
            case JumpType.CLNZ:
                if (!z_) {
                    stack_[sp_] = pc_ + 1;
                    ++sp_;
                    pc_ = Current_().GetNextAdr();
                    return true;
                }
                break;
            case JumpType.CALL:
                stack_[sp_] = pc_ + 1;
                ++sp_;
                pc_ = Current_().GetNextAdr();
                return true;
            case JumpType.RET:
                pc_ = stack_[sp_ - 1];
                --sp_;
                return true;
            case JumpType.JSP:
                pc_ = stack_[sp_ - 1];
                return true;
            case JumpType.JSNZ:
                if (!z_) {
                    pc_ = stack_[sp_ - 1];
                    return true;
                }
                else {
                    --sp_;
                }
                break;
            case JumpType.PUSH:
                stack_[sp_] = pc_ + 1;
                ++sp_;
                break;
            case JumpType.POP:
                --sp_;
                break;
            case JumpType.JSNC4:
                if (!c4_) {
                    pc_ = stack_[sp_ - 1];
                    return true;
                }
                else {
                    --sp_;
                }
                break;
            case JumpType.JZ:
                if (z_) {
                    pc_ = Current_().GetNextAdr();
                    return true;
                }
                break;
            case JumpType.JF3:
                if (f3_) {
                    pc_ = Current_().GetNextAdr();
                    return true;
                }
                break;
            case JumpType.JOVR:
                if (ovr_) {
                    pc_ = Current_().GetNextAdr();
                    return true;
                }
                break;
            case JumpType.JC4:
                if (c4_) {
                    pc_ = Current_().GetNextAdr();
                    return true;
                }
                break;
            }
            ++pc_;
            return false;
        }

        private void ExecMtCommand_()
        {
            int from = Current_().GetRawValue(WordType.I02) % 8;
            int alu = Current_().GetRawValue(WordType.I35) % 8;
            int to = Current_().GetRawValue(WordType.I68) % 8;
            int a = Current_().GetRawValue(WordType.A);
            int b = Current_().GetRawValue(WordType.B);
            int d = Current_().GetRawValue(WordType.D);

            int c0 = Current_().GetFlag(FlagType.C0) ? 1 : 0;
            int m0 = Current_().GetFlag(FlagType.M0) ? 1 : 0;
            int m1 = Current_().GetFlag(FlagType.M1) ? 1 : 0;

            int opA = 0;
            int opB = 0;

            switch (from) {
            case 0:
                opA = regCommon_[a];
                opB = regQ_;
                break;
            case 1:
                opA = regCommon_[a];
                opB = regCommon_[b];
                break;
            case 2:
                opA = 0;
                opB = regQ_;
                break;
            case 3:
                opA = 0;
                opB = regCommon_[b];
                break;
            case 4:
                opA = 0;
                opB = regCommon_[a];
                break;
            case 5:
                opA = d;
                opB = regCommon_[a];
                break;
            case 6:
                opA = d;
                opB = regQ_;
                break;
            case 7:
                opA = d;
                opB = 0;
                break;
            }

            switch (alu) {
            case 0:
                f_ = opA + opB + c0;
                break;
            case 2:
                f_ = opA + (~opB & 15) + c0;
                break;
            case 1:
                f_ = opB + (~opA & 15) + c0;
                break;
            case 3:
                f_ = opB | opA;
                break;
            case 4:
                f_ = opB & opA;
                break;
            case 5:
                f_ = opB & (~opA & 15);
                break;
            case 6:
                f_ = opB ^ opA;
                break;
            case 7:
                f_ = (~(opB ^ opA)) & 15;
                break;
            }

            z_ = (f_ & 15) == 0;
            f3_ = (f_ & 8) != 0;
            c4_ = (f_ & 16) != 0;
            ovr_ = c4_ != f3_;
            f_ = f_ & 15;

            int qLow = regQ_ & 1;
            int qHigh = (regQ_ & 8) >> 3;
            int fLow = f_ & 1;
            int fHigh = (f_ & 8) >> 3;

            switch (to) {
            case 0:
                regQ_ = f_;
                break;
            case 1:
                break;
            case 2:
                regCommon_[b] = f_;
                break;
            case 3:
                regCommon_[b] = f_;
                f_ = regCommon_[a]; // TODO: Check this shit
                break;
            case 4:
                regQ_ = regQ_ >> 1;
                regCommon_[b] = f_ >> 1;
                switch (m1 << 1 + m0) {
                case 1:
                    regCommon_[b] |= fLow << 3;
                    regQ_ |= qLow << 3;
                    break;
                case 2:
                    regCommon_[b] |= qLow << 3;
                    regQ_ |= fLow << 3;
                    break;
                case 3:
                    regCommon_[b] |= fHigh << 3;
                    regQ_ |= fLow << 3;
                    break;
                }
                break;
            case 5:
                regCommon_[b] = f_ >> 1;
                switch (m1 << 1 + m0) {
                case 1:
                    regCommon_[b] |= fLow << 3;
                    break;
                }
                break;
            case 6:
                regQ_ = (regQ_ << 1) & 15;
                regCommon_[b] = (f_ << 1) & 15;
                switch (m1 << 1 + m0) {
                case 1:
                    regCommon_[b] |= fHigh;
                    regQ_ |= qHigh;
                    break;
                case 2:
                    regCommon_[b] |= qHigh;
                    regQ_ |= fHigh;
                    break;
                case 3:
                    regCommon_[b] |= qHigh;
                    break;
                }
                break;
            case 7:
                regCommon_[b] = (f_ << 1) & 15;
                switch (m1 << 1 + m0) {
                case 1:
                    regCommon_[b] |= fHigh;
                    break;
                }
                break;
            }
        }

        public void ExecOne()
        {
            if (commands_.Count() == 0) {
                return;
            }
            if (pc_ >= commands_.Count()) {
                pc_ = 0;
            }

            // Save flags to restore then after command exec
            bool prevZ = z_;
            bool prevF3 = f3_;
            bool prevC4 = c4_;
            bool prevOvr = ovr_;

            switch (Current_().GetCommandType()) {
            case CommandType.MtCommand:
                ExecMtCommand_();
                break;
            }

            // To use old values in jumper
            bool newZ = z_;
            bool newF3 = f3_;
            bool newC4 = c4_;
            bool newOvr = ovr_;
            z_ = prevZ;
            f3_ = prevF3;
            c4_ = prevC4;
            ovr_ = prevOvr;

            // Update to new flags only if jump next
            if (!Jump_()) {
                z_ = newZ;
                f3_ = newF3;
                c4_ = newC4;
                ovr_ = newOvr;
            }
        }

        public bool ExecAll()
        {
            for (int i = 0; i < maxAutoCount_; ++i) {
                ExecOne();
                if (prevPC_ == pc_) {
                    return true;
                }
            }
            return false;
        }

        public int GetPrevPC()
        {
            return prevPC_;
        }

        public int GetPC()
        {
            return pc_;
        }

        public int GetSP()
        {
            return sp_;
        }
        public int GetStackValue(int index)
        {
            return stack_[index];
        }

        public int GetRegQ()
        {
            return regQ_;
        }

        public int GetRegValue(int index)
        {
            return regCommon_[index];
        }

        public int GetF()
        {
            return f_;
        }

        public bool GetZ()
        {
            return z_;
        }

        public bool GetF3()
        {
            return f3_;
        }

        public bool GetC4()
        {
            return c4_;
        }

        public bool GetOvr()
        {
            return ovr_;
        }

        public bool GetG()
        {
            return g_;
        }

        public bool GetP()
        {
            return p_;
        }
    }
}

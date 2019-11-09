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

    enum ListType : byte
    {
        CA = 0,
        I68 = 1,
        I02 = 2,
        I35 = 3,
        PT = 4,
        PS = 5,
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

        private static string[][] labels_ = new string[][] {
            new string[] {
                "AR",
                "CA",
                "M1|I6-I8",
                "M0|I0-I2",
                "C0|I3-I5",
                "A",
                "B",
                "D",
            },
            new string[] {
                "AR",
                "CA",
                "",
                "PT|Inc",
                "F",
                "RA",
                "RB",
                "",
            },
            new string[] {
                "AR",
                "CA",
                "",
                "PT",
                "F",
                "RA",
                "RB",
                "",
            },
            new string[] {
                "AR",
                "CA",
                "",
                "PS",
                "F",
                "RA",
                "RB",
                "",
            },
            new string[] {
                "AR",
                "CA",
                "",
                "PS",
                "F",
                "",
                "RB",
                "",
            },
        };

        private static string[][][] items_ = new string[][][] {
            new string[][] {
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
            },
            new string[][] {
                new string[] {"","000","F->PQ"},
                new string[] {"","001","Нет загрузки"},
                new string[] {"","010","F->РОН(B)"},
                new string[] {"","011","F->РОН(B)"},
                new string[] {"","100","F/2->РОН(B),Q/2->PQ"},
                new string[] {"","101","F/2->РОН(B)"},
                new string[] {"","110","2F->РОН(B),2Q->PQ"},
                new string[] {"","111","2F->РОН(B)"},
            },
            new string[][] {
                new string[] {"","000","РОН(A)","PQ"},
                new string[] {"","001","РОН(A)","РОН(B)"},
                new string[] {"","010","0","PQ"},
                new string[] {"","011","0","РОН(B)"},
                new string[] {"","100","0","РОН(A)"},
                new string[] {"","101","D","РОН(A)"},
                new string[] {"","110","D","PQ"},
                new string[] {"","111","D","0"},
            },
            new string[][] {
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
            },
            new string[][] {
                new string[] {"","0000","Память; P=P"},
                new string[] {"","0001","Память; P=P+1"},
                new string[] {"","0010","Память; P=P-1"},
                new string[] {"","1000","Регистр"},
            },
            new string[][] {
                new string[] {"","0000","4 бита"},
                new string[] {"","0001","8 бит"},
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

        public string GetName(int index)
        {
            string res = "#" + index + "; ";
            foreach (int word in words_) {
                res += Helpers.IntToBinary(word, 4) + " ";
            }
            return res;
        }

        public CommandType GetCommandType()
        {
            if (words_[6] <= 10) {
                return CommandType.MtCommand;
            } 
            else if (words_[6] == 11) {
                if (words_[5] <= 7) {
                    return CommandType.MemoryPointer;
                } else {
                    return CommandType.DevicePointer;
                }
            }
            else if (12 <= words_[6] && words_[6] <= 15) {
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

        //public ListType GetListByTextIndex(int index)
        //{
        //    switch (GetCommandType()) {
        //    case CommandType.MtCommand:
        //        if (3 <= index && index <= 6) {
        //            return (ListType) (index - 3);
        //        }
        //        break;
        //    }
        //    return ListType.NoList;
        //}

        public int GetTextIndexByList(ListType listIndex)
        {
            int index = (int) listIndex;
            if (0 <= index && index <= 3) {
                return 3 + index;
            }
            if (listIndex == ListType.PT || listIndex == ListType.PS) {
                return 5;
            }
            return -1;
        }

        public int GetTextIndexByFlag(FlagType flagIndex)
        {
            switch (flagIndex) {
            case FlagType.M0:
                return 5;
            case FlagType.M1:
                return 4;
            }
            return -1;
        }

        public static string[][] GetList(ListType listIndex)
        {
            return items_[(int)listIndex];
        }

        public string GetLabel(int textIndex)
        {
            return labels_[(int) GetCommandType()][textIndex];
        }

        //public int GetSelectedIndex(ListType listIndex)
        //{
        //    int textIndex = GetTextIndexByList(listIndex);
        //    if (textIndex == -1) {
        //        return 0;
        //    }

        //    switch (GetCommandType()) {
        //    case CommandType.MtCommand:
        //        if (listIndex == ListType.I02 || listIndex == ListType.I68) {
        //            return words_[textIndex] % 8;
        //        }
        //        else {
        //            return words_[textIndex];
        //        }
        //    }
        //    return 0;
        //}

        public void SetBinary(ListType listIndex, int selIndex)
        {
            int textIndex = GetTextIndexByList(listIndex);
            if (textIndex == -1) {
                return;
            }

            if (listIndex == ListType.I02 || listIndex == ListType.I68) {
                int oldHigh = words_[textIndex] / 8;
                words_[textIndex] = oldHigh * 8 + selIndex;
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
            return (JumpType) words_[1];
        }

        public int GetNext()
        {
            return words_[0];
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

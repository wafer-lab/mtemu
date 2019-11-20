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
        Offset,
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
        Inc,
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

    enum IncType : byte
    {
        No = 0,
        Plus,
        Minus,
    }

    partial class Command
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
            { WordType.Inc, 5 },
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
            {
                CommandType.Offset, new string[] {
                    "Offset",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
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
                    new string[] {"","1011","Set Ptr","-"},
                    new string[] {"","1100","Str Mem","-"},
                    new string[] {"","1101","Load Mem","-"},
                    new string[] {"","1110","Str Dev","-"},
                    new string[] {"","1111","Load Dev","-"},
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
                    new string[] {"","0000","Младш. 4 бита"},
                    new string[] {"","0001","Старш. 4 бита"},
                    new string[] {"","0010","8 бит"},
                }
            },
            {
                WordType.Device, new string[][] {
                    new string[] {"","0000","GPIO0"},
                    new string[] {"","0001","GPIO1"},
                    new string[] {"","0010","GPIO2"},
                    new string[] {"","0011","GPIO3"},
                }
            },
        };

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

        public static Command GetDefault()
        {
            return new Command(new string[] {
                "0000", "0000", "0000", "0010", "0001", "0111", "0000", "0000", "0000", "0000",
            });
        }
    }

    partial class Emulator
    {
        private static Command incorrectCommand_ = new Command(new int[] { 0xF, 0xF, 0xF, 0xF, 0xF, 0xF, 0xF, 0xF, 0xF, 0xF });

        private static int commandSize_ = 5;
        private static byte[] fileHeader_ = Encoding.ASCII.GetBytes("MTEM");

        private static int programSize_ = 1 << 12;
        private static int stackSize_ = 1 << 4;
        private static int regSize_ = 1 << 4;
        private static int memSize_ = 1 << 8;
        private static int maxAutoCount_ = 1 << 14;

        public static int GetStackSize()
        {
            return stackSize_;
        }

        public static int GetRegSize()
        {
            return regSize_;
        }

        public static int GetMemorySize()
        {
            return memSize_;
        }
    }
}
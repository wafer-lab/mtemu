using System;

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
                words_[i] = Helpers.Mask(Helpers.BinaryToInt(strWords[i]));
            }
        }

        public Command(int[] words)
        {
            if (words.Length != length_) {
                throw new ArgumentException("Count of words must be equal 10!");
            }
            words_ = new int[length_];
            for (int i = 0; i < length_; ++i) {
                words_[i] = Helpers.Mask(words[i]);
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
            if (GetCommandView() == ViewType.OFFSET) {
                return $"OFFSET = 0x{GetNextAddr():X3}";
            }

            string res = "";

            switch (GetCommandView()) {
            case ViewType.MT_COMMAND:
                if (GetToType() == ToType.NO_LOAD) {
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

            case ViewType.MEMORY_POINTER:
                res += $"MemoryPtr=0x{((GetRawValue(WordType.A) << 4) + GetRawValue(WordType.B)):X2}";
                res += "; NewPtr=";
                switch (GetIncType()) {
                case IncType.NO:
                    res += "OldPtr";
                    break;
                case IncType.PLUS:
                    res += "OldPtr+1";
                    break;
                case IncType.MINUS:
                    res += "OldPtr-1";
                    break;
                }
                break;

            case ViewType.DEVICE_POINTER:
                dev_ptr_str_ = GetItem_(WordType.DEVICE)[2];
                res += $"Interface={ dev_ptr_str_ }";
                break;

            case ViewType.LOAD_HIGH_4BIT:
            case ViewType.LOAD_LOW_4BIT:
            case ViewType.LOAD_8BIT:
                switch (GetFuncType()) {
                case FuncType.STORE_MEMORY:
                    if (GetRawValue(WordType.PS) == 0) {
                        res += $"LOW(Memory(Ptr))=РОН({ GetRawValue(WordType.B) })";
                    }
                    else if (GetRawValue(WordType.PS) == 1) {
                        res += $"HIGH(Memory(Ptr))=РОН({ GetRawValue(WordType.A) })";
                    }
                    else {
                        res += $"Memory(Ptr)=(РОН({ GetRawValue(WordType.A) })<<4)+РОН({ GetRawValue(WordType.B) })";
                    }
                    break;

                case FuncType.LOAD_MEMORY:
                    if (GetRawValue(WordType.PS) == 0) {
                        res += $"РОН({ GetRawValue(WordType.B) })=LOW(Memory(Ptr))";
                    }
                    else if (GetRawValue(WordType.PS) == 1) {
                        res += $"РОН({ GetRawValue(WordType.A) })=HIGH(Memory(Ptr))";
                    }
                    else {
                        res += $"РОН({ GetRawValue(WordType.A) })=HIGH(Memory(Ptr))";
                        res += $"; РОН({ GetRawValue(WordType.B) })=LOW(Memory(Ptr))";
                    }
                    break;

                case FuncType.STORE_DEVICE:
                    switch (GetPointerType()) {
                    case DataPointerType.LOW_4_BIT:
                        res += $"LOW({ dev_ptr_str_ })=РОН({ GetRawValue(WordType.B) })";
                        break;
                    case DataPointerType.HIGH_4_BIT:
                        res += $"HIGH({ dev_ptr_str_ })=РОН({ GetRawValue(WordType.A) })";
                        break;
                    case DataPointerType.FULL_8_BIT:
                        res += $"{ dev_ptr_str_ }=(РОН({ GetRawValue(WordType.A) })<<4)+РОН({ GetRawValue(WordType.B) })";
                        break;
                    }
                    break;

                case FuncType.LOAD_DEVICE:
                    switch (GetPointerType()) {
                    case DataPointerType.LOW_4_BIT:
                        res += $"РОН({ GetRawValue(WordType.B) })=LOW({ dev_ptr_str_ })";
                        break;
                    case DataPointerType.HIGH_4_BIT:
                        res += $"РОН({ GetRawValue(WordType.A) })=HIGH({ dev_ptr_str_ })";
                        break;
                    case DataPointerType.FULL_8_BIT:
                        res += $"РОН({ GetRawValue(WordType.A) })=HIGH({ dev_ptr_str_ })";
                        res += $"; РОН({ GetRawValue(WordType.B) })=LOW({ dev_ptr_str_ })";
                        break;
                    }
                    break;
                }
                break;

            default:
                res += String.Join(" ", words_);
                break;
            }

            return res;
        }

        public string GetJumpName()
        {
            string res = GetItem_(WordType.CA)[2];
            JumpType jt = GetJumpType();
            if (jt == JumpType.JNZ || jt == JumpType.JMP || jt == JumpType.CLNZ
                || jt == JumpType.CALL || jt == JumpType.JZ || jt == JumpType.JF3
                || jt == JumpType.JOVR || jt == JumpType.JC4) {
                res += " " + $"0x{GetNextAddr():X3}";
            }
            return res;
        }

        public bool Check()
        {
            if (isOffset) {
                return true;
            }
            if (GetRawValue(WordType.I35) == 11) {
                if (GetRawValue(WordType.PT) > 2 && GetRawValue(WordType.PT) != 8) {
                    return false;
                }
                if (GetRawValue(WordType.PT) == 8 && GetRawValue(WordType.DEVICE) > 3) {
                    return false;
                }
            }
            if (12 <= GetRawValue(WordType.I35) && GetRawValue(WordType.I35) <= 15) {
                if (GetRawValue(WordType.PS) > 2) {
                    return false;
                }
            }
            return true;
        }

        public ViewType GetCommandView()
        {
            if (isOffset) {
                return ViewType.OFFSET;
            }
            else if (GetRawValue(WordType.I35) <= 10) {
                return ViewType.MT_COMMAND;
            }
            else if (GetRawValue(WordType.I35) == 11) {
                if (GetRawValue(WordType.PT) <= 7) {
                    return ViewType.MEMORY_POINTER;
                }
                else {
                    return ViewType.DEVICE_POINTER;
                }
            }
            else if (12 <= GetRawValue(WordType.I35) && GetRawValue(WordType.I35) <= 15) {
                if (GetRawValue(WordType.PS) == 0) {
                    return ViewType.LOAD_LOW_4BIT;
                }
                else if (GetRawValue(WordType.PS) == 1) {
                    return ViewType.LOAD_HIGH_4BIT;
                }
                else {
                    return ViewType.LOAD_8BIT;
                }
            }
            return ViewType.UNKNOWN;
        }

        public int this[int i] {
            get { return words_[i]; }
            set { words_[i] = Helpers.Mask(value); }
        }

        public string GetLabel(int textIndex)
        {
            return labels_[GetCommandView()][textIndex];
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
            else if (type == WordType.DEVICE) {
                if (raw < 4) {
                    return raw;
                }
                else {
                    return -1;
                }
            }
            else if (type == WordType.PS) {
                if (raw < 3) {
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
                int oldHigh = Helpers.GetBit(words_[textIndex], WORD_SIZE - 1);
                words_[textIndex] = oldHigh * Helpers.GetBitMask(WORD_SIZE - 1) + Helpers.Mask(selIndex, WORD_SIZE - 1);
            }
            else if (type == WordType.PT) {
                if (selIndex == 3) {
                    words_[textIndex] = 8;
                }
                else {
                    words_[textIndex] = Helpers.Mask(selIndex);
                }
            }
            else {
                words_[textIndex] = Helpers.Mask(selIndex);
            }
        }

        public bool GetFlag(FlagType flagIndex)
        {
            int textIndex = GetTextIndexByFlag(flagIndex);
            if (textIndex == -1) {
                return false;
            }
            return Helpers.IsBitSet(words_[textIndex], WORD_SIZE - 1);
        }

        public void SetFlag(FlagType flagIndex, bool value)
        {
            int textIndex = GetTextIndexByFlag(flagIndex);
            if (textIndex == -1) {
                return;
            }

            int oldLow = Helpers.Mask(words_[textIndex], WORD_SIZE - 1);
            words_[textIndex] = (value ? 1 << (WORD_SIZE - 1) : 0) + oldLow;
        }

        public int GetNextAddr()
        {
            return (GetRawValue(WordType.AR_HIGH) << (2 * WORD_SIZE))
                + (GetRawValue(WordType.AR_MID) << WORD_SIZE)
                + GetRawValue(WordType.AR_LOW);
        }

        public int GetI02()
        {
            return Helpers.Mask(GetRawValue(WordType.I02), WORD_SIZE - 1);
        }

        public int GetI35()
        {
            return Helpers.Mask(GetRawValue(WordType.I35), WORD_SIZE - 1);
        }

        public int GetI68()
        {
            return Helpers.Mask(GetRawValue(WordType.I68), WORD_SIZE - 1);
        }

        public bool GetC0()
        {
            return Helpers.IsBitSet(GetRawValue(WordType.I35), WORD_SIZE - 1);
        }

        public JumpType GetJumpType()
        {
            byte value = (byte) GetRawValue(WordType.CA);
            if (Enum.IsDefined(typeof(JumpType), value)) {
                return (JumpType) value;
            }
            return JumpType.Unknown;
        }

        public FuncType GetFuncType()
        {
            byte value = (byte) GetRawValue(WordType.I35);
            if (Enum.IsDefined(typeof(FuncType), value)) {
                return (FuncType) value;
            }
            return FuncType.UNKNOWN;
        }

        public FromType GetFromType()
        {
            byte value = (byte) Helpers.Mask(GetRawValue(WordType.I02), WORD_SIZE - 1);
            if (Enum.IsDefined(typeof(FromType), value)) {
                return (FromType) value;
            }
            return FromType.UNKNOWN;
        }

        public ToType GetToType()
        {
            byte value = (byte) Helpers.Mask(GetRawValue(WordType.I68), WORD_SIZE - 1);
            if (Enum.IsDefined(typeof(ToType), value)) {
                return (ToType) value;
            }
            return ToType.UNKNOWN;
        }

        public ShiftType GetShiftType()
        {
            byte m0 = Convert.ToByte(GetFlag(FlagType.M0));
            byte m1 = Convert.ToByte(GetFlag(FlagType.M1));
            byte m10 = (byte) ((m1 << 1) | m0);

            if (Enum.IsDefined(typeof(ShiftType), m10)) {
                return (ShiftType) m10;
            }
            return ShiftType.UNKNOWN;
        }

        public IncType GetIncType()
        {
            byte value = (byte) GetRawValue(WordType.PT);
            if (Enum.IsDefined(typeof(IncType), value)) {
                return (IncType) value;
            }
            return IncType.UNKNOWN;
        }

        public DataPointerType GetPointerType()
        {
            byte value = (byte) GetRawValue(WordType.PS);
            if (Enum.IsDefined(typeof(DataPointerType), value)) {
                return (DataPointerType) value;
            }
            return DataPointerType.UNKNOWN;
        }

        public DeviceType GetDevice()
        {
            byte value = (byte) GetRawValue(WordType.DEVICE);
            if (Enum.IsDefined(typeof(DeviceType), value))
                return (DeviceType) value;

            return DeviceType.UNKNOWN;
        }
    }
}

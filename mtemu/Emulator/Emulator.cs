using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mtemu
{
    partial class Emulator
    {
        private int prevPC_;
        private int pc_;
        private List<Command> commands_ = new List<Command>();

        private int sp_;
        private int[] stack_ = new int[stackSize_];

        private int regQ_;
        private int[] regCommon_ = new int[regSize_];

        private IncType inc_;
        private int mp_;
        private int[] memory_ = new int[memSize_];

        private int f_;

        private bool z_;
        private bool f3_;
        private bool c4_;
        private bool ovr_;
        private bool g_;
        private bool p_;

        public void Reset()
        {
            prevPC_ = -1;
            pc_ = 0;

            sp_ = 0;
            regQ_ = 0;
            inc_ = IncType.No;
            mp_ = 0;

            f_ = 0;
            z_ = false;
            f3_ = false;
            c4_ = false;
            ovr_ = false;
            g_ = false;
            p_ = false;
        }

        public Emulator()
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

        public void MoveCommandUp(int index)
        {
            if (index == 0) {
                return;
            }
            commands_.Insert(index - 1, commands_[index]);
            commands_.RemoveAt(index + 1);
        }

        public void MoveCommandDown(int index)
        {
            if (index == commands_.Count - 1) {
                return;
            }
            commands_.Insert(index + 2, commands_[index]);
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
                    sp_ %= stackSize_;
                    pc_ = Current_().GetNextAdr();
                    return true;
                }
                break;
            case JumpType.CALL:
                stack_[sp_] = pc_ + 1;
                ++sp_;
                sp_ %= stackSize_;
                pc_ = Current_().GetNextAdr();
                return true;
            case JumpType.RET:
                pc_ = stack_[sp_ - 1];
                --sp_;
                sp_ %= stackSize_;
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
                    sp_ %= stackSize_;
                }
                break;
            case JumpType.PUSH:
                stack_[sp_] = pc_ + 1;
                ++sp_;
                sp_ %= stackSize_;
                break;
            case JumpType.POP:
                --sp_;
                sp_ %= stackSize_;
                break;
            case JumpType.JSNC4:
                if (!c4_) {
                    pc_ = stack_[sp_ - 1];
                    return true;
                }
                else {
                    --sp_;
                    sp_ %= stackSize_;
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

        private void SetMemPtr_()
        {
            switch (Current_().GetRawValue(WordType.PT) % 4) {
            case 1:
                inc_ = IncType.Plus;
                break;
            case 2:
                inc_ = IncType.Minus;
                break;
            default:
                inc_ = IncType.No;
                break;
            }

            mp_ = (Current_().GetRawValue(WordType.A) << 4) + Current_().GetRawValue(WordType.B);
        }

        private void SetDevicePtr_()
        {
            // TODO: Maybe to do device registers
        }

        private void LoadData_(bool is8Bit)
        {
            int func = Current_().GetRawValue(WordType.I35);
            int a = Current_().GetRawValue(WordType.A);
            int b = Current_().GetRawValue(WordType.B);

            switch (func) {
            case 12:
                memory_[mp_] = regCommon_[b];
                if (is8Bit) {
                    memory_[mp_] += regCommon_[a] << 4;
                }
                break;
            case 13:
                regCommon_[b] = memory_[mp_] % 16;
                if (is8Bit) {
                    regCommon_[a] = memory_[mp_] >> 4;
                }
                break;
            case 14:
            case 15:
                // TODO: Maybe to do device registers
                break;
            }
            if (func == 12 || func == 13) {
                if (inc_ == IncType.Plus) {
                    ++mp_;
                    mp_ %= memSize_;
                }
                else if (inc_ == IncType.Minus) {
                    --mp_;
                    mp_ %= memSize_;
                }
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
            case CommandType.MemoryPointer:
                SetMemPtr_();
                break;
            case CommandType.DevicePointer:
                SetDevicePtr_();
                break;
            case CommandType.LoadSmallCommand:
                LoadData_(false);
                break;
            case CommandType.LoadCommand:
                LoadData_(true);
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

        public int GetMP()
        {
            return mp_;
        }
        public int GetMemValue(int index)
        {
            return memory_[index];
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

        public bool SaveFile(string filename)
        {
            using (FileStream fstream = new FileStream(filename, FileMode.OpenOrCreate)) {
                int seek = 0;
                byte[] output = new byte[fileHeader_.Length + Count() * commandSize_];
                for(; seek < fileHeader_.Length; ++seek) {
                    output[seek] = fileHeader_[seek];
                }
                Command[] commandsArr = commands_.ToArray();
                for (int i = 0; i < commandsArr.Length; ++i) {
                    for (int j = 0; j < commandSize_; ++j, ++seek) {
                        output[seek] = (byte) ((commandsArr[i][2*j] << 4) + commandsArr[i][2*j + 1]);
                    }
                }
                fstream.Write(output, 0, output.Length);
                fstream.SetLength(fstream.Position);
                return true;
            }
        }

        public bool OpenFile(string filename)
        {
            using (FileStream fstream = File.OpenRead(filename)) {
                byte[] input = new byte[fstream.Length];
                fstream.Read(input, 0, input.Length);

                int seek = 0;
                for (; seek < fileHeader_.Length; ++seek) {
                    if (input[seek] != fileHeader_[seek]) {
                        return false;
                    }
                }

                commands_.Clear();
                Reset();
                int commandsCount = (input.Length - seek) / commandSize_;
                for (int i = 0; i < commandsCount; ++i) {
                    int[] words = new int[commandSize_ * 2];
                    for (int j = 0; j < commandSize_;  ++j, ++seek) {
                        words[2*j] = input[seek] >> 4;
                        words[2*j+1] = input[seek] % 16;
                    }
                    commands_.Add(new Command(words));
                }
                return true;
            }
        }
    }
}

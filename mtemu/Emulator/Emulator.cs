using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace mtemu
{
    partial class Emulator
    {
        private int prevPC_;
        private int pc_;
        private int callIndex_;
        private List<Command> commands_ = new List<Command>();
        private List<Call> calls_ = new List<Call>();

        private int sp_;
        private int[] stack_ = new int[stackSize_];

        private int regQ_;
        private int[] regCommon_ = new int[regSize_];

        private IncType inc_;
        private int mp_;
        private int[] memory_ = new int[memSize_];

        private int prevRegA_;
        private int prevRegB_;
        private int prevRegQ_;
        private int r_;
        private int s_;

        private int f_;
        private int y_;

        private bool z_;
        private bool f3_;
        private bool c4_;
        private bool ovr_;
        private bool g_;
        private bool p_;

        private bool prevZ_;
        private bool prevF3_;
        private bool prevC4_;
        private bool prevOvr_;
        private bool prevG_;
        private bool prevP_;

        public void Reset()
        {
            prevPC_ = -1;
            pc_ = -1;
            callIndex_ = -1;

            sp_ = 0;
            regQ_ = 0;
            inc_ = IncType.No;
            mp_ = 0;

            prevRegA_ = 0;
            prevRegB_ = 0;
            prevRegQ_ = 0;
            r_ = 0;
            s_ = 0;

            f_ = 0;
            y_ = 0;

            z_ = false;
            f3_ = false;
            c4_ = false;
            ovr_ = false;
            g_ = false;
            p_ = false;

            prevZ_ = false;
            prevF3_ = false;
            prevC4_ = false;
            prevOvr_ = false;
            prevG_ = false;
            prevP_ = false;
        }

        public Emulator()
        {
            Reset();
        }

        private void BackupFlags_()
        {
            prevZ_ = z_;
            prevF3_ = f3_;
            prevC4_ = c4_;
            prevOvr_ = ovr_;
            prevG_ = g_;
            prevP_ = p_;
        }

        private void RestoreFlags()
        {
            z_ = prevZ_;
            f3_ = prevF3_;
            c4_ = prevC4_;
            ovr_ = prevOvr_;
            g_ = prevG_;
            p_ = prevP_;
        }

        private int GetOffset_(Command last = null)
        {
            int offset = 0;
            foreach (Command curr in commands_) {
                if (curr == last) {
                    break;
                }
                if (curr.isOffset) {
                    offset = curr.GetNextAddr();
                }
                else {
                    ++offset;
                }
            }
            return offset;
        }

        private int UpdateOffsets_(int first = 0)
        {
            int offset = first == 0 ? -1 : commands_[first - 1].GetNumber();
            for (int i = first; i < commands_.Count; ++i) {
                if (i > 0 && commands_[i - 1].isOffset) {
                    offset = commands_[i - 1].GetNextAddr();
                }
                else {
                    ++offset;
                }
                commands_[i].SetNumber(offset);
            }
            return offset;
        }

        public Command GetCommand(int index)
        {
            return commands_[index];
        }

        public bool AddCommand(Command command)
        {
            if (!command.Check()) {
                return false;
            }
            command.SetNumber(GetOffset_());
            if (command.GetNumber() > programSize_) {
                return false;
            }
            commands_.Add(command);
            return true;
        }

        public bool UpdateCommand(int index, Command command)
        {
            if (!command.Check()) {
                return false;
            }
            commands_[index] = command;
            UpdateOffsets_(index);
            return true;
        }

        public Command LastCommand()
        {
            return commands_.Last();
        }

        public void RemoveCommand(int index)
        {
            commands_.RemoveAt(index);
            UpdateOffsets_(index);
        }

        public void MoveCommandUp(int index)
        {
            if (index <= 0) {
                return;
            }
            commands_.Insert(index - 1, commands_[index]);
            commands_.RemoveAt(index + 1);
            UpdateOffsets_(index - 1);
        }

        public void MoveCommandDown(int index)
        {
            if (index == commands_.Count - 1) {
                return;
            }
            commands_.Insert(index + 2, commands_[index]);
            commands_.RemoveAt(index);
            UpdateOffsets_(index);
        }

        public int CommandsCount()
        {
            return commands_.Count();
        }

        private int GetIndex_(int addr)
        {
            int curr = 0;
            foreach (Command command in commands_) {
                if (command.GetNumber() - addr == 0 && !command.isOffset) {
                    return curr;
                }
                ++curr;
            }
            return -1;
        }

        public Command ExecutedCommand()
        {
            int i = GetIndex_(prevPC_);
            if (i == -1) {
                return incorrectCommand_;
            }
            return commands_[i];
        }

        private Command Current_()
        {
            int i = GetIndex_(pc_);
            if (i == -1) {
                return incorrectCommand_;
            }
            return commands_[i];
        }

        private JumpResult Jump_()
        {
            prevPC_ = pc_;
            switch (Current_().GetJumpType()) {
            case JumpType.JNZ:
                if (!prevZ_) {
                    pc_ = Current_().GetNextAddr();
                    return JumpResult.Address;
                }
                break;
            case JumpType.JMP:
                pc_ = Current_().GetNextAddr();
                return JumpResult.Address;
            case JumpType.EXIT:
                ++callIndex_;
                if (calls_.Count > 0 && callIndex_ < calls_.Count) {
                    pc_ = calls_[callIndex_].GetAddress();
                }
                else {
                    --callIndex_;
                }
                return JumpResult.Address;
            case JumpType.CLNZ:
                if (!prevZ_) {
                    stack_[sp_] = pc_ + 1;
                    ++sp_;
                    sp_ %= stackSize_;
                    pc_ = Current_().GetNextAddr();
                    return JumpResult.Address;
                }
                break;
            case JumpType.CALL:
                stack_[sp_] = pc_ + 1;
                ++sp_;
                sp_ %= stackSize_;
                pc_ = Current_().GetNextAddr();
                return JumpResult.Address;
            case JumpType.RET:
                pc_ = stack_[sp_ - 1];
                --sp_;
                sp_ %= stackSize_;
                return JumpResult.Address;
            case JumpType.JSP:
                pc_ = stack_[sp_ - 1];
                return JumpResult.Address;
            case JumpType.JSNZ:
                if (!prevZ_) {
                    pc_ = stack_[sp_ - 1];
                    return JumpResult.Address;
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
                if (!prevC4_) {
                    pc_ = stack_[sp_ - 1];
                    return JumpResult.Address;
                }
                else {
                    --sp_;
                    sp_ %= stackSize_;
                }
                break;
            case JumpType.JZ:
                if (prevZ_) {
                    pc_ = Current_().GetNextAddr();
                    return JumpResult.Address;
                }
                break;
            case JumpType.JF3:
                if (prevF3_) {
                    pc_ = Current_().GetNextAddr();
                    return JumpResult.Address;
                }
                break;
            case JumpType.JOVR:
                if (prevOvr_) {
                    pc_ = Current_().GetNextAddr();
                    return JumpResult.Address;
                }
                break;
            case JumpType.JC4:
                if (prevC4_) {
                    pc_ = Current_().GetNextAddr();
                    return JumpResult.Address;
                }
                break;
            }
            ++pc_;
            return JumpResult.Next;
        }

        private void ExecMtCommand_()
        {
            FromType from = Current_().GetFromType();
            FuncType alu = Current_().GetFuncType();
            ToType to = Current_().GetToType();
            ShiftType shift = Current_().GetShiftType();

            int a = Current_().GetRawValue(WordType.A);
            int b = Current_().GetRawValue(WordType.B);
            int d = Current_().GetRawValue(WordType.D);

            prevRegQ_ = regQ_;
            prevRegA_ = regCommon_[a];
            prevRegB_ = regCommon_[b];

            switch (from) {
            case FromType.A_AND_PQ:
                r_ = regCommon_[a];
                s_ = regQ_;
                break;
            case FromType.A_AND_B:
                r_ = regCommon_[a];
                s_ = regCommon_[b];
                break;
            case FromType.ZERO_AND_Q:
                r_ = 0;
                s_ = regQ_;
                break;
            case FromType.ZERO_AND_B:
                r_ = 0;
                s_ = regCommon_[b];
                break;
            case FromType.ZERO_AND_A:
                r_ = 0;
                s_ = regCommon_[a];
                break;
            case FromType.D_AND_A:
                r_ = d;
                s_ = regCommon_[a];
                break;
            case FromType.D_AND_Q:
                r_ = d;
                s_ = regQ_;
                break;
            case FromType.D_AND_ZERO:
                r_ = d;
                s_ = 0;
                break;
            }

            switch (alu) {
            case FuncType.R_PLUS_S:
                f_ = r_ + s_;
                break;
            case FuncType.R_PLUS_S_PLUS_1:
                f_ = r_ + s_ + 1;
                break;
            case FuncType.S_MINUS_R_MINUS_1:
                f_ = s_ - r_ - 1;
                break;
            case FuncType.S_MINUS_R:
                f_ = s_ - r_;
                break;
            case FuncType.R_MINUS_S_MINUS_1:
                f_ = r_ - s_ - 1;
                break;
            case FuncType.R_MINUS_S:
                f_ = r_ - s_;
                break;
            case FuncType.R_OR_S:
                f_ = r_ | s_;
                break;
            case FuncType.R_AND_S:
                f_ = r_ & s_;
                break;
            case FuncType.NO_R_AND_S:
                f_ = Helpers.Mask(~r_) & s_;
                break;
            case FuncType.R_XOR_S:
                f_ = r_ ^ s_;
                break;
            case FuncType.R_EQ_S:
                f_ = Helpers.Mask(~(r_ ^ s_));
                break;
            }

            f3_ = Helpers.IsBitSet(f_, Command.WORD_SIZE - 1);
            g_ = f3_;
            c4_ = Helpers.IsBitSet(f_, Command.WORD_SIZE);
            ovr_ = c4_ != f3_;
            p_ = ovr_;
            f_ = Helpers.Mask(f_);
            z_ = f_ == 0;

            int qLow = Helpers.GetBit(regQ_, 0);
            int qHigh = Helpers.GetBit(regQ_, Command.WORD_SIZE - 1);
            int fLow = Helpers.GetBit(f_, 0);
            int fHigh = Helpers.GetBit(f_, Command.WORD_SIZE - 1);

            switch (to) {
            case ToType.F_IN_Q:
                regQ_ = f_;
                break;
            case ToType.NO_LOAD:
                break;
            case ToType.F_IN_B_AND_A_IN_Y:
            case ToType.F_IN_B:
                regCommon_[b] = f_;
                break;
            case ToType.SR_F_IN_B_AND_SR_Q_IN_Q:
                regQ_ = regQ_ >> 1;
                regCommon_[b] = f_ >> 1;
                switch (shift) {
                case ShiftType.CYCLE:
                    regCommon_[b] |= fLow << 3;
                    regQ_ |= qLow << 3;
                    break;
                case ShiftType.CYCLE_DOUBLE:
                    regCommon_[b] |= qLow << 3;
                    regQ_ |= fLow << 3;
                    break;
                case ShiftType.ARITHMETIC_DOUBLE:
                    regCommon_[b] |= fHigh << 3;
                    regQ_ |= fLow << 3;
                    break;
                }
                break;
            case ToType.SR_F_IN_B:
                regCommon_[b] = f_ >> 1;
                switch (shift) {
                case ShiftType.CYCLE:
                    regCommon_[b] |= fLow << 3;
                    break;
                }
                break;
            case ToType.SL_F_IN_B_AND_SL_Q_IN_Q:
                regQ_ = Helpers.Mask(regQ_ << 1);
                regCommon_[b] = Helpers.Mask(f_ << 1);
                switch (shift) {
                case ShiftType.CYCLE:
                    regCommon_[b] |= fHigh;
                    regQ_ |= qHigh;
                    break;
                case ShiftType.CYCLE_DOUBLE:
                    regCommon_[b] |= qHigh;
                    regQ_ |= fHigh;
                    break;
                case ShiftType.ARITHMETIC_DOUBLE:
                    regCommon_[b] |= qHigh;
                    break;
                }
                break;
            case ToType.SL_F_IN_B:
                regCommon_[b] = Helpers.Mask(f_ << 1);
                switch (shift) {
                case ShiftType.CYCLE:
                    regCommon_[b] |= fHigh;
                    break;
                }
                break;
            }

            if (to == ToType.F_IN_B_AND_A_IN_Y) {
                y_ = regCommon_[a];
            }
            else {
                y_ = f_;
            }
        }

        private void SetMemPtr_()
        {
            inc_ = Current_().GetIncType();
            mp_ = (Current_().GetRawValue(WordType.A) << 4) + Current_().GetRawValue(WordType.B);
        }

        private void SetDevicePtr_()
        {
            // TODO: Maybe to do device registers
        }

        private void LoadData_()
        {
            FuncType func = Current_().GetFuncType();
            PointerSize pointerSize = Current_().GetPointerSize();
            int a = Current_().GetRawValue(WordType.A);
            int b = Current_().GetRawValue(WordType.B);

            switch (func) {
            case FuncType.STORE_MEMORY:
                switch (pointerSize) {
                case PointerSize.LOW_4_BIT:
                    memory_[mp_] = regCommon_[b];
                    break;
                case PointerSize.HIGH_4_BIT:
                    memory_[mp_] = regCommon_[b] << 4;
                    break;
                case PointerSize.FULL_8_BIT:
                    memory_[mp_] = (regCommon_[a] << 4) | regCommon_[b];
                    break;
                }
                break;
            case FuncType.LOAD_MEMORY:
                switch (pointerSize) {
                case PointerSize.LOW_4_BIT:
                    regCommon_[b] = Helpers.Mask(memory_[mp_]);
                    break;
                case PointerSize.HIGH_4_BIT:
                    regCommon_[b] = memory_[mp_] >> 4;
                    break;
                case PointerSize.FULL_8_BIT:
                    regCommon_[a] = memory_[mp_] >> 4;
                    regCommon_[b] = Helpers.Mask(memory_[mp_]);
                    break;
                }
                break;
            case FuncType.STORE_DEVICE:
            // TODO: Maybe to do device registers
            case FuncType.LOAD_DEVICE:
                // TODO: Maybe to do device registers
                break;
            }
            if (func == FuncType.STORE_MEMORY || func == FuncType.LOAD_MEMORY) {
                switch (inc_) {
                case IncType.PLUS:
                    ++mp_;
                    mp_ %= memSize_;
                    break;
                case IncType.MINUS:
                    --mp_;
                    mp_ %= memSize_;
                    break;
                }
            }
        }

        public ResultCode ExecOne()
        {
            if (commands_.Count() == 0) {
                return ResultCode.NoCommands;
            }

            if (pc_ == -1) {
                if (calls_.Count > 0) {
                    pc_ = calls_[0].GetAddress();
                    callIndex_ = 0;
                }
                else {
                    pc_ = 0;
                }
                return ResultCode.Ok;
            }

            if (!Current_().Check()) {
                return ResultCode.IncorrectCommand;
            }

            // Save flags to restore then after command exec
            BackupFlags_();

            switch (Current_().GetCommandView()) {
            case ViewType.MT_COMMAND:
                ExecMtCommand_();
                break;
            case ViewType.MEMORY_POINTER:
                SetMemPtr_();
                break;
            case ViewType.DEVICE_POINTER:
                SetDevicePtr_();
                break;
            case ViewType.LOAD_4BIT:
            case ViewType.LOAD_8BIT:
                LoadData_();
                break;
            }

            // Update to new flags only if jump next
            if (Jump_() != JumpResult.Next) {
                RestoreFlags();
            }
            pc_ %= programSize_;

            return ResultCode.Ok;
        }

        public ResultCode ExecAll()
        {
            for (int i = 0; i < maxAutoCount_; ++i) {
                ResultCode rc = ExecOne();
                if (rc != ResultCode.Ok) {
                    return rc;
                }
                if (callIndex_ >= calls_.Count || prevPC_ == pc_) {
                    return ResultCode.Ok;
                }
            }
            return ResultCode.Loop;
        }

        public int GetPrevIndex()
        {
            return GetIndex_(prevPC_);
        }

        public int GetNextIndex()
        {
            return GetIndex_(pc_);
        }

        public int GetCallIndex()
        {
            if (callIndex_ >= calls_.Count) {
                return -1;
            }
            return callIndex_;
        }

        public int SetPC(int value)
        {
            return pc_ = value;
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

        public int GetY()
        {
            return y_;
        }

        public int GetPrevRegQ()
        {
            return prevRegQ_;
        }

        public int GetPrevRegA()
        {
            return prevRegA_;
        }

        public int GetPrevRegB()
        {
            return prevRegB_;
        }

        public int GetR()
        {
            return r_;
        }

        public int GetS()
        {
            return s_;
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

        public Call GetCall(int index)
        {
            return calls_[index];
        }

        public void AddCall(Call call)
        {
            calls_.Add(call);
        }

        public void UpdateCall(int index, Call call)
        {
            calls_[index] = call;
        }

        public void RemoveCall(int index)
        {
            calls_.RemoveAt(index);
        }

        public void MoveCallUp(int index)
        {
            if (index <= 0) {
                return;
            }
            calls_.Insert(index - 1, calls_[index]);
            calls_.RemoveAt(index + 1);
        }

        public void MoveCallDown(int index)
        {
            if (index >= calls_.Count - 1) {
                return;
            }
            calls_.Insert(index + 2, calls_[index]);
            calls_.RemoveAt(index);
        }

        public int CallsCount()
        {
            return calls_.Count();
        }

        public Call LastCall()
        {
            return calls_.Last();
        }

        private void SaveAsMtemu_(FileStream fstream)
        {
            int callsSize = CallsCount() * (sizeof(UInt16) + Call.COMMENT_MAX_SIZE);
            int commandsSize = CommandsCount() * (commandSize_ + 1);
            byte[] output = new byte[fileHeader_.Length + callsSize + commandsSize + 2 * sizeof(UInt16)];

            int seek = 0;
            for (; seek < fileHeader_.Length; ++seek) {
                output[seek] = fileHeader_[seek];
            }

            Call[] callsArr = calls_.ToArray();
            output[seek++] = (byte) (calls_.Count >> 8);
            output[seek++] = (byte) calls_.Count;

            for (int i = 0; i < callsArr.Length; ++i) {
                output[seek++] = (byte) (callsArr[i].GetAddress() >> 8);
                output[seek++] = (byte) callsArr[i].GetAddress();

                string comment = callsArr[i].GetComment();
                for (int c = 0; c < Call.COMMENT_MAX_SIZE; ++c) {
                    output[seek++] = (byte) (c < comment.Length ? comment[c] : '\0');
                }
            }

            Command[] commandsArr = commands_.ToArray();
            output[seek++] = (byte) (commands_.Count >> 8);
            output[seek++] = (byte) commands_.Count;

            for (int i = 0; i < commandsArr.Length; ++i) {
                output[seek++] = (byte) (commandsArr[i].isOffset ? 1 : 0);
                for (int j = 0; j < commandSize_; ++j) {
                    output[seek++] = (byte) ((commandsArr[i][2 * j] << 4) + commandsArr[i][2 * j + 1]);
                }
            }

            fstream.Write(output, 0, output.Length);
            fstream.SetLength(fstream.Position);
        }

        private void SaveAsBinary_(FileStream fstream)
        {
            int seek = 0;
            byte[] output = new byte[binFileHeader_.Length + programSize_ * commandSize_];
            for (; seek < binFileHeader_.Length; ++seek) {
                output[seek] = binFileHeader_[seek];
            }

            UpdateOffsets_();
            Command[] commandsArr = new Command[programSize_];
            foreach (Command command in commands_) {
                if (!command.isOffset) {
                    commandsArr[command.GetNumber()] = command;
                }
            }

            for (int i = 0; i < commandsArr.Length; ++i) {
                if (commandsArr[i] == null) {
                    commandsArr[i] = incorrectCommand_;
                }
                for (int j = 0; j < commandSize_; ++j, ++seek) {
                    output[seek] = (byte) ((commandsArr[i][2 * j] << 4) + commandsArr[i][2 * j + 1]);
                }
            }
            fstream.Write(output, 0, output.Length);
            fstream.SetLength(fstream.Position);
        }

        public bool SaveFile(string filename)
        {
            using (FileStream fstream = new FileStream(filename, FileMode.OpenOrCreate)) {
                if (Path.GetExtension(filename) == ".bin") {
                    SaveAsBinary_(fstream);
                }
                else {
                    SaveAsMtemu_(fstream);
                }
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

                calls_.Clear();
                commands_.Clear();
                Reset();

                int callsCount = 0;
                callsCount = (input[seek++] << 8) + input[seek++];

                for (int i = 0; i < callsCount; ++i) {
                    int address = (input[seek++] << 8) + input[seek++];

                    string comment = "";
                    for (int c = 0; c < Call.COMMENT_MAX_SIZE; ++c) {
                        if (input[seek++] != 0) {
                            comment += (char) (input[seek - 1]);
                        }
                    }

                    calls_.Add(new Call(address, comment));
                }

                int commandsCount = 0;
                commandsCount = (input[seek++] << 8) + input[seek++];

                for (int i = 0; i < commandsCount; ++i) {
                    bool isOffset = input[seek++] == 1;

                    int[] words = new int[commandSize_ * 2];
                    for (int j = 0; j < commandSize_; ++j, ++seek) {
                        words[2 * j] = input[seek] >> 4;
                        words[2 * j + 1] = input[seek] % 16;
                    }
                    commands_.Add(new Command(words));
                    commands_.Last().isOffset = isOffset;
                }
                UpdateOffsets_();
                return true;
            }
        }
    }
}

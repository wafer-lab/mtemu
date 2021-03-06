﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mtemu
{
    partial class Emulator
    {
        private PortExtender portExtender_;

        private int prevPc_;
        private int pc_;        // Pointer command
        private int callIndex_;
        private bool end_;

        private List<Command> commands_ = new List<Command>();
        private List<Call> calls_ = new List<Call>();

        private int sp_;        // Stack pointer
        private int[] stack_ = new int[stackSize_];

        private int regQ_;
        private int[] regCommon_;

        private IncType inc_;
        private int mp_;
        private int[] memory_ = new int[memSize_];

        private int devPtr_;
        // reserved for future use
        private int devIf_;

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
            prevPc_ = -1;
            pc_ = -1;
            callIndex_ = -1;
            end_ = false;

            sp_ = 0;
            regQ_ = 0;
            regCommon_ = new int[regSize_];
            inc_ = IncType.NO;
            mp_ = 0;
            devPtr_ = -1;

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

        public Emulator(PortExtender portExtender)
        {
            portExtender_ = portExtender;
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

        private int GetOffset_(int last)
        {
            int offset = 0;
            for (int i = 0; i < commands_.Count; ++i) {
                if (i == last) {
                    break;
                }
                if (commands_[i].isOffset) {
                    offset = commands_[i].GetNextAddr();
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

        public bool AddCommand(int index, Command command)
        {
            if (!command.Check()) {
                return false;
            }
            command.SetNumber(GetOffset_(index));
            if (command.GetNumber() > programSize_) {
                return false;
            }
            commands_.Insert(index, command);
            UpdateOffsets_(index);
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
            int i = GetIndex_(prevPc_);
            if (i == -1) {
                return incorrectCommand_;
            }
            return commands_[i];
        }

        private Command Prev_()
        {
            int i = GetIndex_(prevPc_);
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

        private int GetStackAddr_(int sp)
        {
            return (sp + stackSize_) % stackSize_;
        }

        private void Jump_()
        {
            prevPc_ = pc_;

            switch (Current_().GetJumpType()) {
            case JumpType.END:
                callIndex_ += Current_().GetDiffAddr() + 1;
                if (calls_.Count > 0 && callIndex_ < calls_.Count) {
                    pc_ = calls_[callIndex_].GetAddress();
                    return;
                }
                end_ = true;
                return;

            case JumpType.JMP:
                pc_ = Current_().GetNextAddr();
                return;

            case JumpType.JNXT:
                ++pc_;
                return;

            case JumpType.JNZ:
                if (!prevZ_) {
                    pc_ = Current_().GetNextAddr();
                    break;
                }
                ++pc_;
                break;

            case JumpType.JZ:
                if (prevZ_) {
                    pc_ = Current_().GetNextAddr();
                    break;
                }
                ++pc_;
                break;

            case JumpType.JF3:
                if (prevF3_) {
                    pc_ = Current_().GetNextAddr();
                    break;
                }
                ++pc_;
                break;

            case JumpType.JOVR:
                if (prevOvr_) {
                    pc_ = Current_().GetNextAddr();
                    break;
                }
                ++pc_;
                break;

            case JumpType.JC4:
                if (prevC4_) {
                    pc_ = Current_().GetNextAddr();
                    break;
                }
                ++pc_;
                break;

            case JumpType.CALL:
                stack_[sp_] = pc_ + 1;
                sp_ = GetStackAddr_(sp_ + 1);
                pc_ = Current_().GetNextAddr();
                return;

            case JumpType.RET:
                sp_ = GetStackAddr_(sp_ - 1);
                pc_ = stack_[sp_];
                return;

            case JumpType.JSP:
                pc_ = stack_[GetStackAddr_(sp_ - 1)];
                return;

            case JumpType.PUSH:
                stack_[sp_] = pc_ + 1;
                sp_ = GetStackAddr_(sp_ + 1);
                ++pc_;
                return;

            case JumpType.POP:
                sp_ = GetStackAddr_(sp_ - 1);
                ++pc_;
                return;

            case JumpType.CLNZ:
                if (!prevZ_) {
                    stack_[sp_] = pc_ + 1;
                    sp_ = GetStackAddr_(sp_ + 1);
                    pc_ = Current_().GetNextAddr();
                    break;
                }
                ++pc_;
                break;

            case JumpType.JSNZ:
                if (!prevZ_) {
                    pc_ = stack_[GetStackAddr_(sp_ - 1)];
                    break;
                }
                sp_ = GetStackAddr_(sp_ - 1);
                ++pc_;
                break;

            case JumpType.JSNC4:
                if (!prevC4_) {
                    pc_ = stack_[GetStackAddr_(sp_ - 1)];
                    break;
                }
                sp_ = GetStackAddr_(sp_ - 1);
                ++pc_;
                break;
            }

            RestoreFlags();
        }

        private void CountFlags_(FuncType alu)
        {
            bool c0 = (int) alu >= 8;

            int r = r_;
            int s = s_;
            switch (alu) {
            case FuncType.S_MINUS_R_MINUS_1:
            case FuncType.S_MINUS_R:
            case FuncType.NO_R_AND_S:
            case FuncType.R_XOR_S:
                r = Helpers.Mask(~r);
                break;
            case FuncType.R_MINUS_S_MINUS_1:
            case FuncType.R_MINUS_S:
                s = Helpers.Mask(~s);
                break;
            }

            int p = r | s;
            int g = r & s;
            bool p30 = p == 15;
            bool g30 = g > 0;

            switch (alu) {
            case FuncType.R_PLUS_S:
            case FuncType.R_PLUS_S_PLUS_1:
            case FuncType.S_MINUS_R_MINUS_1:
            case FuncType.S_MINUS_R:
            case FuncType.R_MINUS_S_MINUS_1:
            case FuncType.R_MINUS_S:
                p_ = !p30;

                bool g1 = Helpers.IsBitSet(g, 1) || (Helpers.IsBitSet(p, 1) && Helpers.IsBitSet(g, 0));
                bool g2 = Helpers.IsBitSet(g, 2) || (Helpers.IsBitSet(p, 2) && g1);
                bool g3 = Helpers.IsBitSet(g, 3) || (Helpers.IsBitSet(p, 3) && g2);
                g_ = !g3;

                bool c1 = Helpers.IsBitSet(g, 0) || (Helpers.IsBitSet(p, 0) && c0);
                bool c2 = Helpers.IsBitSet(g, 1) || (Helpers.IsBitSet(p, 1) && c1);
                bool c3 = Helpers.IsBitSet(g, 2) || (Helpers.IsBitSet(p, 2) && c2);
                bool c4 = Helpers.IsBitSet(g, 3) || (Helpers.IsBitSet(p, 3) && c3);
                c4_ = c4;
                ovr_ = c3 != c4;

                break;
            case FuncType.R_OR_S:
                p_ = false;
                g_ = p30;
                c4_ = !p30 || c0;
                ovr_ = c4_;
                break;
            case FuncType.R_AND_S:
            case FuncType.NO_R_AND_S:
                p_ = false;
                g_ = !g30;
                c4_ = g30 || c0;
                ovr_ = c4_;
                break;
            case FuncType.R_XOR_S:
            case FuncType.R_EQ_S:
                p_ = g30;

                bool g_1 = Helpers.IsBitSet(g, 1) || (Helpers.IsBitSet(p, 1) && Helpers.IsBitSet(p, 0));
                bool g_2 = Helpers.IsBitSet(g, 2) || (Helpers.IsBitSet(p, 2) && g_1);
                bool g_3 = Helpers.IsBitSet(g, 3) || (Helpers.IsBitSet(p, 3) && g_2);
                g_ = g_3;

                bool c4_1 = Helpers.IsBitSet(g, 1) || (Helpers.IsBitSet(p, 1) && Helpers.IsBitSet(p, 0) && (Helpers.IsBitSet(g, 0) || !c0));
                bool c4_2 = Helpers.IsBitSet(g, 2) || (Helpers.IsBitSet(p, 2) && c4_1);
                bool c4_3 = Helpers.IsBitSet(g, 3) || (Helpers.IsBitSet(p, 3) && c4_2);
                c4_ = !c4_3;

                p = Helpers.Mask(~p);
                g = Helpers.Mask(~g);
                bool ovr_0 = Helpers.IsBitSet(p, 0) || (Helpers.IsBitSet(g, 0) && c0);
                bool ovr_1 = Helpers.IsBitSet(p, 1) || (Helpers.IsBitSet(g, 1) && ovr_0);
                bool ovr_2 = Helpers.IsBitSet(p, 2) || (Helpers.IsBitSet(g, 2) && ovr_1);
                bool ovr_3 = Helpers.IsBitSet(p, 3) || (Helpers.IsBitSet(g, 3) && ovr_2);
                ovr_ = ovr_2 != ovr_3;

                break;
            }

            f3_ = Helpers.IsBitSet(f_, Command.WORD_SIZE - 1);
            z_ = f_ == 0;
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
                f_ = s_ + Helpers.Mask(~r_);
                break;
            case FuncType.S_MINUS_R:
                f_ = s_ + Helpers.Mask(~r_) + 1;
                break;
            case FuncType.R_MINUS_S_MINUS_1:
                f_ = r_ + Helpers.Mask(~s_);
                break;
            case FuncType.R_MINUS_S:
                f_ = r_ + Helpers.Mask(~s_) + 1;
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
            f_ = Helpers.Mask(f_);

            CountFlags_(alu);

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

        private PortExtender.InPort GetInPort(DataPointerType type)
        {
            if (devPtr_ == -1) {
                return PortExtender.InPort.PORT_UNKNOWN;
            }

            var val = devPtr_ << 2;

            switch (type) {
            case DataPointerType.LOW_4_BIT:
                val |= 1;
                break;
            case DataPointerType.HIGH_4_BIT:
                val |= 2;
                break;
            case DataPointerType.FULL_8_BIT:
                val |= 3;
                break;
            }

            var val_b = Convert.ToByte(val);

            if (Enum.IsDefined(typeof(PortExtender.InPort), val_b))
                return (PortExtender.InPort) val_b;

            return PortExtender.InPort.PORT_UNKNOWN;
        }

        private PortExtender.OutPort GetOutPort(DataPointerType type)
        {
            if (devPtr_ == -1) {
                return PortExtender.OutPort.PORT_UNKNOWN;
            }

            var val = devPtr_ << 2;

            switch (type) {
            case DataPointerType.LOW_4_BIT:
                val |= 1;
                break;
            case DataPointerType.HIGH_4_BIT:
                val |= 2;
                break;
            case DataPointerType.FULL_8_BIT:
                val |= 3;
                break;
            }

            var val_b = Convert.ToByte(val);

            if (Enum.IsDefined(typeof(PortExtender.OutPort), val_b))
                return (PortExtender.OutPort) val_b;

            return PortExtender.OutPort.PORT_UNKNOWN;
        }

        private void SetDevicePtr_()
        {
            devPtr_ = Current_().GetRawValue(WordType.A);
            devIf_ = 0;
        }

        private void LoadData_()
        {
            FuncType func = Current_().GetFuncType();
            DataPointerType pointerType = Current_().GetPointerType();
            int a = Current_().GetRawValue(WordType.A);
            int b = Current_().GetRawValue(WordType.B);

            switch (func) {
            case FuncType.STORE_MEMORY:
                switch (pointerType) {
                case DataPointerType.LOW_4_BIT:
                    memory_[mp_] = Helpers.MakeByte(
                        Helpers.HighNibble(memory_[mp_]),
                        regCommon_[b]);
                    break;
                case DataPointerType.HIGH_4_BIT:
                    memory_[mp_] = Helpers.MakeByte(
                        regCommon_[a],
                        Helpers.LowNibble(memory_[mp_]));
                    break;
                case DataPointerType.FULL_8_BIT:
                    memory_[mp_] = Helpers.MakeByte(
                        regCommon_[a],
                        regCommon_[b]);
                    break;
                }
                break;
            case FuncType.LOAD_MEMORY:
                switch (pointerType) {
                case DataPointerType.LOW_4_BIT:
                    regCommon_[b] = Helpers.LowNibble(memory_[mp_]);
                    break;
                case DataPointerType.HIGH_4_BIT:
                    regCommon_[a] = Helpers.HighNibble(memory_[mp_]);
                    break;
                case DataPointerType.FULL_8_BIT:
                    regCommon_[a] = Helpers.HighNibble(memory_[mp_]);
                    regCommon_[b] = Helpers.LowNibble(memory_[mp_]);
                    break;
                }
                break;
            case FuncType.STORE_DEVICE:
                PortExtender.OutPort outPort = GetOutPort(pointerType);
                if (outPort != PortExtender.OutPort.PORT_UNKNOWN) {
                    byte tmp_w = 0;

                    switch (pointerType) {
                    case DataPointerType.LOW_4_BIT:
                        tmp_w = Helpers.MakeLowNibble(regCommon_[b]);
                        break;
                    case DataPointerType.HIGH_4_BIT:
                        tmp_w = Helpers.MakeHighNibble(regCommon_[a]);
                        break;
                    case DataPointerType.FULL_8_BIT:
                        tmp_w = Helpers.MakeByte(regCommon_[a], regCommon_[b]);
                        break;
                    }

                    portExtender_.WritePort(outPort, pointerType, tmp_w);
                }
                else {
                    System.Windows.Forms.MessageBox.Show(
                        "Вы - болван, у Вас проблема с кодом (генетическим), проверьтесь у врача!\nТакие упорствующие индивиды не должны размножаться!",
                        "Невозможно записать в это устройство",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error
                    );
                }
                break;
            case FuncType.LOAD_DEVICE:
                PortExtender.InPort inPort = GetInPort(pointerType);
                if (inPort != PortExtender.InPort.PORT_UNKNOWN) {
                    byte tmp_r;
                    tmp_r = portExtender_.ReadPort(inPort, pointerType);
                    switch (pointerType) {
                    case DataPointerType.LOW_4_BIT:
                        regCommon_[b] = Helpers.LowNibble(tmp_r);
                        break;
                    case DataPointerType.HIGH_4_BIT:
                        regCommon_[a] = Helpers.HighNibble(tmp_r);
                        break;
                    case DataPointerType.FULL_8_BIT:
                        regCommon_[a] = Helpers.HighNibble(tmp_r);
                        regCommon_[b] = Helpers.LowNibble(tmp_r);
                        break;
                    }
                }
                else {
                    System.Windows.Forms.MessageBox.Show(
                        "Вы - болван, у Вас проблема с кодом (генетическим), проверьтесь у врача!\nТакие упорствующие индивиды не должны размножаться!",
                        "Невозможно прочитать из этого устройства",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error
                    );
                }
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

            if (end_) {
                return ResultCode.End;
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
            case ViewType.LOAD_HIGH_4BIT:
            case ViewType.LOAD_LOW_4BIT:
            case ViewType.LOAD_8BIT:
                LoadData_();
                break;
            }

            Jump_();
            pc_ %= programSize_;

            return ResultCode.Ok;
        }

        public ResultCode ExecOneCall()
        {
            int oldIndex = callIndex_;
            for (int i = 0; i < maxAutoCount_; ++i) {
                ResultCode rc = ExecOne();
                if (rc != ResultCode.Ok) {
                    return rc;
                }
                if (Prev_().GetJumpType() == JumpType.END || callIndex_ != oldIndex || prevPc_ == pc_) {
                    return ResultCode.Ok;
                }
            }
            return ResultCode.Loop;
        }

        public ResultCode ExecAll()
        {
            for (int i = 0; i < maxAutoCount_; ++i) {
                ResultCode rc = ExecOne();
                if (rc != ResultCode.Ok) {
                    return rc;
                }
                if (callIndex_ >= calls_.Count || prevPc_ == pc_) {
                    return ResultCode.Ok;
                }
            }
            return ResultCode.Loop;
        }

        public int GetPrevIndex()
        {
            return GetIndex_(prevPc_);
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

        public string GetPort()
        {
            return Command.GetPortName(devPtr_);
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

        public void AddCall(int index, Call call)
        {
            calls_.Insert(index, call);
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

                byte[] comment = Encoding.UTF8.GetBytes(callsArr[i].GetComment());
                for (int c = 0; c < Call.COMMENT_MAX_SIZE; ++c) {
                    output[seek++] = (byte) (c < comment.Length ? comment[c] : 0);
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

        public bool OpenRaw(byte[] input)
        {
            if (fileHeader_.Length + 2 * sizeof(UInt16) > input.Length) {
                return false;
            }

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
            if (seek + callsCount * (sizeof(UInt16) + Call.COMMENT_MAX_SIZE) > input.Length) {
                return false;
            }

            for (int i = 0; i < callsCount; ++i) {
                int address = (input[seek++] << 8) + input[seek++];

                byte[] comment = new byte[Call.COMMENT_MAX_SIZE];
                for (int c = 0; c < Call.COMMENT_MAX_SIZE; ++c) {
                    if (input[seek++] != 0) {
                        comment[c] = input[seek - 1];
                    }
                }

                calls_.Add(new Call(address, Encoding.UTF8.GetString(comment)));
            }

            int commandsCount = 0;
            commandsCount = (input[seek++] << 8) + input[seek++];
            if (seek + commandsCount * (commandSize_ + 1) > input.Length) {
                return false;
            }

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

        public bool OpenFile(string filename)
        {
            using (FileStream fstream = File.OpenRead(filename)) {
                byte[] input = new byte[fstream.Length];
                fstream.Read(input, 0, input.Length);
                return OpenRaw(input);
            }
        }
    }
}

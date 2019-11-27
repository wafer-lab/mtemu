using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mtemu
{
    partial class Call
    {
        int address_;
        string comment_;

        public Call(int address, string comment)
        {
            address_ = Helpers.Mask(address, ADDRESS_SIZE_BIT);
            comment_ = comment;
        }

        public Call(Call other)
        {
            address_ = other.address_;
            comment_ = other.comment_;
        }

        public int GetAddress()
        {
            return address_;
        }

        public void SetAddress(int address)
        {
            address_ = Helpers.Mask(address, ADDRESS_SIZE_BIT);
        }

        public string GetComment()
        {
            return comment_;
        }

        public void SetComment(string comment)
        {
            comment_ = comment;
        }
    }
}

using System;
using System.Linq;

namespace KHMemory
{
    public class AddressConverter
    {
        public static int To0Offset(int address)
        {
            int newAddress = 0;
            string addr = address.ToString("x8");
            if (addr[0] == '2')
            {
                newAddress = address - 536870912;
            }
            else if (addr[0] == '1')
            {
                newAddress = address - 268435456;
            }

            return newAddress;
        }

        public static int To2Offset(int address)
        {
            int newAddress = 0;
            string addr = address.ToString("x8");
            if (addr[0] == '0')
            {
                newAddress = address + 536870912;
            }
            else if (addr[0] == '1')
            {
                newAddress = address + 268435456;
            }
            else
                return address;

            return newAddress;
        }
        public static long To10Offset(int address)
        {
            var reversed = BitConverter.GetBytes(address).Reverse().ToArray();
            string newstring = "10" + reversed[4].ToString("X") + reversed[7].ToString("X") + reversed[6].ToString("X") + reversed[5].ToString("X");
            return Convert.ToInt64(newstring, 16);
        }
    }
}

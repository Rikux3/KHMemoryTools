using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KHMemory;

namespace KHMovesetMemory
{
    class AIReader
    {
        public List<AIEntry> entries = new List<AIEntry>();
        private short _ucm;

        public AIReader(short ucm)
        {
            _ucm = ucm;
        }
        public void ScanAI()
        {
            List<AIEntry> temp_entries = new List<AIEntry>();

            int found = AddressConverter.To2Offset(
                                        PCSX2_RAM.SearchBytes(
                                         BitConverter.GetBytes(0x0000 + _ucm))) + 8;
            int jumpto_addr = AddressConverter.To2Offset(PCSX2_RAM.ReadInteger(found));
            int aientries = AddressConverter.To2Offset(PCSX2_RAM.ReadInteger(jumpto_addr + 72));

            //Start reverse parsing
            int CounterZeroBytes = 0;
            bool bFoundFirstReverseChar = false;
            bool bExecute = true;
            bool bInAttack = false;
            bool bStopAttack = false;
            //string temp = string.Empty;
            byte prev = 0x00;
            AIEntry temp = new AIEntry();
            int i = aientries;
            while (bExecute)
            {
                Console.WriteLine("Reading address " + i.ToString("X"));
                byte current = PCSX2_RAM.ReadBytes(i, 1)[0];

                if (!IsAllowedAsciiByte(prev) && IsAllowedAsciiByte(current))
                {
                    CounterZeroBytes = 0;
                    bStopAttack = false;
                    bInAttack = true;
                    //Maybe new AI entry?
                    temp = new AIEntry();
                    
                    //this is the address of the last character
                    //afterwards, call (Address - Length-1) to get real start
                    temp.Address = i; 
                }

                else if (IsAllowedAsciiByte(prev) && !IsAllowedAsciiByte(current))
                {
                    //Valid AI entry ended, separator
                    //set flag
                    CounterZeroBytes++;
                    bStopAttack = true;
                    bInAttack = false;
                }

                else if (!IsAllowedAsciiByte(current)|| (!IsAllowedAsciiByte(prev) && !IsAllowedAsciiByte(current)))
                {
                    //After jumping to end of AI list, 0x00 or other invalid bytes.
                    //Also trying to figure out if we fully parsed the list,
                    //b/c before AI list are 0x00 or other inv. bytes
                    CounterZeroBytes++;
                    prev = current;
                    i--;

                    //This is for the begining of the AI list. 
                    //Could cause problem b/c the aientries variable
                    //can be over 6 characters from reverse start. 
                    //Is there any possibility to get to the start of 
                    //AI list rather than the end?
                    if (CounterZeroBytes > 6)
                        bExecute = false;
                    continue;
                }
                else
                {
                    if (!bFoundFirstReverseChar)
                    {
                        //Found first character in reverse parsing
                        bFoundFirstReverseChar = true;
                        CounterZeroBytes = 0;
                        bInAttack = true;
                    }
                }

                if (bInAttack)
                {
                    //we currently parse an AI command
                    temp.Name += System.Text.Encoding.ASCII.GetString(new[] { current });
                }

                if (bStopAttack)
                {
                    //AI entry ended, add to temporary list
                    //TODO: filtering invalid entries, such as
                    //single or two-byte entries (i.e: entry name '45')
                    //entries containing a space (ask Xaddgx)
                    temp_entries.Add(temp);
                    temp = new AIEntry();
                }

                //if (CounterZeroBytes > 4)
                //    bExecute = false;
                i--;
                prev = current;
            }
            SortAIEntries(temp_entries);
        }
        public bool IsAllowedAsciiByte(byte input)
        {
            if (input == 33 || 
                (input >= 48 && input <= 57) || 
                (input >= 65 && input <= 90) || 
                (input >= 97 && input <= 122) || 
                input == 95 ||
                input == 32)
                return true;
            return false;
        }

        public void SortAIEntries(List<AIEntry> temp)
        {
            temp.Reverse();
            temp.ForEach(x =>
           {
               AIEntry a = new AIEntry();
               a.Name = ReverseString(x.Name);
               a.Length = x.Name.Length;
               a.Address = x.Address - (x.Name.Length - 1);
               entries.Add(a);
           });
        }

        public string ReverseString(string s)
        {
            char[] arr = s.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
    }

    public struct AIEntry
    {
        public int Address;
        public int Length;
        public string Name;
        public string FriendlyName
        {
            get
            {
                return Name + " | " + Address.ToString("X");
            }
        }
    }
}

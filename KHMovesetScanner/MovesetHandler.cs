using System;
using System.Collections.Generic;
using System.Linq;
using KHMemory;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;

namespace KHMovesetMemory
{
    class MovesetHandler
    {
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        private short _ucm;
        private byte _ucmB;
        private List<Moveset> _movesets = new List<Moveset>();
        private int _slotcounter;

        public MovesetHandler(short ucm)
        {
            _ucm = ucm;
        }

        public void InitMovesetScan()
        {
            //  AllocConsole();
            SearchHeartBlock(0x21a00000);
            Communicator.Instance.Movesets = _movesets.OrderBy(x => x.Name).ToList();
        }

        public void WriteToLog(string msg)
        {
            using (var writer = File.AppendText(@"Log.txt"))
            {
                writer.WriteLine(msg);
            }
        }

        public void SearchHeartBlock(int offset)
        {
            int HeartBlock = 0;
            int barPointer = 0;
            HeartBlock = AddressConverter.To2Offset(
                        PCSX2_RAM.SearchBytes(
                         BitConverter.GetBytes(0x0000 + _ucm), offset));
            Console.WriteLine("Found possible HeartBlock at " + HeartBlock.ToString("X8"));
            WriteToLog("Found possible HeartBlock at " + HeartBlock.ToString("X8"));

            barPointer = AddressConverter.To2Offset(PCSX2_RAM.ReadInteger(HeartBlock + 16));
            if (IsBARPointer(barPointer))
            {
                Console.WriteLine("BAR pointer at " + barPointer.ToString("X8"));
                WriteToLog("BAR pointer at " + barPointer.ToString("X8"));
                ReadBARFile(barPointer);
            }
            else
            { 
                SearchHeartBlock(HeartBlock + 4);
            }
        }

        public bool IsBARPointer(int offset)
        {
            if (PCSX2_RAM.ReadString(offset) == "BAR")
            { return true; }
            return false;
        }
        public void ReadBARFile(int offset)
        {
            if (PCSX2_RAM.ReadString(offset) == "BAR")
            {
                int headerCount = PCSX2_RAM.ReadInteger(offset + 4);
                int StartOfHeaderEntries = offset + 16;
                int i = StartOfHeaderEntries;

                for (int j = 0; j < headerCount; j++)
                {
                    string EntryName = PCSX2_RAM.ReadString(i + 4);
                    if (EntryName == "DUMM")
                    {
                        _slotcounter++;
                        i += 16;
                        continue;
                    }
                    Console.WriteLine("BAR entry at " + i.ToString("X"));
                    WriteToLog("BAR entry at " + i.ToString("X"));
                    int AddressRefInRAM = i + 8;
                    int barMovesetPointer = 0;

                        barMovesetPointer = PCSX2_RAM.ReadInteger(AddressRefInRAM);
                    //else
                    //    barMovesetPointer = AddressConverter.To10Offset(PCSX2_RAM.ReadInteger(AddressRefInRAM));
                    //Check if current moveset is a duplicate
                    //If yes, get moveset with same label and add current reference address
                    Moveset item = _movesets.FirstOrDefault(x => x.ANBAddress == barMovesetPointer) as Moveset;
                    if (item != null)
                    {
                        if (item.ANBAddress == barMovesetPointer)
                        {
                            item.AddressReferences.Add(AddressRefInRAM);
                        }
                        i += 16;
                        _slotcounter++;
                        continue;

                    }
                    Regex rgx = new Regex("\\d{2}[A-Z]"); // 00A, 01A, 01B etc
                    Regex rgx2 = new Regex("[A-Z]\\d{3}"); //A000
                    Match match = rgx.Match(EntryName);
                    Match match2 = rgx2.Match(EntryName);
                    if (match.Success || match2.Success)
                    {
                        Moveset m = new Moveset();
                        m.Name = EntryName;
                        m.AddressReferences.Add(AddressRefInRAM);
                        m.ANBAddress = barMovesetPointer;
                        m.SlotNumber = _slotcounter;
                        ReadMoveset(m);
                        i += 16;
                        _slotcounter++;
                    }
                    else
                    {
                        i += 16;
                        _slotcounter++;
                    }

                }
                //    string tempMsetString = string.Empty;
                //    string tempMsetString2 = string.Empty;

                //    //Reading the BAR file
                //    while (tempMsetString2 != "BAR")
                //    {
                //        Console.WriteLine("BAR entry at " + i.ToString("x8"));
                //        _slotcounter++;
                //        tempMsetString2 = PCSX2_RAM.ReadString(i);
                //        tempMsetString = PCSX2_RAM.ReadString(i + 4);
                //        if (tempMsetString == "DUMM")
                //        {
                //            i += 16;
                //            continue;
                //        }

                //        tempMsetString = tempMsetString.Replace("\0", string.Empty);
                //        Regex rgx = new Regex("\\d{2}[A-Z]"); // 00A, 01A, 01B etc
                //        Regex rgx2 = new Regex("[A-Z]\\d{3}"); //A000
                //        Match match = rgx.Match(tempMsetString);
                //        Match match2 = rgx2.Match(tempMsetString);
                //        if (match.Success || match2.Success)
                //        {

                //        }
                //        else
                //            i += 16;
                //    }

                //}
                //else
                //{
                //    int newOffset = AddressConverter.To2Offset(
                //                            PCSX2_RAM.SearchBytes(
                //                             BitConverter.GetBytes(0x0000 + _ucm), offset))+16;

                //    ReadBARFile(newOffset);
                //}
            }
        }

        public void ReadMoveset(Moveset mset)
        {
            Console.WriteLine("ReadMoveset Method - " + mset.ANBAddress.ToString("x8") + " - " + mset.Name);
            WriteToLog("ReadMoveset Method - " + mset.ANBAddress.ToString("x8") + " - " + mset.Name);
            int startBarOff = 0;

                startBarOff = AddressConverter.To2Offset(mset.ANBAddress + 20);

            MovesetAnimation manim = new MovesetAnimation();
            manim.Address = startBarOff + 4;
            manim.Value = PCSX2_RAM.ReadInteger(manim.Address);
            mset.Animation = manim;

            MovesetEffect meffect = new MovesetEffect();
            meffect.Address = startBarOff + 20;
            meffect.Value = PCSX2_RAM.ReadInteger(meffect.Address);

            meffect.BoneStructure = new MovesetEffectBoneStructure();
            meffect.BoneStructure.Address = startBarOff + 32;
            meffect.BoneStructure.Value = PCSX2_RAM.ReadInteger(meffect.BoneStructure.Address);

            //Wth can effects have a 0 value??
            if (meffect.Value != 0)
            {
                //Read Effect Data Modifiers (name according to Xaddgx)
                List<MovesetEffectDataModifierRaw> modifiers = new List<MovesetEffectDataModifierRaw>();
                int startOfDataModifiers = AddressConverter.To2Offset(meffect.Value);
                string tempDataMod = string.Empty;
                int countertemp = 1;
                while (tempDataMod != "BAR")
                {
                    Console.WriteLine("Reading effect data modifier - " + startOfDataModifiers.ToString("X8"));
                    WriteToLog("Reading effect data modifier - " + startOfDataModifiers.ToString("X8"));
                    MovesetEffectDataModifierRaw mod = new MovesetEffectDataModifierRaw();
                    mod.Address = startOfDataModifiers;
                    mod.Value = PCSX2_RAM.ReadInteger(AddressConverter.To2Offset(startOfDataModifiers));
                    mod.DisplayNumber = countertemp;
                    modifiers.Add(mod);

                    startOfDataModifiers += 4;
                    countertemp++;
                    tempDataMod = PCSX2_RAM.ReadString(startOfDataModifiers);
                }

                meffect.DataModifiers = modifiers;
            }

            //Parsing that into Effect Casters (name according to SoraikoSan)
            //if (meffect.Value != 0)
            //{
            //    ECasterList eclist = new ECasterList();
            //    eclist.Children = new List<ECaster>();
            //    var start = AddressConverter.To2Offset(meffect.Value);
            //    eclist.Header = PCSX2_RAM.ReadBytes(start, 4);
            //    eclist.Group1Entries = Convert.ToInt32(eclist.Header[0]);
            //    eclist.Group2Entries = Convert.ToInt32(eclist.Header[1]);
            //    start += 4;

            //    for(int i = 0; i<eclist.Group1Entries; i++)
            //    {
            //        ECaster ec = new ECaster();
            //        ec.StartAddress = start;
            //        ec.Group = ECasterGroup.Group1;
            //        ec.StartAnimationFrame = PCSX2_RAM.ReadShort(start);
            //        ec.EndAnimationFrame = PCSX2_RAM.ReadShort(start + 2);
            //        ec.EffectType = PCSX2_RAM.ReadBytes(start + 4, 1)[0];
            //        Group1Effects eff = (Group1Effects)ec.EffectType;
            //        var addbytes = PCSX2_RAM.ReadBytes(start + 5, 1)[0];
            //        if (addbytes > 0)
            //        {
            //            ec.AdditionalBytes = PCSX2_RAM.ReadBytes(start+6, addbytes*2);
            //        }
            //        ec.Length = 6 + (addbytes * 2);

            //        eclist.Children.Add(ec);

            //        start += ec.Length;
            //    }

            //    for (int j = 0; j<eclist.Group2Entries; j++)
            //    {
            //        ECaster ec = new ECaster();
            //        ec.StartAddress = start;
            //        ec.Group = ECasterGroup.Group2;
            //        ec.StartAnimationFrame = PCSX2_RAM.ReadShort(start);
            //        ec.EffectType = PCSX2_RAM.ReadBytes(start + 2, 1)[0];
            //        Group2Effects ec2 = (Group2Effects)ec.EffectType;
            //        var addbytes = PCSX2_RAM.ReadBytes(start + 3, 1)[0];
            //        if (addbytes > 0)
            //        {
            //            ec.AdditionalBytes = PCSX2_RAM.ReadBytes(start + 4, addbytes * 2);
            //        }

            //        ec.Length = 4 + (addbytes * 2);
            //        eclist.Children.Add(ec);

            //        start += ec.Length;
            //    }
            //    meffect.EffectCasterList = eclist;
            //}
            mset.Effect = meffect;

            _movesets.Add(mset);
        }
    }
}

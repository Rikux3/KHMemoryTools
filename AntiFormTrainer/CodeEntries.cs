using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Linq;
using KHMemory;

namespace AntiFormTrainer
{
    public class CodeEntries
    {
        private ObservableCollection<SingleEntry> _entries;
        public ObservableCollection<SingleEntry> Entries { get { return _entries; } }
        private bool silenceCodeCreated = false;

        public CodeEntries()
        {
            _entries = new ObservableCollection<SingleEntry>();
        }

        public CodeEntries(SerializationInfo info, StreamingContext ctxt)
        {
            _entries = (ObservableCollection<SingleEntry>)info.GetValue("Entries", typeof(ObservableCollection<SingleEntry>));
        }

        public void Add(int address, object value, Type t)
        {
            SingleEntry en = new SingleEntry();
            en.Address = address;
            en.Value = value;
            en.ValueType = t;
            en.PrevValue = ReadByType(address, t);
            _entries.Add(en);
        }

        public void Add(int address, object value, object PrevVal, Type t)
        {
            SingleEntry en = new SingleEntry();
            en.Address = address;
            en.Value = value;
            en.ValueType = t;
            en.PrevValue = PrevVal;
            _entries.Add(en);
        }

        private object ReadByType(int address, Type t)
        {
            if (t == typeof(int))
                return PCSX2_RAM.ReadInteger(address);
            else if (t == typeof(short))
                return PCSX2_RAM.ReadShort(address);
            else if (t == typeof(string))
                return PCSX2_RAM.ReadString(address);
            else if (t == typeof(byte[]))
                return PCSX2_RAM.ReadBytes(address, 4);
            else if (t == typeof(uint))
                return PCSX2_RAM.ReadUInteger(address);
            else
                return PCSX2_RAM.ReadBytes(address, 4);
        }

        private void WriteByType(int address, object value, Type t)
        {
            if (t == typeof(int))
                PCSX2_RAM.WriteInteger(address, Convert.ToInt32(value));
            else if (t == typeof(short))
                PCSX2_RAM.WriteShort(address, Convert.ToInt16(value));
            else if (t == typeof(byte[]))
                PCSX2_RAM.WriteBytes(address, (byte[])value);
            else if (t == typeof(uint))
                PCSX2_RAM.WriteUInteger(address, Convert.ToUInt32(value));
            else
                PCSX2_RAM.WriteBytes(address, (byte[])value);
        }

        public void Edit(int address, short value)
        {
            var item = _entries.Where(x => x.Address == address).FirstOrDefault();
            item.Value = value;
        }

        public void Remove(int address)
        {
            SingleEntry en = _entries.Where(x => x.Address == address).Single();
            if (en != null)
                _entries.Remove(en);
        }

        public void Execute()
        {
            if (!silenceCodeCreated)
            {
                int AddrOfPtr = (int)KHMemory.PCSX2_RAM.BaseAddr + 0x00954F30;
                int AddrIopVoice = KHMemory.PCSX2_RAM.ReadInteger(AddrOfPtr) + 0x460;
                Communicator.Instance.Codes.Add(AddrIopVoice, 0x00000000, 0x56706F49, typeof(uint));
                Communicator.Instance.Codes.Add(AddrIopVoice + 4, 0x00000000, 0x6563696F, typeof(uint));
                silenceCodeCreated = true;
            }
            while (Communicator.Instance.IsExecutionRunning)
            {
                foreach (var item in _entries)
                {
                    WriteByType(item.Address, item.Value, item.ValueType);
                }
            }
            RevertAllCodes();
        }

        public void RevertAllCodes()
        {
            foreach (var item in _entries)
            {
                WriteByType(item.Address, item.PrevValue, item.ValueType);
            }
        }
    }

    public class SingleEntry
    {
        public int Address { get; set; }
        public object Value { get; set; }
        public object PrevValue { get; set; }

        public Type ValueType { get; set; }

        public SingleEntry() { }

        public override bool Equals(object obj)
        {
            if (obj is SingleEntry)
            {
                var other = (SingleEntry)obj;
                if ((Address == other.Address) && (Value == other.Value))
                    return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

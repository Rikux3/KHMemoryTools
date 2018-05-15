using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Linq;
using KHMemory;

namespace KHMovesetMemory
{
    [Serializable()]
    public class CodeEntries : ISerializable
    {
        private ObservableCollection<SingleEntry> _entries;
        public ObservableCollection<SingleEntry> Entries { get { return _entries; } }

        public CodeEntries()
        {
            _entries = new ObservableCollection<SingleEntry>();
        }

        public CodeEntries(SerializationInfo info, StreamingContext ctxt)
        {
            _entries = (ObservableCollection<SingleEntry>)info.GetValue("Entries", typeof(ObservableCollection<SingleEntry>));
        }

        public void Add(int address, int value)
        {
            SingleEntry en = new SingleEntry();
            en.Address = address;
            en.Value = value;
            _entries.Add(en);
        }

        public void Edit(int address, int value)
        {
            var item = _entries.Where(x => x.Address == address).FirstOrDefault();
            item.Value = value;
        }

        public void Remove(SingleEntry item)
        {
            _entries.Remove(_entries.Where(x => x.Equals(item)).Single());
        }

        public void Execute()
        {
            foreach (var item in _entries)
            {
                PCSX2_RAM.WriteInteger(item.Address, item.Value);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Entries", _entries);
        }
    }

    [Serializable()]
    public class SingleEntry : ISerializable
    {
        public int Address { get; set; }
        public int Value { get; set; }

        public SingleEntry() { }

        public SingleEntry(SerializationInfo info, StreamingContext ctxt)
        {
            Address = (int)info.GetValue("Address", typeof(int));
            Value = (int)info.GetValue("Value", typeof(int));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Address", Address);
            info.AddValue("Value", Value);
        }

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

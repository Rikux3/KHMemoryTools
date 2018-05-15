using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Linq;
using KHMemory;

namespace KHTriggerRunner
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

        public void Add(int address, int value, bool bRandom, RandomizerValue eRand)
        {
            SingleEntry en = new SingleEntry();
            en.Address = address;
            en.Value = value;
            en.IsRandomUCM = bRandom;
            en.eRand = eRand;
            _entries.Add(en);
        }

        public void Add(int address, int value, bool bRandom)
        {
            Add(address, value, bRandom, RandomizerValue.Undefined);
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
                var newaddr = AddressConverter.To2Offset(item.Address);
                PCSX2_RAM.WriteInteger(newaddr, item.Value);
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
        public bool IsRandomUCM { get; set; }
        public RandomizerValue eRand { get; set; }

        public SingleEntry() { }

        public SingleEntry(SerializationInfo info, StreamingContext ctxt)
        {
            Address = (int)info.GetValue("Address", typeof(int));
            Value = (int)info.GetValue("Value", typeof(int));
            IsRandomUCM = (bool)info.GetValue("RandomUcm", typeof(bool));
            eRand = (RandomizerValue)info.GetValue("RandomizerType", typeof(RandomizerValue));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Address", Address);
            info.AddValue("Value", Value);
            info.AddValue("RandomUcm", IsRandomUCM);
            info.AddValue("RandomizerType", eRand);
        }

        public override bool Equals(object obj)
        {
            if (obj is SingleEntry)
            {
                var other = (SingleEntry)obj;
                if ((Address == other.Address) && (Value == other.Value) && (IsRandomUCM == other.IsRandomUCM))
                    return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public enum RandomizerValue
    {
        Undefined,
        Any,
        Enemy,
        Boss
    }
}

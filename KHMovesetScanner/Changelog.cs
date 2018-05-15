using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace KHMovesetMemory
{
    [Serializable]
    public abstract class Change
    {
        public abstract string DisplayName { get; set; }
        public int ChangeNum;
    }

    public enum TypeOfChange
    {
        eAnimation,
        eEffect,
        eBoneStructure,
    }

    public enum TypeOfWhole
    {
        eAll,
        eSingle
    }

    [Serializable()]
    public class AnimEffectBoneChange : Change, ISerializable
    {
        public TypeOfChange type;
        //Do this as index from moveset list? Strings can be duplicated (000A)
        //Leave it as it is atm, check later
        public int FromMset;
        public override string DisplayName
        {
            get
            {
                if (type == TypeOfChange.eAnimation)
                    return "Copy Animation from " + Communicator.Instance.Movesets[FromMset].Name;
                else if (type == TypeOfChange.eEffect)
                    return "Copy Effect from " + Communicator.Instance.Movesets[FromMset].Name;
                else
                    return "Copy Bone Structure from " + Communicator.Instance.Movesets[FromMset].Name;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public AnimEffectBoneChange() { }

        public AnimEffectBoneChange(SerializationInfo info, StreamingContext ctxt)
        {
            type = (TypeOfChange)info.GetValue("Type", typeof(TypeOfChange));
            FromMset = (int)info.GetValue("FromMset", typeof(int));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Type", type);
            info.AddValue("FromMset", FromMset);
        }
    }

    [Serializable()]
    public class EffectDataChange : Change, ISerializable
    {
        public int LineNumber;
        public int NewValue;
        public override string DisplayName
        {
            get
            {
                return "Replace Base Effect Line " + LineNumber+1 + " with " + NewValue.ToString("X8");
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public EffectDataChange() { }

        public EffectDataChange(SerializationInfo info, StreamingContext context)
        {
            LineNumber = (int)info.GetValue("LineNumber", typeof(int));
            NewValue = (int)info.GetValue("NewValue", typeof(int));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("LineNumber", LineNumber);
            info.AddValue("NewValue", NewValue);
        }
    }

    [Serializable()]
    public class WholeMovesetChange : Change, ISerializable
    {
        public int OldMset;
        public int NewMset;
        public TypeOfWhole TypeOfRef;
        public int SingleRef;
        public override string DisplayName
        {
            get
            {
                if (NewMset == -1)
                    return "Replace with FAKE";
                else
                {
                    if (TypeOfRef == TypeOfWhole.eSingle)
                        return "Replace " + SingleRef.ToString("X8")+ " Reference with " + Communicator.Instance.Movesets[NewMset].Name;
                    else
                        return "Replace with " + Communicator.Instance.Movesets[NewMset].Name;
                }
                    
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public WholeMovesetChange() { }

        public WholeMovesetChange(SerializationInfo info, StreamingContext context)
        {
            OldMset = (int)info.GetValue("OldMset", typeof(int));
            NewMset = (int)info.GetValue("NewMset", typeof(int));
            TypeOfRef = (TypeOfWhole)info.GetValue("TypeOfRef", typeof(TypeOfWhole));
            SingleRef = (int)info.GetValue("SingleRef", typeof(int));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("OldMset", OldMset);
            info.AddValue("NewMset", NewMset);
            info.AddValue("TypeOfRef", TypeOfRef);
            info.AddValue("SingleRef", SingleRef);
        }
    }

    [Serializable()]
    public class Changelog : ISerializable
    {
        private short _ucm;
        private Dictionary<int, ObservableCollection<Change>> _listOfChanges;
        private int _changeid;
       // private int _msetcount;

        public Changelog(short ucm)
        {
            _ucm = ucm;
            _listOfChanges = new Dictionary<int, ObservableCollection<Change>>();
            _changeid = 0;
        }

        public Changelog(SerializationInfo info, StreamingContext ctxt)
        {
            _ucm = (short)info.GetValue("UCM", typeof(short));
            _listOfChanges = (Dictionary<int, ObservableCollection<Change>>)info.GetValue("Dict", typeof(Dictionary<int, ObservableCollection<Change>>));
            _changeid = (int)info.GetValue("_changeid", typeof(int));
            //_msetcount = (int)info.GetValue("MovesetCount", typeof(int));
        }

        public short UCM
        {
            get
            {
                return _ucm;
            }
        }

        public Dictionary<int, ObservableCollection<Change>> ListOfChanges
        {
            get
            {
                return _listOfChanges;
            }
            set
            {
                _listOfChanges = value;
            }
        }

        public int GetNextChangeId
        {
            get
            {
                return _changeid++;
            }
        }

        //public int MovesetCount
        //{
        //    get
        //    {
        //        return _msetcount;
        //    }
        //    set
        //    {
        //        _msetcount = value;
        //    }
        //}

        public CodeEntries ConvertToCodes()
        {
            CodeEntries en = new CodeEntries();
            foreach(KeyValuePair<int, ObservableCollection<Change>> kvp in _listOfChanges)
            {
                Moveset m = Communicator.Instance.Movesets[kvp.Key];
                foreach(var cha in kvp.Value)
                {
                    if (cha is WholeMovesetChange)
                    {
                        WholeMovesetChange wmc = (WholeMovesetChange)cha;

                        if (wmc.TypeOfRef == TypeOfWhole.eAll)
                        {
                            int writeTo;
                            if (wmc.NewMset == -1)
                                writeTo = 0;
                            else
                                writeTo = Communicator.Instance.Movesets[wmc.NewMset].ANBAddress;
                            foreach (var item in m.AddressReferences)
                            {
                                en.Add(item, writeTo);
                            }
                        }
                        else
                        {
                            int writeTo;
                            if (wmc.NewMset == -1)
                                writeTo = 0;
                            else
                                writeTo = Communicator.Instance.Movesets[wmc.NewMset].ANBAddress;

                            en.Add(wmc.SingleRef, writeTo);
                        }
                        //Moveset other = Communicator.Instance.Movesets[wmc.NewMset];
                        //foreach(var item in m.AddressReferences)
                        //{
                        //    en.Add(item, other.ANBAddress);
                        //}
                    }
                    else if (cha is AnimEffectBoneChange)
                    {
                        AnimEffectBoneChange aebc = (AnimEffectBoneChange)cha;
                        Moveset oth = Communicator.Instance.Movesets[aebc.FromMset];
                        if (aebc.type == TypeOfChange.eAnimation)
                            en.Add(m.Animation.Address, oth.Animation.Value);
                        else if (aebc.type == TypeOfChange.eEffect)
                            en.Add(m.Effect.Address, oth.Effect.Value);
                        else if (aebc.type == TypeOfChange.eBoneStructure)
                            en.Add(m.Effect.BoneStructure.Address, oth.Effect.BoneStructure.Value);
                    }
                    else if (cha is EffectDataChange)
                    {
                        EffectDataChange edc = (EffectDataChange)cha;
                        en.Add(m.Effect.DataModifiers[edc.LineNumber].Address, edc.NewValue);
                    }
                }
            }
            return en;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("UCM", _ucm);
            info.AddValue("Dict", _listOfChanges);
            info.AddValue("_changeid", _changeid);
            //info.AddValue("MovesetCount", _msetcount);
        }
    }
}

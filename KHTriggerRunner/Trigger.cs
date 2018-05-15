using System;
using System.Runtime.Serialization;

namespace KHTriggerRunner
{
    [Serializable()]

    public class Trigger : ITrigger, ISerializable
    {
        public RoomInformation RoomInfo;
        public CodeEntries CodeEntries;
        public bool StopWhenMapIsLoaded = false;
        public bool IsWorldWarp = false;
        public bool HasEvent = false;
        
        public override string DisplayName
        {
            get
            {
                int w = WorldInfo.GetWorldIndex(RoomInfo.WorldNumber);
                int r = WorldInfo.GetRoomIndex(w, RoomInfo.RoomNumber);
                int e = WorldInfo.GetEventIndex(w, r, RoomInfo.EventNumberMain);
                string world = WorldInfo.Worlds[w];
                world = world.Substring(world.IndexOf("-")+2);
                string room = WorldInfo.Rooms[w][r];
                room = room.Substring(room.IndexOf("-")+2);

                if (HasEvent)
                {
                    string ev = WorldInfo.AllEvents[w][r][e];
                    ev = ev.Substring(ev.IndexOf("-") + 2);
                    return world + " | " + room + " | " + ev;
                }
                else
                    return world + " | " + room;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Trigger()
        {
            RoomInfo = new RoomInformation();
            CodeEntries = new CodeEntries();
        }

        public Trigger(int w, int r, int e)
        {
            RoomInfo = new RoomInformation();
            RoomInfo.WorldNumber = w;
            RoomInfo.RoomNumber = r;
            RoomInfo.EventNumberMain = e;
            CodeEntries = new CodeEntries();
        }

        public Trigger(SerializationInfo info, StreamingContext ctxt)
        {
            RoomInfo = (RoomInformation)info.GetValue("RoomInformation", typeof(RoomInformation));
            CodeEntries = (CodeEntries)info.GetValue("CodeEntries", typeof(CodeEntries));
            StopWhenMapIsLoaded = (bool)info.GetValue("StopWhenMapIsLoaded", typeof(bool));
            IsWorldWarp = (bool)info.GetValue("IsWorldWarp", typeof(bool));
            HasEvent = (bool)info.GetValue("HasEvent", typeof(bool));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("RoomInformation", RoomInfo);
            info.AddValue("CodeEntries", CodeEntries);
            info.AddValue("StopWhenMapIsLoaded", StopWhenMapIsLoaded);
            info.AddValue("IsWorldWarp", IsWorldWarp);
            info.AddValue("HasEvent", HasEvent);
        }

        public bool IsTriggered(RoomInformation other)
        {
            if (HasEvent)
            {
                if (other.Equals(RoomInfo))
                    return true;
            }
            else
            {
                if (other.Equals2(RoomInfo))
                    return true;
            }

            return false;
        }

        public bool IsEqual(Trigger other)
        {
            if (RoomInfo.Equals(other.RoomInfo) || RoomInfo.Equals2(other.RoomInfo))
                return true;
            return false;
        }

        public override bool HasChildren()
        {
            return false;
        }
    }

    [Serializable()]
    public class RoomInformation : ISerializable
    {
        public int WorldNumber;
        public int RoomNumber;
        public int EventNumberMain;
        public int EventNumber2;
        public int EventNumber3;

        public RoomInformation() { }
        public RoomInformation(SerializationInfo info, StreamingContext ctxt)
        {
            WorldNumber = (int)info.GetValue("WorldNumber", typeof(int));
            RoomNumber = (int)info.GetValue("RoomNumber", typeof(int));
            EventNumberMain = (int)info.GetValue("EventNumberMain", typeof(int));
            EventNumber2 = (int)info.GetValue("EventNumber2", typeof(int));
            EventNumber3 = (int)info.GetValue("EventNumber3", typeof(int));
        }

        public override bool Equals(object obj)
        {
            if (obj is RoomInformation)
            {
                var item = (RoomInformation)obj;
                if ((item.WorldNumber == WorldNumber) && (item.RoomNumber == RoomNumber) &&
                    (item.EventNumberMain == EventNumberMain))
                    return true;
            }
            return false;
        }

        public bool Equals2(object obj)
        {
            if (obj is RoomInformation)
            {
                var item = (RoomInformation)obj;
                    if ((item.WorldNumber == WorldNumber) && (item.RoomNumber == RoomNumber))
                        return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("WorldNumber", WorldNumber);
            info.AddValue("RoomNumber", RoomNumber);
            info.AddValue("EventNumberMain", EventNumberMain);
            info.AddValue("EventNumber2", EventNumber2);
            info.AddValue("EventNumber3", EventNumber3);
        }
    }
}

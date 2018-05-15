using System.Collections.Generic;

namespace KHMovesetMemory
{
    public class DataDict
    {
        //public const int nBgmOne = 0x21C6CC48;
        //public const int nBgmTwo = 0x21C6CC4A;

        public static Dictionary<string, short> dictUCM = new Dictionary<string, short>
        {
            {"Sora", 0x054 },
            {"Sora (Valor Form)", 0x055 },
            {"Sora (Wisdom Form)", 0x056 },
            {"Terra", 0x96F },
            {"Axel (Limit Cut)", 0x9C4 },
            {"Xigbar (Limit Cut)", 0x9C5 },
            {"Saïx (Limit Cut)", 0x9C6 },
            {"Luxord (Limit Cut)", 0x9C8 },
            {"Xemnas I (Limit Cut)", 0x9C9 },
            {"Xemnas II (Limit Cut)", 0x9CA },
            {"Xaldin (Limit Cut)", 0x9CB },
            {"Demyx (Limit Cut)", 0x9CC },
            {"Roxas (Limit Cut)", 0x951 },
            {"Marluxia (Absent Silhouette)", 0x923 },
            {"Larxene (Absent Silhouette)", 0x962 },
            {"Zexion (Absent Silhouette)", 0x97B },
            {"Vexen (Absent Silhouette)", 0x933 },
            {"Lexaeus (Absent Silhouette)", 0x935 },
            {"Sephiroth", 0x8B6 },
            {"Riku (hooded)", 0x63C },
            {"Roxas", 0x05A }
        };

        public static Dictionary<string, int> dictUCMAddresses = new Dictionary<string, int>
        {
            {"Sora", 0x21CE0B68 },
            {"Donald", 0x21CE0B6A },
            {"Goofy", 0x21CE0B6C },
            {"Terra", 0x21C556E0 }
        };

        public Dictionary<string, short> GetUCMDigits()
        {
            return dictUCM;
        }

        public Dictionary<string, int> GetUCMAddresses()
        {
            return dictUCMAddresses;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KHMemory
{
    public class ObjEntry
    {
        public short UCM { get; set; }
        public byte Type { get; set; }
        public string ModelName { get; set; }
        public string MSETName { get; set; }
        public string ReadableName { get; set; }
    }

    //I guess that should be all of the most important ones. (for attackable objects with AI aka enemies and bosses)
    public enum ObjType
    {
        PLAYER = 0x0,
        PARTYMEMBERS = 0x1,
        BOSS = 0x3,
        ENEMIES = 0x4,
        BOSS2 = 0xC,
        ENEMIES2 = 0x15
    }
}

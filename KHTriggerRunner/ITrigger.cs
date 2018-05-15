using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace KHTriggerRunner
{
    [Serializable]
    public abstract class ITrigger
    {
        public abstract bool HasChildren();
        public abstract string DisplayName { get; set; }
    }
}

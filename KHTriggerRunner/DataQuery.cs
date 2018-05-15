using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace KHTriggerRunner
{
    public class DataQuery
    {
        private static DataQuery instance;
        private DataQuery()
        {
            AllTriggers = new ObservableCollection<ITrigger>();
        }
        public static DataQuery Instance
        {
            get
            {
                if (instance == null)
                    instance = new DataQuery();
                return instance;
            }
        }
        public ObservableCollection<ITrigger> AllTriggers { get; set; }

        public TriggerList FindParentList(Trigger t)
        {
            foreach(var item in AllTriggers)
            {
                if (item is TriggerList)
                {
                    TriggerList x = (TriggerList)item;
                    foreach(var newitem in x.Children)
                    {
                        if (newitem == t)
                            return x;
                    }
                }
            }
            return null;
        }
    }
}

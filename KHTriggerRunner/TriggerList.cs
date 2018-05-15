using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace KHTriggerRunner
{
    [Serializable()]
    public class TriggerList : ITrigger, ISerializable, INotifyPropertyChanged
    {
        public override string DisplayName { get; set; }
        public List<Trigger> Children { get; set; }

        public TriggerList()
        {
            Children = new List<Trigger>();
        }

        public TriggerList(SerializationInfo info, StreamingContext ctxt)
        {
            DisplayName = (string)info.GetValue("ListName", typeof(string));
            Children = (List<Trigger>)info.GetValue("Children", typeof(List<Trigger>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ListName", DisplayName);
            info.AddValue("Children", Children);
        }

        public override bool HasChildren()
        {
            if (Children.Count > 0)
                return true;
            return false;
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return this.isSelected; }
            set
            {
                if (value != this.isSelected)
                {
                    this.isSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }

        private bool isExpanded;
        public bool IsExpanded
        {
            get { return this.isExpanded; }
            set
            {
                if (value != this.isExpanded)
                {
                    this.isExpanded = value;
                    NotifyPropertyChanged("IsExpanded");
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}

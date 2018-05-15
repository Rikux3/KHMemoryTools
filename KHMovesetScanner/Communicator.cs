using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Threading;

namespace KHMovesetMemory
{
    public class Communicator : INotifyPropertyChanged
    {
        private static Communicator instance;
        private Changelog _changelog;
        private List<Moveset> _msets;
        private CodeEntries _entries;
        private Thread _th;
        private bool _runningexec;
        private Communicator() { }
        public static Communicator Instance
        {
            get
            {
                if (instance == null)
                    instance = new Communicator();
                return instance;
            }
        }

        public Changelog Changelog
        {
            get
            {
                return _changelog;
            }
            set
            {
                _changelog = value;
            }
        }

        public List<Moveset> Movesets
        {
            get
            {
                return _msets;
            }
            set
            {
                _msets = value;
            }
        }

        public CodeEntries Entries
        {
            get
            {
                return _entries;
            }
            set
            {
                _entries = value;
            }
        }
        public bool IsExecutionRunning
        {
            get
            {
                return _runningexec;
            }
            set
            {
                _runningexec = value;
                NotifyPropertyChanged("IsExecutionRunning");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public void StopExecution()
        {
            IsExecutionRunning = false;
        }

        public void StartExecution()
        {
            IsExecutionRunning = true;
            _th = new Thread(new ThreadStart(Communicator.Instance.Entries.Execute));
            _th.Start();
        }
    }
}

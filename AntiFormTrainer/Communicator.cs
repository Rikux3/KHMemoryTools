using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Threading;

namespace AntiFormTrainer
{
    public class Communicator : INotifyPropertyChanged
    {
        private static Communicator instance;
        private static UI.KHButton selected;
        private bool _pcsx2running;
        private CodeEntries _codes;
        private bool _runningexec = false;
        //private bool _usedonald = true;
        //private bool _usegoofy = true;
        private Thread _th;
        private Communicator()
        {
            _codes = new CodeEntries();
        }
        public static Communicator Instance
        {
            get
            {
                if (instance == null)
                    instance = new Communicator();
                return instance;
            }
        }

        public UI.KHButton SelectedButton
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                NotifyPropertyChanged("SelectedButton");
            }
        }
        public bool IsPCSX2Running
        {
            get
            {
                return _pcsx2running;
            }
            set
            {
                _pcsx2running = value;
                NotifyPropertyChanged("IsPCSX2Running");
            }
        }
        public CodeEntries Codes
        {
            get
            {
                return _codes;
            }
            set
            {
                _codes = value;
                NotifyPropertyChanged("Codes");
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
        //public bool UseDonald
        //{
        //    get
        //    {
        //        return _usedonald;
        //    }
        //    set
        //    {
        //        _usedonald = value;
        //        NotifyPropertyChanged("UseDonald");
        //    }
        //}
        //public bool UseGoofy
        //{
        //    get
        //    {
        //        return _usegoofy;
        //    }
        //    set
        //    {
        //        _usegoofy = value;
        //        NotifyPropertyChanged("UseGoofy");
        //    }
        //}
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
            _th = new Thread(new ThreadStart(Communicator.Instance.Codes.Execute));
            _th.Start();
        }
    }
}

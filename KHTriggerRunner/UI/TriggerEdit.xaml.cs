using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KHTriggerRunner
{
    /// <summary>
    /// Interaktionslogik für TriggerEdit.xaml
    /// </summary>
    public partial class TriggerEdit : Window
    {
        private Trigger _trigger;
        private bool bEditView = false;
        private TriggerList _parent;
        public TriggerEdit()
        {
            InitializeComponent();
            comboWorld.ItemsSource = WorldInfo.Worlds;
        }

        private void comboWorld_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboRoom.ItemsSource = WorldInfo.Rooms[comboWorld.SelectedIndex];
        }

        private void comboRoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboRoom.SelectedIndex != -1)
                comboEvent.ItemsSource = WorldInfo.AllEvents[comboWorld.SelectedIndex][comboRoom.SelectedIndex];
        }

        private void btnAddCode_Click(object sender, RoutedEventArgs e)
        {
            if (_trigger == null)
                _trigger = new Trigger();
            if (txtAddress.Text == string.Empty)
                return;
            int addr = Convert.ToInt32(txtAddress.Text, 16);
            int val = 0;
            RandomizerValue eRand = RandomizerValue.Undefined;
            if (chkRandomUcm.IsChecked == true)
            {
                if (rdEnemy.IsChecked == true)
                    eRand = RandomizerValue.Enemy;
                else if (rdBoss.IsChecked == true)
                    eRand = RandomizerValue.Boss;
                else if (rdAny.IsChecked == true)
                    eRand = RandomizerValue.Any;

                val = GetRandomUCM(eRand);
            }
            else
            {
                if (txtValue.Text == string.Empty)
                    return; 
                val = Convert.ToInt32(txtValue.Text, 16);
            }

            if (chkRandomUcm.IsChecked == true)
                _trigger.CodeEntries.Add(addr, val, chkRandomUcm.IsChecked ?? false, eRand);
            else
                _trigger.CodeEntries.Add(addr, val, chkRandomUcm.IsChecked ?? false);
            Update();
        }

        private int GetRandomUCM(RandomizerValue eRand)
        {
            Random rnd = new Random();
            switch (eRand)
            {
                case RandomizerValue.Boss:
                    goto getBoss;
                case RandomizerValue.Enemy:
                    goto getEnemy;
                case RandomizerValue.Any:
                    int x = rnd.Next(0, 2);
                    if (x == 0) goto getBoss;
                    else goto getEnemy;
                default:
                    return 0;

            }
            getBoss:
                int i = rnd.Next(0, WorldInfo.dictBosses.Count);
                return WorldInfo.dictBosses.ElementAt(i).Key;

            getEnemy:
                int j = rnd.Next(0, WorldInfo.dictEnemies.Count);
                return WorldInfo.dictEnemies.ElementAt(j).Key;
        }

        private void btnSaveTrigger_Click(object sender, RoutedEventArgs e)
        {
            if (bEditView)
            {
                //var prevItem = DataQuery.Instance.AllTriggers.Where(i => i.IsEqual(_trigger)).First();
                //var index = DataQuery.Instance.AllTriggers.IndexOf(prevItem);

                //DataQuery.Instance.AllTriggers[index] = _trigger;
                foreach (ITrigger item in DataQuery.Instance.AllTriggers)
                {
                    if (item.HasChildren())
                    {
                        var temp = item as TriggerList;
                        var prevItem = temp.Children.FirstOrDefault(i => i.IsEqual(_trigger));
                        var index = temp.Children.IndexOf(prevItem);

                        if (prevItem != null && index != -1)
                            temp.Children[index] = _trigger;
                    }
                    else
                    {
                        var trigger = item as Trigger;
                        if (trigger.IsEqual(_trigger))
                        {
                            var index = DataQuery.Instance.AllTriggers.IndexOf(trigger);
                            if (index != -1)
                            {
                                DataQuery.Instance.AllTriggers[index] = _trigger;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                _trigger.RoomInfo.WorldNumber = GetSubstring(comboWorld.SelectedItem.ToString());
                _trigger.RoomInfo.RoomNumber = GetSubstring(comboRoom.SelectedItem.ToString());
                if (chkEnableEvent.IsChecked == true)
                {
                    _trigger.HasEvent = true;
                    _trigger.RoomInfo.EventNumberMain = GetSubstring(comboEvent.SelectedItem.ToString());
                }
                else
                {
                    _trigger.HasEvent = false;
                    _trigger.RoomInfo.EventNumberMain = 0;
                }
                _trigger.StopWhenMapIsLoaded = chkStopAfterLoad.IsChecked ?? false;
                _trigger.IsWorldWarp = chkWorldWarp.IsChecked ?? false;

                if (_parent == null)
                    DataQuery.Instance.AllTriggers.Add(_trigger);
                else
                {
                    TriggerList item = (TriggerList)DataQuery.Instance.AllTriggers.Where(x => x.DisplayName == _parent.DisplayName).First();
                    item.Children.Add(_trigger);
                }
            }
            this.Close();
        }

        public int GetSubstring(string s)
        {
            string[] splitted = s.Split(new string[] { " - " }, StringSplitOptions.None);
            return Convert.ToInt32(splitted[0], 16);
        }

        public void ShowNewTrigger(TriggerList parent)
        {
            _parent = parent;
            ShowNewTrigger();
        }

        public void ShowNewTrigger()
        {
            _trigger = new Trigger();
            Update();
            ShowDialog();
        }

        public void ShowEditTrigger(Trigger t)
        {
            _trigger = t;
            bEditView = true;
            ShowDialog();
        }

        public void Update()
        {
            gridCodes.ItemsSource = _trigger.CodeEntries.Entries;
        }

        private void gridCodes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = gridCodes.SelectedItem;
            if (item != null && item is SingleEntry)
            {
                SingleEntry entry = item as SingleEntry;
                txtAddress.Text = entry.Address.ToString("x8");
                txtValue.Text = entry.Value.ToString("x8");
                chkRandomUcm.IsChecked = entry.IsRandomUCM;
                RandomizerValue eR = entry.eRand;
                switch (eR)
                {
                    case RandomizerValue.Enemy:
                        rdEnemy.IsChecked = true;
                        break;
                    case RandomizerValue.Boss:
                        rdBoss.IsChecked = true;
                        break;
                    case RandomizerValue.Any:
                        rdAny.IsChecked = true;
                        break;
                    default:
                        break;
                }
            }
        }

        private void btnRemoveCode_Click(object sender, RoutedEventArgs e)
        {
            if (gridCodes.SelectedItem == null)
                return;
            SingleEntry en = (SingleEntry)gridCodes.SelectedItem;
            //int addr = Convert.ToInt32(txtAddress.Text, 16);
            //int val = Convert.ToInt32(txtValue.Text, 16);
            //SingleEntry item = new SingleEntry();
            //item.Address = addr;
            //item.Value = val;
            //item.IsRandomUCM = (bool)chkRandomUcm.IsChecked;
            _trigger.CodeEntries.Remove(en);
            Update();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (bEditView)
            {
                comboEvent.IsEnabled = false;
                comboRoom.IsEnabled = false;
                comboWorld.IsEnabled = false;
                gridCodes.ItemsSource = _trigger.CodeEntries.Entries;

                int w = WorldInfo.GetWorldIndex(_trigger.RoomInfo.WorldNumber);
                int r = WorldInfo.GetRoomIndex(w, _trigger.RoomInfo.RoomNumber);
                int ev = 0;
                if (_trigger.HasEvent)
                    ev = WorldInfo.GetEventIndex(w, r, _trigger.RoomInfo.EventNumberMain);

                comboWorld.SelectedIndex = w;
                comboRoom.SelectedIndex = r;

                chkEnableEvent.IsEnabled = false;
                if (_trigger.HasEvent)
                {
                    chkEnableEvent.IsChecked = true;
                    comboEvent.SelectedIndex = ev;
                }
                else
                    chkEnableEvent.IsChecked = false;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnImportCode_Click(object sender, RoutedEventArgs e)
        {
            string[] codes = TriggerSerializer.ReadCodeList();
            foreach (string codeline in codes)
            {
                string[] splitted = codeline.Split(' ');
                SingleEntry en = new SingleEntry();
                en.Address = Convert.ToInt32(splitted[0], 16);
                en.Value = Convert.ToInt32(splitted[1], 16);
                _trigger.CodeEntries.Entries.Add(en);
            }
            Update();
        }

        private void chkEnableEvent_Checked(object sender, RoutedEventArgs e)
        {
            comboEvent.IsEnabled = true;
        }

        private void chkEnableEvent_Unchecked(object sender, RoutedEventArgs e)
        {
            comboEvent.IsEnabled = false;
        }

        private void chkRandomUcm_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}

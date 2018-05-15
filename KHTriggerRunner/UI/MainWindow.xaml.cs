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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading.Tasks;
using System.Threading;
using KHMemory;

namespace KHTriggerRunner
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool bRun = false;
        Thread th;
        public string World { get; set; }
        public string Room { get; set; }
        public string Event { get; set; }
        private List<string> expandedNodes = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
            if (PCSX2_RAM.SeekPCSX2())
                lblStatusPCSX2.Content = "PCSX2 found!";
            else
                lblStatusPCSX2.Content = "PCSX2 NOT found!";

            DataQuery.Instance.AllTriggers.CollectionChanged += AllTriggers_CollectionChanged;
            TreeViewItem groups = new TreeViewItem();
            groups.Expanded += Groups_Expanded;
            groups.Collapsed += Groups_Collapsed;
        }

        private void Groups_Collapsed(object sender, RoutedEventArgs e)
        {
            TreeViewItem treeNode = sender as TreeViewItem;
            string header = treeNode.Header.ToString();
            if (expandedNodes.Contains(header))
            {
                expandedNodes.Remove(header);
            }
        }

        private void Groups_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem treeNode = sender as TreeViewItem;
            string header = treeNode.Header.ToString();
            if (expandedNodes.Contains(header) == false)
            {
                expandedNodes.Add(header);
            }
        }

        private void AllTriggers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            lstBoxTriggers.ItemsSource = DataQuery.Instance.AllTriggers;
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public static void ClearTreeViewSelection(TreeView tv)
        {
            if (tv != null)
                ClearTreeViewItemsControlSelection(tv.Items, tv.ItemContainerGenerator);
        }
        private static void ClearTreeViewItemsControlSelection(ItemCollection ic, ItemContainerGenerator icg)
        {
            if ((ic != null) && (icg != null))
                for (int i = 0; i < ic.Count; i++)
                {
                    TreeViewItem tvi = icg.ContainerFromIndex(i) as TreeViewItem;
                    if (tvi != null)
                    {
                        ClearTreeViewItemsControlSelection(tvi.Items, tvi.ItemContainerGenerator);
                        tvi.IsSelected = false;
                    }
                }
        }

        private void btnAddCode_Click(object sender, RoutedEventArgs e)
        {
            TriggerEdit ed = new TriggerEdit();
            var item = lstBoxTriggers.SelectedItem;
            if (item is TriggerList)
            {
                ed.ShowNewTrigger(item as TriggerList);
                lstBoxTriggers.Items.Refresh();

                foreach (var it in lstBoxTriggers.Items)
                {
                    if (it is TriggerList)
                    {
                        TriggerList item2 = item as TriggerList;
                        TriggerList bg34 = it as TriggerList;
                        if (item2.DisplayName == bg34.DisplayName)
                            item2.IsExpanded = true;
                    }
                }
            }
            else
            {
                //trying to get parent itemlist
                TriggerList trytofind = DataQuery.Instance.FindParentList(item as Trigger);
                if (trytofind != null)
                    ed.ShowNewTrigger(trytofind);
                else
                    ed.ShowNewTrigger();

                lstBoxTriggers.Items.Refresh();

                foreach (var it in lstBoxTriggers.Items)
                {
                    if (it is TriggerList)
                    {
                        TriggerList bg34 = it as TriggerList;
                        if (trytofind.DisplayName == bg34.DisplayName)
                            bg34.IsExpanded = true;
                    }
                }
            }

            //Refresh gui
            //http://stackoverflow.com/questions/11540459/create-refresh-a-wpf-treeview-and-remember-expanded-nodes-without-xaml
            //lstBoxTriggers.UpdateLayout();
        }

        private void lstBoxTriggers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selected = lstBoxTriggers.SelectedItem;
            if (selected is Trigger)
            {
                TriggerEdit ed = new TriggerEdit();
                ed.ShowEditTrigger(selected as Trigger);
            }

        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            waiting.Visibility = Visibility.Visible;
            bRun = true;
            th = new Thread(new ThreadStart(RunLoop));
            th.Start();
        }

        private void RunLoop()
        {
            while (bRun)
            {
                RoomInformation r = ReadRoomInforation();
                World= r.WorldNumber.ToString("X");
                Room = r.RoomNumber.ToString("X");
                Event = r.EventNumberMain.ToString("X");
                foreach (var item in DataQuery.Instance.AllTriggers)
                {
                    if (item.HasChildren())
                    {
                        var list = item as TriggerList;
                        foreach (Trigger child in list.Children)
                        {
                            RunTrigger(child, r);
                        }
                    }
                    else
                    {
                        var t = item as Trigger;
                        RunTrigger(t, r);
                    }
                }
            }
        }

        public void RunTrigger(Trigger t, RoomInformation r)
        {
            if (t.IsTriggered(r))
            {
                if (t.StopWhenMapIsLoaded)
                {
                    if (PCSX2_RAM.IsRoomWithPlayerLoaded())
                    {
                        return;
                    }
                }
                //Console.WriteLine("Executing trigger - " + t.RoomInfo.ToString());
                t.CodeEntries.Execute();
            }
        }

        public static RoomInformation ReadRoomInforation()
        {
            RoomInformation r = new RoomInformation();
            r.WorldNumber = PCSX2_RAM.ReadBytes(IngameConstants.WORLD_PTR, 1)[0];
            r.RoomNumber = PCSX2_RAM.ReadShort(IngameConstants.ROOM_PTR);
            r.EventNumberMain = PCSX2_RAM.ReadShort(IngameConstants.EVENT1_PTR);
            return r;
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            var list = TriggerSerializer.DeserializeTrigger();
            foreach (var item in list)
            {
                DataQuery.Instance.AllTriggers.Add(item);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            TriggerSerializer.SerializeTrigger(DataQuery.Instance.AllTriggers);
        }

        private void btnTestMap_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("" + PCSX2_RAM.IsRoomWithPlayerLoaded());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //TriggerList l = new TriggerList();
            //l.DisplayName = "TEST";
            //l.Children.Add(new Trigger(1, 2, 3));
            //DataQuery.Instance.AllTriggers.Add(l);
            //DataQuery.Instance.AllTriggers.Add(new Trigger(5,6,5));

            //TriggerList x = new TriggerList();
            //x.DisplayName = "TEST2";
            //x.Children.Add(new Trigger(6, 4, 7));
            //DataQuery.Instance.AllTriggers.Add(x);
        }

        private void lstBoxTriggers_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ClearTreeViewSelection(lstBoxTriggers);
        }

        private void btnAddGroup_Click(object sender, RoutedEventArgs e)
        {
            InputBox.Visibility = Visibility.Visible;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            if (InputTextBox.Text != string.Empty)
            {
                TriggerList l = new TriggerList();
                l.DisplayName = InputTextBox.Text;
                DataQuery.Instance.AllTriggers.Add(l);
                InputBox.Visibility = Visibility.Collapsed;
            }
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            InputBox.Visibility = Visibility.Collapsed;
            InputTextBox.Text = string.Empty;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            bRun = false;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            bRun = false;
            th.Abort();
            waiting.Visibility = Visibility.Collapsed;
        }

        private void lstBoxTriggers_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = lstBoxTriggers.SelectedItem;
            if (item is TriggerList)
                lstBoxTriggers.ContextMenu = lstBoxTriggers.Resources["contextGroup"] as System.Windows.Controls.ContextMenu;
            else if (item is Trigger)
                lstBoxTriggers.ContextMenu = lstBoxTriggers.Resources["contextItem"] as System.Windows.Controls.ContextMenu;
        }

        private void lstBoxTriggers_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        private void contextEditTrigger_Click(object sender, RoutedEventArgs e)
        {
            var selected = lstBoxTriggers.SelectedItem;
            if (selected is Trigger)
            {
                TriggerEdit ed = new TriggerEdit();
                ed.ShowEditTrigger(selected as Trigger);
            }
        }
    }
}

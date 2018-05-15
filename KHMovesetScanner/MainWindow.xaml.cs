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
using WpfAnimatedGif;
using System.Collections.ObjectModel;
using KHMemory;

namespace KHMovesetMemory
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PCSX2_RAM.SeekPCSX2();
        }

        public short SelectedUCM { get; set; }
        public Moveset SelectedMoveset { get; set; }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ScanMovesets();
        }

        async void ScanMovesets()
        {
            if (comboBox1.SelectedValue != null || txtAltUCM.Text != string.Empty)
            {
                //System.Diagnostics.Stopwatch w = new System.Diagnostics.Stopwatch();
                //w.Start();
                short ucm;
                if (chkAltUCM.IsChecked == true)
                    ucm = Convert.ToInt16(txtAltUCM.Text, 16);
                else
                    ucm = (short)comboBox1.SelectedValue;
                MovesetHandler msetHandler = new MovesetHandler(ucm);
                Communicator.Instance.Changelog = new Changelog(ucm);

                waiting.Visibility = Visibility.Visible;
                await Task.Run(() => msetHandler.InitMovesetScan());
                comboBox.ItemsSource = Communicator.Instance.Movesets;
                comboBox.DisplayMemberPath = "Name";
                waiting.Visibility = Visibility.Collapsed;
                //Communicator.Instance.Changelog.MovesetCount = Communicator.Instance.Movesets.Count;
                //w.Stop();
                //MessageBox.Show("Time: " + w.Elapsed.Minutes + ":" + w.Elapsed.Seconds + "." + w.Elapsed.Milliseconds);
            }
            else
                MessageBox.Show("Please select an character!");
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //dirty solution
            Moveset item = comboBox.SelectedValue as Moveset;

            //Header info
            //txtSlotNum.Text = item.SlotNumber.ToString();
            txtAnbAddress.Text = item.ANBAddress.ToString("X8");
            lstRefBAR.Items.Clear();
            foreach (var ic in item.AddressReferences)
            {
                lstRefBAR.Items.Add(ic.ToString("X8"));
            }

            //Animation
            txtAnimAddr.Text = item.Animation.Address.ToString("X8");
            txtAnimValue.Text = item.Animation.Value.ToString("X8");

            //Effect
            txtEffectAddr.Text = item.Effect.Address.ToString("X8");
            txtEffectValue.Text = item.Effect.Value.ToString("X8");

            //Bone Structure
            txtBoneAddr.Text = item.Effect.BoneStructure.Address.ToString("X8");
            txtBoneValue.Text = item.Effect.BoneStructure.Value.ToString("X8");

            gridDataMods.Items.Clear();
            //gridECgrp1.Items.Clear();
            //gridECgrp2.Items.Clear();
            if (item.Effect.DataModifiers != null)
            {
                //Effect Data Mods
                for (int i = 0; i < item.Effect.DataModifiers.Count; i++)
                {
                    gridDataMods.Items.Add(item.Effect.DataModifiers[i]);

                }

                //ECaster
                //for (int i=0; i<item.Effect.EffectCasterList.Children.Count; i++)
                //{
                //    if (item.Effect.EffectCasterList.Children[i].Group == ECasterGroup.Group1)
                //    {
                //        gridECgrp1.Items.Add(item.Effect.EffectCasterList.Children[i]);
                //    }
                //    else
                //    {
                //        gridECgrp2.Items.Add(item.Effect.EffectCasterList.Children[i]);
                //    }
                //}
            }
            ObservableCollection<Change> list;
            Communicator.Instance.Changelog.ListOfChanges.TryGetValue(comboBox.SelectedIndex, out list);
            if (list != null)
                lstChanges.ItemsSource = list;
            else
                lstChanges.ItemsSource = null;
        }

        private void gridDataMods_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
        }

        //private void ucmScanAI_Click(object sender, RoutedEventArgs e)
        //{
        //    short ucm = (short)ucmAI.SelectedValue;
        //    AIReader air = new AIReader(ucm);
        //    air.ScanAI();
        //    lstAI.ItemsSource = air.entries;
        //}

        //private void lstAI_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var item = (AIEntry)lstAI.SelectedItem;
        //    txtAIAddr.Text = item.Address.ToString("X");
        //    txtAIName.Text = item.Name;
        //    txtAILength.Text = item.Length.ToString();
        //}

        private void gridDataMods_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var item = (MovesetEffectDataModifierRaw)gridDataMods.SelectedItem;
            //txtSingleEffect.Text = item.Value.ToString("X");
        }

        private void btnEditMoveset_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox.SelectedItem != null)
            {
                EditMovesetWindow emw = new EditMovesetWindow();
                emw.Closing += Emw_Closing;
                emw.Show(comboBox.SelectedIndex);
            }
        }

        private void Emw_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ObservableCollection<Change> list;
            Communicator.Instance.Changelog.ListOfChanges.TryGetValue(comboBox.SelectedIndex, out list);
            if (list != null)
                lstChanges.ItemsSource = list;
        }

        private void btnConvert_Click(object sender, RoutedEventArgs e)
        {
            if (Communicator.Instance.Changelog == null)
                MessageBox.Show("No changes. Could not execute them :/");
            else
            {
                Communicator.Instance.Entries = Communicator.Instance.Changelog.ConvertToCodes();
                Communicator.Instance.Entries.Execute();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (Communicator.Instance.Changelog == null)
                MessageBox.Show("Your Changelog is empty and therefore can not be saved!");
            else
                ChangeSerializer.SerializeChangelist(Communicator.Instance.Changelog);
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            if (Communicator.Instance.Movesets == null)
                MessageBox.Show("Please scan an moveset first!");
            else
            {
                Changelog l = ChangeSerializer.DeserializeChangelist();
                if (l != null)
                {
                    if (l.UCM != (short)comboBox1.SelectedValue)
                        MessageBox.Show("This Moveset Mod is not compatible with the selected Character!");
                    //else if (l.MovesetCount != Communicator.Instance.Movesets.Count)
                    //    MessageBox.Show("The scanned moveset does not equal the moveset ram countwise.\r\n" +
                    //                    "Please restart this tool and PCSX2 and rescan the moveset!");
                    else
                        Communicator.Instance.Changelog = l;
                }
            }
        }

        private void lstChanges_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Load selected change
            //Change ch = (Change)lstChanges.SelectedItem;
            //EditMovesetWindow emw = new EditMovesetWindow();
            //emw.Show(ch);
        }

        private void contextDeleteChange_Click(object sender, RoutedEventArgs e)
        {
            Change selChange = (Change)lstChanges.SelectedItem;
            ObservableCollection<Change> list;
            Communicator.Instance.Changelog.ListOfChanges.TryGetValue(comboBox.SelectedIndex, out list);
            if (list != null)
            {
                list.Remove(list.Where(x => x.ChangeNum == selChange.ChangeNum).Single());
            }
        }

        private void chkAltUCM_Checked(object sender, RoutedEventArgs e)
        {
            txtAltUCM.IsEnabled = true;
            comboBox1.IsEnabled = false;
        }

        private void chkAltUCM_Unchecked(object sender, RoutedEventArgs e)
        {
            txtAltUCM.IsEnabled = false;
            comboBox1.IsEnabled = true;
        }

        //private void radioPCSX2_Checked(object sender, RoutedEventArgs e)
        //{
        //    Communicator.Instance.ScMode = ScanningMode.ePCSX2;
        //    PCSX2_RAM.SeekPCSX2();
        //}

        //private void radioRPCS3_Checked(object sender, RoutedEventArgs e)
        //{
        //    Communicator.Instance.ScMode = ScanningMode.eRPCS3;
        //    PCSX2_RAM.SeekRPCS3();
        //}
    }
}

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace KHMovesetMemory
{
    /// <summary>
    /// Interaktionslogik für EditMovesetWindow.xaml
    /// </summary>
    public partial class EditMovesetWindow : Window
    {
        private Moveset _originalMset;
        private int _index;
        private Change _selectedChange;
        public EditMovesetWindow()
        {
            InitializeComponent();
        }

        private void comboSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (comboSelect.SelectedIndex)
            {
                case 0:
                    stackPFromMset.Visibility = Visibility.Visible;
                    stackPbaselinemod.Visibility = Visibility.Collapsed;
                    stackPValue.Visibility = Visibility.Collapsed;
                    stackPFAKE.Visibility = Visibility.Visible;
                    if (_originalMset.AddressReferences.Count > 1)
                        stackPWhichRef.Visibility = Visibility.Visible;
                    else
                        stackPWhichRef.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                case 2:
                case 3:
                    stackPbaselinemod.Visibility = Visibility.Collapsed;
                    stackPValue.Visibility = Visibility.Visible;
                    stackPFromMset.Visibility = Visibility.Visible;
                    stackPFAKE.Visibility = Visibility.Collapsed;
                    stackPWhichRef.Visibility = Visibility.Collapsed;
                    LoadInfoIntoTextbox(comboSelect.SelectedIndex);
                    break;
                case 4:
                    stackPValue.Visibility = Visibility.Visible;
                    stackPFromMset.Visibility = Visibility.Collapsed;
                    stackPbaselinemod.Visibility = Visibility.Visible;
                    stackPWhichRef.Visibility = Visibility.Collapsed;
                    stackPFAKE.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void LoadInfoIntoTextbox(int index)
        {
            switch(index)
            {
                case 1:
                    txtValue.Text = _originalMset.Animation.Value.ToString("X8");
                    break;
                case 2:
                    txtValue.Text = _originalMset.Effect.Value.ToString("X8");
                    break;
                case 3:
                    txtValue.Text = _originalMset.Effect.BoneStructure.Value.ToString("X8");
                    break;
                default:
                    return;
            }
        }

        public void Show(int index)
        {
            _originalMset = Communicator.Instance.Movesets[index];
            _index = index;
            comboEffectData.ItemsSource = _originalMset.Effect.DataModifiers;
            comboMset.ItemsSource = Communicator.Instance.Movesets;
            comboRef.ItemsSource = _originalMset.AddressReferences;
            this.Title = "Edit Moveset - " + _originalMset.Name;
            base.Show();
        }

        //public void Show(Change ch, int index)
        //{
        //    _selectedChange = ch;
        //    Show(index);
        //    if (_selectedChange is WholeMovesetChange)
        //    {
        //        stackPFromMset.Visibility = Visibility.Visible;
        //        stackPbaselinemod.Visibility = Visibility.Collapsed;
        //        stackPValue.Visibility = Visibility.Collapsed;
        //        comboSelect.SelectedIndex = 0;
        //        comboMset.SelectedIndex = 
        //    }
        //}

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Change> testIfExists;
            Communicator.Instance.Changelog.ListOfChanges.TryGetValue(_index, out testIfExists);
            if (testIfExists == null)
                Communicator.Instance.Changelog.ListOfChanges.Add(_index, new ObservableCollection<Change>());

            switch (comboSelect.SelectedIndex)
            {
                case 0:
                    WholeMovesetChange c = new WholeMovesetChange();
                    c.ChangeNum = Communicator.Instance.Changelog.GetNextChangeId;
                    c.OldMset = _index;
                    if (chkUseFAKEValue.IsChecked == true)
                        c.NewMset = -1;
                    else
                        c.NewMset = comboMset.SelectedIndex;

                    if (radioAll.IsChecked == true)
                        c.TypeOfRef = TypeOfWhole.eAll;
                    else
                    {
                        c.TypeOfRef = TypeOfWhole.eSingle;
                        c.SingleRef = _originalMset.AddressReferences[comboRef.SelectedIndex];
                    }

                    Communicator.Instance.Changelog.ListOfChanges[_index].Add(c);
                    break;
                case 1:
                case 2:
                case 3:
                    AnimEffectBoneChange c2 = new AnimEffectBoneChange();
                    c2.ChangeNum = Communicator.Instance.Changelog.GetNextChangeId;
                    c2.FromMset = comboMset.SelectedIndex;
                    if (comboSelect.SelectedIndex == 1)
                        c2.type = TypeOfChange.eAnimation;
                    else if (comboSelect.SelectedIndex == 2)
                        c2.type = TypeOfChange.eEffect;
                    else if (comboSelect.SelectedIndex == 3)
                        c2.type = TypeOfChange.eBoneStructure;
                    Communicator.Instance.Changelog.ListOfChanges[_index].Add(c2);
                    break;
                case 4:
                    EffectDataChange c3 = new EffectDataChange();
                    c3.ChangeNum = Communicator.Instance.Changelog.GetNextChangeId;
                    c3.LineNumber = comboEffectData.SelectedIndex;
                    c3.NewValue = Convert.ToInt32(txtValue.Text, 16);
                    Communicator.Instance.Changelog.ListOfChanges[_index].Add(c3);
                    break;
            }
            Close();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MovesetEffectDataModifierRaw sel = (MovesetEffectDataModifierRaw)comboEffectData.SelectedItem;
            txtValue.Text = sel.Value.ToString("X8");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Application curApp = Application.Current;
            Window mainWindow = curApp.MainWindow;
            this.Left = mainWindow.Left + (mainWindow.Width - this.ActualWidth) / 2;
            this.Top = mainWindow.Top + (mainWindow.Height - this.ActualHeight) / 2;
        }
    }
}

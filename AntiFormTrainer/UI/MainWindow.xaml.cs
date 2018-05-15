using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace AntiFormTrainer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _mainFrame.Navigate(new UI.Home());
            Communicator.Instance.IsPCSX2Running = KHMemory.PCSX2_RAM.SeekPCSX2();

            //Set up anti-form codes (move to extra class)
            Communicator.Instance.Codes.Add(KHMemory.IngameConstants.CURRENT_DRIVE_PTR, 6, 0, typeof(short));
            Communicator.Instance.Codes.Add(0x21CE0B68, 0x59, 0x54, typeof(short)); //Sora standard
            Communicator.Instance.Codes.Add(0x21CE121C, 0x672, 0x657, typeof(short)); //Timeless River
            Communicator.Instance.Codes.Add(0x21CE0FAC, 0x3ea, 0x2b5, typeof(short)); //Halloween Town
            Communicator.Instance.Codes.Add(0x21CE0FE0, 0x95a, 0x955, typeof(short)); //Christmas
            Communicator.Instance.Codes.Add(0x21CE1250, 0x59, 0x28a, typeof(short)); //Lion
            Communicator.Instance.Codes.Add(0x21CE11E8, 0x671, 0x656, typeof(short)); //Space Paranoids

        }

        private void _mainFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            var item = (Page)e.Content;
            if (item.Title == "Home")
                BorderCurrentSub.Visibility = Visibility.Collapsed;
            else
            {
                BorderCurrentSub.Visibility = Visibility.Visible;
                BorderCurrentSubText.Text = item.Title;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Communicator.Instance.StopExecution();
            Communicator.Instance.Codes.RevertAllCodes();
        }
    }
}

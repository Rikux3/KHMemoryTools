using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace AntiFormTrainer.UI
{
    /// <summary>
    /// Interaktionslogik für Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UI.Options());
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (!Communicator.Instance.IsPCSX2Running)
                Communicator.Instance.IsPCSX2Running = KHMemory.PCSX2_RAM.SeekPCSX2();

            if (!Communicator.Instance.IsPCSX2Running)
                new InformationWindow().Show("PCSX2 is not running!");
            else
            {
                Communicator.Instance.IsExecutionRunning = !Communicator.Instance.IsExecutionRunning;
                if (Communicator.Instance.IsExecutionRunning)
                {
                    Communicator.Instance.StartExecution();
                    btnToggle.Content = "Deactivate";
                    btnToggle.Description = "Deactivate the trainer.";
                }
                else
                {
                    btnToggle.Content = "Activate";
                    btnToggle.Description = "Activate the trainer.";
                }
            }
        }

        private void btnCredits_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UI.Pages.Credits());
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}

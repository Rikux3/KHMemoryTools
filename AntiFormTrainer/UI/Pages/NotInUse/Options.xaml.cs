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

namespace AntiFormTrainer.UI
{
    /// <summary>
    /// Interaktionslogik für Options.xaml
    /// </summary>
    public partial class Options : Page
    {
        public Options()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            //Kill bitches?
            //bool isRunning = (Communicator.Instance.IsExecutionRunning == true);
            //Communicator.Instance.IsExecutionRunning = false;
            //if (Communicator.Instance.UseDonald)
            //    Communicator.Instance.Codes.EnableDonald();
            //else
            //    Communicator.Instance.Codes.DisableDonald();

            //if (Communicator.Instance.UseGoofy)
            //    Communicator.Instance.Codes.EnableGoofy();
            //else
            //    Communicator.Instance.Codes.DisableGoofy();

            //if (isRunning)
            //{
            //    Communicator.Instance.IsExecutionRunning = true;
            //    Communicator.Instance.StartExecution();
            //}

            NavigationService.GoBack();
        }

        private void btnEnableDonald_Click(object sender, RoutedEventArgs e)
        {
            //Communicator.Instance.UseDonald = true;
        }

        private void btnDisableDonald_Click(object sender, RoutedEventArgs e)
        {
            //Communicator.Instance.UseDonald = false;
        }

        private void btnEnableGoofy_Click(object sender, RoutedEventArgs e)
        {
            //Communicator.Instance.UseGoofy = true;
        }

        private void btnDisableGoofy_Click(object sender, RoutedEventArgs e)
        {
            //Communicator.Instance.UseGoofy = false;
        }
    }
}

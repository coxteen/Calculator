using Calculator.ViewModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        private MainVM _mainVM;
        public MainWindow()
        {
            InitializeComponent();
            _mainVM = new MainVM
            {
                DigitGrouping = Properties.Settings.Default.digitGrouping,
                Programmer = Properties.Settings.Default.programmer,
                BaseForCalculations = Properties.Settings.Default.baseForCalculations,
                OperationPriority = Properties.Settings.Default.operationPriority
            };

            this.DataContext = _mainVM;
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ghiujan Costin Daniel, Grupa 10LF232, Informatica, Anul 2", "Detalii despre mine", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void DigitGrouping_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.digitGrouping = !Properties.Settings.Default.digitGrouping;
            _mainVM.DigitGrouping = Properties.Settings.Default.digitGrouping;
            Properties.Settings.Default.Save();
        }
        private void Programmer_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.programmer = !Properties.Settings.Default.programmer;
            _mainVM.Programmer = Properties.Settings.Default.programmer;
            Properties.Settings.Default.Save();
        }
        private void OperationPriority_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.operationPriority = !Properties.Settings.Default.operationPriority;
            _mainVM.OperationPriority = Properties.Settings.Default.operationPriority;
            Properties.Settings.Default.Save();
        }
        private void BaseForCalculations_Click(object sender, RoutedEventArgs e)
        {
            // To do
        }
    }
}

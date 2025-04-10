using Calculator.ViewModel;
using System.Collections.Generic;
using System.Windows;

namespace Calculator
{
    public partial class MemoryWindow : Window
    {
        public decimal SelectedValue { get; private set; }
        public MemoryWindow(List<decimal> memoryStack)
        {
            InitializeComponent();
            MemoryListBox.ItemsSource = memoryStack;
        }
        private void MemoryListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (MemoryListBox.SelectedItem != null)
            {
                SelectedValue = (decimal)MemoryListBox.SelectedItem;
                DialogResult = true;
                Close();
            }
        }
    }
}

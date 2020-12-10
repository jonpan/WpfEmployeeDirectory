
using System.Windows;
using System.Windows.Controls;
using WpfEmployeeDirectory.Models;
using WpfEmployeeDirectory.ViewModels;

namespace WpfEmployeeDirectory.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        EmployeeViewModel evm;
        public MainWindow()
        {
            InitializeComponent();
            // set datacontext for binding
            evm = new EmployeeViewModel();
            base.DataContext = evm;
        }

    }
}

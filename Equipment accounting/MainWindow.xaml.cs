using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Equipment_accounting
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StartLabel.Visibility = Visibility.Visible;
        }

        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            StartLabel.Visibility = Visibility.Hidden;

            ResizeMode = ResizeMode.CanResize;
        }
        AddWindow addWindow;
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            addWindow = new AddWindow
            {
                Owner = this,
                DataContext = this.DataContext
            };
            ViewModel viewModel = DataContext as ViewModel;
            viewModel.OpenCommand.Execute("Add");
            addWindow.ShowDialog();
        }
        ReplaceWindow replaceWindow;
        private void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            replaceWindow = new ReplaceWindow
            {
                Owner = this,
                DataContext = this.DataContext
            };
            ViewModel viewModel = DataContext as ViewModel;
            viewModel.OpenCommand.Execute("Replace");
            replaceWindow.ShowDialog();
        }
    }
}

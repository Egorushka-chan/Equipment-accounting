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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddWindow addWindow = new AddWindow
            {
                Owner = this,
                DataContext = this.DataContext
            };
            (DataContext as ViewModel).OpenCommand.Execute("AddEq");
            addWindow.ShowDialog();
        }

        private void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            ReplaceWindow replaceWindow = new ReplaceWindow
            {
                Owner = this,
                DataContext = this.DataContext
            };
            (DataContext as ViewModel).OpenCommand.Execute("ReplaceEq");
            replaceWindow.ShowDialog();
        }
        
        private void AllDataButton_Click(object sender, RoutedEventArgs e)
        {
            AllDataWindow allDataWindow = new AllDataWindow
            {
                Owner = this,
                DataContext = this.DataContext
            };
            allDataWindow.Show();
        }
    }
}

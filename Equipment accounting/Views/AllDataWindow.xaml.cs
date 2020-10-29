using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Equipment_accounting
{
    /// <summary>
    /// Логика взаимодействия для AllDataWindow.xaml
    /// </summary>
    public partial class AllDataWindow : Window
    {
        public AllDataWindow()
        {
            InitializeComponent();
        }

        private void AddStateButton_Click(object sender, RoutedEventArgs e)
        {
            AddStateWindow addStateWindow = new AddStateWindow
            {
                DataContext = this.DataContext
            };
            (DataContext as ViewModel).OpenCommand.Execute("AddSt");
            addStateWindow.ShowDialog();
        }

        private void AddSubdivisionButton_Click(object sender, RoutedEventArgs e)
        {
            AddSubdivisionWindow addSubdivisionWindow = new AddSubdivisionWindow
            {
                DataContext = this.DataContext
            };
            (DataContext as ViewModel).OpenCommand.Execute("AddSub");
            addSubdivisionWindow.ShowDialog();
        }
    }
}

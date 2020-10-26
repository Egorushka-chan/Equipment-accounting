using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
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
using MySql.Data.MySqlClient;

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
            DataContext = new ViewModel();
        }

        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            StartLabel.Visibility = Visibility.Hidden;

            ResizeMode = ResizeMode.CanResize;
        }
    }

    class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string properity = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(properity));
        }

        public static string ConnectionInfo = "server = localhost;port = 3306;username=root;password=tamara23;database=equipment accounting";

        public ObservableCollection<Equipment> Equipments { get; set; }

        public ObservableCollection<ComboBoxItem> StateItems { get; set; }

        public ObservableCollection<ComboBoxItem> FilterStateItems { get; set; }


        private Equipment selectedEquipment;
        public Equipment SelectedEquipment
        {
            get { return selectedEquipment; }
            set
            {
                selectedEquipment = value;
                OnPropertyChanged("SelectedEquipment");
            }
        }

        public ViewModel()
        {
            DataSet dataSet = new DataSet();

            using (MySqlConnection connection = new MySqlConnection(ConnectionInfo))
            {
                MySqlCommand command = new MySqlCommand("Select `equipment`.`inventory number` AS `IN`, `equipment`.`name` AS `Name`, `subdivisions`.`name` AS `Subdivision`, `states`.`state` AS `State` " +
                    "from(`equipment` JOIN `subdivisions` ON `equipment`.`subdivision_id` = `subdivisions`.`id`) JOIN `states` ON `equipment`.`state_id` = `states`.`id` " +
                    "order by `IN`",
                    connection);

                using (DataTable dataTable = new DataTable("Base"))
                {
                    connection.Open();
                    dataTable.Load(command.ExecuteReader());
                    connection.Close();
                    dataSet.Tables.Add(dataTable);
                }

                command.CommandText = "select `states`.`id`, `states`.`state` from `states`";

                using (DataTable dataTable = new DataTable("ComboBox"))
                {
                    connection.Open();
                    dataTable.Load(command.ExecuteReader());
                    connection.Close();
                    dataSet.Tables.Add(dataTable);
                }
            }

            Equipments = new ObservableCollection<Equipment>();

            foreach (DataRow dataRow in dataSet.Tables["Base"].Rows)
            {
                Equipments.Add(new Equipment 
                { 
                    IN = (int)dataRow["IN"], 
                    Name = (string)dataRow["Name"], 
                    Subdivision = (string)dataRow["Subdivision"], 
                    State = (string)dataRow["State"] 
                });
            }

            StateItems = new ObservableCollection<ComboBoxItem>();
            FilterStateItems = new ObservableCollection<ComboBoxItem>
            {
                new ComboBoxItem{ Content = "0. Все"}
            };

            foreach (DataRow dataRow in dataSet.Tables["ComboBox"].Rows)
            {
                StateItems.Add(new ComboBoxItem { Content = $"{dataRow["id"]}. {dataRow["state"]}"});
            }

            FilterStateItems.
        }
    }
}

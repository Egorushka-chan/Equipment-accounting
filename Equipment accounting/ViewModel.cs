using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using MySql.Data.MySqlClient;

namespace Equipment_accounting
{
    class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string properity = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(properity));
            }
                
        }

        public static string ConnectionInfo = "server = localhost;port = 3306;username=root;password=root;database=equipment accounting";

        private ObservableCollection<string> QueriesToBeExecuted = new ObservableCollection<string>();
        public ObservableCollection<Equipment> Equipments { get; set; } // Коллекции записей
        public ObservableCollection<State> States { get; set; }
        public ObservableCollection<Subdivision> Subdivisions { get; set; }

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

        private int inAddEquipment;
        public int INAddEquipment // tb1 окон AddWindow и ReplaceWindow
        {
            get => inAddEquipment;
            set
            {
                inAddEquipment = Math.Abs(value);
                OnPropertyChanged("INAddEquipment");
            }
        }
        private string nameAddEquipment;
        public string NameAddEquipment // tb2 окон AddWindow и ReplaceWindow
        {
            get => nameAddEquipment;
            set
            {
                
                nameAddEquipment = value;
                OnPropertyChanged("NameAddEquipment");
            }
        }

        private Subdivision subdivisionAddEquipment;
        public Subdivision SubdivisionAddEquipment // tb3 окон AddWindow и ReplaceWindow
        {
            get => subdivisionAddEquipment;
            set
            {
                subdivisionAddEquipment = value;
                OnPropertyChanged("SubdivisionAddEquipment");
            }
        }

        private State stateAddEquipment; // tb4 окон AddWindow и ReplaceWindow
        public State StateAddEquipment
        {
            get => stateAddEquipment;
            set
            {
                stateAddEquipment = value;
                OnPropertyChanged("StateAddEquipment");
            }
        }

        private bool isNofificationsOn = false; // CheckBox "Уведомлять о изменениях"
        public bool IsNotificationsOn
        {
            get => isNofificationsOn;
            set
            {
                isNofificationsOn = value;
                OnPropertyChanged("IsNotificationsOn");
            }
        }

        private bool changesButtonsEnability = false;
        public bool ChangesButtonsEnability
        {
            get => changesButtonsEnability;
            set
            {
                changesButtonsEnability = value;
                OnPropertyChanged("ChangesButtonsEnability");
            }
        }

        private RelayCommand openCommand;
        public RelayCommand OpenCommand // Команда для автозаполнения данных в окнах ReplaceWindow и AddWindow
        {
            get
            {
                return openCommand ??
                  (openCommand = new RelayCommand(obj =>
                  {
                      string str = obj as string;
                      if (str == "Add")
                          INAddEquipment = Equipments.Count + 1;
                      else if (str == "Replace")
                          INAddEquipment = SelectedEquipment.IN;
                      NameAddEquipment = SelectedEquipment.Name;
                      SubdivisionAddEquipment = SelectedEquipment.Subdivision;
                      StateAddEquipment = SelectedEquipment.State;
                  }));
            }
        }

        private RelayCommand addCommand;
        public RelayCommand AddCommand // Команда добавления новой записи из окна AddWindow
             => addCommand ??
                    (addCommand = new RelayCommand(obj =>
                    {
                        Equipment equipment = new Equipment()
                        {
                            IN = INAddEquipment,
                            Name = NameAddEquipment,
                            Subdivision = subdivisionAddEquipment,
                            State = stateAddEquipment
                        };


                        bool IsINUnique = true;
                        foreach (Equipment equipmentForeach in Equipments)
                            if (equipmentForeach.IN == equipment.IN)
                                IsINUnique = false;

                        if (IsFieldsNotNull(equipment) & IsINUnique)
                        {
                            Equipments.Insert(Equipments.Count, equipment);
                            SelectedEquipment = equipment;
                            QueriesToBeExecuted.Add(
                                $"INSERT INTO equipment VALUES " +
                                $"({equipment.IN}," + // IN
                                $"'{equipment.Name}', " + // Name
                                $"{equipment.Subdivision.ID}, " + // subdivision_id
                                $"{equipment.State.ID})");  // state_id
                        }
                        else
                            MessageBox.Show("Неправильные данные");
                    }));

        private RelayCommand replaceCommand;
        public RelayCommand ReplaceCommand 
        {
            get
            {
                return replaceCommand ??
                  (replaceCommand = new RelayCommand(obj =>
                  {
                      Equipment equipment = new Equipment()
                      {
                          IN = INAddEquipment,
                          Name = NameAddEquipment,
                          Subdivision = subdivisionAddEquipment,
                          State = stateAddEquipment
                      };

                      bool IsINOkay = true;
                      foreach (Equipment equipmentForeach in Equipments)
                          if (equipmentForeach.IN == equipment.IN)
                              if(SelectedEquipment.IN != equipment.IN)
                                  IsINOkay = false;

                      if (IsFieldsNotNull(equipment) & IsINOkay)
                      {
                          Equipments[Equipments.IndexOf(SelectedEquipment)] = equipment;
                          //SelectedEquipment.IN = equipment.IN;
                          //SelectedEquipment.Name = equipment.Name;
                          //SelectedEquipment.State = equipment.State;
                          //SelectedEquipment.Subdivision = equipment.Subdivision;
                          QueriesToBeExecuted.Add(
                              $"UPDATE `equipment` " +
                              $"SET `name` = '{equipment.Name}', " +
                              $"`subdivision_id` = {equipment.Subdivision.ID}, " +
                              $"`state_id` = {equipment.State.ID} " +
                              $"Where `inventory number` = {equipment.IN}");
                      }
                      else
                          MessageBox.Show("Неправильные данные");
                  }));
            }
        }

        private RelayCommand deleteCommand;
        public RelayCommand DeleteCommand // Удаление записи из Equipment
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand(obj =>
                  {
                      var result = MessageBox.Show($"Вы уверены, что хотите удалить ({SelectedEquipment.IN}, '{SelectedEquipment.Name}', {SelectedEquipment.Subdivision.Name}, {SelectedEquipment.State.StateName}) ?",
                          "Проверка",MessageBoxButton.YesNo);
                      if(result == MessageBoxResult.Yes)
                      {
                          QueriesToBeExecuted.Add($"DELETE FROM `equipment` WHERE `inventory number` = {SelectedEquipment.IN}");
                          Equipments.Remove(SelectedEquipment);
                          selectedEquipment = Equipments[0];
                      }
                  }));
            }
        }

        private RelayCommand saveChangesCommand;
        public RelayCommand SaveChangesCommand // Удаление записи из Equipment
        {
            get
            {
                return saveChangesCommand ??
                  (saveChangesCommand = new RelayCommand(obj =>
                  {
                      string queries = Environment.NewLine;
                      foreach(string query in QueriesToBeExecuted)
                          queries += query + Environment.NewLine + Environment.NewLine;
                      if (MessageBox.Show($"Будут выполнены следующие запросы: {queries}","Выполнить?",MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                      {
                          foreach (string query in QueriesToBeExecuted)
                          {
                              try
                              {
                                  ExecuteDataQuery(query, "Тут не выводится таблицы, так что все равно");
                              }
                              catch(Exception e)
                              {
                                  MessageBox.Show($"Провалено выполенение: {Environment.NewLine}{query}{Environment.NewLine}Ошибка: {e.Message}");
                              }
                          }
                          QueriesToBeExecuted.Clear();
                      }
                  }));
            }
        }

        private RelayCommand cancelChangesCommand;
        public RelayCommand CancelChangesCommand
        {
            get
            {
                return cancelChangesCommand ??
                  (cancelChangesCommand = new RelayCommand(obj =>
                  {
                      if(MessageBox.Show("Отменить изменения?","Вопрос", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                      {
                          bool Is = IsNotificationsOn;
                          IsNotificationsOn = false;
                          QueriesToBeExecuted.Clear();
                          InitializeCollections();
                          IsNotificationsOn = Is;
                      }
                  }));
            }
        }

        bool IsFieldsNotNull(Equipment equipment)
        {
            bool[] IsFieldNotNull = new bool[4];

            IsFieldNotNull[0] = equipment.IN != default;
            IsFieldNotNull[1] = equipment.Name != default;
            IsFieldNotNull[2] = equipment.Subdivision.Name != default;
            IsFieldNotNull[3] = equipment.State.StateName != default;
            bool IsFieldsNotNull = true;
            foreach (bool entity in IsFieldNotNull)
                if (!entity)
                    IsFieldsNotNull = false;

            return IsFieldsNotNull;
        }

        public ViewModel() // Инициализация
        {
            States = new ObservableCollection<State>();
            Subdivisions = new ObservableCollection<Subdivision>();
            Equipments = new ObservableCollection<Equipment>();
            InitializeCollections();
            Equipments.CollectionChanged += Equipments_CollectionChanged;
            QueriesToBeExecuted.CollectionChanged += QueriesToBeExecuted_CollectionChanged;
        }

        private void QueriesToBeExecuted_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ChangesButtonsEnability = QueriesToBeExecuted.Count != 0;
        }

        private void InitializeCollections()
        {
            DataSet dataSet = new DataSet();

            dataSet.Tables.Add(ExecuteDataQuery("Select `equipment`.`inventory number` AS `IN`, `equipment`.`name` AS `Name`, `subdivisions`.`name` AS `Subdivision`, `states`.`state` AS `State` " +
                "from(`equipment` JOIN `subdivisions` ON `equipment`.`subdivision_id` = `subdivisions`.`id`) JOIN `states` ON `equipment`.`state_id` = `states`.`id` " +
                "order by `IN`",
                "Equipment"));

            dataSet.Tables.Add(ExecuteDataQuery("select `states`.`id` AS ID, `states`.`state` AS State, `states`.`note` AS Note from `states`",
                "State"));

            dataSet.Tables.Add(ExecuteDataQuery("select db.id AS ID, db.name AS Name, db.responsibility AS Responsibility from subdivisions db",
                "Subdivision"));

            States.Clear();
            foreach (DataRow dataRow in dataSet.Tables["State"].Rows)
            {
                States.Add(new State
                {
                    ID = (int)dataRow["ID"],
                    StateName = (string)dataRow["State"],
                    Note = (string)dataRow["Note"]
                });
            }
            
            Subdivisions.Clear();
            foreach (DataRow dataRow in dataSet.Tables["Subdivision"].Rows)
            {
                Subdivisions.Add(new Subdivision
                {
                    ID = (int)dataRow["ID"],
                    Name = (string)dataRow["Name"],
                    Responsibility = (string)dataRow["Responsibility"]
                });
            }
            
            Equipments.Clear();
            foreach (DataRow dataRow in dataSet.Tables["Equipment"].Rows)
            {
                foreach (Subdivision subdivision in Subdivisions)
                {
                    foreach (State state in States)
                    {
                        if ((string)dataRow["Subdivision"] == subdivision.Name)
                            if ((string)dataRow["State"] == state.StateName)
                            {
                                Equipments.Add(new Equipment
                                {
                                    IN = (int)dataRow["IN"],
                                    Name = (string)dataRow["Name"],
                                    Subdivision = subdivision,
                                    State = state
                                });
                            }
                    }
                }
            }
            selectedEquipment = Equipments[0];
        }

        private void Equipments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsNotificationsOn)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        Equipment equipment = e.NewItems[0] as Equipment;
                        MessageBox.Show($"Добавлено снаряжение: ({equipment.IN}, {equipment.Name}, {equipment.Subdivision.Name}, {equipment.State.StateName})");
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        Equipment replacedEquipment = e.OldItems[0] as Equipment;
                        Equipment replacingEquipment = e.NewItems[0] as Equipment;
                        MessageBox.Show($"({replacedEquipment.IN}, {replacedEquipment.Name}, {replacedEquipment.Subdivision.Name}, {replacingEquipment.State.ID}) " +
                            $"заменен на " +
                            $"({replacingEquipment.IN}, {replacingEquipment.Name}, {replacingEquipment.Subdivision.Name}, {replacingEquipment.State.ID})");
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        Equipment removedEquipment = e.OldItems[0] as Equipment;
                        MessageBox.Show($"Удалено снаряжение: {removedEquipment.IN} {removedEquipment.Name} {removedEquipment.Subdivision.Name} {removedEquipment.State.StateName}");
                        break;
                }
            }
        }

        private DataTable ExecuteDataQuery(string CommandText, string TableName)
        {
            DataTable dataTable = new DataTable(TableName);
            MySqlConnection connection = new MySqlConnection(ConnectionInfo);
            MySqlCommand command = new MySqlCommand(CommandText, connection);

            connection.Open();
            dataTable.Load(command.ExecuteReader());
            connection.Close();

            return dataTable;
        }
    }
}

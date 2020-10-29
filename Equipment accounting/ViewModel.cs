using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using MySql.Data.MySqlClient;
using System.Linq;
using Equipment_accounting.Model;
using System.Windows.Data;

namespace Equipment_accounting
{
    class ViewModel : BaseVM
    {
        public static string ConnectionInfo = "server = localhost;port = 3306;username=root;password=tamara23;database=equipment accounting";

        private List<string> QueriesToBeExecuted = new List<string>();

        public ObservableCollection<SortFilter> SortFilters { get; set; }
        public ObservableCollection<Equipment> Equipments { get; set; } // Коллекции записей
        public ObservableCollection<State> States { get; set; }
        public ObservableCollection<Subdivision> Subdivisions { get; set; }

        public ICollectionView EquipmentsView { get; set; } // Фильрация

        private Equipment selectedEquipment;
        public Equipment SelectedEquipment
        {
            get => selectedEquipment;
            set
            {
                selectedEquipment = value;
                OnPropertyChanged("SelectedEquipment");
            }
        }

        private State selectedState;
        public State SelectedState
        {
            get => selectedState;
            set
            {
                selectedState = value;
                OnPropertyChanged("SelectedState");
            }
        }

        private Subdivision selectedSubdivision;
        public Subdivision SelectedSubdivision
        {
            get => selectedSubdivision;
            set
            {
                selectedSubdivision = value;
                OnPropertyChanged("SelectedSubdivision");
            }
        }

        private SortFilter selectedFilter;
        public SortFilter SelectedFilter
        {
            get => selectedFilter;
            set
            {
                selectedFilter = value;
                OnPropertyChanged(nameof(SelectedFilter));
            }
        }

        private int inAddEquipment; // Поля окна AddWindow
        private string nameAddEquipment;
        private Subdivision subdivisionAddEquipment;
        private State stateAddEquipment;

        private int idAddStateSubdivision; // Поля окон AddStateWindow и AddSubdivisionWindow
        private string nameAddStateSubdivision;
        private string noteAddStateSubdivision;

        private bool isNofificationsOn = false;

        public int IDAddStateSubdivision
        {
            get => idAddStateSubdivision;
            set
            {
                idAddStateSubdivision = Math.Abs(value);
                OnPropertyChanged(nameof(IDAddStateSubdivision));
            }
        }

        public string NameAddStateSubdivision
        {
            get => nameAddStateSubdivision;
            set
            {
                nameAddStateSubdivision = value;
                OnPropertyChanged(nameof(NameAddStateSubdivision));
            }
        }

        public string NoteAddStateSubdivision
        {
            get => noteAddStateSubdivision;
            set
            {
                noteAddStateSubdivision = value;
                OnPropertyChanged(nameof(NoteAddStateSubdivision));
            }
        }

        public int INAddEquipment // tb1 окон AddWindow и ReplaceWindow
        {
            get => inAddEquipment;
            set
            {
                inAddEquipment = Math.Abs(value);
                OnPropertyChanged("INAddEquipment");
            }
        }
        
        public string NameAddEquipment // tb2 окон AddWindow и ReplaceWindow
        {
            get => nameAddEquipment;
            set
            {
                
                nameAddEquipment = value;
                OnPropertyChanged("NameAddEquipment");
            }
        }

        
        public Subdivision SubdivisionAddEquipment // tb3 окон AddWindow и ReplaceWindow
        {
            get => subdivisionAddEquipment;
            set
            {
                subdivisionAddEquipment = value;
                OnPropertyChanged("SubdivisionAddEquipment");
            }
        }

        public State StateAddEquipment   // tb4 окон AddWindow и ReplaceWindow
        {
            get => stateAddEquipment;
            set
            {
                stateAddEquipment = value;
                OnPropertyChanged("StateAddEquipment");
            }
        }

        public bool IsNotificationsOn // CheckBox "Уведомлять о изменениях"
        {
            get => isNofificationsOn;
            set
            {
                isNofificationsOn = value;
                OnPropertyChanged("IsNotificationsOn");
            }
        }

        private RelayCommand addSortCommand;
        public RelayCommand AddSortCommand
        {
            get
            {
                return addSortCommand ??
                  (addSortCommand = new RelayCommand(obj =>
                  {
                      SortFilters.Add(new SortFilter());
                  }));
            }
        }

        private RelayCommand deleteSortCommand;
        public RelayCommand DeleteSortCommand // Удаление записи из Equipment
        {
            get
            {
                return deleteSortCommand ??
                  (deleteSortCommand = new RelayCommand(obj =>
                  {
                      SortFilters.Remove(SelectedFilter);
                  }, obj => SelectedFilter != null));
            }
        }

        private RelayCommand sortCommand;
        public RelayCommand SortCommand
        {

            get
            {
                return sortCommand ??
                  (sortCommand = new RelayCommand(obj =>
                  {
                      //List<Equipment> ds = Equipments.ToList();
                      //foreach (SortFilter sort in SortFilters)
                      //{
                      //    switch (sort.Field)
                      //    {
                      //        case "Name":
                      //            var nameQuery = from equipment in Equipments
                      //                        where sort.Value == equipment.Name
                      //                        select equipment;
                      //            ds = ds.Intersect(nameQuery).ToList();
                      //            break;
                      //        case "Subdivision":
                      //            var subQuery = from equipment in Equipments
                      //                            where sort.Value == equipment.Subdivision.Name
                      //                            select equipment;
                      //            ds = ds.Intersect(subQuery).ToList();
                      //            break;
                      //        case "State":
                      //            var stQuery = from equipment in Equipments
                      //                           where sort.Value == equipment.State.StateName
                      //                           select equipment;
                      //            ds = ds.Intersect(stQuery).ToList();
                      //            break;
                      //        case "IN":
                      //            switch (sort.Action)
                      //            {
                      //                case "=":
                      //                    var inEqQuery = from equipment in Equipments
                      //                                    where equipment.IN.ToString() == sort.Value
                      //                                    select equipment; 
                      //                    ds = ds.Intersect(inEqQuery).ToList();
                      //                    break;
                      //                case ">":
                      //                    var inMoreQuery = from equipment in Equipments
                      //                                    where equipment.IN > int.Parse(sort.Value)
                      //                                    select equipment;
                      //                    ds = ds.Intersect(inMoreQuery).ToList();
                      //                    break;
                      //                case ">=":
                      //                    var inMoreEqQuery = from equipment in Equipments
                      //                                      where equipment.IN >= int.Parse(sort.Value)
                      //                                      select equipment;
                      //                    ds = ds.Intersect(inMoreEqQuery).ToList();
                      //                    break;
                      //                case "<":
                      //                    var inLessQuery = from equipment in Equipments
                      //                                        where equipment.IN < int.Parse(sort.Value)
                      //                                        select equipment;
                      //                    ds = ds.Intersect(inLessQuery).ToList();
                      //                    break;
                      //                case "<=":
                      //                    var inLessEqQuery = from equipment in Equipments
                      //                                      where equipment.IN <= int.Parse(sort.Value)
                      //                                      select equipment;
                      //                    ds = ds.Intersect(inLessEqQuery).ToList();
                      //                    break;
                      //            }
                      //            break;

                      //    }
                      //}
                      //string s = "След. записи: " + Environment.NewLine;
                      //for (int i = 0; i < ds.Count; i++)
                      //{
                      //    s += ds[i].IN + Environment.NewLine;
                      //}
                      //MessageBox.Show(s);

                      foreach (SortFilter sort in SortFilters)
                      {
                          EquipmentsView.Filter = (objFilter) =>
                          {
                              if (objFilter is Equipment equipmentMain)
                              {
                                  switch (sort.Field)
                                  {
                                      case "Name":
                                          return equipmentMain.Name == sort.Value;
                                      case "Subdivision":
                                          return equipmentMain.Subdivision.Name == sort.Value;
                                      case "State":
                                          return equipmentMain.State.StateName == sort.Value;
                                      case "IN":
                                          switch (sort.Action)
                                          {
                                              case "=":
                                                  return equipmentMain.IN.ToString() == sort.Value;
                                              case ">":
                                                  return equipmentMain.IN > int.Parse(sort.Value);
                                              case ">=":
                                                  return equipmentMain.IN >= int.Parse(sort.Value);
                                              case "<":
                                                  return equipmentMain.IN < int.Parse(sort.Value);
                                              case "<=":
                                                  return equipmentMain.IN <= int.Parse(sort.Value);
                                          }
                                          break;
                                  }
                              };

                              return false;
                          };
                      }

                  }, obj => SortFilters.Count > 0));
            }
        }

        private RelayCommand sortRefreshCommand;
        public RelayCommand SortRefreshCommand
        {
            get
            {
                return sortRefreshCommand ??
                  (sortRefreshCommand = new RelayCommand(obj =>
                  {
                      EquipmentsView.Filter = (objFilter) => { return true; };
                  }));
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
                      if(str == "AddEq" || str == "ReplaceEq")
                      {
                          if(SelectedEquipment != null)
                          {
                              if (str == "AddEq")
                                  INAddEquipment = Equipments.Count + 1;
                              else if (str == "ReplaceEq")
                                  INAddEquipment = SelectedEquipment.IN;
                              NameAddEquipment = SelectedEquipment.Name;
                              SubdivisionAddEquipment = SelectedEquipment.Subdivision;
                              StateAddEquipment = SelectedEquipment.State;
                          }
                      }
                      else if ( str == "AddSt")
                      {
                          IDAddStateSubdivision = States.Count + 1;
                          NameAddStateSubdivision = SelectedState.StateName;
                          NoteAddStateSubdivision = SelectedState.Note;
                      }
                      else if (str == "AddSub")
                      {
                          IDAddStateSubdivision = Subdivisions.Count + 1;
                          NameAddStateSubdivision = SelectedSubdivision.Name;
                          NoteAddStateSubdivision = SelectedSubdivision.Responsibility;
                      }
                  }));
            }
        }

        private RelayCommand addEquipmentCommand;
        public RelayCommand AddEquipmentCommand // Команда добавления новой записи из окна AddWindow
             => addEquipmentCommand ??
                    (addEquipmentCommand = new RelayCommand(obj =>
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

        private RelayCommand addStateCommand;
        public RelayCommand AddStateCommand
        {
            get
            {
                return addStateCommand ??
                  (addStateCommand = new RelayCommand(obj =>
                  {
                      State state = new State
                      {
                          ID = IDAddStateSubdivision,
                          StateName = NameAddStateSubdivision,
                          Note = NoteAddStateSubdivision
                      };

                      bool IsINUnique = true;
                      foreach (State stateForeach in States)
                          if (stateForeach.ID == state.ID)
                              IsINUnique = false;

                      if (IsINUnique && state.StateName != default)
                      {
                          States.Insert(States.Count, state);
                          QueriesToBeExecuted.Add(
                              "INSERT INTO states VALUES " +
                              $"({state.ID}, " +
                              $"'{state.StateName}', " +
                              $"'{state.Note}')");
                      }
                      else
                          MessageBox.Show("Неправильные данные");
                  }));
            }
        }

        private RelayCommand addSubdivisionCommand;
        public RelayCommand AddSubdivisionCommand
        {
            get
            {
                return addSubdivisionCommand ??
                  (addSubdivisionCommand = new RelayCommand(obj =>
                  {
                      Subdivision subdivision = new Subdivision
                      {
                          ID = IDAddStateSubdivision,
                          Name = NameAddStateSubdivision,
                          Responsibility = NoteAddStateSubdivision
                      };

                      bool IsINUnique = true;
                      foreach (Subdivision subdivisionsForeach in Subdivisions)
                          if (subdivisionsForeach.ID == subdivision.ID)
                              IsINUnique = false;

                      if (IsINUnique && subdivision.Name != default)
                      {
                          Subdivisions.Insert(Subdivisions.Count, subdivision);
                          QueriesToBeExecuted.Add(
                              "INSERT INTO subdivisions VALUES " +
                              $"({subdivision.ID}, " +
                              $"'{subdivision.Name}', " +
                              $"'{subdivision.Responsibility}')");
                      }
                      else
                          MessageBox.Show("Неправильные данные");
                  }));
            }
        }

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
                          QueriesToBeExecuted.Add(
                              $"UPDATE `equipment` " +
                              $"SET `name` = '{equipment.Name}', " +
                              $"`subdivision_id` = {equipment.Subdivision.ID}, " +
                              $"`state_id` = {equipment.State.ID} " +
                              $"Where `inventory number` = {equipment.IN}");
                      }
                      else
                          MessageBox.Show("Неправильные данные");
                  }, obj => Equipments.Count > 0));
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


        private RelayCommand deleteEquipmentCommand;
        public RelayCommand DeleteEquipmentCommand // Удаление записи из Equipment
        {
            get
            {
                return deleteEquipmentCommand ??
                  (deleteEquipmentCommand = new RelayCommand(obj =>
                  {
                      var result = MessageBox.Show($"Вы уверены, что хотите удалить ({SelectedEquipment.IN}, '{SelectedEquipment.Name}', {SelectedEquipment.Subdivision.Name}, {SelectedEquipment.State.StateName}) ?",
                          "Проверка",MessageBoxButton.YesNo);
                      if(result == MessageBoxResult.Yes)
                      {
                          QueriesToBeExecuted.Add($"DELETE FROM `equipment` WHERE `inventory number` = {SelectedEquipment.IN}");
                          Equipments.Remove(SelectedEquipment);
                          if (Equipments.Count > 0)
                              SelectedEquipment = Equipments[0];
                          else SelectedEquipment = null;
                      }
                  }, obj => SelectedEquipment != null));
            }
        }

        private RelayCommand deleteStateCommand;
        public RelayCommand DeleteStateCommand
        {
            get
            {
                return deleteStateCommand ??
                  (deleteStateCommand = new RelayCommand(obj =>
                  {
                      bool IsStateNotUsing = true;
                      foreach(Equipment equipment in Equipments)
                          if (SelectedState.ID == equipment.State.ID)
                              IsStateNotUsing = false;

                      if (IsStateNotUsing)
                      {
                          var result = MessageBox.Show($"Вы уверены, что хотите удалить ({SelectedState.ID}, '{SelectedState.StateName}', '{SelectedState.Note}') ?",
                          "Проверка", MessageBoxButton.YesNo);
                          if (result == MessageBoxResult.Yes)
                          {
                              QueriesToBeExecuted.Add($"DELETE FROM `states` WHERE `id` = {SelectedState.ID}");
                              States.Remove(SelectedState);
                              SelectedState = States[0];
                          }
                      }
                      else
                          MessageBox.Show("Это состояние используется в данный момент и не может быть удалено");
                  }, obj => SelectedState != null && States.Count > 0));
            }
        }

        private RelayCommand deleteSubdivisionCommand;
        public RelayCommand DeleteSubdivisionCommand
        {
            get
            {
                return deleteSubdivisionCommand ??
                  (deleteSubdivisionCommand = new RelayCommand(obj =>
                  {
                      bool IsSubdivisionNotUsing = true;
                      foreach (Equipment equipment in Equipments)
                          if (SelectedSubdivision.ID == equipment.Subdivision.ID)
                              IsSubdivisionNotUsing = false;

                      if (IsSubdivisionNotUsing)
                      {
                          var result = MessageBox.Show($"Вы уверены, что хотите удалить ({SelectedSubdivision.ID}, '{SelectedSubdivision.Name}', '{SelectedSubdivision.Responsibility}') ?",
                          "Проверка", MessageBoxButton.YesNo);
                          if (result == MessageBoxResult.Yes)
                          {
                              QueriesToBeExecuted.Add($"DELETE FROM `subdivisions` WHERE `id` = {SelectedSubdivision.ID}");
                              Subdivisions.Remove(SelectedSubdivision);
                              SelectedSubdivision = Subdivisions[0];
                          }
                      }
                      else
                          MessageBox.Show("Это подразделение используется в данный момент и не может быть удалено");
                  }, obj => SelectedSubdivision != null && Subdivisions.Count > 0));
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
                      bool IsExecutingStable = true;
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
                                  IsExecutingStable = false;
                              }
                          }
                          QueriesToBeExecuted.Clear();
                          if (!IsExecutingStable)
                          {
                              InitializeCollections();
                          }
                      }
                  }, obj => QueriesToBeExecuted.Count > 0));
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
                  }, obj => QueriesToBeExecuted.Count > 0));
            }
        }

        public ViewModel() // Констуктор (как не странно)
        {
            States = new ObservableCollection<State>();
            Subdivisions = new ObservableCollection<Subdivision>();
            Equipments = new ObservableCollection<Equipment>();
            SortFilters = new ObservableCollection<SortFilter>();

            InitializeCollections();
            EquipmentsView = CollectionViewSource.GetDefaultView(Equipments);

            Equipments.CollectionChanged += Equipments_CollectionChanged;
            States.CollectionChanged += States_CollectionChanged;
            Subdivisions.CollectionChanged += Subdivisions_CollectionChanged;
        }

        private void InitializeCollections() // Заполнение коллекций данных
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
            selectedState = States[0];
            selectedSubdivision = Subdivisions[0];
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

        private void Subdivisions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsNotificationsOn)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        Subdivision subdivision = e.NewItems[0] as Subdivision;
                        MessageBox.Show($"Добавлено подразделение : ({subdivision.ID}, {subdivision.Name}, {subdivision.Responsibility})");
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        Subdivision removedSubdivision = e.OldItems[0] as Subdivision;
                        MessageBox.Show($"Удалено снаряжение: ({removedSubdivision.ID}, {removedSubdivision.Name}, {removedSubdivision.Responsibility})");
                        break;
                }
            }
        }

        private void States_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsNotificationsOn)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        State state = e.NewItems[0] as State;
                        MessageBox.Show($"Добавлено состояние : ({state.ID}, {state.StateName}, {state.Note})");
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        State removedState = e.OldItems[0] as State;
                        MessageBox.Show($"Удалено снаряжение: ({removedState.ID}, {removedState.StateName}, {removedState.Note})");
                        break;
                }
            }
        }

        /// <summary>
        /// Выполняет запрос к базе данных
        /// </summary>
        /// <returns>
        /// Если запрос не предпологает возращения данных, выходное значение можно не принимать
        /// </returns>
        private DataTable ExecuteDataQuery(string CommandText, string TableName = "None")
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

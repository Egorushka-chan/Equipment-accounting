using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Equipment_accounting.Model
{
    class SortFilter : BaseVM
    {
        private string[] fields = new Equipment().GetPropertyNames();
        public string[] Fields
        {
            get => fields;
        }

        private string field;
        public string Field
        {
            get => field;
            set
            {
                field = value;
                OnPropertyChanged(nameof(field));
            }
        }

        private string[] actions = new string[] { "=", ">", ">=", "<", "<="};
        public string[] Actions
        {
            get => actions;
        }

        private string action;
        public string Action
        {
            get => action;
            set
            {
                switch (field)
                {
                    case "Name":
                        action = "=";
                        break;
                    case "Subdivision":
                        action = "=";
                        break;
                    case "State":
                        action = "=";
                        break;
                    default:
                        action = value;
                        break;
                }
                OnPropertyChanged(nameof(Action));
            }
        }

        private string value;
        public string Value
        {
            get => value;
            set
            {
                this.value = value;
                OnPropertyChanged(nameof(Value));
            }
        }
    }
}

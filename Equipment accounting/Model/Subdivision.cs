using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Equipment_accounting.Model
{
    class Subdivision : BaseVM
    {
        private int id;
        private string name;
        private string resposibility;

        public int ID
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged("ID");
            }
        }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Responsibility
        {
            get => resposibility;
            set
            {
                resposibility = value;
                OnPropertyChanged("Responsibility");
            }
        }
    }
}

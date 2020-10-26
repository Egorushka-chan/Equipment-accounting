using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Equipment_accounting
{
    class Subdivision : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnProperyChanged([CallerMemberName] string properity = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(properity));
        }

        private int id;
        private string name;
        private string resposibility;

        public int ID
        {
            get => id;
            set
            {
                id = value;
                OnProperyChanged("ID");
            }
        }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnProperyChanged("Name");
            }
        }

        public string Responsibility
        {
            get => resposibility;
            set
            {
                resposibility = value;
                OnProperyChanged("Responsibility");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Equipment_accounting
{
    class State : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnProperyChanged([CallerMemberName] string properity = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(properity));
        }

        private int id;
        private string state;
        private string note;

        public int ID
        {
            get => id;
            set
            {
                id = value;
                OnProperyChanged("ID");
            }
        }

        public string StateName
        {
            get => state;
            set
            {
                state = value;
                OnProperyChanged("StateName");
            }
        }

        public string Note
        {
            get => note;
            set
            {
                note = value;
                OnProperyChanged("Note");
            }
        }
    }
}

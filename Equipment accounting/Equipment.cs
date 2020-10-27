using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Equipment_accounting
{
    class Equipment : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnProperyChanged([CallerMemberName] string properity = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(properity));
        }

        private int iN;
        private string name;
        private Subdivision subdivision;
        private State state;

        public string Name 
        { 
            get => name;
            set
            {
                name = value;
                OnProperyChanged("Name");
            } 
        }
        
        public int IN
        {
            get => iN;
            set
            {
                iN = value;
                OnProperyChanged("IN");
            }
        }

        public Subdivision Subdivision
        {
            get => subdivision;
            set
            {
                subdivision = value;
                OnProperyChanged("Subdivision");
            }
        }

        public State State
        {
            get => state;
            set
            {
                state = value;
                OnProperyChanged("State");
            }
        }
    }
}

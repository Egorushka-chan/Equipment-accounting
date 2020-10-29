using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Equipment_accounting.Model
{
    class State : BaseVM
    {
        private int id;
        private string state;
        private string note;

        public int ID
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged("ID");
            }
        }

        public string StateName
        {
            get => state;
            set
            {
                state = value;
                OnPropertyChanged("StateName");
            }
        }

        public string Note
        {
            get => note;
            set
            {
                note = value;
                OnPropertyChanged("Note");
            }
        }
    }
}

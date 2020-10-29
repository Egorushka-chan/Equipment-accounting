using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Equipment_accounting.Model
{
    class Equipment : BaseVM
    {
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
                OnPropertyChanged("Name");
            } 
        }
        
        public int IN
        {
            get => iN;
            set
            {
                iN = value;
                OnPropertyChanged("IN");
            }
        }

        public Subdivision Subdivision
        {
            get => subdivision;
            set
            {
                subdivision = value;
                OnPropertyChanged("Subdivision");
            }
        }

        public State State
        {
            get => state;
            set
            {
                state = value;
                OnPropertyChanged("State");
            }
        }

        /// <summary>
        /// Возращает названия всех свойств Equipment
        /// </summary>
        /// <returns>
        /// Возращает в виде массива string 
        /// </returns>
        public string[] GetPropertyNames()
        {
            return new string[] { nameof(IN), nameof(Name), nameof(Subdivision), nameof(State) };
        }

    }
}

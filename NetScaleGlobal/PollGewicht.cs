using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace NetScaleGlobal
{
    public  class PollGewicht : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private decimal _PGewicht;

        public decimal PGewicht
        {
            get { return _PGewicht; }
            set { _PGewicht = value;
                PGewichtChanged();
            }
        }
        private void PGewichtChanged()
        {
            {
                if (this.PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs("PGewicht"));
            }
        }
    }
}

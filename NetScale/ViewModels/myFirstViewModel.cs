using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace HWB.NETSCALE.FRONTEND.WPF.ViewModels
{
    public class myFirstViewModel
    {
       

        private ICommand test;
        public ICommand Test
        {
            get
            {
                if (test == null)
                {
                    test = new Messenger();
                }

                return test;
            }
            set
            {
                test = value;
            }
        }

        public class Messenger : ICommand
        {
            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                MessageBox.Show("Hallo Welt");
            }
        }
    }
}

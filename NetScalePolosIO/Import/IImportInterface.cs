using System.ComponentModel;

namespace HWB.NETSCALE.POLOSIO
{
    internal interface IImportInterface :  INotifyPropertyChanged
    {
        void ImportStammdaten();
    }
}
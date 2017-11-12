using System.ComponentModel;
namespace BinPackingWPF.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private double _binVolume;
        public double BinVolume
        {
            get { return _binVolume; }
            set
            {
                if (_binVolume != value)
                {
                    _binVolume = value;
                    NotifyPropertyChanged("BinVolume");
                }
            }
        }

        private int _numPackages;
        public int NumPackages
        {
            get { return _numPackages; }
            set
            {
                if (_numPackages != value)
                {
                    _numPackages = value;
                    NotifyPropertyChanged("NumPackages");
                }
            }
        }
    }
}

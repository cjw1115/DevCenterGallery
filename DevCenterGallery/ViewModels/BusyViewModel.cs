using GalaSoft.MvvmLight;

namespace DevCenterGallary.ViewModels
{
    public class BusyViewModel: ViewModelBase
    {
        private bool _isProcessing = false;
        public bool IsProcessing
        {
            get { return _isProcessing; }
            set { Set(ref _isProcessing, value); }
        }
    }
}

using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

using System.ComponentModel;

namespace FlowstorageDesktop.Temporary {
    public class TemporaryDataSharing : INotifyPropertyChanged {

        private string _sharingDisabledStatus = "";
        private string _sharingAuthStatus = "";

        public string SharingDisabledStatus {
            get { return _sharingDisabledStatus; }
            set {
                _sharingDisabledStatus = value;
                OnPropertyChanged(nameof(SharingDisabledStatus));
            }
        }

        public string SharingAuthStatus {
            get { return _sharingAuthStatus; }
            set {
                _sharingAuthStatus = value;
                OnPropertyChanged(nameof(SharingAuthStatus));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}

using System.ComponentModel;

namespace FlowstorageDesktop.Temporary {
    public class TemporaryDataUser : INotifyPropertyChanged {

        static private string username = "";
        static private string email = "";
        static private string accountType = "";

        public string Username {
            get { return username; }
            set {
                username = value;
                OnPropertyChanged(nameof(username));
            }
        }

        public string Email {
            get { return email; }
            set {
                email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string AccountType {
            get { return accountType; }
            set {
                accountType = value;
                OnPropertyChanged(nameof(AccountType));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}

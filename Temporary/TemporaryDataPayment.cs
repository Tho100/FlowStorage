using System.ComponentModel;

namespace FlowSERVER1.Global {

    class TemporaryDataPayment : INotifyPropertyChanged {

        private string _countryCode = "";
        private string _countryCurrency = "";
        private double _currencyConversionRate = 0.0;

        public string CountryCode {
            get { return _countryCode; }
            set {
                _countryCode = value;
                OnPropertyChanged(nameof(CountryCode));
            }
        }

        public string CountryCurrency {
            get { return _countryCurrency; }
            set {
                _countryCurrency = value;
                OnPropertyChanged(nameof(CountryCurrency));
            }
        }

        public double CurrencyConversionRate {
            get { return _currencyConversionRate; }
            set {
                _currencyConversionRate = value;
                OnPropertyChanged(nameof(CurrencyConversionRate));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}

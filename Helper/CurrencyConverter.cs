using FlowstorageDesktop.Global;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlowstorageDesktop.Helper {
    public class CurrencyConverter {

        readonly static private TemporaryDataPayment tempData = new TemporaryDataPayment();

        public async Task ConvertToLocalCurrency() {

            var countryCodeToCurrency = new Dictionary<string, string>{
              { "US", "USD" },
              { "DE", "EUR" },
              { "GB", "GBP" },
              { "ID", "IDR" },
              { "MY", "MYR" },
              { "BN", "BND" },
              { "SG", "SGD" },
              { "TH", "THB" },
              { "PH", "PHP" },
              { "VN", "VND" },
              { "CN", "CNY" },
              { "HK", "HKD" },
              { "TW", "TWD" },
              { "KO", "KRW" },
              { "BR", "BRL" },
              { "ME", "MXN" },
              { "AU", "AUD" },
              { "NZ", "NZD" },
              { "IN", "INR" },
              { "LK", "LKR" },
              { "PA", "PKR" },
              { "SA", "SAR" },
              { "AR", "AED" },
              { "IS", "ILS" },
              { "EG", "EGP" },
              { "TU", "TND" },
              { "CH", "CHF" },
              { "ES", "EUR" },
              { "SW", "SEK" },
            };

            string countryCode = "US";
            string countryCurrency = "USD";
            double conversionRate = 2.0;

            if (string.IsNullOrEmpty(tempData.CountryCode) || tempData.CurrencyConversionRate == 0.0) {
                countryCode = await CountryCode();
                countryCurrency = countryCodeToCurrency[countryCode];

                tempData.CountryCode = countryCode;
                tempData.CountryCurrency = countryCurrency;

                using (HttpClient client = new HttpClient()) {
                    HttpResponseMessage response = await client.GetAsync("https://api.freecurrencyapi.com/v1/latest?apikey=fca_live_2N9mYDefob9ZEMqWT3cXAjl964IFfNkPMr01YS5v");

                    if (response.IsSuccessStatusCode) {
                        string content = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, double>>>(content);
                        conversionRate = data["data"][countryCurrency];
                        tempData.CurrencyConversionRate = conversionRate;

                        await Task.Run(() => UpdateUI(conversionRate));

                    }
                    else {
                        throw new Exception("Failed to load exchange rates");
                    }
                   
                }

            } else {
                countryCode = tempData.CountryCode;
                countryCurrency = tempData.CountryCurrency;
                conversionRate = tempData.CurrencyConversionRate;

                await Task.Run(() => UpdateUI(conversionRate));

            }
        
        }

        private async Task<string> CountryCode() {

            using (HttpClient client = new HttpClient()) {

                HttpResponseMessage response = await client.GetAsync("http://apiip.net/api/check?accessKey=61d755d2-ac10-4b0c-afb8-487a1f4f2cdd");

                if (response.IsSuccessStatusCode) {
                    string content = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<IpApiResponse>(content);
                    return data.CountryCode;

                }
                else {
                    throw new Exception("Failed to load user country");

                }
            }

        }

        private static void UpdateUI(double conversionRate) {
            SettingsForm.instance.lblMaxPricing.Text = $"{tempData.CountryCurrency}{Math.Floor(3.0 * conversionRate)}";
            SettingsForm.instance.lblExpressPricing.Text = $"{tempData.CountryCurrency}{Math.Floor(8.0 * conversionRate)}";
            SettingsForm.instance.lblSupremePricing.Text = $"{tempData.CountryCurrency}{Math.Floor(20.0 * conversionRate)}";
        }

        private class IpApiResponse {
            public string CountryCode { get; set; }

        }

    }
}
using Stripe;
using System;

namespace FlowSERVER1.Model {
    public class StripeModel {

        static public void AddNewCustomer(String email, String name) {

            const string key = "sk_test_51MO4YYF2lxRV33xsBfTJLQypyLBjhoxYdz18VoLrZZ6hin4eJrAV9O6NzduqR02vosmC4INFgBgxD5TkrkpM3sZs00hqhx3ZzN"; // Replace with valid KEY
            Stripe.StripeConfiguration.ApiKey = key;

            var service = new Stripe.CustomerService();
            var options = new CustomerCreateOptions {
                Email = email,
                Name = name,
            };

            service.Create(options);
        }

        static public void CancelCustomerSubscription(String email) {

            const string key = "sk_test_51MO4YYF2lxRV33xsBfTJLQypyLBjhoxYdz18VoLrZZ6hin4eJrAV9O6NzduqR02vosmC4INFgBgxD5TkrkpM3sZs00hqhx3ZzN"; // Replace with valid KEY
            Stripe.StripeConfiguration.ApiKey = key;

            var options = new CustomerListOptions {
                Email = email,
                Limit = 1 
            };

            var service = new CustomerService();
            var customers = service.List(options);

            var customer = customers.Data[0];
            var customerId = customer.Id;

            if (customers.Data.Count == 0) {
                return;
            }

            var subscriptionService = new SubscriptionService();
            var subscriptions = subscriptionService.List(new SubscriptionListOptions {
                Customer = customerId,
                Limit = 1
            });

            if (subscriptions.Data.Count == 0) {
                return;
            }

            var subscription = subscriptions.Data[0];
            subscriptionService.Cancel(subscription.Id, null);

        }

        static public void DeleteCustomer(String email) {

            const string key = "sk_test_51MO4YYF2lxRV33xsBfTJLQypyLBjhoxYdz18VoLrZZ6hin4eJrAV9O6NzduqR02vosmC4INFgBgxD5TkrkpM3sZs00hqhx3ZzN"; // Replace with valid KEY
            Stripe.StripeConfiguration.ApiKey = key;

            var service = new CustomerService();

            var options = new Stripe.CustomerListOptions {
                Email = email,
                Limit = 1
            };

            var customersOptions = service.List(options);

            var customerBuyer = customersOptions.Data[0];
            var customerId = customerBuyer.Id;

            service.Delete(customerId, null);

        }
    }
}

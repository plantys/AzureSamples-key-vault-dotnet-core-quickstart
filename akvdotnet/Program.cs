﻿// <directives>
using System;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
// </directives>
namespace akvdotnet
{
    class Program
    {
        static void Main(string[] args)
        {
            string secretName = "mySecret";

            Program P = new Program();
            var fetchedSecret = P.GetSecret(secretName);

            string secret = fetchedSecret.Result;
            Console.WriteLine("Your secret is " + secret);
        }

        public async Task<string> GetSecret(string secretName)
        {
            // <authentication>
              
            string clientId = Environment.GetEnvironmentVariable("akvClientId");
            string clientSecret = Environment.GetEnvironmentVariable("akvClientSecret");
            string tenantId = Environment.GetEnvironmentVariable("akvTenantId"); 
            string subscriptionId = Environment.GetEnvironmentVariable("akvSubscriptionId"); 

            AzureCredentials credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(clientId, clientSecret, tenantId, AzureEnvironment.AzureGlobalCloud).WithDefaultSubscription(subscriptionId);

            var keyClient = new KeyVaultClient(async (authority, resource, scope) =>
            {
                var adCredential = new ClientCredential(clientId, clientSecret);
                var authenticationContext = new AuthenticationContext(authority, null);
                return (await authenticationContext.AcquireTokenAsync(resource, adCredential)).AccessToken;
            });
            // </authentication>

            // <getsecret>
            string akvUri = "https://mykv-msb.vault.azure.net";

            var keyvaultSecret = await keyClient.GetSecretAsync($"{akvUri}/secrets/{secretName}").ConfigureAwait(false);
            // </getsecret>
            return keyvaultSecret.Value;
        }
    }
}
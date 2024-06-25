using Azure.Identity;
using Domain.Entities.AD;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace AD_Integration_Application.Services
{
    public interface IADService
    {
        //Task<string> GetADResponseAsync();
        Task<List<ADProfile>> GetUsers();
    }

    public class ADService(IConfiguration configuration) : IADService
    {

        private readonly IConfiguration _config = configuration;

        public async Task<List<ADProfile>> GetUsers()
        {
            var scopes = new[] { "" };

            // Values from app registration
            var clientId = _config[""];
            var tenantId = _config[""];

            // using Azure.Identity;
            var options = new ClientSecretCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
            };

            var clientSecretCredential = new ClientSecretCredential();
            
            var graphClient = new GraphServiceClient(clientSecretCredential, scopes);


            // API CALL BEGINS HERE

            // TODO: add error logging
            // TODO: add delta call to be able to relate properties in the database

            List<ADProfile> pdpEmployees = [];

            var users = await graphClient.Users.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Top = 999;
                requestConfiguration.QueryParameters.Filter = "startsWith(companyName,'')";
                requestConfiguration.QueryParameters.Select =
                [
                    "employeeId",
                    "userPrincipalName",
                    "surname",
                    "givenName",
                    "mail",
                    "jobTitle",
                    "department",
                    "accountEnabled",
                    "lastPasswordChangeDateTime",
                    "companyName"
                ];
                requestConfiguration.QueryParameters.Count = true;
                requestConfiguration.Headers.Add("ConsistencyLevel", "eventual");
            });
            var pageIterator = PageIterator<User, UserCollectionResponse>
                .CreatePageIterator(
                    graphClient,
                    users,
                    // Callback executed for each item in the collection
                    (usr) =>
                    {
                        var adProfile = new ADProfile
                        {
                            Payroll = usr.EmployeeId,
                            UserPrincipalName = usr.UserPrincipalName,
                            LastName = usr.Surname,
                            FirstName = usr.GivenName,
                            EmailAddress = usr.Mail,
                            Title = usr.JobTitle,
                            Department = usr.Department,
                            Enabled = usr.AccountEnabled,
                            LastPasswordChange = usr.LastPasswordChangeDateTime,
                            CompanyName = usr.CompanyName
                        };
                        pdpEmployees.Add(adProfile);
                        return true;
                    },
                    // Used to configure subsequent page requests
                    (req) =>
                    {
                        // Re-add the header to subsequent requests
                        req.Headers.Add("ConsistencyLevel", "eventual");
                        return req;
                    });

            await pageIterator.IterateAsync();

            return pdpEmployees;
        }
    }
}


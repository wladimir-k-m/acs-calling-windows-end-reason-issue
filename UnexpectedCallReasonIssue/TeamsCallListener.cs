using Azure.Communication.Calling.WindowsClient;
using Azure.Communication.Identity;
using Microsoft.Identity.Client;

namespace UnexpectedCallReasonIssue
{
    internal class TeamsCallListener
    {
        static private CallClient _callClient;
        static private CallTokenRefreshOptions _callTokenRefreshOptions = new CallTokenRefreshOptions(false);
        public static CallAgent CallAgent { get; set; }

        public static async Task<string> GetTokenForTeamsUser()
        {
            string appId = "<AppId>";
            string tenantId = "<TenantId>";
            string connectionString = "<ConnectionString>";

            string authority = $"https://login.microsoftonline.com/{tenantId}";
            string redirectUri = "http://localhost";

            var aadClient = PublicClientApplicationBuilder
                            .Create(appId)
                            .WithAuthority(authority)
                            .WithRedirectUri(redirectUri)
                            .Build();

            List<string> scopes = new List<string>() {
                "https://auth.msft.communication.azure.com/Teams.ManageCalls",
                "https://auth.msft.communication.azure.com/Teams.ManageChats"
            };

            var result = await aadClient
                    .AcquireTokenInteractive(scopes)
                    .ExecuteAsync();
            string teamsUserAadToken = result.AccessToken;
            string userObjectId = result.UniqueId;

            var client = new CommunicationIdentityClient(connectionString);

            var options = new GetTokenForTeamsUserOptions(teamsUserAadToken, appId, userObjectId);
            var accessToken = await client.GetTokenForTeamsUserAsync(options);
            return accessToken.Value.Token;
        }

        private static async Task InitCallAgent(string accessToken)
        {
            _callClient = new CallClient();

            var tokenCredential = new CallTokenCredential(accessToken, _callTokenRefreshOptions);

            var callAgentOptions = new CallAgentOptions()
            {
                DisplayName = $"{Environment.MachineName}/{Environment.UserName}",
            };

            CallAgent = await _callClient.CreateCallAgentAsync(tokenCredential, callAgentOptions);
        }

        public static async Task LoginAndListen()
        {
            var token = await GetTokenForTeamsUser();
            await InitCallAgent(token);
        }
    }
}

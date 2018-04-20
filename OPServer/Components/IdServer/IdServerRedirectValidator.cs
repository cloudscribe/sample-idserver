using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace OPServer.Components.IdServer
{
    public class IdServerRedirectValidator : StrictRedirectUriValidator, IRedirectUriValidator
    {

        public IdServerRedirectValidator(ILogger<IdServerRedirectValidator> logger)
        {
            _log = logger;
        }

        private ILogger _log;

        public override async Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
        {
            _log.LogDebug($"validating logout redirect url {requestedUri} for {client.ClientName}");

            //TODO: change this to check the client id
            if (requestedUri == "xamarinformsclients://callback")
            {
                return true;
            }


            //if (requestedUri == "https://notused")
            //{
            //    return true;
            //}


            return await base.IsPostLogoutRedirectUriValidAsync(requestedUri, client);
        }

        public override async Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
        {
            //TODO: change this to check the client id


            if (requestedUri == "xamarinformsclients://callback")
            {
                return true;
            }

            //if (requestedUri == "https://notused")
            //{
            //    return true;
            //}

            return await base.IsRedirectUriValidAsync(requestedUri, client);
        }

    }

}

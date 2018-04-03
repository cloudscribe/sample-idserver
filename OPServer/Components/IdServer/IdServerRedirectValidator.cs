using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Threading.Tasks;

namespace OPServer.Components.IdServer
{
    public class IdServerRedirectValidator : StrictRedirectUriValidator, IRedirectUriValidator
    {


        public override async Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
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

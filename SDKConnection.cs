using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;

public class SDKConnection
{
        public ClientConfiguration? clientConfiguration { get; set; }
    public ServiceClient CreateServiceClient()
    {
        if (clientConfiguration == null)
        {
            throw new InvalidOperationException("Client configuration is not set.");
        }
        
        string conectionString = $"AuthType=ClientSecret;" +
                                    $"ClientId={clientConfiguration.ClientId};" +
                                  $"ClientSecret={clientConfiguration.ClientSecret};" +
                                  $"Url={clientConfiguration.Resource};" +
                                $"LoginPrompt=Auto;RequireNewInstance=True";

        /*
        string conectionString = $"AuthType=OAuth;" +
                                        $"AppId={clientConfiguration.ClientId};" +
                                        $"UserName={clientConfiguration.User};" +
                                      $"Password={clientConfiguration.Password};" +
                                      $"Url={clientConfiguration.Resource};" +
                                    $"LoginPrompt=Auto;RequireNewInstance=True";
*/
        
        ServiceClient serviceClient = new ServiceClient(conectionString);
        return serviceClient;

    }
}
using Microsoft.PowerPlatform.Dataverse.Client;
ClientConfiguration clientConfiguration = ClientConfiguration.LoadFromXml(@"conf.xml");


SDKConnection sdkConnection = new SDKConnection(){
    clientConfiguration = clientConfiguration   
};


ServiceClient serviceClient = sdkConnection.CreateServiceClient();

// Inicializar handlers
AccountHandler accountHandler = new AccountHandler();





//CRMSDK.getAccountInfoExample(serviceClient, accountHandler); 
//CRMSDK.createAccountExample(accountHandler, serviceClient);
//CRMSDK.updateAccountExample(accountHandler, serviceClient);    
//CRMSDK.deleteAccountExample(accountHandler, serviceClient);
//CRMSDK.updateTelephoneByCountryCodeExample(accountHandler, serviceClient);
//CRMSDK.CreateAccountsBulk(accountHandler, serviceClient);
//CRMSDK.getAccountPagination(accountHandler, serviceClient);
//CRMSDK.getAccountPerCountryExample(accountHandler, serviceClient);

//CREAR O ACTUALIZAR UNA CUENTA BÚSCANDOLA POR CÓDIGO 
//CRMSDK.createOrUpdate(accountHandler, serviceClient);

//OBTENER REGISTROS MEDIANTE FETCHXML
//CRMSDK.getAccountByFetchXML(accountHandler, serviceClient);

//FILTRAR REGISTROS MEDIANTE FETCHXML
//CRMSDK.getAccountByERPCodeFetchXML(accountHandler, serviceClient);

//AGREGAR ENTIDADES RELACIONADAS EN FETCHXML
//CRMSDK.getAccountByERPCodeFetchXMLLink(accountHandler, serviceClient);

//AGREGAR CONDICIONES EN ENTIDADES RELACIONADAS EN FETCHXML
//CRMSDK.getAcountByCurrencyFetchLinkEntity(accountHandler, serviceClient);



Console.ReadLine();




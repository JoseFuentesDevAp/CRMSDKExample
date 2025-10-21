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

// ========================================
// PRODUCT HANDLER EXAMPLES
// ========================================

ProductHandler productHandler = new ProductHandler();

//OBTENER INFORMACIÓN DE UN PRODUCTO
//CRMSDK.getProductInfoExample(serviceClient, productHandler);

//CREAR UN PRODUCTO
//CRMSDK.createProductExample(productHandler, serviceClient);

//ACTUALIZAR UN PRODUCTO
//CRMSDK.updateProductExample(productHandler, serviceClient);

//ELIMINAR UN PRODUCTO
//CRMSDK.deleteProductExample(productHandler, serviceClient);

//BUSCAR PRODUCTOS POR NOMBRE
//CRMSDK.getProductsByNameExample(productHandler, serviceClient);

//ACTUALIZAR DESCRIPCIÓN DE PRODUCTO
//CRMSDK.updateProductDescriptionExample(productHandler, serviceClient);

//CREAR PRODUCTOS EN MASIVO
//CRMSDK.CreateProductsBulk(productHandler, serviceClient);

//CREAR O ACTUALIZAR PRODUCTO POR NÚMERO
//CRMSDK.createOrUpdateProduct(productHandler, serviceClient);

//OBTENER PRODUCTOS MEDIANTE FETCHXML
//CRMSDK.getProductByFetchXML(productHandler, serviceClient);

//FILTRAR PRODUCTO POR NÚMERO CON FETCHXML
//CRMSDK.getProductByNumberFetchXML(productHandler, serviceClient);

//OBTENER PRODUCTO CON ENTIDADES RELACIONADAS (FETCHXML LINK-ENTITY)
//CRMSDK.getProductByNumberFetchXMLLink(productHandler, serviceClient);

//FILTRAR PRODUCTOS POR UOM SCHEDULE (FETCHXML CON LINK-ENTITY)
//CRMSDK.getProductsByUomScheduleFetchLinkEntity(productHandler, serviceClient);


Console.ReadLine();




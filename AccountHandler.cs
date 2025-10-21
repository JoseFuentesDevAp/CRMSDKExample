using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;


public class AccountHandler
{



    public void getAccountInfo(string accountId, ServiceClient serviceClient)
    {

        Entity accountInfo = serviceClient.Retrieve("account", new Guid(accountId), new ColumnSet(true));

        Console.WriteLine($"Account Name: {accountInfo.Attributes["name"]}");
        Console.WriteLine($"Account ExchRate: {accountInfo.Attributes["exchangerate"]}");
        /*
        customertypecode
        msdyn_company
        transactioncurrencyid
        msdyn_language
        msdyn_segmentid
        */
    }

    public void CreateAccount(Account _account, ServiceClient serviceClient)
    {
        Entity account = new Entity("account");
        account["accountnumber"] = _account.AccountNum;
        account["name"] = _account.Name;
        account["exchangerate"] = _account.ExchangeRate;
        account["emailaddress1"] = _account.EmailAddress1;
        account["telephone1"] = _account.Telephone1;
        account["address1_country"] = _account.Address1_Country;
        account["address1_postalcode"] = _account.Address1_PostalCode;
        account["transactioncurrencyid"] = this.GetCurrencyReference(_account.CurrencyId, serviceClient);
        account["customertypecode"] = new OptionSetValue(_account.CustomerTypeCode); 
        account["shippingmethodcode"] = new OptionSetValue(_account.ShippingMethodCode); 
        account["industrycode"] = new OptionSetValue(_account.IndustryCode); 
        account["creditlimit"] = new Money(_account.Creditlimit);
        account["creditonhold"] = (bool)_account.CreditOnHold;
        account["lastonholdtime"] = _account.LastOnHoldTime;


        Guid accountId = serviceClient.Create(account);
        Console.WriteLine($"Account created with ID: {accountId.ToString()}");
    }


    public void UpdateAccount(Account _account, ServiceClient serviceClient)
    {
        Entity account = new Entity("account", new Guid(_account.AccountId));
        account["name"] = _account.Name;
        account["exchangerate"] = _account.ExchangeRate;
        account["emailaddress1"] = _account.EmailAddress1;
        account["telephone1"] = _account.Telephone1;

        serviceClient.Update(account);
        Console.WriteLine($"Account updated with ID: {_account.AccountId}");
    }

    public void DeleteAccount(string accountId, ServiceClient serviceClient)
    {
        serviceClient.Delete("account", new Guid(accountId));
        Console.WriteLine($"Account deleted with ID: {accountId}");
    }


    public void getAccountPerCountry(string country, ServiceClient serviceClient)
    {
        Console.WriteLine($"Retrieving accounts in country: {country}");


        QueryExpression query = new QueryExpression("account");
        query.ColumnSet = new ColumnSet("name", "exchangerate", "telephone1", "address1_country");
        query.Criteria.AddCondition("address1_country", ConditionOperator.Equal, country);



        EntityCollection accounts = serviceClient.RetrieveMultiple(query);

        foreach (var account in accounts.Entities)
        {
            Console.WriteLine($"Account Name: {account["name"]}, Exchange Rate: {account["exchangerate"]}, Country: {account["address1_country"]}");
        }
    }


    public void updateTelephoneByCountryCode(string countryCode, string phoneCode, ServiceClient serviceClient)
    {
        Console.WriteLine($"Updating telephone for accounts in country code: {countryCode}");

        QueryExpression query = new QueryExpression("account");
        query.ColumnSet = new ColumnSet("name", "telephone1", "address1_country");
        query.Criteria.AddCondition("address1_country", ConditionOperator.Equal, countryCode);

        EntityCollection accounts = serviceClient.RetrieveMultiple(query);

        foreach (var account in accounts.Entities)
        {

            if (account["telephone1"] is null)
            {
                continue;
            }
            string newTelephone = account["telephone1"].ToString();
            if (newTelephone.StartsWith(phoneCode))
            {
                Console.WriteLine($"Account {account["name"]} already has the phone code {phoneCode}. Skipping update.");
                continue;
            }
            account["telephone1"] = phoneCode + account["telephone1"].ToString();

            serviceClient.Update(account);
            Console.WriteLine($"Updated Account Name: {account["name"]}, New Telephone: {newTelephone}");
        }
    }

    public void createOrUpdateAccount(Account _account, ServiceClient serviceClient)
    {

        /*
       creditlimit
       creditonhold
       paymenttermscode
       transactioncurrencyid
       */
        bool existsAccount = false;

        if (string.IsNullOrEmpty(_account.AccountNum))
        {
            Console.WriteLine("Code is required for creating or updating an account.");
            return;
        }

        QueryExpression accountQuery = new QueryExpression("account");
        accountQuery.ColumnSet = new ColumnSet("accountnumber", "accountid");
        accountQuery.Criteria.AddCondition("accountnumber", ConditionOperator.Equal, _account.AccountNum);
        EntityCollection accounts = serviceClient.RetrieveMultiple(accountQuery);
        Entity account;
        if (accounts.Entities.Count > 0)
        {
            existsAccount = true;
            _account.AccountId = accounts.Entities[0].Id.ToString();
            account = new Entity("account", new Guid(_account.AccountId));

        }
        else
        {
            existsAccount = false;
            account = new Entity("account");
            account["accountnumber"] = _account.AccountNum;
        }


        account["name"] = _account.Name;
        account["exchangerate"] = _account.ExchangeRate;
        account["emailaddress1"] = _account.EmailAddress1;
        account["telephone1"] = _account.Telephone1;
        account["address1_country"] = _account.Address1_Country;
        account["address1_postalcode"] = _account.Address1_PostalCode;

        account["creditlimit"] = new Money(_account.Creditlimit); // Tipo Moneda
        account["creditonhold"] = _account.CreditOnHold; // Dos opciones
        account["paymenttermscode"] = new OptionSetValue(_account.CustomerTypeCode); // Condciones de Pago (opción múltiple)
                                                                                    //account["transactioncurrencyid"] = new EntityReference("transactioncurrency", new Guid(_account.CurrencyId)); 

        EntityReference currencyReference = GetCurrencyReference(_account.CurrencyId, serviceClient);
        if (currencyReference != null)
        {
            account["transactioncurrencyid"] = currencyReference; // Dato referencia de búsqueda (lookup)
        }
        else
        {
            Console.WriteLine($"Currency with ISO code {_account.CurrencyId} not found.");
            return;
        }


        if (existsAccount)
        {
            serviceClient.Update(account);
            Console.WriteLine($"Account updated with ID: {_account.AccountId}");
        }
        else
        {
            Guid accountId = serviceClient.Create(account);
            Console.WriteLine($"Account created with ID: {accountId.ToString()}");
        }



    }

    public EntityReference GetCustomerGroup(string _customerGroupId, ServiceClient serviceClient)
    {
        if (string.IsNullOrEmpty(_customerGroupId))
        {
            return null;
        }

        /*
        Entidad Divisa: transactioncurrency
        Criterio de búsqueda: isocurrencycode
        */

        QueryExpression query = new QueryExpression("msdyn_customergroup");
        query.ColumnSet = new ColumnSet("msdyn_groupid", "msdyn_customergroupid");
        query.Criteria.AddCondition("msdyn_groupid", ConditionOperator.Equal, _customerGroupId);

        EntityCollection currencyCollection = serviceClient.RetrieveMultiple(query);
        if (currencyCollection.Entities.Count == 0)
        {
            return null; 
        }


        Entity currency = currencyCollection.Entities[0];
        return new EntityReference("msdyn_customergroup", currency.Id);
    }

    public EntityReference GetCurrencyReference(string _isoCurrencyCode, ServiceClient serviceClient)
    {
        if (string.IsNullOrEmpty(_isoCurrencyCode))
        {
            return null;
        }

        /*
        Entidad Divisa: transactioncurrency
        Criterio de búsqueda: isocurrencycode
        */

        QueryExpression query = new QueryExpression("transactioncurrency");
        query.ColumnSet = new ColumnSet("isocurrencycode", "transactioncurrencyid");
        query.Criteria.AddCondition("isocurrencycode", ConditionOperator.Equal, _isoCurrencyCode);

        EntityCollection currencyCollection = serviceClient.RetrieveMultiple(query);
        if (currencyCollection.Entities.Count == 0)
        {
            return null; // No se encontró la divisa con el código ISO proporcionado
        }


        Entity currency = currencyCollection.Entities[0];
        return new EntityReference("transactioncurrency", currency.Id);
    }


public void CreateAccountsBulk(List<Account> accounts, ServiceClient serviceClient)
{
    // Configurar ExecuteMultipleRequest
    var executeMultipleRequest = new ExecuteMultipleRequest()
    {
        Settings = new ExecuteMultipleSettings()
        {
            ContinueOnError = true, // Continuar aunque fallen algunos registros
            ReturnResponses = true   // Devolver respuestas individuales
        },
        Requests = new OrganizationRequestCollection()
    };

    // Agregar cada account como CreateRequest
    foreach (var _account in accounts)
    {
        Entity account = new Entity("account");
        account["accountnumber"] = _account.AccountNum;
        account["name"] = _account.Name;
        account["exchangerate"] = _account.ExchangeRate;
        account["emailaddress1"] = _account.EmailAddress1;
        account["telephone1"] = _account.Telephone1;
        account["address1_country"] = _account.Address1_Country;
        account["address1_postalcode"] = _account.Address1_PostalCode;
        account["transactioncurrencyid"] = this.GetCurrencyReference(_account.CurrencyId, serviceClient);
        account["customertypecode"] = new OptionSetValue(_account.CustomerTypeCode);
        account["shippingmethodcode"] = new OptionSetValue(_account.ShippingMethodCode);
        account["industrycode"] = new OptionSetValue(_account.IndustryCode);
        account["creditlimit"] = new Money(_account.Creditlimit);
        account["creditonhold"] = (bool)_account.CreditOnHold;
        account["lastonholdtime"] = _account.LastOnHoldTime;

        CreateRequest createRequest = new CreateRequest { Target = account };
        executeMultipleRequest.Requests.Add(createRequest);
    }

    // Ejecutar la inserción masiva
    Console.WriteLine($"Iniciando inserción masiva de {accounts.Count} cuentas...");
    var executeMultipleResponse = (ExecuteMultipleResponse)serviceClient.Execute(executeMultipleRequest);

    // Procesar resultados
    int successCount = 0;
    int errorCount = 0;

    for (int i = 0; i < executeMultipleResponse.Responses.Count; i++)
    {
        var responseItem = executeMultipleResponse.Responses[i];
        
        if (responseItem.Fault == null)
        {
            var createdId = ((CreateResponse)responseItem.Response).id;
            Console.WriteLine($"✓ Account '{accounts[i].Name}' creada con ID: {createdId}");
            successCount++;
        }
        else
        {
            Console.WriteLine($"✗ Error al crear '{accounts[i].Name}': {responseItem.Fault.Message}");
            errorCount++;
        }
    }

    Console.WriteLine($"\nResumen: {successCount} exitosas, {errorCount} fallidas de {accounts.Count} totales");
}

    public void getAccountFetch(ServiceClient serviceClient)
    {
        string fetchXml = @"
            <fetch version='1.0' output-format='xml-platform' mapping='logical' no-lock='false' distinct='true'>
                <entity name='account'>
                    <attribute name='accountid'/>
                    <attribute name='telephone1'/>
                    <attribute name='name'/>
                    <attribute name='statecode'/>
                <filter type='and'>
			        <condition attribute='telephone1' operator='not-null' />
		        </filter>
                </entity>
            </fetch>";

        string fetchXMLERP = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' no-lock='false'>
	<entity name='account'>
		<attribute name='statecode'/>
		<attribute name='name'/>
		<attribute name='accountid'/>
		<attribute name='transactioncurrencyid'/>
		<attribute name='creditlimit'/>
		<attribute name='shippingmethodcode'/>
		<attribute name='accountnumber'/>
		<order attribute='accountnumber' descending='false'/>
		<filter type='and'>
			<condition attribute='accountnumber' operator='not-null'/>
		</filter>
	</entity>
</fetch>";

        FetchExpression fetchExpression = new FetchExpression(fetchXMLERP);

        EntityCollection accounts = serviceClient.RetrieveMultiple(fetchExpression);

        if (accounts.Entities.Count == 0)
        {
            Console.WriteLine("No accounts found.");
            return;
        }
        Console.WriteLine("Accounts found:");

        foreach (Entity account in accounts.Entities)
        {
            Console.WriteLine($"ERP Code {account["accountnumber"]} - Name: {account["name"]} - Credit Limit {((Money)account["creditlimit"]).Value} - Payment Terms : {((OptionSetValue)account["shippingmethodcode"]).Value}");

        }

    }

    public void getAcountByCodeFetch(ServiceClient serviceClient, string erpCode)
    {

        string fetchXMLERP = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' no-lock='false'>
	<entity name='account'>
		<attribute name='statecode'/>
		<attribute name='name'/>
		<attribute name='accountid'/>
		<attribute name='transactioncurrencyid'/>
		<attribute name='creditlimit'/>
		<attribute name='shippingmethodcode'/>
		<attribute name='accountnumber'/>
		<order attribute='accountnumber' descending='false'/>
		<filter type='and'>
			<condition attribute='accountnumber' operator='eq' value='{0}'/>
		</filter>
	</entity>
</fetch>";

        fetchXMLERP = string.Format(fetchXMLERP, erpCode);

        FetchExpression fetchExpression = new FetchExpression(fetchXMLERP);

        EntityCollection accounts = serviceClient.RetrieveMultiple(fetchExpression);

        if (accounts.Entities.Count == 0)
        {
            Console.WriteLine("No accounts found.");
            return;
        }
        Console.WriteLine("Accounts found:");

        foreach (Entity account in accounts.Entities)
        {
            string currencyIsoCode = string.Empty;
            if (account.Contains("transactioncurrencyid"))
            {
                EntityReference currencyRef = (EntityReference)account["transactioncurrencyid"];
                Entity currency = serviceClient.Retrieve(currencyRef.LogicalName, currencyRef.Id, new ColumnSet("isocurrencycode"));
                if (currency != null && currency.Contains("isocurrencycode"))
                {
                    currencyIsoCode = currency["isocurrencycode"].ToString();
                }
            }

            Console.WriteLine($"ERP Code {account["accountnumber"]} - Name: {account["name"]} - Currency {currencyIsoCode} - Credit Limit {((Money)account["creditlimit"]).Value} - Payment Terms : {((OptionSetValue)account["shippingmethodcode"]).Value}");




        }

    }

    public void getAcountByCodeFetchLinkEntity(ServiceClient serviceClient, string erpCode)
    {

        string fetchXMLERP = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' no-lock='false'>
	<entity name='account'>
    	<attribute name='accountnumber'/>
		<attribute name='statecode'/>
		<attribute name='name'/>
		<attribute name='accountid'/>
		<attribute name='transactioncurrencyid'/>
		<attribute name='creditlimit'/>
		<attribute name='paymenttermscode'/>
		<filter type='and'>
			<condition attribute='accountnumber' operator='eq' value='{0}'/>
		</filter>
        <link-entity alias='currency' name='transactioncurrency' to='transactioncurrencyid' from='transactioncurrencyid' link-type='outer' >
			<attribute name='isocurrencycode'/>
		</link-entity>
	</entity>
</fetch>";

        fetchXMLERP = string.Format(fetchXMLERP, erpCode);

        FetchExpression fetchExpression = new FetchExpression(fetchXMLERP);

        EntityCollection accounts = serviceClient.RetrieveMultiple(fetchExpression);

        if (accounts.Entities.Count == 0)
        {
            Console.WriteLine("No accounts found.");
            return;
        }
        Console.WriteLine("Accounts found:");

        foreach (Entity account in accounts.Entities)
        {
            string currencyIsoCode = string.Empty;

            if (account.Contains("transactioncurrencyid"))
            {
                AliasedValue currencyAliased = (AliasedValue)account["currency.isocurrencycode"];
                if (currencyAliased != null && currencyAliased.Value != null)
                {
                    currencyIsoCode = currencyAliased.Value.ToString();
                }
            }

            Console.WriteLine($"Code {(account.Contains("accountnumber") ? account["accountnumber"] : "")} - " +
                  $"Name: {(account.Contains("name") ? account["name"] : "")} - " +
                  $"Currency {currencyIsoCode} - " +
                  $"Credit Limit {(account.Contains("creditlimit") ? ((Money)account["creditlimit"]).Value : 0)} - " +
                  $"Payment Terms: {(account.Contains("paymenttermscode") ? ((OptionSetValue)account["paymenttermscode"]).Value : 0)}");
            
           
           

        }

    }


    public void getAcountByCurrencyFetchLinkEntity(ServiceClient serviceClient, string _currencyIsoCode)
    {

        string fetchXMLERP = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' no-lock='false'>
	<entity name='account'>
		<attribute name='statecode'/>
		<attribute name='name'/>
		<attribute name='accountid'/>
		<attribute name='transactioncurrencyid'/>
		<attribute name='creditlimit'/>
		<attribute name='paymenttermscode'/>
		<attribute name='accountnumber'/>
		<order attribute='accountnumber' descending='false'/>
		<filter type='and'>
			<condition attribute='accountnumber' operator='not-null'/>
		</filter>
        <link-entity alias='currency' name='transactioncurrency' to='transactioncurrencyid' from='transactioncurrencyid' link-type='inner' >
		<attribute name='isocurrencycode'/>
            <filter type='and'>
			<condition attribute='isocurrencycode' operator='eq' value='{0}'/>
		</filter>
		</link-entity>
	</entity>
</fetch>";

        fetchXMLERP = string.Format(fetchXMLERP, _currencyIsoCode);

        FetchExpression fetchExpression = new FetchExpression(fetchXMLERP);

        EntityCollection accounts = serviceClient.RetrieveMultiple(fetchExpression);

        if (accounts.Entities.Count == 0)
        {
            Console.WriteLine("No accounts found.");
            return;
        }
        Console.WriteLine($"Accounts found with Currency Code:{_currencyIsoCode}");

        foreach (Entity account in accounts.Entities)
        {
            string currencyIsoCode = string.Empty;

            if (account.Contains("transactioncurrencyid"))
            {
                AliasedValue currencyAliased = (AliasedValue)account["currency.isocurrencycode"];
                if (currencyAliased != null && currencyAliased.Value != null)
                {
                    currencyIsoCode = currencyAliased.Value.ToString();
                }
            }

            Console.WriteLine($"Code {(account.Contains("accountnumber") ? account["accountnumber"] : "")} - " +
                  $"Name: {(account.Contains("name") ? account["name"] : "")} - " +
                  $"Currency {currencyIsoCode} - " +
                  $"Credit Limit {(account.Contains("creditlimit") ? ((Money)account["creditlimit"]).Value : 0)} - " +
                  $"Payment Terms: {(account.Contains("paymenttermscode") ? ((OptionSetValue)account["paymenttermscode"]).Value : 0)}");
            
           
           
            
        }
        
    }

}
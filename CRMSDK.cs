using Microsoft.PowerPlatform.Dataverse.Client;

public class CRMSDK
{
    public static void getAccountInfoExample(ServiceClient serviceClient, AccountHandler accountHandler)
    {
        string accountId = "14a9885b-4046-f011-8779-7c1e521b8aaa"; // Replace with a valid account ID

        accountHandler.getAccountInfo(accountId, serviceClient);
    }

    public static void createAccountExample(AccountHandler accountHandler, ServiceClient serviceClient)
    {


        Account account = new Account
        {
            AccountNum = "CLI-000001",
            Name = "Carlos Eduardo Mendoza",
            ExchangeRate = 1.0M,
            EmailAddress1 = "cmendoza@dumbmail.com",
            Telephone1 = "1234-5678",
            Address1_Country = "Guatemala",
            Address1_PostalCode = "10001",
            Creditlimit = 2000,
            CustomerTypeCode = 3,//cliente
            ShippingMethodCode = 1,//
            CreditOnHold = false,
            IndustryCode = 34,
            CurrencyId = "GTQ",
            //CustomerGroupId = "Local",
            LastOnHoldTime = new DateTime(2023, 1, 15)

        };

        accountHandler.CreateAccount(account, serviceClient);



    }

    public static void updateAccountExample(AccountHandler accountHandler, ServiceClient serviceClient)
    {
        Account account = new Account
        {
            AccountId = "78718a29-10a9-f011-bbd3-00224824662c",
            CustomerGroupId = "EX"

        };


        accountHandler.UpdateAccount(account, serviceClient);
    }

    public static void deleteAccountExample(AccountHandler accountHandler, ServiceClient serviceClient)
    {
        string accountId = "78d54d5e-21a9-f011-bbd3-0022480a13b8";

        accountHandler.DeleteAccount(accountId, serviceClient);
    }

    public static void getAccountPerCountryExample(AccountHandler accountHandler, ServiceClient serviceClient)
    {
        accountHandler.getAccountPerCountry("Guatemala", serviceClient);
        accountHandler.getAccountPerCountry("Honduras", serviceClient);
        accountHandler.getAccountPerCountry("México", serviceClient);
    }

    public static void updateTelephoneByCountryCodeExample(AccountHandler accountHandler, ServiceClient serviceClient)
    {
        accountHandler.updateTelephoneByCountryCode("Guatemala", "+502 ", serviceClient);
        accountHandler.updateTelephoneByCountryCode("Honduras", "+504 ", serviceClient);
        accountHandler.updateTelephoneByCountryCode("México", "+52 ", serviceClient);
    }

    public static void CreateAccountsBulk(AccountHandler accountHandler, ServiceClient serviceClient)
    {
        string errorMessage;
        List<Account> accounts = Account.TryReadAccountsFromJson(@"C:\Users\JoseFuentesLopez\Documents\Accounts_CRM_20251014_123400.json", out errorMessage);

        if (accounts == null)
        {
            Console.WriteLine($"Error loading accounts: {errorMessage}");
            return;
        }

        foreach (var account in accounts)
        {
            accountHandler.CreateAccount(account, serviceClient);
        }
    }


    public static void createOrUpdate(AccountHandler accountHandler, ServiceClient serviceClient)
    {
        /*

        creditlimit
        creditonhold
        paymenttermscode
        transactioncurrencyid
        */


        /*
         pago 30 1
         2/% 2
         Pago 45 3
         Pago 60 4
        */

        var account1 = new Account
        {
            AccountNum = "ERP001",
            Name = "Corporación Tecnológica Moderna S.A.",
            ExchangeRate = 1.00m,
            EmailAddress1 = "contacto@tecnomoderna.com",
            Telephone1 = "+502-2345-6789",
            Address1_Country = "Guatemala",
            Address1_PostalCode = "01001",
            Creditlimit = 50000.00m,
            CustomerTypeCode = 3,
            CreditOnHold = false,
            CurrencyId = "GTQ"
        };

        accountHandler.createOrUpdateAccount(account1, serviceClient);

        var account2 = new Account
        {
            AccountNum = "ERP002",
            Name = "Industrias del Café Chapín Ltda.",
            ExchangeRate = 7.85m,
            EmailAddress1 = "ventas@cafechapin.gt",
            Telephone1 = "+502-7890-1234",
            Address1_Country = "Guatemala",
            Address1_PostalCode = "01010",
            Creditlimit = 75000.00m,
            CustomerTypeCode = 3,
            CreditOnHold = false,
            CurrencyId = "USD"
        };

        accountHandler.createOrUpdateAccount(account2, serviceClient);

    }


    public static void getAccountByFetchXML(AccountHandler accountHandler, ServiceClient serviceClient)
    {
        accountHandler.getAccountFetch(serviceClient);

    }

    public static void getAccountByERPCodeFetchXML(AccountHandler accountHandler, ServiceClient serviceClient)
    {
        accountHandler.getAcountByCodeFetch(serviceClient, "ERP001");
    }


    public static void getAccountByERPCodeFetchXMLLink(AccountHandler accountHandler, ServiceClient serviceClient)
    {
        accountHandler.getAcountByCodeFetchLinkEntity(serviceClient, "ERP001");
    }

    public static void getAcountByCurrencyFetchLinkEntity(AccountHandler accountHandler, ServiceClient serviceClient)
    {
        accountHandler.getAcountByCurrencyFetchLinkEntity(serviceClient, "GTQ");
        accountHandler.getAcountByCurrencyFetchLinkEntity(serviceClient, "USD");
    }

    	

}
using System.Text.Json;

public class Account
{
    public string AccountNum { get; set; } 
    public string AccountId { get; set; }
    public string Name { get; set; }
    public decimal? ExchangeRate { get; set; }
    public string EmailAddress1 { get; set; }
    public string Telephone1 { get; set; }
    public string Address1_Country { get; set; }
    public string Address1_PostalCode { get; set; }
    public decimal Creditlimit { get; set; } //Tipo Moneda
    public int CustomerTypeCode { get; set; } // (optión múltiple)
    public int ShippingMethodCode { get; set; } 
    public int IndustryCode { get; set; } // (opción múltiple)
    public bool CreditOnHold { get; set; } //Dos opciones 
    public DateTime LastOnHoldTime { get; set; } //Fecha y Hora
    public string CurrencyId { get; set; } //Dato referencia de búsqueda (lookup)
    public string CustomerGroupId { get; set; }


    public static List<Account> TryReadAccountsFromJson(string filePath, out string errorMessage)
    {
        errorMessage = string.Empty;

        try
        {
            if (!File.Exists(filePath))
            {
                errorMessage = $"File not found: {filePath}";
                return null;
            }

            string jsonContent = File.ReadAllText(filePath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            };

            List<Account> accounts = JsonSerializer.Deserialize<List<Account>>(jsonContent, options);

            if (accounts == null || accounts.Count == 0)
            {
                errorMessage = "No records found in the file.";
                return new List<Account>();
            }

            return accounts;
        }
        catch (Exception ex)
        {
            errorMessage = $"Error reading file: {ex.Message}";
            return null;
        }
    }
    
}
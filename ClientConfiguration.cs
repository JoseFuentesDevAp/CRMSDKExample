using System.Diagnostics;
using System.Xml.Serialization;

public class ClientConfiguration
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string TenantId { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public string Resource { get; set; }
   
   public static ClientConfiguration LoadFromXml(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"El archivo de configuración no existe: {filePath}");
        }

        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ClientConfiguration));
            
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                ClientConfiguration config = (ClientConfiguration)serializer.Deserialize(fileStream);
                return config;
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al cargar la configuración desde XML: {ex.Message}", ex);
        }
    }
}
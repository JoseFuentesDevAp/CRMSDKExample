using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;

public class ProductHandler
{
    /// <summary>
    /// Obtiene la información de un producto por su ID
    /// </summary>
    public void getProductInfo(string productId, ServiceClient serviceClient)
    {
        Entity productInfo = serviceClient.Retrieve("product", new Guid(productId), new ColumnSet(true));

        Console.WriteLine($"Product Number: {productInfo.Attributes["productnumber"]}");
        Console.WriteLine($"Product Name: {productInfo.Attributes["name"]}");
        
        if (productInfo.Contains("description"))
        {
            Console.WriteLine($"Description: {productInfo.Attributes["description"]}");
        }
        
        if (productInfo.Contains("quantitydecimal"))
        {
            Console.WriteLine($"Quantity Decimal: {productInfo.Attributes["quantitydecimal"]}");
        }
    }

    /// <summary>
    /// Crea un nuevo producto
    /// </summary>
    public void CreateProduct(Product _product, ServiceClient serviceClient)
    {
        Entity product = new Entity("product");
        
        product["productnumber"] = _product.ProductNumber;
        product["name"] = _product.Name;
        
        if (!string.IsNullOrEmpty(_product.Description))
        {
            product["description"] = _product.Description;
        }
        
        if (_product.QuantityDecimal.HasValue)
        {
            product["quantitydecimal"] = _product.QuantityDecimal.Value;
        }
        
        // Configurar lookup para defaultuomscheduleid (uomschedule)
        if (!string.IsNullOrEmpty(_product.DefaultUomScheduleId))
        {
            EntityReference uomScheduleReference = GetUomScheduleReference(_product.DefaultUomScheduleId, serviceClient);
            if (uomScheduleReference != null)
            {
                product["defaultuomscheduleid"] = uomScheduleReference;
            }
            else
            {
                Console.WriteLine($"UOM Schedule with ID {_product.DefaultUomScheduleId} not found.");
                return;
            }
        }
        
        // Configurar lookup para defaultuomid (uom)
        if (!string.IsNullOrEmpty(_product.DefaultUomId))
        {
            EntityReference uomReference = GetUomReference(_product.DefaultUomId, serviceClient);
            if (uomReference != null)
            {
                product["defaultuomid"] = uomReference;
            }
            else
            {
                Console.WriteLine($"UOM with ID {_product.DefaultUomId} not found.");
                return;
            }
        }

        Guid productId = serviceClient.Create(product);
        Console.WriteLine($"Product created with ID: {productId.ToString()}");
    }

    /// <summary>
    /// Actualiza un producto existente
    /// </summary>
    public void UpdateProduct(Product _product, ServiceClient serviceClient)
    {
        if (string.IsNullOrEmpty(_product.ProductId))
        {
            Console.WriteLine("Product ID is required for update.");
            return;
        }

        Entity product = new Entity("product", new Guid(_product.ProductId));
        
        if (!string.IsNullOrEmpty(_product.Name))
        {
            product["name"] = _product.Name;
        }
        
        if (!string.IsNullOrEmpty(_product.Description))
        {
            product["description"] = _product.Description;
        }
        
        if (_product.QuantityDecimal.HasValue)
        {
            product["quantitydecimal"] = _product.QuantityDecimal.Value;
        }

        serviceClient.Update(product);
        Console.WriteLine($"Product updated with ID: {_product.ProductId}");
    }

    /// <summary>
    /// Elimina un producto por su ID
    /// </summary>
    public void DeleteProduct(string productId, ServiceClient serviceClient)
    {
        serviceClient.Delete("product", new Guid(productId));
        Console.WriteLine($"Product deleted with ID: {productId}");
    }

    /// <summary>
    /// Obtiene productos filtrando por un criterio específico
    /// </summary>
    public void getProductsByName(string productName, ServiceClient serviceClient)
    {
        Console.WriteLine($"Retrieving products with name containing: {productName}");

        QueryExpression query = new QueryExpression("product");
        query.ColumnSet = new ColumnSet("productnumber", "name", "description", "quantitydecimal");
        query.Criteria.AddCondition("name", ConditionOperator.Like, $"%{productName}%");

        EntityCollection products = serviceClient.RetrieveMultiple(query);

        if (products.Entities.Count == 0)
        {
            Console.WriteLine("No products found.");
            return;
        }

        foreach (var product in products.Entities)
        {
            Console.WriteLine($"Product Number: {product["productnumber"]}, Name: {product["name"]}");
        }
    }

    /// <summary>
    /// Actualiza la descripción de productos basándose en un criterio
    /// </summary>
    public void updateProductDescriptionByNumber(string productNumber, string newDescription, ServiceClient serviceClient)
    {
        Console.WriteLine($"Updating description for product number: {productNumber}");

        QueryExpression query = new QueryExpression("product");
        query.ColumnSet = new ColumnSet("productnumber", "name", "productid");
        query.Criteria.AddCondition("productnumber", ConditionOperator.Equal, productNumber);

        EntityCollection products = serviceClient.RetrieveMultiple(query);

        if (products.Entities.Count == 0)
        {
            Console.WriteLine($"No product found with number: {productNumber}");
            return;
        }

        foreach (var product in products.Entities)
        {
            product["description"] = newDescription;
            serviceClient.Update(product);
            Console.WriteLine($"Updated Product: {product["name"]}, New Description: {newDescription}");
        }
    }

    /// <summary>
    /// Crea o actualiza un producto buscándolo por su número de producto
    /// </summary>
    public void createOrUpdateProduct(Product _product, ServiceClient serviceClient)
    {
        bool existsProduct = false;

        if (string.IsNullOrEmpty(_product.ProductNumber))
        {
            Console.WriteLine("Product Number is required for creating or updating a product.");
            return;
        }

        QueryExpression productQuery = new QueryExpression("product");
        productQuery.ColumnSet = new ColumnSet("productnumber", "productid");
        productQuery.Criteria.AddCondition("productnumber", ConditionOperator.Equal, _product.ProductNumber);
        EntityCollection products = serviceClient.RetrieveMultiple(productQuery);
        
        Entity product;
        if (products.Entities.Count > 0)
        {
            existsProduct = true;
            _product.ProductId = products.Entities[0].Id.ToString();
            product = new Entity("product", new Guid(_product.ProductId));
        }
        else
        {
            existsProduct = false;
            product = new Entity("product");
            product["productnumber"] = _product.ProductNumber;
        }

        product["name"] = _product.Name;
        
        if (!string.IsNullOrEmpty(_product.Description))
        {
            product["description"] = _product.Description;
        }
        
        if (_product.QuantityDecimal.HasValue)
        {
            product["quantitydecimal"] = _product.QuantityDecimal.Value;
        }

        // Configurar lookup para defaultuomscheduleid si se proporciona
        if (!string.IsNullOrEmpty(_product.DefaultUomScheduleId))
        {
            EntityReference uomScheduleReference = GetUomScheduleReference(_product.DefaultUomScheduleId, serviceClient);
            if (uomScheduleReference != null)
            {
                product["defaultuomscheduleid"] = uomScheduleReference;
            }
            else
            {
                Console.WriteLine($"UOM Schedule with ID {_product.DefaultUomScheduleId} not found.");
                return;
            }
        }

        // Configurar lookup para defaultuomid si se proporciona
        if (!string.IsNullOrEmpty(_product.DefaultUomId))
        {
            EntityReference uomReference = GetUomReference(_product.DefaultUomId, serviceClient);
            if (uomReference != null)
            {
                product["defaultuomid"] = uomReference;
            }
            else
            {
                Console.WriteLine($"UOM with ID {_product.DefaultUomId} not found.");
                return;
            }
        }

        if (existsProduct)
        {
            serviceClient.Update(product);
            Console.WriteLine($"Product updated with ID: {_product.ProductId}");
        }
        else
        {
            Guid productId = serviceClient.Create(product);
            Console.WriteLine($"Product created with ID: {productId.ToString()}");
        }
    }

    /// <summary>
    /// Obtiene la referencia de UOM Schedule por nombre
    /// </summary>
    public EntityReference GetUomScheduleReference(string uomScheduleName, ServiceClient serviceClient)
    {
        if (string.IsNullOrEmpty(uomScheduleName))
        {
            return null;
        }

        QueryExpression query = new QueryExpression("uomschedule");
        query.ColumnSet = new ColumnSet("name", "uomscheduleid");
        query.Criteria.AddCondition("name", ConditionOperator.Equal, uomScheduleName);

        EntityCollection uomScheduleCollection = serviceClient.RetrieveMultiple(query);
        if (uomScheduleCollection.Entities.Count == 0)
        {
            return null;
        }

        Entity uomSchedule = uomScheduleCollection.Entities[0];
        return new EntityReference("uomschedule", uomSchedule.Id);
    }

    /// <summary>
    /// Obtiene la referencia de UOM por nombre
    /// </summary>
    public EntityReference GetUomReference(string uomName, ServiceClient serviceClient)
    {
        if (string.IsNullOrEmpty(uomName))
        {
            return null;
        }

        QueryExpression query = new QueryExpression("uom");
        query.ColumnSet = new ColumnSet("name", "uomid");
        query.Criteria.AddCondition("name", ConditionOperator.Equal, uomName);

        EntityCollection uomCollection = serviceClient.RetrieveMultiple(query);
        if (uomCollection.Entities.Count == 0)
        {
            return null;
        }

        Entity uom = uomCollection.Entities[0];
        return new EntityReference("uom", uom.Id);
    }

    /// <summary>
    /// Crea productos en masa utilizando ExecuteMultipleRequest
    /// </summary>
    public void CreateProductsBulk(List<Product> products, ServiceClient serviceClient)
    {
        var executeMultipleRequest = new ExecuteMultipleRequest()
        {
            Settings = new ExecuteMultipleSettings()
            {
                ContinueOnError = true,
                ReturnResponses = true
            },
            Requests = new OrganizationRequestCollection()
        };

        foreach (var _product in products)
        {
            Entity product = new Entity("product");
            product["productnumber"] = _product.ProductNumber;
            product["name"] = _product.Name;
            
            if (!string.IsNullOrEmpty(_product.Description))
            {
                product["description"] = _product.Description;
            }
            
            if (_product.QuantityDecimal.HasValue)
            {
                product["quantitydecimal"] = _product.QuantityDecimal.Value;
            }

            // Configurar lookups si están disponibles
            if (!string.IsNullOrEmpty(_product.DefaultUomScheduleId))
            {
                EntityReference uomScheduleReference = GetUomScheduleReference(_product.DefaultUomScheduleId, serviceClient);
                if (uomScheduleReference != null)
                {
                    product["defaultuomscheduleid"] = uomScheduleReference;
                }
            }

            if (!string.IsNullOrEmpty(_product.DefaultUomId))
            {
                EntityReference uomReference = GetUomReference(_product.DefaultUomId, serviceClient);
                if (uomReference != null)
                {
                    product["defaultuomid"] = uomReference;
                }
            }

            CreateRequest createRequest = new CreateRequest { Target = product };
            executeMultipleRequest.Requests.Add(createRequest);
        }

        Console.WriteLine($"Iniciando inserción masiva de {products.Count} productos...");
        var executeMultipleResponse = (ExecuteMultipleResponse)serviceClient.Execute(executeMultipleRequest);

        int successCount = 0;
        int errorCount = 0;

        for (int i = 0; i < executeMultipleResponse.Responses.Count; i++)
        {
            var responseItem = executeMultipleResponse.Responses[i];
            
            if (responseItem.Fault == null)
            {
                var createdId = ((CreateResponse)responseItem.Response).id;
                Console.WriteLine($"Product '{products[i].Name}' creado con ID: {createdId}");
                successCount++;
            }
            else
            {
                Console.WriteLine($"Error al crear '{products[i].Name}': {responseItem.Fault.Message}");
                errorCount++;
            }
        }

        Console.WriteLine($"\nResumen: {successCount} exitosos, {errorCount} fallidos de {products.Count} totales");
    }

    /// <summary>
    /// Obtiene productos utilizando FetchXML
    /// </summary>
    public void getProductFetch(ServiceClient serviceClient)
    {
        string fetchXml = @"
            <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' no-lock='false'>
                <entity name='product'>
                    <attribute name='productid'/>
                    <attribute name='productnumber'/>
                    <attribute name='name'/>
                    <attribute name='description'/>
                    <attribute name='quantitydecimal'/>
                    <order attribute='productnumber' descending='false'/>
                    <filter type='and'>
                        <condition attribute='productnumber' operator='not-null'/>
                    </filter>
                </entity>
            </fetch>";

        FetchExpression fetchExpression = new FetchExpression(fetchXml);
        EntityCollection products = serviceClient.RetrieveMultiple(fetchExpression);

        if (products.Entities.Count == 0)
        {
            Console.WriteLine("No products found.");
            return;
        }
        
        Console.WriteLine("Products found:");
        foreach (Entity product in products.Entities)
        {
            string description = product.Contains("description") ? product["description"].ToString() : "N/A";
            string quantityDecimal = product.Contains("quantitydecimal") ? product["quantitydecimal"].ToString() : "N/A";
            
            Console.WriteLine($"Product Number: {product["productnumber"]} - Name: {product["name"]} - Description: {description} - Quantity Decimal: {quantityDecimal}");
        }
    }

    /// <summary>
    /// Obtiene un producto por número usando FetchXML
    /// </summary>
    public void getProductByNumberFetch(ServiceClient serviceClient, string productNumber)
    {
        string fetchXml = @"
            <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' no-lock='false'>
                <entity name='product'>
                    <attribute name='productid'/>
                    <attribute name='productnumber'/>
                    <attribute name='name'/>
                    <attribute name='description'/>
                    <attribute name='quantitydecimal'/>
                    <attribute name='defaultuomscheduleid'/>
                    <attribute name='defaultuomid'/>
                    <filter type='and'>
                        <condition attribute='productnumber' operator='eq' value='{0}'/>
                    </filter>
                </entity>
            </fetch>";

        fetchXml = string.Format(fetchXml, productNumber);
        FetchExpression fetchExpression = new FetchExpression(fetchXml);
        EntityCollection products = serviceClient.RetrieveMultiple(fetchExpression);

        if (products.Entities.Count == 0)
        {
            Console.WriteLine("No product found.");
            return;
        }

        Console.WriteLine("Product found:");
        foreach (Entity product in products.Entities)
        {
            string description = product.Contains("description") ? product["description"].ToString() : "N/A";
            string quantityDecimal = product.Contains("quantitydecimal") ? product["quantitydecimal"].ToString() : "N/A";
            
            Console.WriteLine($"Product Number: {product["productnumber"]} - Name: {product["name"]} - Description: {description} - Quantity Decimal: {quantityDecimal}");
        }
    }

    /// <summary>
    /// Obtiene productos con entidades relacionadas usando FetchXML con link-entity
    /// </summary>
    public void getProductByNumberFetchLinkEntity(ServiceClient serviceClient, string productNumber)
    {
        string fetchXml = @"
            <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' no-lock='false'>
                <entity name='product'>
                    <attribute name='productid'/>
                    <attribute name='productnumber'/>
                    <attribute name='name'/>
                    <attribute name='description'/>
                    <attribute name='quantitydecimal'/>
                    <filter type='and'>
                        <condition attribute='productnumber' operator='eq' value='{0}'/>
                    </filter>
                    <link-entity alias='uomschedule' name='uomschedule' to='defaultuomscheduleid' from='uomscheduleid' link-type='outer'>
                        <attribute name='name'/>
                    </link-entity>
                    <link-entity alias='uom' name='uom' to='defaultuomid' from='uomid' link-type='outer'>
                        <attribute name='name'/>
                    </link-entity>
                </entity>
            </fetch>";

        fetchXml = string.Format(fetchXml, productNumber);
        FetchExpression fetchExpression = new FetchExpression(fetchXml);
        EntityCollection products = serviceClient.RetrieveMultiple(fetchExpression);

        if (products.Entities.Count == 0)
        {
            Console.WriteLine("No product found.");
            return;
        }

        Console.WriteLine("Product found with related entities:");
        foreach (Entity product in products.Entities)
        {
            string uomScheduleName = "N/A";
            string uomName = "N/A";

            if (product.Contains("uomschedule.name"))
            {
                AliasedValue uomScheduleAliased = (AliasedValue)product["uomschedule.name"];
                if (uomScheduleAliased != null && uomScheduleAliased.Value != null)
                {
                    uomScheduleName = uomScheduleAliased.Value.ToString();
                }
            }

            if (product.Contains("uom.name"))
            {
                AliasedValue uomAliased = (AliasedValue)product["uom.name"];
                if (uomAliased != null && uomAliased.Value != null)
                {
                    uomName = uomAliased.Value.ToString();
                }
            }

            string description = product.Contains("description") ? product["description"].ToString() : "N/A";

            Console.WriteLine($"Product Number: {product["productnumber"]} - Name: {product["name"]} - " +
                            $"Description: {description} - UOM Schedule: {uomScheduleName} - UOM: {uomName}");
        }
    }

    /// <summary>
    /// Obtiene productos filtrando por UOM Schedule usando FetchXML con link-entity
    /// </summary>
    public void getProductsByUomScheduleFetchLinkEntity(ServiceClient serviceClient, string uomScheduleName)
    {
        string fetchXml = @"
            <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' no-lock='false'>
                <entity name='product'>
                    <attribute name='productid'/>
                    <attribute name='productnumber'/>
                    <attribute name='name'/>
                    <attribute name='description'/>
                    <order attribute='productnumber' descending='false'/>
                    <filter type='and'>
                        <condition attribute='productnumber' operator='not-null'/>
                    </filter>
                    <link-entity alias='uomschedule' name='uomschedule' to='defaultuomscheduleid' from='uomscheduleid' link-type='inner'>
                        <attribute name='name'/>
                        <filter type='and'>
                            <condition attribute='name' operator='eq' value='{0}'/>
                        </filter>
                    </link-entity>
                </entity>
            </fetch>";

        fetchXml = string.Format(fetchXml, uomScheduleName);
        FetchExpression fetchExpression = new FetchExpression(fetchXml);
        EntityCollection products = serviceClient.RetrieveMultiple(fetchExpression);

        if (products.Entities.Count == 0)
        {
            Console.WriteLine($"No products found with UOM Schedule: {uomScheduleName}");
            return;
        }

        Console.WriteLine($"Products found with UOM Schedule: {uomScheduleName}");
        foreach (Entity product in products.Entities)
        {
            string uomScheduleNameResult = "N/A";
            if (product.Contains("uomschedule.name"))
            {
                AliasedValue uomScheduleAliased = (AliasedValue)product["uomschedule.name"];
                if (uomScheduleAliased != null && uomScheduleAliased.Value != null)
                {
                    uomScheduleNameResult = uomScheduleAliased.Value.ToString();
                }
            }

            string description = product.Contains("description") ? product["description"].ToString() : "N/A";

            Console.WriteLine($"Product Number: {product["productnumber"]} - Name: {product["name"]} - " +
                            $"Description: {description} - UOM Schedule: {uomScheduleNameResult}");
        }
    }
}

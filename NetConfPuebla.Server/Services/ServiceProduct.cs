using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NetConfPuebla.Entities;

namespace NetConfPuebla.Server.Services
{
    public class ServiceProduct
    {
        private readonly HttpClient _httpClient;

        public ServiceProduct(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Product> InsertProduct(Product product)
        {
            Product result = null;
            var jsonProduct = JsonSerializer.Serialize(product);
            var content = new StringContent(jsonProduct, Encoding.UTF8, "application/json");
            var response = await _httpClient
                .PostAsync("https://netconfapi20201114124013.azurewebsites.net/api/products", content);
            if (response.IsSuccessStatusCode)
            {
                jsonProduct = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<Product>(
                    jsonProduct, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            }
            return result;
        }

        public async Task<bool> UpdateProduct(int id, Product product)
        {
            var jsonProduct = JsonSerializer.Serialize(product);
            var content = new StringContent(jsonProduct, Encoding.UTF8, "application/json");
            var response = await _httpClient
                .PutAsync($"https://netconfapi20201114124013.azurewebsites.net/api/products/{id}", content);
            return response.IsSuccessStatusCode;
        }
    }
}

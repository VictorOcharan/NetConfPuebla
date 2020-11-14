using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.AspNetCore.SignalR.Client;
using NetConfPuebla.Entities;

namespace NetConfPuebla.WpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Product> products;

        private readonly HubConnection hubConnection;
        public MainWindow()
        {
            InitializeComponent();

            hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/northwindhub")
                .WithAutomaticReconnect()
                .Build();

            hubConnection.On<Product>("ReceiveInsertProduct", product => InsertProduct(product));

            hubConnection.On<int, Product>("ReceiveUpdateProduct", (id, product) => UpdateProduct(id, product));

            hubConnection.StartAsync().GetAwaiter();

            buttonGetProducts.Click += async (sender, e) =>
            {
                var httpClient = new HttpClient();
                var response = await httpClient
                    .GetAsync("https://netconfapi20201114124013.azurewebsites.net/api/products");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    products = JsonSerializer
                        .Deserialize<List<Product>>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    dataGridProducts.ItemsSource = products;
                }
            };

            buttonInsertProduct.Click += async (sender, e) =>
            {
                var product = new Product { Name = "Producto nuevo desde WPF", UnitPrice = 23.34m, UnitsInStock = 100, CategoryId = 1 };

                await hubConnection.InvokeAsync("SendInsertProduct", product);
            };

            buttonUpdateProduct.Click += async (sender, e) =>
            {
                var product = new Product { Id = 2, Name = "Producto modificado desde WPF", UnitPrice = 23.34m, UnitsInStock = 100, CategoryId = 1 };

                await hubConnection.InvokeAsync("SendUpdateProduct", 2, product);
            };
        }

        private void InsertProduct(Product product)
        {
            products.Add(product);
            dataGridProducts.Items.Refresh();
        }

        private void UpdateProduct(int id, Product product)
        {
            products.RemoveAt(id - 1);
            products.Insert(id -1, product);
            dataGridProducts.Items.Refresh();
        }
    }
}

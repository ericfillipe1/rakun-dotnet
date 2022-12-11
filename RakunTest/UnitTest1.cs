using System.Collections;
using System.Security.AccessControl;
using Rakun;
using Rakun.Extensions;

namespace RakunTest
{
    [TestClass]
    public class UnitTest1
    {

        class Product
        {
            public string? Id { get; set; }

            public string? Name { get; set; }
            public double? Amount { get; set; }
            public double Price { get; set; }

        }

        class Client
        {
            public string? Id { get; set; }

            public string? Name { get; set; }

        }

        class OrderItem
        {
            public string? Id { get; set; }
            public string? IdProduct { get; set; }

            public string? Name { get; set; }
            public double Amount { get; set; }

        }

        class Order
        {
            public string? Id { get; set; }
            public string? IdClient { get; set; }
            public Client? Client { get; set; }

            public IEnumerable<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
            public double? Value { get; set; }

        }


        [TestMethod]
        public  async Task TestMethod1()
        {

           var  order  = new Order {
                IdClient="123",
 
                OrderItems = new List<OrderItem> {
                    new OrderItem {
                        IdProduct="1",
                        Amount=5
                    }
                }   
           
           };
           var orderUpdate = await order.Mono()
                .PipeZipMono(GetOrderItems,GetClient)
                .Pipe(ApplyValues)
                .BuildTask();


        }

        private IMono<Client> GetClient(Order order)
        {
            return new Client
            {
                Name = $"client test {order.IdClient}",
                Id = order.IdClient
            }.Mono();
        }

        private Order ApplyValues((Order order, IEnumerable<(OrderItem orderItem, Product product)> productValues, Client client) values)
        {

            values.order.Value = GetValueOrder(values.productValues);
            values.order.Client = values.client;
            return values.order;
        }

        private double? GetValueOrder(IEnumerable<(OrderItem orderItem, Product product)> productValues)
        {

            return productValues.Sum((values) => values.product.Price * values.orderItem.Amount);
        }


        private IMono<IEnumerable<(OrderItem v, Product Result)>> GetOrderItems(Order order)
        {

            return order.OrderItems.Flux()
                .PipeZipMono(GetProduct)
                .Enumerable();
        }

        private IMono<Product> GetProduct(OrderItem orderItem)
        {
            return new Product
            {
                Amount = 1.0,
                Price = 2.0,
                Id = orderItem.IdProduct,
                Name = "dsgs"
            }.Mono();
        }
    }
}
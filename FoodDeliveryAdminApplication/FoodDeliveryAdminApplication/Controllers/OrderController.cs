using FoodDeliveryAdminApplication.Models;
using GemBox.Document;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection.Metadata;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Document = iTextSharp.text.Document;

namespace FoodDeliveryAdminApplication.Controllers
{
    public class OrderController : Controller
    {

        public OrderController()
        {
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
        }

        public IActionResult Index()
        {
            HttpClient client = new HttpClient();

            string URL = "https://fooddeliverywebapp211076211032212029.azurewebsites.net/api/Admin/GetAllActiveOrders";

            HttpResponseMessage response = client.GetAsync(URL).Result;

            var data = response.Content.ReadAsAsync<List<Order>>().Result;

            return View(data);
        }

        public IActionResult Details(Guid id)
        {
            HttpClient client = new HttpClient();

            string URL = "https://fooddeliverywebapp211076211032212029.azurewebsites.net/api/Admin/GetDetailsForOrder";

            var model = new
            {
                Id = id
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var data = response.Content.ReadAsAsync<Order>().Result;

            return View(data);
        }

        [HttpPost]
        public IActionResult DeleteOrder(Guid id)
        {
            HttpClient client = new HttpClient();
            string URL = "https://fooddeliverywebapp211076211032212029.azurewebsites.net/api/Admin/DeleteOrder"; 

            var model = new
            {
                Id = id
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to delete the order. Please try again.");
                return View("Index");
            }
        }

        public FileContentResult CreateInvoice(Guid id)
        {

            HttpClient client = new HttpClient();

            string URL = "https://fooddeliverywebapp211076211032212029.azurewebsites.net/api/Admin/GetDetailsForOrder";

            var model = new
            {
                Id = id
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var result = response.Content.ReadAsAsync<Order>().Result;

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Invoice.docx");
            var document = DocumentModel.Load(templatePath); 

            document.Content.Replace("{{OrderNumber}}", result.Id.ToString());
            document.Content.Replace("{{UserName}}", result.Owner.UserName);
            document.Content.Replace("{{DeliveryAddress }}", result.address);
            document.Content.Replace("{{ContactNumber}}", result.contactPhone);

            StringBuilder sb = new StringBuilder();

            foreach (var item in result.DishInOrders)
            {
                sb.AppendLine("Dish: " + item.OrderedDish.DishName + ", quantity: " + item.Quantity + ", price: " + item.OrderedDish.Price + " ден.");
            }
            document.Content.Replace("{{DishesList}}", sb.ToString());
            document.Content.Replace("{{TotalPrice}}", result.totalPrice.ToString() + " ден.");

            var stream = new MemoryStream();
            document.Save(stream, new PdfSaveOptions());

            return File(stream.ToArray(), new PdfSaveOptions().ContentType, "ExportInvoice.pdf");
        }

        [HttpGet]
        public FileContentResult ExportAllOrders()
        {
            string fileName = "Orders.pdf";
            string contentType = "application/pdf";

            using (var memoryStream = new MemoryStream())
            {
                Document document = new Document();
                PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                PdfPTable table = new PdfPTable(6); 
                table.WidthPercentage = 105;
                table.SetWidths(new float[] { 20f, 40f, 20f, 25f, 20f, 17f });

                // Adding headers
                table.AddCell("OrderID");
                table.AddCell("Customer's Username");
                table.AddCell("Delivery Address");
                table.AddCell("Contact Number");
                table.AddCell("Dishes");
                table.AddCell("Total Price");

                HttpClient client = new HttpClient();
                string URL = "https://fooddeliverywebapp211076211032212029.azurewebsites.net/api/Admin/GetAllActiveOrders";
                HttpResponseMessage response = client.GetAsync(URL).Result;
                var data = response.Content.ReadAsAsync<List<Order>>().Result;

                foreach (var item in data)
                {
                    table.AddCell(item.Id.ToString());
                    table.AddCell(item.Owner.UserName);
                    table.AddCell(item.address);
                    table.AddCell(item.contactPhone);

                    string dishes = "";
                    foreach (var dishInOrder in item.DishInOrders)
                    {
                        dishes += $"{dishInOrder.Quantity} {dishInOrder.OrderedDish.DishName}\n ";
                    }

                    table.AddCell(dishes);
                    table.AddCell(item.totalPrice.ToString() + " MKD");

                }

                document.Add(table);
                document.Close();

                var content = memoryStream.ToArray();
                return File(content, contentType, fileName);
            }
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;
using Insurance.Dto;
using System.Collections.Generic;
using Insurance.Model;
using Newtonsoft.Json.Linq;

namespace Insurance.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Database _database;

        public HomeController(IWebHostEnvironment environment, ILogger<HomeController> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _environment = environment;
            _logger = logger;
            _configuration=configuration;

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };
            _serializerSettings=new() { ContractResolver = contractResolver, Formatting=Formatting.Indented };
            _httpClientFactory = httpClientFactory;

            _database=new Database(_configuration.GetConnectionString("Default_ConnectionString"));
        }


        public IActionResult Index()
        {
            List<Client> clients =_database.GetClients();
            return View(clients);
        }

        


        [HttpPost]
        public async Task<IActionResult> Index([FromBody] FormInput input)
        {
            ClientDataRequest clientDataRequest = new(input);

            var serializedRequest = SerializeRequest(clientDataRequest);
            string request = await serializedRequest.ReadAsStringAsync();
            _logger.LogInformation("Getting insurance Data Request: \n{request}", request);

            HttpClient httpClient = _httpClientFactory.CreateClient("insurance");
            string url = _configuration.GetValue<string>("InsuranceDataUrl");
            var response = await httpClient.PostAsync(url, serializedRequest);
            string result = await response.Content.ReadAsStringAsync();
            LogResponse(response, result);

            JObject o = JObject.Parse(result);
            var data = o.GetValue("data");
            var status = ((int)o.GetValue("status"));

            if (status==200) {
                var output = data.ToObject<JObject>().GetValue("output");


                List<Client> clients = new();
                foreach (ClientData clientData in clientDataRequest.ClientsData)
                {
                    var customer = output.ToObject<JObject>().GetValue(clientData.IdNumber);

                    var carBase64File = customer.ToObject<JObject>().GetValue("extracted_cars_file").ToString();
                    var insuranceBase64File = customer.ToObject<JObject>().GetValue("extracted_insurance_file").ToString();

                    string? carFile = null;
                    string insuranceFile = null;

                    if (!string.IsNullOrEmpty(carBase64File))
                        carFile=SaveFile(carBase64File, "pdf");
                    if (!string.IsNullOrEmpty(insuranceBase64File))
                        insuranceFile=SaveFile(insuranceBase64File, "xlsx");

                    Client client = new(clientData.IdNumber, clientData.Name, clientData.BirthDate, clientData.IssueDate, insuranceFile, carFile, clientData.HasPartner);
                    clients.Add(client);
                }

                _database.SaveClients(clients);
            }

            
            return Json(true);
        }


        [HttpPost]
        public IActionResult Delete(string Id)
        {
            _database.DeleteClient(Id);
            return RedirectToAction("Index");
        }


        public IActionResult Contact()
        {
            return View();
        }



        private string SaveFile(string base64Data, string ext)
        {
            string path = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileName = Guid.NewGuid().ToString()+"."+ext;
            byte[] fileBytes=Convert.FromBase64String(base64Data);
            Stream fileStream=new MemoryStream(fileBytes);
            using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                fileStream.CopyTo(stream);
            }

            //Return file name to be saved to database
            return $"/uploads/{fileName}";
        }

        private StringContent SerializeRequest(object data)
        {
            var finalReqBody = JsonConvert.SerializeObject(data, _serializerSettings);
            var serializedData = new StringContent(finalReqBody, Encoding.Default, "application/json");
            return serializedData;
        }


        private void LogResponse(HttpResponseMessage response, string result)
        {
            if (response == null) { return; }

            var parsedJson = JsonConvert.DeserializeObject(result);
            result = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Request succeeded with response: {result}", result);
            }
            else
            {
                _logger.LogInformation("Request failed with response: {result}", result);
            }
        }


    }
}

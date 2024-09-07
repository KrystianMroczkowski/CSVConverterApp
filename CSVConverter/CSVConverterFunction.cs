using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using CSVConverter.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace CSVConverter
{
    public class CSVConverterFunction
    {
        private readonly ILogger _logger;

        public CSVConverterFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CSVConverterFunction>();
        }

        [Function("CSVConverterFunction")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            var data = await JsonSerializer.DeserializeAsync<List<PersonModel>>(req.Body);

			var csv = ConvertToCSV(data);

			var response = req.CreateResponse(HttpStatusCode.OK);

            response.Headers.Add("Content-Type", "text/csv; charset=utf-8");
			response.Headers.Add("Content-Disposition", "attachment; filename=\"people.csv\"");
			await response.WriteStringAsync(csv);

			return response;
        }

		private string ConvertToCSV(List<PersonModel> data)
		{
			var csvBuilder = new StringBuilder();
			csvBuilder.AppendLine("Id,FirstName,LastName");

			foreach (var person in data)
			{
				csvBuilder.AppendLine($"{person.Id},{person.FirstName},{person.LastName}");
			}

			return csvBuilder.ToString();
		}
	}
}

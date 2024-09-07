using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Azure;
using CSVConverter.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CSVConverter
{
    public class CSVConverterFunction
    {
        private readonly ILogger _logger;
		private readonly IConfiguration _config;

        public CSVConverterFunction(ILoggerFactory loggerFactory, IConfiguration config)
        {
            _logger = loggerFactory.CreateLogger<CSVConverterFunction>();
			_config = config;
        }

        [Function("CSVConverterFunction")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            var data = await JsonSerializer.DeserializeAsync<List<PersonModel>>(req.Body);

			if (data is null || data.Count == 0)
			{
				_logger.LogError("No data found in request body.");
				return req.CreateResponse(HttpStatusCode.BadRequest);	
			}

			bool? includeHeader = _config.GetValue<bool>("includeHeaders");

			if (includeHeader is null)
			{
				_logger.LogError("No includeHeaders value found in configuration.");
				return req.CreateResponse(HttpStatusCode.InternalServerError);
			}

			var csv = ConvertToCSV(data, includeHeader);

			var response = req.CreateResponse(HttpStatusCode.OK);

            response.Headers.Add("Content-Type", "text/csv; charset=utf-8");
			response.Headers.Add("Content-Disposition", "attachment; filename=\"people.csv\"");
			await response.WriteStringAsync(csv);

			return response;
        }

		private string ConvertToCSV(List<PersonModel> data, bool? includeHeader)
		{
			var csvBuilder = new StringBuilder();
			if (includeHeader is true)
			{
				csvBuilder.AppendLine("Id,FirstName,LastName");
			}

			foreach (var person in data)
			{
				csvBuilder.AppendLine($"{person.Id},{person.FirstName},{person.LastName}");
			}

			return csvBuilder.ToString();
		}
	}
}

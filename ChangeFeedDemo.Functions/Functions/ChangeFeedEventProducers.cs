using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ChangeFeedDemo.Functions.Helpers;
using ChangeFeedDemo.Functions.Models;
using Microsoft.Azure.Documents.Client;
using System.Linq;

namespace ChangeFeedDemo.Functions
{
    public class ChangeFeedEventProducers
    {
        [FunctionName("CreateTaskItem")]
        public async Task<IActionResult> CreateTaskItem(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "tasks")] HttpRequest req,
            [CosmosDB(
                databaseName: Constants.CosmosDBName,
                collectionName: Constants.CosmosCollectionName,
                ConnectionStringSetting = Constants.CosmosConnectionString)] IAsyncCollector<object> taskItems)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            
            var input = JsonConvert.DeserializeObject<TaskItemCreateModel>(requestBody);

            var taskItem = new TaskItem()
            {
                TaskName = input.TaskName,
                TaskDescription = input.TaskDescription,
                IsCompleted = input.IsCompleted
            };

            await taskItems.AddAsync(new
            {
                id = taskItem.Id,
                taskItem.TaskName,
                taskItem.TaskDescription,
                taskItem.IsCompleted
            });

            return new OkObjectResult(taskItem);
        }

        [FunctionName("UpdateTaskItem")]
        public static async Task<IActionResult> UpdateTaskItem(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "task/{id}")]HttpRequest req,
            [CosmosDB(ConnectionStringSetting = Constants.CosmosConnectionString)] DocumentClient client,
            ILogger logger,
            string id)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var updatedTask = JsonConvert.DeserializeObject<TaskItemDTO>(requestBody);

            Uri taskCollectionUri = UriFactory.CreateDocumentCollectionUri(Constants.CosmosDBName, Constants.CosmosCollectionName);

            var document = client.CreateDocumentQuery(taskCollectionUri)
                .Where(t => t.Id == id)
                .AsEnumerable()
                .FirstOrDefault();

            if (document == null)
            {
                logger.LogError($"TaskItem {id} not found. It may not exist!");
                return new NotFoundResult();
            }

            document.SetPropertyValue("IsCompleted", updatedTask.IsCompleted);
            if (!string.IsNullOrEmpty(updatedTask.TaskDescription))
            {
                document.SetPropertyValue("TaskDescription", updatedTask.TaskDescription);
            }

            await client.ReplaceDocumentAsync(document);

            TaskItem updatedTaskItemDocument = (dynamic)document;

            return new OkObjectResult(updatedTaskItemDocument);
        }
    }
}

using System;
using System.Collections.Generic;
using ChangeFeedDemo.Functions.Helpers;
using ChangeFeedDemo.Functions.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ChangeFeedDemo.Functions
{
    public static class ChangeFeedListener
    {
        [FunctionName("ChangeFeedListener")]
        public static void Run([CosmosDBTrigger(
            databaseName: Constants.CosmosDBName,
            collectionName: Constants.CosmosCollectionName,
            ConnectionStringSetting = Constants.CosmosConnectionString,
            LeaseCollectionName = "LeaseCollection",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input, ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);
            }
        }
    }
}

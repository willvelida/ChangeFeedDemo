
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChangeFeedDemo.Functions.Models
{
    public class TaskItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("n");
        [JsonProperty(PropertyName = "TaskName")]
        public string TaskName { get; set; }
        [JsonProperty(PropertyName = "TaskDescription")]
        public string TaskDescription { get; set; }
        [JsonProperty(PropertyName = "IsCompleted")]
        public bool IsCompleted { get; set; }
    }
}

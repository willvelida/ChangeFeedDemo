using System;
using System.Collections.Generic;
using System.Text;

namespace ChangeFeedDemo.Functions.Models
{
    public class TaskItemCreateModel
    {
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public bool IsCompleted { get; set; }

    }
}

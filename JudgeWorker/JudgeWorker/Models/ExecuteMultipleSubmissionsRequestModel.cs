using System.Collections.Generic;

namespace JudgeWorker.Models
{
    public class ExecuteMultipleSubmissionsRequestModel
    {
        public IEnumerable<ExecutionRequestModel> Submissions { get; set; }
    }
}

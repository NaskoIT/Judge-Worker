using System.Collections.Generic;

namespace JudgeWorker.Models
{
    public class MultipleSubmissionResultResponseModel
    {
        public IEnumerable<ExecutionResultResponseModel> Submissions { get; set; }
    }
}

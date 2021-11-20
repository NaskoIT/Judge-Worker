namespace JudgeWorker
{
    public class ExecutionRequestModel
    {
        public string Source_Code { get; set; }

        public int Language_Id { get; set; }

        public string StdIn { get; set; }
    }
}

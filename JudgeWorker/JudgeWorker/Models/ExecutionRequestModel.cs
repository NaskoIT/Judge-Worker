using Newtonsoft.Json;

namespace JudgeWorker.Models
{
    public class ExecutionRequestModel
    {
        [JsonProperty("source_code")]
        public string SourceCode { get; set; }

        [JsonProperty("language_id")]
        public int LanguageId { get; set; }

        [JsonProperty("stdin")]
        public string StdIn { get; set; }
    }
}

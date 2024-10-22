﻿using Newtonsoft.Json;

namespace JudgeWorker.Models
{
    public class ExecutionResultResponseModel
    {
        public string StdOut { get; set; }

        public string Time { get; set; }

        public string Memory { get; set; }

        public string StdErr { get; set; }

        [JsonProperty("Compile_Output")]
        public string CompileOutput { get; set; }

        public string Message { get; set; }

        public ExecutionStatusResponseModel Status { get; set; }
    }
}

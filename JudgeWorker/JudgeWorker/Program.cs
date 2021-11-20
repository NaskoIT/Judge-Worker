using JudgeWorker.Common;
using JudgeWorker.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using JudgeWorker.Models;
using System.Threading;
using System.IO;
using System.Linq;

namespace JudgeWorker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string code = File.ReadAllText("../../../source.cpp");
            var inputs = File.ReadAllText("../../../inputs.txt")
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            IHttpClientService httpClient = new HttpClientService(new HttpClient
            {
                BaseAddress = new Uri(GlobalConstants.BaseUrl),
            });

            var runSubmissionTasks = inputs
                .Select(input => RunSubmission(code, input, httpClient))
                .ToList();

            var executionResponses = await Task.WhenAll(runSubmissionTasks);

            Console.WriteLine("The code was submitted!");
            Console.WriteLine("Waitnig to be executed...");

            Thread.Sleep(5000);

            var submissionResults = await Task.WhenAll(executionResponses.Select(r => CheckSubmission(r.Token, httpClient)));

            foreach (var submissionResult in submissionResults)
            {
                if (string.IsNullOrEmpty(submissionResult.StdErr) && !string.IsNullOrEmpty(submissionResult.StdOut))
                {
                    Console.WriteLine("Test ran successfully - output: " + submissionResult.StdOut);
                }
                else
                {
                    Console.WriteLine("Test ran with error: " + submissionResult.StdErr);
                }
            }
        }

        private static async Task<ExecutionResponseModel> RunSubmission(string code, string input, IHttpClientService httpClient)
        {
            var requestModel = new ExecutionRequestModel
            {
                LanguageId = 54,
                SourceCode = code,
                StdIn = input
            };

            return await httpClient.Post<ExecutionResponseModel>(requestModel, "submissions/?base64_encoded=false&wait=false");
        }

        private static async Task<ExecutionResultResponseModel> CheckSubmission(string token, IHttpClientService httpClient) =>
            await httpClient.Get<ExecutionResultResponseModel>($"submissions/{token}?base64_encoded=false");
    }
}

using JudgeWorker.Common;
using JudgeWorker.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using JudgeWorker.Models;
using System.Threading;
using System.IO;
using System.Linq;
using System.Collections.Generic;

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

            //var submissionResults = await RunTestsWithMultipleApiRequests(code, inputs, httpClient);
            var enquedSubmissions = await RunBatchSubmissions(code, inputs, httpClient);
            Thread.Sleep(5000);

            var submissionResultsResponse = await CheckBatchSubmissions(enquedSubmissions, httpClient);
            var submissionResults = submissionResultsResponse.Submissions;

            foreach (var submissionResult in submissionResults)
            {
                if (submissionResult.Status.Description == "Compilation Error")
                {
                    Console.WriteLine("Compilation result: " + submissionResult.CompileOutput?.ToUTF8());
                    break;
                }
                else if (submissionResult.Status.Description == "Accepted")
                {
                    Console.WriteLine("Test ran successfully - output: " + submissionResult.StdOut?.ToUTF8());
                }
                else
                {
                    Console.WriteLine("Test ran with error: " + submissionResult.StdErr?.ToUTF8());
                }
            }
        }

        private static async Task<MultipleSubmissionResultResponseModel> CheckBatchSubmissions(
            IEnumerable<ExecutionResponseModel> submissions,
            IHttpClientService httpClient)
            => await httpClient.Get<MultipleSubmissionResultResponseModel>(
                $"submissions/batch?tokens={string.Join(',', submissions.Select(s => s.Token))}&base64_encoded=true");

        private static async Task<IEnumerable<ExecutionResponseModel>> RunBatchSubmissions(
            string code,
            IEnumerable<string> inputs,
            IHttpClientService httpClient)
        {
            var submisisons = inputs.Select(input => new ExecutionRequestModel
            {
                LanguageId = 54,
                SourceCode = code,
                StdIn = input,
            });

            return await httpClient.Post<IEnumerable<ExecutionResponseModel>>(new ExecuteMultipleSubmissionsRequestModel
            {
                Submissions = submisisons,
            },
            "submissions/batch?base64_encoded=false");
        }

        private static async Task<IEnumerable<ExecutionResultResponseModel>> RunTestsWithMultipleApiRequests(string code, string[] inputs, IHttpClientService httpClient)
        {
            var runSubmissionTasks = inputs
                            .Select(input => RunSubmission(code, input, httpClient))
                            .ToList();

            var executionResponses = await Task.WhenAll(runSubmissionTasks);

            Console.WriteLine("The code was submitted!");
            Console.WriteLine("Waitnig to be executed...");

            Thread.Sleep(5000);

            return await Task.WhenAll(executionResponses.Select(r => CheckSubmission(r.Token, httpClient)));
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
            await httpClient.Get<ExecutionResultResponseModel>($"submissions/{token}?base64_encoded=true");
    }
}

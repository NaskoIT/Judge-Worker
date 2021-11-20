using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JudgeWorker.Services
{
    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient client;

        public HttpClientService(HttpClient client)
        {
            this.client = client;
            this.client.DefaultRequestHeaders.Add(GlobalConstants.HttpHeaders.Accept, GlobalConstants.MimeTypes.ApplicationJson);
        }

        public async Task<TResponse> Post<TResponse>(object model, string url)
        {
            if (model == null)
            {
                throw new ArgumentException("The model cannot be null!");
            }

            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("The model cannot be null!");
            }

            string body = model.ToJson();
            var requestContent = new StringContent(body, Encoding.UTF8, ApplicationJson);

            return await Post<TResponse>(requestContent, url);
        }

        public async Task<TResponse> PostForm<TResponse>(MultipartFormDataContent form, string url)
        {
            if (form == null)
            {
                throw new ArgumentException(FormCannotBeNull);
            }

            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException(UrlCannotBeNull);
            }

            return await Post<TResponse>(form, url);
        }

        public async Task<TResponse> Get<TResponse>(string url)
        {
            HttpResponseMessage responseMessage = await GetResponse(url);
            string content = await responseMessage.Content.ReadAsStringAsync();
            return content.FromJson<TResponse>();
        }

        public async Task<byte[]> Get(string url)
        {
            HttpResponseMessage responseMessage = await GetResponse(url);
            return await responseMessage.Content.ReadAsByteArrayAsync();
        }

        private async Task<HttpResponseMessage> GetResponse(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException(UrlCannotBeNull);
            }

            HttpResponseMessage responseMessage = await client.GetAsync(url);
            await ValidateResponseMessage(responseMessage);

            return responseMessage;
        }

        private async Task<TResponse> Post<TResponse>(HttpContent content, string url)
        {
            HttpResponseMessage responseMessage = await client.PostAsync(url, content);
            await ValidateResponseMessage(responseMessage);
            string result = await responseMessage.Content.ReadAsStringAsync();

            return result.FromJson<TResponse>();
        }

        private async Task ValidateResponseMessage(HttpResponseMessage responseMessage)
        {
            if (!responseMessage.IsSuccessStatusCode)
            {
                string errorMessage = $"{responseMessage.RequestMessage.Method.Method} request to {responseMessage.RequestMessage.RequestUri} failed. " +
                    $"Status code: {responseMessage.StatusCode}.";
                string content = await responseMessage.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(content))
                {
                    errorMessage += Environment.NewLine + content;
                }

                throw new BusinessServiceException(errorMessage);
            }
        }
    }
}

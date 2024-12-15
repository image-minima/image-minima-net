using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageMinima
{
    public class Repository : IDisposable
    {
        public static readonly Uri ApiEndpoint = new Uri("https://api.imageminima.com/api/");
        public static readonly ushort RetryCount = 1;
        public static readonly ushort RetryDelay = 500;

        HttpClient client;

        public Repository(string key)
        {
            client = new HttpClient()
            {
                BaseAddress = ApiEndpoint,
                Timeout = Timeout.InfiniteTimeSpan,
            };

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("api:" + key));
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials);
        }

        public Task<HttpResponseMessage> Request(HttpMethod method, string url, object options)
        {
            if (method == HttpMethod.Get && options != null)
            {
                return Request(method, url);
            }
            else
            {
                var data = new StringContent(JsonConvert.SerializeObject(options), Encoding.UTF8, "application/json");

                return Request(method, url, data);
            }
        }

        public Task<HttpResponseMessage> Request(HttpMethod method, string url, byte[] file, string fileName)
        {
            var body = new MultipartFormDataContent();

            var fileContent = new ByteArrayContent(file);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            body.Add(new StreamContent(new MemoryStream(file)), "file", fileName);

            return Request(method, url, body);
        }

        public Task<HttpResponseMessage> Request(
            HttpMethod method, string url, byte[] file,
            string fileName, byte[] secondFile, string secondFileName,
            Dictionary<string, object> options
        )
        {
            var body = new MultipartFormDataContent();

            var fileContent = new ByteArrayContent(file);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            body.Add(new StreamContent(new MemoryStream(file)), "file", fileName);
            body.Add(new StreamContent(new MemoryStream(secondFile)), "watermarkImage", secondFileName);

            foreach (var option in options)
                body.Add(new StringContent(JsonConvert.SerializeObject(option.Value)), option.Key);

            return Request(method, url, body);
        }

        public Task<HttpResponseMessage> Request(HttpMethod method, string url, byte[] file, string fileName, Dictionary<string, object> options)
        {
            if (method == HttpMethod.Get && options != null)
            {
                return Request(method, url);
            }
            else
            {
                var body = new MultipartFormDataContent();

                foreach (var option in options)
                    body.Add(new StringContent(JsonConvert.SerializeObject(option.Value)));

                body.Add(new StreamContent(new MemoryStream(file)), "file", fileName);

                return Request(method, url, body);
            }
        }

        public async Task<HttpResponseMessage> Request(HttpMethod method, string url, HttpContent body = null)
        {
            for (short retries = (short)RetryCount; retries >= 0; retries--)
            {
                if (retries < RetryCount)
                {
                    await Task.Delay(RetryDelay);
                }

                var request = new HttpRequestMessage(method, url)
                {
                    Content = body
                };

                HttpResponseMessage response;
                try
                {
                    response = await client.SendAsync(request).ConfigureAwait(false);
                }
                catch (OperationCanceledException err)
                {
                    if (retries > 0) continue;
                    throw new ConnectionException("Timeout while connecting", err);
                }
                catch (Exception err)
                {
                    if (err.InnerException != null)
                    {
                        err = err.InnerException;
                    }

                    if (retries > 0) continue;
                    throw new ConnectionException("Error while connecting: " + err.Message, err);
                }

                if (response.Headers.Contains("Compression-Count"))
                {
                    var compressionCount = response.Headers.GetValues("Compression-Count").First();
                    uint parsed;
                    if (uint.TryParse(compressionCount, out parsed))
                    {
                        //ImageMinima.CompressionCount = parsed;
                    }
                }

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }

                var data = new { message = "", error = "" };
                try
                {
                    data = JsonConvert.DeserializeAnonymousType(
                        await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                        data
                    );
                }
                catch (Exception err)
                {
                    data = new
                    {
                        message = "Error while parsing response: " + err.Message,
                        error = "ParseError"
                    };
                }

                if (retries > 0 && (uint)response.StatusCode >= 500) continue;
                throw ImageMinimaException.Create(data.message, data.error, (uint)response.StatusCode);
            }

            return null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (client != null)
                {
                    client.Dispose();
                    client = null;
                }
            }
        }
    }
}

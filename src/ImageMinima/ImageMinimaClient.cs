using ImageMinima.DTO;
using ImageMinima.Results;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace ImageMinima
{
    public interface IImageMinimaClient
    {
        Task<List<Result>> Process(Source source);
    }

    public class ImageMinimaClient : IImageMinimaClient
    {
        private string _Key;

        public string Key
        {
            get
            {
                return _Key;
            }

            set
            {
                _Key = value;
            }
        }

        public async Task<List<Result>> Process(Source source)
        {
            HttpResponseMessage response;

            foreach(var command in source.Commands)
            {
                switch(command.Key)
                {
                    case Commands.SHRINK:
                    {
                        response = await new Repository(Key).Request(HttpMethod.Post, "shrink", source.Buffer).ConfigureAwait(false);

                        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        var data = JsonConvert.DeserializeObject<ShrinkResponse>(body);

                        source.Results.Add(new ShrinkResult(response.Headers, response.Content.Headers, data));
                        break;
                    }
                    case Commands.RESIZE:
                    {
                        response = await new Repository(Key).Request(HttpMethod.Get, "resize", command.Value).ConfigureAwait(false);
                        var body = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

                        source.Results.Add(new Result(response.Headers, response.Content.Headers));
                        break;
                    }
                    case Commands.TO_FILE:

                        Type type = command.Value.GetType();
                        PropertyInfo prop = type.GetProperty("path");
                        object value = prop.GetValue(command.Value, null);

                        using (var file = File.Create((string)value))
                        {
                            var url = ((ShrinkResult)source.Results[source.Results.Count - 1]).ShrinkData.output;
                            response = await new Repository(Key).Request(HttpMethod.Get, url).ConfigureAwait(false);

                            var content = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

                            await file.WriteAsync(content, 0, content.Length).ConfigureAwait(false);
                        };

                        break;
                    default:
                        break;
                }
            }

            return source.Results;
        }
    }
}
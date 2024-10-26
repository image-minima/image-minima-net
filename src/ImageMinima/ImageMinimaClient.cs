using ImageMinima.DTO;
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
                        response = await new Repository(Key).Request(
                            HttpMethod.Post,
                            "shrink",
                            source.FileBuffer,
                            source.FileName
                        ).ConfigureAwait(false);

                        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        var data = JsonConvert.DeserializeObject<ShrinkResponse>(body);

                        var result = new Result(response.Headers, response.Content.Headers);
                        
                        result.Location = data.output;                        

                        source.Results.Add(result);

                        source.Url = new Uri(data.output);

                        break;
                    }
                    case Commands.WATERMARK:
                    {
                        response = await new Repository(Key)
                            .Request(
                                HttpMethod.Post,
                                "watermark",
                                source.FileBuffer,
                                source.FileName,
                                source.WatermarkBuffer,
                                source.WatermarkFileName
                            )
                            .ConfigureAwait(false);

                        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        var data = JsonConvert.DeserializeObject<ShrinkResponse>(body);

                        var result = new Result(response.Headers, response.Content.Headers);
                        
                        result.Location = data.output;                        

                        source.Results.Add(result);

                        source.Url = new Uri(data.output);

                        break;
                    }
                    case Commands.RESIZE:
                    {
                        if (source.Url == null)
                            break;

                        response = await new Repository(Key)
                            .Request(HttpMethod.Post, source.Url.ToString(), command.Value)
                            .ConfigureAwait(false);

                        var body = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

                        source.Results.Add(new Result(response.Headers, response.Content.Headers, body));
                        break;
                    }
                    case Commands.TO_FILE:

                        Type type = command.Value.GetType();
                        PropertyInfo prop = type.GetProperty("path");
                        object value = prop.GetValue(command.Value, null);

                        using (var file = File.Create((string)value))
                        {
                            var lastResult = source.Results[source.Results.Count - 1];

                            var content = lastResult.data;

                            if (content == null)
                            {
                                var url = source.Results[source.Results.Count - 1].Location;
                                response = await new Repository(Key).Request(HttpMethod.Get, url.ToString()).ConfigureAwait(false);

                                content = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                            }

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
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ImageMinima
{
    public interface IImageMinimaClient
    {

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

        public async Task<Source> FromFile(string path)
        {
            using (var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var buffer = new MemoryStream())
            {
                await file.CopyToAsync(buffer).ConfigureAwait(false);
                return await FromBuffer(buffer.ToArray()).ConfigureAwait(false);
            }
        }

        public async Task<Source> FromBuffer(byte[] buffer)
        {
            var response = await new Repository(Key).Request(HttpMethod.Post, "/shrink", buffer).ConfigureAwait(false);
            var location = response.Headers.Location;

            return new Source(location);
        }

        public async Task<Source> FromUrl(string url)
        {
            var body = new Dictionary<string, object>();
            body.Add("source", new { url = url });

            var response = await new Repository(Key).Request(HttpMethod.Post, "/shrink", body).ConfigureAwait(false);
            var location = response.Headers.Location;

            return new Source(location);
        }

        //public Source Resize(this Source source, object options)
        //{
        //    source.commands.Add("resize", options);

        //    return source;
        //}

        //public async Task<ResultMeta> Store(this Source source, object options)
        //{
        //    source.commands.Add("store", options);
        //    var response = await new Repository(Key).Request(HttpMethod.Post, source.url, source.commands).ConfigureAwait(false);

        //    return new ResultMeta(response.Headers);
        //}

        //public async Task<Result> GetResult(this Source source)
        //{
        //    HttpResponseMessage response;
        //    if (source.commands.Count == 0)
        //    {
        //        response = await new Repository(Key).Request(HttpMethod.Get, source.url).ConfigureAwait(false);
        //    }
        //    else
        //    {
        //        response = await new Repository(Key).Request(HttpMethod.Post, source.url, source.commands).ConfigureAwait(false);
        //    }

        //    var body = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        //    return new Result(response.Headers, response.Content.Headers, body);
        //}

        //public async Task ToFile(string path)
        //{
        //    await GetResult().ToFile(path).ConfigureAwait(false);
        //}

        //public async Task<byte[]> ToBuffer()
        //{
        //    return await GetResult().ToBuffer().ConfigureAwait(false);
        //}
    }
}
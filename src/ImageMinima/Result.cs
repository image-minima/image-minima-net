﻿using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ImageMinima
{
    public class Result : ResultMeta
    {
        protected HttpContentHeaders content;
        public byte[] data;

        internal Result(HttpResponseHeaders meta, HttpContentHeaders content) : base(meta)
        {
            this.content = content;
        }
        internal Result(HttpResponseHeaders meta, HttpContentHeaders content, byte[] data) : base(meta)
        {
            this.content = content;
            this.data = data;
        }

        public async Task ToFile(string path)
        {
            using (var file = File.Create(path))
            {
                await file.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
            };
        }

        public byte[] ToBuffer()
        {
            return data;
        }

        public ulong? Size
        {
            get
            {
                return (ulong?)this.content.ContentLength;
            }
        }

        public string MediaType
        {
            get
            {
                var header = this.content.ContentType;
                if (header == null)
                {
                    return null;
                }

                return header.MediaType;
            }
        }

        public string ContentType
        {
            get
            {
                return MediaType;
            }
        }
    }
}

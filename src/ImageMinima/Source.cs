using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ImageMinima
{
    public class Source
    {
        public Uri Url { get; set; }
        public Dictionary<string, object> Commands { get; set; }

        public byte[] Buffer { get; set; }

        public List<Result> Results { get; set; }

        public Source(byte[] buffer)
        {
            Commands = new Dictionary<string, object>();
            Buffer = new byte[buffer.Length];
            buffer.CopyTo(this.Buffer, 0);
            Results = new List<Result>();
        }

        public static async Task<Source> FromFile(string path)
        {
            using (var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var buffer = new MemoryStream())
            {
                await file.CopyToAsync(buffer).ConfigureAwait(false);
                return FromBuffer(buffer.ToArray());
            }
        }

        public static Source FromBuffer(byte[] buffer)
        {
            return new Source(buffer);
        }

        public void ToFile(string path)
        {
            Commands.Add(ImageMinima.Commands.TO_FILE, new { path = path});
        }
    }
}

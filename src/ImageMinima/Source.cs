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

        public byte[] FileBuffer { get; set; }

        public byte[] WatermarkBuffer { get; set; }

        public List<Result> Results { get; set; }

        public string FileName { get; set; }

        public string WatermarkFileName { get; set; }

        public Source(byte[] buffer)
        {
            Commands = new Dictionary<string, object>();
            Results = new List<Result>();

            FileBuffer = new byte[buffer.Length];
            buffer.CopyTo(FileBuffer, 0);
        }

        public Source(byte[] buffer, byte[] watermarkBuffer)
        {
            Commands = new Dictionary<string, object>();
            Results = new List<Result>();

            FileBuffer = new byte[buffer.Length];
            buffer.CopyTo(FileBuffer, 0);

            WatermarkBuffer = new byte[watermarkBuffer.Length];
            watermarkBuffer.CopyTo(WatermarkBuffer, 0);
        }

        public static async Task<Source> FromFile(string path)
        {
            using (var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var buffer = new MemoryStream())
            {
                await file.CopyToAsync(buffer).ConfigureAwait(false);
                return FromBuffer(buffer.ToArray(), file.Name);
            }
        }

        public static async Task<Source> FromFile(string path, string waterfallPath)
        {
            using (var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var watermarkFile = File.Open(waterfallPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var watermarkBuffer = new MemoryStream())
                    {
                        using (var buffer = new MemoryStream())
                        {
                            await file.CopyToAsync(buffer).ConfigureAwait(false);
                            await watermarkFile.CopyToAsync(watermarkBuffer).ConfigureAwait(false);

                            return FromBuffer(
                                buffer.ToArray(),
                                Path.GetFileName(path),
                                watermarkBuffer.ToArray(),
                                Path.GetFileName(waterfallPath)
                            );
                        }
                    }
                }
            }
        }

        public static Source FromBuffer(byte[] buffer, string fileName)
        {
            var source = new Source(buffer);
            source.FileName = fileName;
            return source;
        }

        public static Source FromBuffer(byte[] buffer, string fileName, byte[] watermarkBuffer, string watermarkFileName)
        {
            var source = new Source(buffer, watermarkBuffer);
            source.FileName = fileName;
            source.WatermarkFileName = watermarkFileName;

            return source;
        }

        public void ToFile(string path)
        {
            Commands.Add(ImageMinima.Commands.TO_FILE, new { path });
        }
    }
}

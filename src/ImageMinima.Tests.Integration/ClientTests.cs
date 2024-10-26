using ImageMinima.DTO.Commands;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ImageMinima.Tests.Integration
{
    public class ClientTests
    {
        public ClientTests()
        {
            Environment.SetEnvironmentVariable("IMAGE_MINIMA_TOKEN", "teste123");

        }

        sealed class TempFile : IDisposable
        {
            private string path;

            public string Path
            {
                get { return path; }
            }

            public TempFile()
            {
                path = System.IO.Path.GetTempFileName();
            }

            ~TempFile()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
            }

            private void Dispose(bool disposing)
            {
                if (disposing) GC.SuppressFinalize(this);
                try { File.Delete(path); } catch { }
                path = null;
            }
        }

        [Fact]
        public async Task ShouldCompressFile()
        {
            var client = Helper.GetAuthenticatedClient();

            var unoptimizedPath = AppContext.BaseDirectory + "../../../assets/sample.jpg";

            var optimized = await Source.FromFile(unoptimizedPath);

            using (var file = new TempFile())
            {
                var shrinkOptions = new { };

                optimized
                    .Shrink(shrinkOptions)
                    .ToFile(file.Path);

                await client.Process(optimized);

                var size = new FileInfo(file.Path).Length;

                Assert.True(size > 706000);
                Assert.True(size < 707000);
            }
        }

        [Fact]
        public async Task ShouldCompressFileAndFillSourceUrl()
        {
            var client = Helper.GetAuthenticatedClient();

            var unoptimizedPath = AppContext.BaseDirectory + "../../../assets/sample.jpg";

            var optimized = await Source.FromFile(unoptimizedPath);

            using (var file = new TempFile())
            {
                var resizeOptions = new { width = 100, height = 60 };
                var shrinkOptions = new { };

                optimized
                    .Shrink(shrinkOptions)
                    .ToFile(file.Path);

                await client.Process(optimized);

                Assert.False(string.IsNullOrEmpty(optimized.Url.ToString()));
            }
        }

        [Fact]
        public async Task ShouldCompressAndResizeFile()
        {
            var client = Helper.GetAuthenticatedClient();

            var unoptimizedPath = AppContext.BaseDirectory + "../../../assets/sample.jpg";

            var optimized = await Source.FromFile(unoptimizedPath);

            using (var file = new TempFile())
            {
                var resizeOptions = new ResizeRequest() { Width = 100, Height = 60 };
                var shrinkOptions = new { };

                optimized
                    .Shrink(shrinkOptions)
                    .Resize(resizeOptions)
                    .ToFile(file.Path);

                await client.Process(optimized);

                var size = new FileInfo(file.Path).Length;

                Assert.True(size > 1140);
                Assert.True(size < 1150);
            }
        }

        [Fact]
        public async Task ShouldAddWatermarkToAFile()
        {
            var client = Helper.GetAuthenticatedClient();

            var unoptimizedPath = AppContext.BaseDirectory + "../../../assets/sample.jpg";
            var watemarkPath = AppContext.BaseDirectory + "../../../assets/watermark.jpg";

            var sourceFile = await Source.FromFile(unoptimizedPath, watemarkPath);

            using (var file = new TempFile())
            {
                var resizeOptions = new ResizeRequest() { Width = 100, Height = 60 };
                var watermarkOptions = new { };

                sourceFile
                    .Watermark(watermarkOptions)
                    .ToFile(file.Path);

                await client.Process(sourceFile);

                var size = new FileInfo(file.Path).Length;

                Console.WriteLine(size);

                Assert.True(size == 1075624);
            }
        }
    }
}

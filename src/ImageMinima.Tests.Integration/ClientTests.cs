using ImageMinima.Tests.Integration.Helpers;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ImageMinima.Tests.Integration
{
    public class ClientTests
    {

        [IntegrationTest]
        public async Task ShouldCompressFile()
        {
            //var imageMinimaClient = Helper.GetAuthenticatedClient();

            //var unoptimizedPath = AppContext.BaseDirectory + "/assets/voormedia.png";

            //var optimized = imageMinimaClient.FromFile(unoptimizedPath);

            //using (var file = new TempFile())
            //{
            //    optimized.ToFile(file.Path).Wait();

            //    var size = new FileInfo(file.Path).Length;
            //    var contents = File.ReadAllBytes(file.Path);

            //    Assert.Greater(size, 1000);
            //    Assert.Less(size, 1500);

            //    /* width == 137 */
            //    CollectionAssert.IsSubsetOf(new byte[] { 0, 0, 0, 0x89 }, contents);
            //    CollectionAssert.IsNotSubsetOf(
            //        Encoding.ASCII.GetBytes("Copyright Voormedia"),
            //        contents
            //    );
            //}
        }
    }
}

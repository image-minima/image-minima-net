using System;

namespace ImageMinima.Tests.Integration
{
    internal class Helper
    {
        static readonly Lazy<string> TestKey = new Lazy<string>(() =>
        {
            var token = Environment.GetEnvironmentVariable("IMAGE_MINIMA_TOKEN");

            if (token != null)
                return token;

            return "";
        });

        public static IImageMinimaClient GetAuthenticatedClient()
        {
            return new ImageMinimaClient()
            {
                Key = TestKey.Value
            };
        }
    }
}

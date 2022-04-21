using ImageMinima.DTO;
using System.Net.Http.Headers;

namespace ImageMinima.Results
{
    public class ShrinkResult : Result
    {
        public ShrinkResponse ShrinkData { get; set; }

        public ShrinkResult(HttpResponseHeaders meta, HttpContentHeaders content, ShrinkResponse data) : base(meta, content)
        {
            ShrinkData = data;
        }
    }
}

using System.Collections.Generic;
using ImageMinima.DTO.Commands;

namespace ImageMinima
{
    public static class SourceExtensions
    {
        public static Source Resize(this Source source, ResizeRequest options)
        {
           source.Commands.Add(Commands.RESIZE, options);

           return source;
        }

        public static Source Shrink(this Source source, object options)
        {
           source.Commands.Add(Commands.SHRINK, options);

           return source;
        }

         public static Source Watermark(this Source source, WatermarkRequest options)
        {
           source.Commands.Add(Commands.WATERMARK, new Dictionary<string, object>(){
               {
                  "markRatio", options.markRatio
               },
               {
                  "transparency", options.transparency
               }
           });
           return source;
        }
    }
}
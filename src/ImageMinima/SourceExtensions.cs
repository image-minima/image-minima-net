namespace ImageMinima
{
    public static class SourceExtensions
    {
        public static Source Resize(this Source source, object options)
        {
           source.Commands.Add(Commands.RESIZE, options);

           return source;
        }

        public static Source Shrink(this Source source, object options)
        {
           source.Commands.Add(Commands.SHRINK, options);

           return source;
        }
    }
}
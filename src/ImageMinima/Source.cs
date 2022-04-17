using System;
using System.Collections.Generic;

namespace ImageMinima
{
    public class Source
    {
        public Uri url { get; set; }
        public Dictionary<string, object> commands { get; set; }

        internal Source(Uri url, Dictionary<string, object> commands = null)
        {
            this.url = url;
            if (commands == null) commands = new Dictionary<string, object>();
            this.commands = commands;
        }
    }
}

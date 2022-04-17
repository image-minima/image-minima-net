using System;
using System.Collections.Generic;
using System.Text;

namespace ImageMinima
{
    public class ImageMinimaException : System.Exception
    {
        internal static ImageMinimaException Create(string message, string type, uint status)
        {
            if (status == 401 || status == 429)
            {
                return new AccountException(message, type, status);
            }
            else if (status >= 400 && status <= 499)
            {
                return new ClientException(message, type, status);
            }
            else if (status >= 500 && status <= 599)
            {
                return new ServerException(message, type, status);
            }
            else
            {
                return new ImageMinimaException(message, type, status);
            }
        }

        public uint Status = 0;

        internal ImageMinimaException() : base() { }

        internal ImageMinimaException(string message, System.Exception err = null) : base(message, err) { }

        internal ImageMinimaException(string message, string type, uint status) :
            base(message + " (HTTP " + status + "/" + type + ")")
        {
            this.Status = status;
        }
    }

    public class AccountException : ImageMinimaException
    {
        internal AccountException() : base() { }

        internal AccountException(string message, System.Exception err = null) : base(message, err) { }

        internal AccountException(string message, string type, uint status) : base(message, type, status) { }
    }

    public class ClientException : ImageMinimaException
    {
        internal ClientException() : base() { }

        internal ClientException(string message, System.Exception err = null) : base(message, err) { }

        internal ClientException(string message, string type, uint status) : base(message, type, status) { }
    }

    public class ServerException : ImageMinimaException
    {
        internal ServerException() : base() { }

        internal ServerException(string message, System.Exception err = null) : base(message, err) { }

        internal ServerException(string message, string type, uint status) : base(message, type, status) { }
    }

    public class ConnectionException : ImageMinimaException
    {
        internal ConnectionException() : base() { }

        internal ConnectionException(string message, System.Exception err = null) : base(message, err) { }

        internal ConnectionException(string message, string type, uint status) : base(message, type, status) { }
    }
}

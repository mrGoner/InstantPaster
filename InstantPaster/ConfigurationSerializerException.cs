using System;

namespace InstantPaster
{
    public class ConfigurationSerializerException : Exception
    {
        public ConfigurationSerializerException(string _message) : base(_message)
        {
        }

        public ConfigurationSerializerException(string _message, Exception _innerException) : base(_message, _innerException)
        {
        }
    }
}
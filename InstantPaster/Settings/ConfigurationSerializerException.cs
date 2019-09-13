using System;

namespace InstantPaster.Settings
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
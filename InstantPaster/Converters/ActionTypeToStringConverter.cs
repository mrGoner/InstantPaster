using System;
using System.Globalization;
using System.Windows.Data;
using InstantPaster.Properties;
using InstantPaster.Settings;

namespace InstantPaster.Converters
{
    public class ActionTypesToLocalizedCollectionConverter : IValueConverter
    {
        public object Convert(object _value, Type _targetType, object _parameter, CultureInfo _culture)
        {
            if (_value is ActionType[])
            {
                return new[]
                {
                    Resources.ActionTypeInsertTextString,
                    Resources.ActionTypeExecuteProcessString
                };
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object _value, Type _targetType, object _parameter, CultureInfo _culture)
        {
            return Binding.DoNothing;
        }
    }

    public class ActionTypeToStringConverter : IValueConverter
    {
        public object Convert(object _value, Type _targetType, object _parameter, CultureInfo _culture)
        {
            if (_value is ActionType action)
            {
                switch (action)
                {
                    case ActionType.InsertText:
                        return Resources.ActionTypeInsertTextString;
                    case ActionType.ExecutePath:
                        return Resources.ActionTypeExecuteProcessString;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object _value, Type _targetType, object _parameter, CultureInfo _culture)
        {
            if (_value is string strValue)
            {
                if (strValue == Resources.ActionTypeInsertTextString)
                    return ActionType.InsertText;
                if (strValue == Resources.ActionTypeExecuteProcessString)
                    return ActionType.ExecutePath;
            }

            return Binding.DoNothing;
        }
    }
}

using Microsoft.UI.Xaml.Data;
using System;
using Extension_Packager_Library.src.DataModels;

namespace Extension_Packager.src.Converter
{
    internal class ExtToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Extension ext)
            {
                return $"{ext.Name}, Version {ext.Version}";
            }

            throw new ArgumentException("Wrong data type");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

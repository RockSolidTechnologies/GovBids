using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace GovBids.Classes
{
    /// <summary>
    /// Use for marking bids as read or unread
    /// </summary>
    public class OpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            Boolean result = true;

            if (parameter != null) Boolean.TryParse(parameter.ToString(), out result);

            if (value != null && value is Boolean)
            {
                if (System.Convert.ToBoolean(value) == result)
                {
                    return 0.4f;
                }
                else
                {
                    return 1;
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Use for marking bids as read or unread
    /// </summary>
    public class TextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            Boolean result = true;

            if (parameter != null) Boolean.TryParse(parameter.ToString(), out result);

            if (value != null && value is Boolean)
            {
                if (System.Convert.ToBoolean(value) == result)
                {
                    return "Marcar como no leído";
                }
                else
                {
                    return "Marcar como leído";
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //favs
    public class FavTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            Boolean result = true;

            if (parameter != null) Boolean.TryParse(parameter.ToString(), out result);

            if (value != null && value is Boolean)
            {
                if (System.Convert.ToBoolean(value) != result)
                {
                    return "Añadir a mis favoritos";
                }
                else
                {
                    return "Remover de mis favoritos";
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

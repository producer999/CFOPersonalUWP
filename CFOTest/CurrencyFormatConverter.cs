using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace CFOTest
{
    public class CurrencyFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value != null)
            {
                int i = (int)value;
                double d = (double)i / 100;
                return d.ToString("C");
            }
            
            return "$0.00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null)
                {
                    string s = value as string;
                    string result = s.Replace("$", "").Replace(",", "").Replace(".", "");

                    return System.Convert.ToInt32(result);
                }

                throw new NullReferenceException();
            }
            catch
            {
                Debug.WriteLine("ERROR: Problem converting back from currency String to Int32.");
                return 0;
            }        
        }
    }

    public class DoubleCurrencyFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                double d = (double)value;
                return d.ToString("C");
            }

            return "$0.00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return 0;
        }
    }
}

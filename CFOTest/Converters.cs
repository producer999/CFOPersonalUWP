using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace CFOTest
{
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !((bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return !((bool)value);
        }
    }

    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value != null)
            {
                int v = (int)value;

                if (v > 0)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return !((bool)value);
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value != null)
            {
                return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }

    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((bool)value) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }

    public class InverseIntBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool flag;

            if((int)value == 0)
            {
                flag = true;
            }
            else
            {
                IntToBoolConverter converter = new IntToBoolConverter();
                flag = (bool)converter.Convert((int)value, null, null, null);
            }

            return (flag) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }

    public class BoolToItalicConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((bool)value) ? FontStyle.Italic : FontStyle.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }

    public class BoolToForegroundColorDarkGrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((bool)value) ? new SolidColorBrush(Colors.DarkGray) : new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }

    public class BoolToForegroundColorSeaGreenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((bool)value) ? new SolidColorBrush(Colors.LightSeaGreen) : new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }

    public class BoolToForegroundColorYellowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((bool)value) ? new SolidColorBrush(Colors.Yellow) : new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }

    public class InverseBoolToForegroundColorRedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !((bool)value) ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }

    public class BoolToBackgroundColorBlackConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((bool)value) ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }

    public class BoolToBackgroundColorMediumBlueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            HexStringToColorConverter hexConvert = new HexStringToColorConverter();

            return ((bool)value) ? hexConvert.Convert("#053b91", null, null, null) : new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }

    public class BoolToBorderThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((bool)value) ? new Thickness(1.0) : new Thickness(0.0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }

    public class BoolToEditButtonGlyphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((bool)value) ? "\xE74E" : "\xE70F";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }

    public class HexStringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string hex = value as string;

            hex = hex.Replace("#", String.Empty);

            byte a = 255;
            byte r = (byte)(System.Convert.ToUInt32(hex.Substring(0, 2), 16));
            byte g = (byte)(System.Convert.ToUInt32(hex.Substring(2, 2), 16));
            byte b = (byte)(System.Convert.ToUInt32(hex.Substring(4, 2), 16));

            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }

    public class DateFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value != null)
            {
                return ((DateTime)value).ToString(@"MM\/dd\/yyyy");
            }
            return DateTime.Now.ToString(@"MM\/dd\/yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }

    public class ShortDateFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                DateTime date = (DateTime)value;
                if(date.Year > 2000)
                {
                    return (date.ToString(@"M\/dd"));
                }
                else
                {
                    return "N/A";
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }

    public class DateOffsetFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                DateTimeOffset dto = (DateTimeOffset)value;
                DateTime date = dto.Date;
                return date.ToString(@"MM\/dd\/yyyy");
            }
            return DateTimeOffset.Now.ToString(@"MM\/dd\/yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }

    public class DateOffsetTrialDaysFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                DateTimeOffset dto = (DateTimeOffset)value;
                DateTime date = dto.Date;

                int daysRemaining = (date - DateTime.Now).Days;

                if(daysRemaining >= 0)
                {
                    return daysRemaining + " Days Left";
                }
                else
                {
                    return "Expired";
                }
            }
            return DateTimeOffset.Now.ToString(@"MM\/dd\/yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }

    public class InverseDateOffsetFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                DateTime date = (DateTime)value;
                
                return new DateTimeOffset(date);
            }
            return DateTimeOffset.Now;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if(value != null)
            {
                DateTimeOffset date = (DateTimeOffset)value;

                return new DateTime(date.Year, date.Month, date.Day);
            }

            return DateTime.Now;
        }
    }

    public class MonthYearFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string month;

            switch (((DateTime)value).Month)
            {
                case 1:
                    month = "January";
                    break;
                case 2:
                    month = "February";
                    break;
                case 3:
                    month = "March";
                    break;
                case 4:
                    month = "April";
                    break;
                case 5:
                    month = "May";
                    break;
                case 6:
                    month = "June";
                    break;
                case 7:
                    month = "July";
                    break;
                case 8:
                    month = "August";
                    break;
                case 9:
                    month = "September";
                    break;
                case 10:
                    month = "October";
                    break;
                case 11:
                    month = "November";
                    break;
                case 12:
                    month = "December";
                    break;
                default:
                    month = " ";
                    break;
            }

            return month + " " + ((DateTime)value).Year;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }

    public class PathToUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string path = value as string;

            if (!String.IsNullOrWhiteSpace(path))
            {
                Uri uri = new Uri(path);
                return uri;
            }
            else
            {
                return new Uri(SettingsHelper.ReceiptImagePlaceholderPath);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }

    public class PathToImageConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            if(value != null)
            {
                string path = value as string;
                return new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(path, UriKind.Absolute));
            }
            return new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/receiptblur3.jpg"));
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }

    public class ColorListToColorModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is List<Brush>)
            {
                Syncfusion.UI.Xaml.Charts.ChartColorModel model = new Syncfusion.UI.Xaml.Charts.ChartColorModel();
                model.CustomBrushes = value as List<Brush>;
                return model;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }
}

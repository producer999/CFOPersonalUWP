using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Windows.UI.Xaml.Data;
using System.ComponentModel;

namespace CFOTest
{
    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Enum e = value as Enum;

            Type enumType = e.GetType();
            MemberInfo memberInfo = enumType.GetMember(e.ToString()).First();
            var descriptionAttribute = memberInfo.GetCustomAttribute<DescriptionAttribute>();

            if(descriptionAttribute != null)
            {
                return descriptionAttribute.Description;
            }

            return e.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Zhoubin.Infrastructure.Common.Tools;

namespace Zhoubin.Infrastructure.Common.Extent
{
    public static class EnumExtent
    {
        public static string ToDescription(this Enum value)
        {
            if (value == null)
            {
                return "";
            }
            return EnumHelper.GetEnumDescriptionDictionary(value.GetType(), value);
        }
    }
}

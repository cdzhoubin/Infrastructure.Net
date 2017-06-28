using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Management.Instrumentation;

namespace Zhoubin.Infrastructure.Common.Tools
{
    /// <summary>
    /// 用于读取枚举字典信息
    /// </summary>
    public static class EnumHelper
    {
        static readonly Dictionary<Type, Dictionary<int, string>> EnumDictionary = new Dictionary<Type, Dictionary<int, string>>();

        /// <summary>
        /// 获取枚举字典数据
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="defaultValue">设置默认default值</param>
        /// <returns>返回枚举字典数据</returns>
        public static Dictionary<int, string> GetEnumDictionary(Type enumType,string defaultValue = null)
        {
            if (!EnumDictionary.ContainsKey(enumType))
            {
                lock (typeof(EnumHelper))
                {
                    if (!EnumDictionary.ContainsKey(enumType))
                    {
                        var dic = new Dictionary<int, string>();
                        if (!string.IsNullOrEmpty(defaultValue))
                        {
                            dic.Add(-1,defaultValue);
                        }

                        var fields = enumType.GetFields();

                        foreach (var field in fields)
                        {
                            if (field.IsSpecialName)
                            {
                                continue;
                            }

                            var enumAttributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                            var str = enumAttributes.Length > 0 ? enumAttributes[0].Description : field.Name;

                            dic.Add(((int)Enum.Parse(enumType, field.Name)), str);
                        }

                        

                        EnumDictionary.Add(enumType, dic);
                    }
                }
            }

            return EnumDictionary[enumType];
        }

        /// <summary>
        /// 获取枚举字典数据
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回枚举字典数据</returns>
        /// <exception cref="InstrumentationException">当输入类型不是一个枚举时，抛出此异常</exception>
        public static Dictionary<int, string> GetEnumDictionary<T>(string defaultValue = null)
        {
            Type enumType = typeof (T);
            if (enumType.IsEnum)
            {
                throw new InstrumentationException("类型T，只支持Enum。");
            }

            return GetEnumDictionary(enumType,defaultValue);
        }
    }
}

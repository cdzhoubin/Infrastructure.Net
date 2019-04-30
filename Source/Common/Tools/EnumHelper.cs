using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Zhoubin.Infrastructure.Common.Tools
{
    /// <summary>
    /// 用于读取枚举字典信息
    /// </summary>
    public static class EnumHelper
    {
        static readonly Dictionary<Type, ReadOnlyDictionary<int, string>> EnumDictionary = new Dictionary<Type, ReadOnlyDictionary<int, string>>();

        static readonly Dictionary<Type, ReadOnlyDictionary<Enum, string>> EnumDescriptionDictionary = new Dictionary<Type, ReadOnlyDictionary<Enum, string>>();

        static readonly Hashtable EnumDescriptionLock = new Hashtable();
        static readonly Hashtable EnumLock = new Hashtable();
        /// <summary>
        /// 获取枚举字典数据
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="val">设置默认default值</param>
        /// <returns>返回枚举字典数据</returns>
        public static string GetEnumDescriptionDictionary(Type enumType,Enum val)
        {
            if (!EnumDescriptionDictionary.ContainsKey(enumType))
            {
                lock (EnumDescriptionLock.SyncRoot)
                {
                    if (!EnumDescriptionDictionary.ContainsKey(enumType))
                    {
                        Dictionary<Enum, string> dic = GetEnumDircotry<Enum>(enumType);

                        EnumDescriptionDictionary.Add(enumType, new ReadOnlyDictionary<Enum, string>(dic));
                    }
                }
            }

            return EnumDescriptionDictionary[enumType][val];
        }

        /// <summary>
        /// 获取枚举字典数据
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="val">设置默认default值</param>
        /// <returns>返回枚举字典数据</returns>
        public static string GetEnumDescriptionDictionary<T>(Enum val)
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new InfrastructureException("类型T，只支持Enum。");
            }
            return GetEnumDescriptionDictionary(enumType,val);
        }

        private static Dictionary<T, string> GetEnumDircotry<T>(Type enumType)
        {
            var dic = new Dictionary<T, string>();

            var fields = enumType.GetFields();

            foreach (var field in fields)
            {
                if (field.IsSpecialName)
                {
                    continue;
                }

                var enumAttributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                var str = enumAttributes.Length > 0 ? enumAttributes[0].Description : field.Name;

                dic.Add(((T)Enum.Parse(enumType, field.Name)), str);
            }

            return dic;
        }

        /// <summary>
        /// 获取枚举字典数据
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="defaultValue">设置默认default值</param>
        /// <returns>返回枚举字典数据</returns>
        public static ReadOnlyDictionary<int, string> GetEnumDictionary(Type enumType,string defaultValue = null)
        {
            if (!EnumDictionary.ContainsKey(enumType))
            {
                lock (EnumLock)
                {
                    if (!EnumDictionary.ContainsKey(enumType))
                    {
                        var dic2 = GetEnumDircotry<int>(enumType); 
                        EnumDictionary.Add(enumType, new ReadOnlyDictionary<int, string>(dic2));
                    }
                }
            }

            var dic = EnumDictionary[enumType];

            if (!string.IsNullOrEmpty(defaultValue))
            {
                var dic1 = new Dictionary<int, string>();
                dic1.Add(-1, defaultValue);
                foreach (var key in dic)
                {
                    dic1.Add(key.Key, key.Value);
                }
                dic = new ReadOnlyDictionary<int, string>(dic1);
            }
            return dic;
        }

        /// <summary>
        /// 获取枚举字典数据
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回枚举字典数据</returns>
        /// <exception cref="InstrumentationException">当输入类型不是一个枚举时，抛出此异常</exception>
        public static ReadOnlyDictionary<int, string> GetEnumDictionary<T>(string defaultValue = null)
        {
            Type enumType = typeof (T);
            if (!enumType.IsEnum)
            {
                throw new InfrastructureException("类型T，只支持Enum。");
            }

            return GetEnumDictionary(enumType,defaultValue);
        }
    }
}

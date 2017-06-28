using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Zhoubin.Infrastructure.Common.Extent
{
    /// <summary>
    /// 对象扩展方法
    /// </summary>
    public static class ObjectExtent
    {
    //    /// <summary>
    //    /// 比较两个对象是否相等
    //    /// </summary>
    //    /// <typeparam name="T">类型</typeparam>
    //    /// <param name="objectFromCompare">源对象</param>
    //    /// <param name="objectToCompare">目标对象</param>
    //    /// <returns>相等返回：true,其它返回：false</returns>
        //public static object CompareEquals<T>(this T objectFromCompare, T objectToCompare)
        //{
        //    if (objectFromCompare == null && objectToCompare == null)
        //        return true;

        //    else if (objectFromCompare == null && objectToCompare != null)
        //        return false;

        //    else if (objectFromCompare != null && objectToCompare == null)
        //        return false;

        //    PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        //    foreach (PropertyInfo prop in props)
        //    {
        //        object dataFromCompare =
        //        objectFromCompare.GetType().GetProperty(prop.Name).GetValue(objectFromCompare, null);

        //        object dataToCompare =
        //        objectToCompare.GetType().GetProperty(prop.Name).GetValue(objectToCompare, null);

        //        Type type =
        //        objectFromCompare.GetType().GetProperty(prop.Name).GetValue(objectToCompare, null).GetType();

        //        if (prop.PropertyType.IsClass &&
        //        !prop.PropertyType.FullName.Contains("System.String"))
        //        {
        //            dynamic convertedFromValue = Convert.ChangeType(dataFromCompare, type);
        //            dynamic convertedToValue = Convert.ChangeType(dataToCompare, type);

        //            object result = ObjectExtent.CompareEquals(convertedFromValue, convertedToValue);

        //            bool compareResult = (bool)result;
        //            if (!compareResult)
        //                return false;
        //        }

        //        else if (!dataFromCompare.Equals(dataToCompare))
        //            return false;
        //    }

        //    return true;
        //}
        /// <summary>
        /// 比较两个对象是否相等
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="objectFromCompare">源对象</param>
        /// <param name="objectToCompare">目标对象</param>
        /// <returns>相等返回：true,其它返回：false</returns>
        public static bool CompareEquals<T>(this T objectFromCompare, T objectToCompare)
        {
            bool result = (objectFromCompare == null && objectToCompare == null);

            if (!result)
            {
                try
                {
                    Type fromType = objectFromCompare.GetType();
                    if (fromType.IsPrimitive)
                    {
                        result = objectFromCompare.Equals(objectToCompare);
                    }

                    else if (fromType.FullName.Contains("System.String"))
                    {
                        result = ((objectFromCompare as string) == (objectToCompare as string));
                    }

                    else if (fromType.FullName.Contains("DateTime"))
                    {
                        result = (DateTime.Parse(objectFromCompare.ToString()).Ticks == DateTime.Parse(objectToCompare.ToString()).Ticks);
                    }

                    // stringbuilder handling here is optional, but doing it this way cuts down
                    // on reursive calls to this method
                    else if (fromType.FullName.Contains("System.Text.StringBuilder"))
                    {
                        result = ((objectFromCompare as StringBuilder).ToString() == (objectToCompare as StringBuilder).ToString());
                    }

                    else if (fromType.FullName.Contains("System.Collections.Generic.Dictionary"))
                    {

                        PropertyInfo countProp = fromType.GetProperty("Count");
                        PropertyInfo keysProp = fromType.GetProperty("Keys");
                        PropertyInfo valuesProp = fromType.GetProperty("Values");
                        int fromCount = (int)countProp.GetValue(objectFromCompare, null);
                        int toCount = (int)countProp.GetValue(objectToCompare, null);

                        result = (fromCount == toCount);
                        if (result && fromCount > 0)
                        {
                            var fromKeys = keysProp.GetValue(objectFromCompare, null);
                            var toKeys = keysProp.GetValue(objectToCompare, null);
                            result = CompareEquals(fromKeys, toKeys);
                            if (result)
                            {
                                var fromValues = valuesProp.GetValue(objectFromCompare, null);
                                var toValues = valuesProp.GetValue(objectToCompare, null);
                                result = CompareEquals(fromValues, toValues);
                            }
                        }
                    }

                    // collections presented a unique problem in that the original code always returned
                    // false when they're encountered. The following code was tested with generic
                    // lists (of both primitive types and complex classes). I see no reason why an
                    // ObservableCollection shouldn't also work here (unless the properties or
                    // methods already used are not appropriate).
                    else if (fromType.IsGenericType || fromType.IsArray)
                    {
                        string propName = (fromType.IsGenericType) ? "Count" : "Length";
                        string methName = (fromType.IsGenericType) ? "get_Item" : "Get";
                        PropertyInfo propInfo = fromType.GetProperty(propName);
                        MethodInfo methInfo = fromType.GetMethod(methName);
                        if (propInfo != null && methInfo != null)
                        {
                            int fromCount = (int)propInfo.GetValue(objectFromCompare, null);
                            int toCount = (int)propInfo.GetValue(objectToCompare, null);
                            result = (fromCount == toCount);
                            if (result && fromCount > 0)
                            {
                                for (int index = 0; index < fromCount; index++)
                                {
                                    // Get an instance of the item in the list object 
                                    object fromItem = methInfo.Invoke(objectFromCompare, new object[] { index });
                                    object toItem = methInfo.Invoke(objectToCompare, new object[] { index });
                                    result = CompareEquals(fromItem, toItem);
                                    if (!result)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        PropertyInfo[] props = fromType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                        foreach (PropertyInfo prop in props)
                        {
                            Type type = fromType.GetProperty(prop.Name).GetValue(objectToCompare, null).GetType();
                            object dataFromCompare = fromType.GetProperty(prop.Name).GetValue(objectFromCompare, null);
                            object dataToCompare = fromType.GetProperty(prop.Name).GetValue(objectToCompare, null);
                            result = CompareEquals(Convert.ChangeType(dataFromCompare, type), Convert.ChangeType(dataToCompare, type));
                            // no point in continuing beyond the first property that isn't equal.
                            if (!result)
                            {
                                break;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            return result;

        }
        /// <summary>
        /// 比较两个对象一个属性是否相等
        /// 来源：http://www.codeproject.com/Tips/1046498/Comparing-Two-Complex-or-primitive-objects-of-same
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="objectFromCompare">源对象</param>
        /// <param name="objectToCompare">目标对象</param>
        /// <param name="propertyName">属性列表</param>
        /// <returns>相等返回true,其他返回false</returns>
        public static bool CompareEquals<T>(this T objectFromCompare, T objectToCompare, string propertyName)
        {
            bool result = (objectFromCompare == null && objectToCompare == null);
            if (!result)
            {
                try
                {
                    Type fromType = objectFromCompare.GetType();
                    PropertyInfo prop = fromType.GetProperty(propertyName);
                    if (prop != null)
                    {
                        Type type = prop.GetValue(objectToCompare, null).GetType();
                        object dataFromCompare = prop.GetValue(objectFromCompare, null);
                        object dataToCompare = prop.GetValue(objectToCompare, null);
                        result = CompareEquals(Convert.ChangeType(dataFromCompare, type), Convert.ChangeType(dataToCompare, type));
                    }
                }
                catch (Exception)
                {
                }
            }
            return result;
        }
        /// <summary>
        /// 比较两个对象一组属性是否相等
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="objectFromCompare">源对象</param>
        /// <param name="objectToCompare">目标对象</param>
        /// <param name="propertyNames">属性列表</param>
        /// <returns>相等返回true,其他返回false</returns>
        public static bool CompareEquals<T>(this T objectFromCompare, T objectToCompare, string[] propertyNames)
        {
            bool result = (objectFromCompare == null && objectToCompare == null);
            if (!result)
            {
                try
                {
                    foreach (string propertyName in propertyNames)
                    {
                        result = CompareEquals(objectFromCompare, objectToCompare, propertyName);
                        if (!result)
                        {
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            return result;
        }

        /// <summary>
        /// 比较两个对象
        /// </summary>
        /// <param name="objectFrom"></param>
        /// <param name="objectTo"></param>
        /// <returns>相等返回0</returns>
        public static int Compare(this object objectFrom, object objectTo)
        {
            return comparerHelper.Compare(objectFrom, objectTo);
        }
        private static Comparer comparerHelper = new Comparer();
    }
}

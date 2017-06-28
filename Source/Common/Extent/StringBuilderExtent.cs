using System;
using System.Linq;
using System.Text;

namespace Zhoubin.Infrastructure.Common.Extent
{
    /// <summary>Extension methods that add some functionality of <c>string</c> to <c>StringBuilder</c>.</summary>
    public static class StringBuilderExt
    {
        /// <summary>Extracts a substring from the specified StringBuiler.</summary>
        /// <param name="sb">上下文对象</param>
        /// <param name="startIndex">开始索引</param>
        /// <param name="length">截取长度</param>
        /// <returns>返回截取的字符串值</returns>
        public static string Substring(this StringBuilder sb, int startIndex, int length)
        {
            return sb.ToString(startIndex, length);
        }

        /// <summary>Returns the last character of the string</summary>
        /// <param name="str">上下文对象</param>
        /// <exception cref="InvalidOperationException">当字符串长度为0时，抛出此异常。</exception>
        /// <returns>返回字符</returns>
        public static char Last(this StringBuilder str)
        {
            if (str.Length == 0)
                throw new InvalidOperationException("Empty string has no last character");
            return str[str.Length - 1];
        }

        /// <summary>Returns the last character of the string,
        /// or a default character if the string is empty.
        /// </summary>
        /// <param name="str">上下文</param>
        /// <param name="default">默认字符</param>
        /// <returns>返回最后一个字符，如果长度为0，就返回default值</returns>
        public static char LastOrDefault(this StringBuilder str, char @default = '\0')
        {
            if (str.Length == 0)
                return @default;
            return str[str.Length - 1];
        }

        static readonly char[] DefaultTrimChars = new[] { ' ', '\t' };

        /// <summary>Removes all leading occurrences of spaces
        /// and tabs from the StringBuilder object.</summary>
        /// <param name="sb">上下文</param>
        /// <returns>返回上下文对象</returns>
        public static StringBuilder TrimStart(this StringBuilder sb)
        { return TrimStart(sb, DefaultTrimChars); }

        /// <summary>Removes all leading occurrences of the specified
        /// set of characters from the StringBuilder object.</summary>
        /// <param name="sb">上下文</param>
        /// <param name="trimChars">An array of Unicode characters to remove.</param>
        /// <returns>返回上下文对象</returns>
        public static StringBuilder TrimStart(this StringBuilder sb, params char[] trimChars)
        {
            int i = 0;
            while (i < sb.Length && trimChars.Contains(sb[i]))
                i++;
            sb.Remove(0, i);
            return sb;
        }

        /// <summary>Removes all trailing occurrences of spaces
        /// and tabs from the StringBuilder object.</summary>
        /// <param name="sb">上下文</param>
        /// <returns>返回上下文对象</returns>
        public static StringBuilder TrimEnd(this StringBuilder sb)
        { return TrimEnd(sb, DefaultTrimChars); }

        /// <summary>Removes all trailing occurrences of the specified
        /// set of characters from the StringBuilder object.</summary>
        /// <param name="sb">上下文</param>
        /// <param name="trimChars">An array of Unicode characters to remove.</param>
        /// <returns>返回上下文对象</returns>
        public static StringBuilder TrimEnd(this StringBuilder sb, params char[] trimChars)
        {
            int i = sb.Length;
            while (i > 0 && trimChars.Contains(sb[i - 1]))
                i--;
            sb.Remove(i, sb.Length - i);
            return sb;
        }

        /// <summary>Removes all leading and trailing occurrences of
        /// spaces and tabs from the StringBuilder object.
        /// </summary>
        /// <param name="sb">上下文</param>
        /// <returns>返回上下文对象</returns>
        public static StringBuilder Trim(this StringBuilder sb)
        { return Trim(sb, DefaultTrimChars); }

        /// <summary>Removes all leading and trailing occurrences of the
        /// specified set of characters from the StringBuilder object.</summary>
        /// <param name="trimChars">An array of Unicode characters to remove.</param>
        /// <param name="sb">上下文</param>
        /// <returns>返回上下文对象</returns>
        public static StringBuilder Trim(this StringBuilder sb, params char[] trimChars)
        {
            return TrimStart(TrimEnd(sb, trimChars), trimChars);
        }

        /// <summary>Gets the index of a character in a StringBuilder</summary>
        /// <param name="sb">上下文</param>
        /// <param name="value">字符</param>
        /// <param name="startIndex">开始索引</param>
        /// <returns>Index of the first instance of the specified
        /// character in the string, or -1 if not found</returns>
        public static int IndexOf(this StringBuilder sb, char value, int startIndex = 0)
        {
            for (int i = startIndex; i < sb.Length; i++)
                if (sb[i] == value)
                    return i;
            return -1;
        }

        /// <summary>Gets the index of a substring in a StringBuilder</summary>
        /// <param name="sb">上下文</param>
        /// <param name="searchStr">字符串</param>
        /// <param name="startIndex">开始索引</param>
        /// <param name="ignoreCase">忽略大小写，默认值false</param>
        /// <returns>Index of the first instance of the specified
        /// substring in the StringBuilder, or -1 if not found</returns>
        public static int IndexOf(this StringBuilder sb, string searchStr, int startIndex = 0, bool ignoreCase = false)
        {
            var stopAt = sb.Length - searchStr.Length;
            for (int i = startIndex; i <= stopAt; i++)
                if (SubstringEqualHelper(sb, i, searchStr, ignoreCase))
                    return i;
            return -1;
        }

        /// <summary>Gets the index of a character in a StringBuilder</summary>
        /// <param name="sb">上下文</param>
        /// <param name="searchChar">字符</param>
        /// <param name="startIndex">开始索引</param>
        /// <returns>Index of the last instance of the specified
        /// character in the StringBuilder, or -1 if not found</returns>
        public static int LastIndexOf(this StringBuilder sb, char searchChar, int startIndex = int.MaxValue)
        {
            if (startIndex > sb.Length - 1)
                startIndex = sb.Length - 1;
            for (int i = startIndex; i >= 0; i--)
                if (sb[i] == searchChar)
                    return i;
            return -1;
        }

        /// <summary>Gets the index of a substring in a StringBuilder</summary>
        /// <param name="sb">上下文</param>
        /// <param name="searchStr">字符串</param>
        /// <param name="startIndex">开始索引</param>
        /// <param name="ignoreCase">忽略大小写，默认值false</param>
        /// <returns>Index of the last instance of the specified
        /// substring in the StringBuilder, or -1 if not found</returns>
        public static int LastIndexOf(this StringBuilder sb, string searchStr, int startIndex = int.MaxValue, bool ignoreCase = false)
        {
            if (startIndex > sb.Length - searchStr.Length)
                startIndex = sb.Length - searchStr.Length;
            for (int i = startIndex; i >= 0; i--)
                if (SubstringEqualHelper(sb, i, searchStr, ignoreCase))
                    return i;
            return -1;
        }

        /// <summary>Finds out whether the StringBuilder ends
        /// with the specified substring.</summary>
        /// <param name="sb">上下文</param>
        /// <param name="what">字符串</param>
        /// <param name="ignoreCase">忽略大小写，默认值false</param>
        /// <returns>成功返回true</returns>
        public static bool EndsWith(this StringBuilder sb, string what, bool ignoreCase = false)
        {
            if (what.Length > sb.Length)
                return false;
            return SubstringEqualHelper(sb, sb.Length - what.Length, what, ignoreCase);
        }

        /// <summary>Finds out whether the StringBuilder starts
        /// with the specified substring.</summary>
        /// <param name="sb">上下文</param>
        /// <param name="what">字符串</param>
        /// <param name="ignoreCase">忽略大小写，默认值false</param>
        /// <returns>成功返回true</returns>
        public static bool StartsWith(this StringBuilder sb, string what, bool ignoreCase = false)
        {
            if (what.Length > sb.Length)
                return false;
            return SubstringEqualHelper(sb, 0, what, ignoreCase);
        }

        /// <summary>Checks if the sequences of characters <c>what</c> is equal to
        /// <c>sb.Substring(start, what.Length)</c>, without actually creating a
        /// substring object.</summary>
        /// <param name="sb">上下文</param>
        /// <param name="start">起始位置</param>
        /// <param name="what">字符串</param>
        /// <param name="ignoreCase">忽略大小写，默认值false</param>
        /// <returns>成功返回true</returns>
        public static bool SubstringEquals(StringBuilder sb, int start, string what, bool ignoreCase = false)
        {
            if (start > sb.Length - what.Length)
                return false;
            return SubstringEqualHelper(sb, start, what, ignoreCase);
        }
        /// <summary>Checks if the sequences of characters <c>what</c> is equal to
        /// <c>sb.Substring(start, what.Length)</c>, without actually creating a
        /// substring object.</summary>
        /// <param name="sb">上下文</param>
        /// <param name="start">起始位置</param>
        /// <param name="what">字符串</param>
        /// <param name="ignoreCase">忽略大小写，默认值false</param>
        /// <returns>成功返回true</returns>
        static bool SubstringEqualHelper(StringBuilder sb, int start, string what, bool ignoreCase = false)
        {
            if (ignoreCase)
                for (int i = 0; i < what.Length; i++)
                {
                    if (char.ToUpperInvariant(sb[start + i]) != char.ToUpperInvariant(what[i]))
                        return false;
                }
            else
                for (int i = 0; i < what.Length; i++)
                {
                    if (sb[start + i] != what[i])
                        return false;
                }
            return true;
        }
    }

}

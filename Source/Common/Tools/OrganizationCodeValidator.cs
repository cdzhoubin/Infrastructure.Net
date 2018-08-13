using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Zhoubin.Infrastructure.Common.Tools
{
    /// <summary>
    /// 组织机构代码验证
    /// </summary>
    public class OrganizationCodeValidator
    {

        static readonly Regex RegCode = new Regex("^([0-9A-Z]){8}-[0-9|X]$");
        private const String BaseCode = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static bool IsValidEntpCode(String code,out string msg)
        {
            msg = "";
            code = code.ToUpper();
            int[] ws = { 3, 7, 9, 10, 5, 8, 4, 2 };
            

            if (!RegCode.IsMatch(code))
            {
                msg = "组织机构代码格式不正确，正确格式示例：E0000000-X";
                return false;
            }
            int sum = 0;
            for (var i = 0; i < 8; i++)
            {
                sum += BaseCode.IndexOf(code[i]) * ws[i];
            }

            int c9 = 11 - (sum % 11);

            String sc9 = c9.ToString(CultureInfo.InvariantCulture);
            if (11 == c9)
            {
                sc9 = "0";
            }
            else if (10 == c9)
            {
                sc9 = "X";
            }

            if (sc9[0] != code[9])
            {
                msg = "校验位不正确，请检查。";
                return false;
            }
           
            return true;
        }

    }
}

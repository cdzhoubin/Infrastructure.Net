using System;
using System.Linq;

namespace Zhoubin.Infrastructure.Common.Tools
{
    /// <summary>
    /// 身份证校验工具类
    /// </summary>
    public class CheckIdCard
    {
        ///// <summary>
        ///// asp.net web form注册脚本方法
        ///// </summary>
        //public static void RegisterScript()
        //{
        //    var page = HttpContext.Current.CurrentHandler as Page;
        //    if (page != null)
        //    {
        //        page.ClientScript.RegisterClientScriptResource(typeof(ChinaIdCardValidator), ConstHelper.ValidFilePath);
        //    }
        //}
        ///// <summary>
        ///// 身份证号码错误消息
        ///// </summary>
        //public static string ErrorIdCardMessage
        //{
        //    get { return Properties.Resources.IdCardTipMessage; }
        //}
        //位权值数组
        private static readonly byte[] Weight = new byte[17];
        //身份证行政区划代码部分长度
        private const byte FPart = 6;
        //算法求模关键参数
        private const byte FMode = 11;
        //旧身份证长度
        private const byte OIdLen = 15;
        //新身份证长度
        private const byte NIdLen = 18;
        //新身份证年份标记值
        private const string YearFlag = "19";
        //校验字符串
        private const string CheckCode = "10X98765432";
        //最小行政区划分代码
        //private static int minCode = 110000;
        //最大行政区划分代码
        //private static int maxCode = 820000;
        //private static Random rand = new Random();

        /// <summary>
        /// 计算位权值数组
        /// </summary>
        private static void SetWBuffer()
        {
            for (int i = 0; i < Weight.Length; i++)
            {
                var k = (int)Math.Pow(2, (Weight.Length - i));
                Weight[i] = (byte)(k % FMode);
            }
        }

        /// <summary>
        /// 获取新身份证最后一位校验位
        /// </summary>
        /// <param name="idCard">身份证号码</param>
        /// <returns></returns>
        private static string GetCheckCode(string idCard)
        {
            int sum = Weight.Select((t, i) => int.Parse(idCard.Substring(i, 1)) * t).Sum();
            //进行加权求和计算
            //求模运算得到模值
            var mode = (byte)(sum % FMode);
            return CheckCode.Substring(mode, 1);
        }

        /// <summary>
        /// 检查身份证长度是否合法
        /// </summary>
        /// <param name="idCard">身份证号码</param>
        /// <returns></returns>
        private static bool CheckLen(string idCard)
        {
            return idCard.Length == OIdLen || idCard.Length == NIdLen;
        }

        /// <summary>
        /// 验证是否是新身份证
        /// </summary>
        /// <param name="idCard">身份证号码</param>
        /// <returns></returns>
        private static bool IsNew(string idCard)
        {
            return idCard.Length == NIdLen;
        }

        /// <summary>
        /// 获取时间串
        /// </summary>
        /// <param name="idCard">身份证号码</param>
        /// <returns></returns>
        private static string GetDate(string idCard)
        {
            string str;
            if (IsNew(idCard))
            {
                str = idCard.Substring(FPart, 8);
            }
            else
            {
                str = YearFlag + idCard.Substring(FPart, 6);
            }
            return str;
        }

        /// <summary>
        /// 检查时间是否合法
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns></returns>
        private static bool CheckDate(string idCard)
        {

            var strDate = GetDate(idCard);
            int year;
            if(!int.TryParse(strDate.Substring(0, 4),out year))
            {
                return false;
            }
            int month;
            if (!int.TryParse(strDate.Substring(4, 2), out month))
            {
                return false;
            }

            int day;
            if (!int.TryParse(strDate.Substring(6, 2), out day))
            {
                return false;
            }

            DateTime dt;
            return DateTime.TryParse(string.Format("{0}-{1}-{2}", year, month, day), out dt);
        }

        /// <summary>
        /// 检查身份证是否合法
        /// </summary>
        /// <param name="idCard">待检查身份证号</param>
        /// <param name="msg">消息</param>
        /// <returns>验证成功返回true,其他返回false</returns>
        public static bool CheckCard(string idCard, out string msg)
        {
            //身份证是否合法标志
            bool flag;
            msg = string.Empty;
            SetWBuffer();
            if (!CheckLen(idCard))
            {
                msg = "身份证长度不符合要求";
                flag = false;
            }
            else
            {
                if (!CheckDate(idCard))
                {
                    msg = "身份证日期不符合要求";
                    flag = false;
                }
                else
                {
                    if (!IsNew(idCard))
                    {
                        idCard = GetNewIdCard(idCard);
                    }
                    string checkCode = GetCheckCode(idCard);
                    string lastCode = idCard.Substring(idCard.Length - 1, 1);
                    if (checkCode == lastCode)
                    {
                        flag = true;
                    }
                    else
                    {
                        msg = "身份证验证错误";
                        flag = false;
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// 旧身份证号码转换成新身份证号码
        /// </summary>
        /// <param name="oldIdCard">旧身份证号码</param>
        /// <returns>新身份证号码</returns>
        public static string GetNewIdCard(string oldIdCard)
        {
            if (oldIdCard.Length == 15)
            {
                string newIdCard = oldIdCard.Substring(0, FPart);
                newIdCard += YearFlag;
                newIdCard += oldIdCard.Substring(FPart, 9);
                newIdCard += GetCheckCode(newIdCard);
                return newIdCard;
            }
            return string.Empty;
        }

        /// <summary>
        /// 新身份证号码转换成旧身份证号码
        /// </summary>
        /// <param name="newIdCard">新身份证号码</param>
        /// <returns>旧身份证号码</returns>
        public static string GetOldIdCard(string newIdCard)
        {
            if (newIdCard.Length == 18)
            {
                string oldIdCard = newIdCard.Substring(0, FPart);
                oldIdCard += newIdCard.Substring(8, 9);
                return oldIdCard;
            }
            return string.Empty;
        }
    }
}
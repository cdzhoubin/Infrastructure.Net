using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Zhoubin.Infrastructure.Common.Tools
{
    /// <summary>
    /// IPLocation实体 
    /// </summary>
    public class IPLocation
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// 本地名称
        /// </summary>
        public string Local { get; set; }

        private static object parseLock = new object();
        private bool _isParse = false;
        private string _province;
        private string _city;
        private static List<string> ProvList = new List<string>()
        {
            "北京市","上海市","天津市","重庆市"
        };
        private static List<string> ProvList1 = new List<string>()
        {
            "香港","澳门","广西","西藏","新疆","内蒙古","宁夏"
        };

        /// <summary>
        /// 解析IP
        /// </summary>
        /// <param name="country">IP地址</param>
        /// <returns>返回解析后的，省市</returns>
        public static string[] Parse(string country)
        {
            var province = "";
            var city = "";
            try
            {
                var strs = country.Split('省');
                if (strs.Length == 2)
                {
                    province = strs[0] + "省";
                    city = strs[1];
                }
                else
                {
                    if (ProvList.Any(country.StartsWith))
                    {
                        strs = country.Split(new[] { '市' }, 2);
                        province = strs[0] + "市";

                        city = strs[1];
                    }
                    else
                    {
                        if (!ProvList1.Any(country.StartsWith))
                        {
                            return new[] { "", "" };
                        }

                        var loc = 2;
                        if (country.StartsWith("内蒙"))
                        {
                            loc = 3;

                        }
                        province = country.Substring(0, loc);
                        city = country.Substring(loc); ;
                    }
                }

                if (string.IsNullOrEmpty(city))
                {
                    city = province;
                }

            }
            catch (Exception exception)//解析出错暂时不作记录，直接使用空值
            {
                //Log.LogFactory.GetDefaultLogger().Write(string.Format("待解析地区信息：{0}", country));
                //Log.LogFactory.GetDefaultLogger().Write(exception);
                throw;
            }

            return new[] { province, city };
        }

        private void Parse()
        {
            if (!_isParse)
            {
                lock (parseLock)
                {
                    if (!_isParse)
                    {
                        _province = "";
                        _city = "";
                        var strs = Parse(Country);
                        _province = strs[0];
                        _city = strs[1];
                        _isParse = true;

                    }
                }
            }
        }

        /// <summary>
        /// 从本地名称中分离出省份
        /// 如果分离出错返回空
        /// </summary>
        public string Province
        {
            get
            {

                Parse();
                return _province;

            }
        }

        /// <summary>
        /// 从本地名称中分离出市
        /// 如果分离出错返回空
        /// </summary>
        public string City
        {
            get
            {
                Parse();
                return _city;
            }
        }
    }
    /// <summary>
    /// QQ纯真数据库实现
    /// </summary>
   public  class QQWryLocator : IReadIP
    {
        private byte[] data;
        readonly Regex _regex = new Regex(@"(((\d{1,2})|(1\d{2})|(2[0-4]\d)|(25[0-5]))\.){3}((\d{1,2})|(1\d{2})|(2[0-4]\d)|(25[0-5]))");
        long _firstStartIpOffset;
        long _lastStartIpOffset;
        long _ipCount;
        private static byte[] _defaultData;

        /// <summary>
        /// 设置默认纯真IP数据库
        /// </summary>
        /// <param name="datas">纯真IP数据库</param>
        public static void SetDefaultData(byte[] datas)
        {
            _defaultData = new byte[datas.Length];
            for (int i = 0; i < datas.Length; i++)
            {
                _defaultData[i] = datas[i];
            }
            
        }

        /// <summary>
        /// Ip数量
        /// </summary>
        public long Count { get { return _ipCount; } }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileName">数据库路径</param>
        public void Initiation(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
            }
            Initiation1();
        }

        private void Initiation1()
        {
            byte[] buffer = new byte[8];
            Array.Copy(data, 0, buffer, 0, 8);
            _firstStartIpOffset = ((buffer[0] + (buffer[1] * 0x100)) + ((buffer[2] * 0x100) * 0x100)) + (((buffer[3] * 0x100) * 0x100) * 0x100);
            _lastStartIpOffset = ((buffer[4] + (buffer[5] * 0x100)) + ((buffer[6] * 0x100) * 0x100)) + (((buffer[7] * 0x100) * 0x100) * 0x100);
            _ipCount = Convert.ToInt64((double)(((double)(_lastStartIpOffset - _firstStartIpOffset)) / 7.0));

            if (_ipCount <= 1L)
            {
                throw new ArgumentException("ip FileDataError");
            }
        }

        /// <summary>
        /// 初始化,使用资源中的QQWry文件
        /// </summary>
        public void Initiation()
        {
            if (_defaultData == null)
            {
                throw new QQWryInitDataException();
            }
            data = _defaultData;
            Initiation1();
        }
        private static long IpToInt(string ip)
        {
            char[] separator = new char[] { '.' };
            if (ip.Split(separator).Length == 3)
            {
                ip = ip + ".0";
            }
            string[] strArray = ip.Split(separator);
            long num2 = ((long.Parse(strArray[0]) * 0x100L) * 0x100L) * 0x100L;
            long num3 = (long.Parse(strArray[1]) * 0x100L) * 0x100L;
            long num4 = long.Parse(strArray[2]) * 0x100L;
            long num5 = long.Parse(strArray[3]);
            return (((num2 + num3) + num4) + num5);
        }
        private static string IntToIP(long ip_Int)
        {
            long num = (long)((ip_Int & 0xff000000L) >> 0x18);
            if (num < 0L)
            {
                num += 0x100L;
            }
            long num2 = (ip_Int & 0xff0000L) >> 0x10;
            if (num2 < 0L)
            {
                num2 += 0x100L;
            }
            long num3 = (ip_Int & 0xff00L) >> 8;
            if (num3 < 0L)
            {
                num3 += 0x100L;
            }
            long num4 = ip_Int & 0xffL;
            if (num4 < 0L)
            {
                num4 += 0x100L;
            }
            return (num.ToString() + "." + num2.ToString() + "." + num3.ToString() + "." + num4.ToString());
        }

        /// <summary>
        /// 根据IP地址查询地区
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <returns>返回解析后的IP信息</returns>
        public IPLocation Query(string ip)
        {
            if (!_regex.Match(ip).Success)
            {
                throw new ArgumentException("IP格式错误");
            }
            IPLocation ipLocation = new IPLocation() { IP = ip };
            long intIP = IpToInt(ip);
            if ((intIP >= IpToInt("127.0.0.1") && (intIP <= IpToInt("127.255.255.255"))))
            {
                ipLocation.Country = "本机内部环回地址";
                ipLocation.Local = "";
            }
            else
            {
                if ((((intIP >= IpToInt("0.0.0.0")) && (intIP <= IpToInt("2.255.255.255"))) || ((intIP >= IpToInt("64.0.0.0")) && (intIP <= IpToInt("126.255.255.255")))) ||
                ((intIP >= IpToInt("58.0.0.0")) && (intIP <= IpToInt("60.255.255.255"))))
                {
                    ipLocation.Country = "网络保留地址";
                    ipLocation.Local = "";
                }
            }
            long right = _ipCount;
            long left = 0L;
            long middle = 0L;
            long startIp = 0L;
            long endIpOff = 0L;
            long endIp = 0L;
            int countryFlag = 0;
            while (left < (right - 1L))
            {
                middle = (right + left) / 2L;
                startIp = GetStartIp(middle, out endIpOff);
                if (intIP == startIp)
                {
                    left = middle;
                    break;
                }
                if (intIP > startIp)
                {
                    left = middle;
                }
                else
                {
                    right = middle;
                }
            }
            startIp = GetStartIp(left, out endIpOff);
            endIp = GetEndIp(endIpOff, out countryFlag);
            if ((startIp <= intIP) && (endIp >= intIP))
            {
                string local;
                ipLocation.Country = GetCountry(endIpOff, countryFlag, out local);
                ipLocation.Local = local;
            }
            else
            {
                ipLocation.Country = "未知";
                ipLocation.Local = "";
            }
            return ipLocation;
        }
        private long GetStartIp(long left, out long endIpOff)
        {
            long leftOffset = _firstStartIpOffset + (left * 7L);
            byte[] buffer = new byte[7];
            Array.Copy(data, leftOffset, buffer, 0, 7);
            endIpOff = (Convert.ToInt64(buffer[4].ToString()) + (Convert.ToInt64(buffer[5].ToString()) * 0x100L)) + ((Convert.ToInt64(buffer[6].ToString()) * 0x100L) * 0x100L);
            return ((Convert.ToInt64(buffer[0].ToString()) + (Convert.ToInt64(buffer[1].ToString()) * 0x100L)) + ((Convert.ToInt64(buffer[2].ToString()) * 0x100L) * 0x100L)) + (((Convert.ToInt64(buffer[3].ToString()) * 0x100L) * 0x100L) * 0x100L);
        }
        private long GetEndIp(long endIpOff, out int countryFlag)
        {
            byte[] buffer = new byte[5];
            Array.Copy(data, endIpOff, buffer, 0, 5);
            countryFlag = buffer[4];
            return ((Convert.ToInt64(buffer[0].ToString()) + (Convert.ToInt64(buffer[1].ToString()) * 0x100L)) + ((Convert.ToInt64(buffer[2].ToString()) * 0x100L) * 0x100L)) + (((Convert.ToInt64(buffer[3].ToString()) * 0x100L) * 0x100L) * 0x100L);
        }
        /// <summary>
        /// Gets the country.
        /// </summary>
        /// <param name="endIpOff">The end ip off.</param>
        /// <param name="countryFlag">The country flag.</param>
        /// <param name="local">The local.</param>
        /// <returns>country</returns>
        private string GetCountry(long endIpOff, int countryFlag, out string local)
        {
            string country = "";
            long offset = endIpOff + 4L;
            switch (countryFlag)
            {
                case 1:
                case 2:
                    country = GetFlagStr(ref offset, ref countryFlag, ref endIpOff);
                    offset = endIpOff + 8L;
                    local = (1 == countryFlag) ? "" : GetFlagStr(ref offset, ref countryFlag, ref endIpOff);
                    break;
                default:
                    country = GetFlagStr(ref offset, ref countryFlag, ref endIpOff);
                    local = GetFlagStr(ref offset, ref countryFlag, ref endIpOff);
                    break;
            }
            return country;
        }
        private string GetFlagStr(ref long offset, ref int countryFlag, ref long endIpOff)
        {
            int flag = 0;
            byte[] buffer = new byte[3];

            while (true)
            {
                //用于向前累加偏移量
                long forwardOffset = offset;
                flag = data[forwardOffset++];
                //没有重定向
                if (flag != 1 && flag != 2)
                {
                    break;
                }
                Array.Copy(data, forwardOffset, buffer, 0, 3);
                forwardOffset += 3;
                if (flag == 2)
                {
                    countryFlag = 2;
                    endIpOff = offset - 4L;
                }
                offset = (Convert.ToInt64(buffer[0].ToString()) + (Convert.ToInt64(buffer[1].ToString()) * 0x100L)) + ((Convert.ToInt64(buffer[2].ToString()) * 0x100L) * 0x100L);
            }
            if (offset < 12L)
            {
                return "";
            }
            return GetStr(ref offset);
        }
        private string GetStr(ref long offset)
        {
            byte lowByte = 0;
            byte highByte = 0;
            StringBuilder stringBuilder = new StringBuilder();
            byte[] bytes = new byte[2];
            Encoding encoding = Encoding.GetEncoding("GB2312");
            while (true)
            {
                lowByte = data[offset++];
                if (lowByte == 0)
                {
                    return stringBuilder.ToString();
                }
                if (lowByte > 0x7f)
                {
                    highByte = data[offset++];
                    bytes[0] = lowByte;
                    bytes[1] = highByte;
                    if (highByte == 0)
                    {
                        return stringBuilder.ToString();
                    }
                    stringBuilder.Append(encoding.GetString(bytes));
                }
                else
                {
                    stringBuilder.Append((char)lowByte);
                }
            }
        }
    }
    /// <summary>
    /// QQWry使用的纯真数据库未初始化
    /// </summary>
    public class QQWryInitDataException : InfrastructureException
    {
        /// <summary>
        /// 纯真数据库初始化构造函数
        /// </summary>
        public QQWryInitDataException():base("QQWryLocator默认IP数据库未初始化，请调用QQWryLocator.SetDefaultData进行初始化。") { }
    }


    /// <summary>
    /// 读取IP接口
    /// </summary>
    public interface IReadIP
    {
        /// <summary>
        /// 初始化,使用资源中的QQWry文件
        /// </summary>
        void Initiation();

        /// <summary>
        /// 初始化,使用指定的文件
        /// </summary>
        /// <param name="fileName">初始化文件名</param>
        void Initiation(string fileName);

        /// <summary>
        /// Ip数量
        /// </summary>
        long Count { get; }

        /// <summary>
        /// 查询IP
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <returns>返回解析的IP对应区域信息</returns>
        IPLocation Query(string ip);
    }

    /// <summary>
    /// 读取IP工厂类
    /// </summary>
    public static class LocatorFactory
    {
        /// <summary>
        /// 获取位置读取处理器
        /// </summary>
        /// <param name="locator">IP地域转换类型</param>
        /// <returns>返回处理器</returns>
        public static IReadIP GetLocator(Locator locator)
        {
            switch (locator)
            {
                case Locator.QQWry:
                    return new QQWryLocator();
            }

            return new QQWryLocator();
        }
    }

    /// <summary>
    /// 位置获取枚举
    /// </summary>
    public enum Locator
    {
        /// <summary>
        /// QQ纯真数据库
        /// </summary>
        QQWry = 1,
    }
}

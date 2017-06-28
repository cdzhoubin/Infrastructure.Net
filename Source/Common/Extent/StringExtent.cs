using System;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Serialization;
using Zhoubin.Infrastructure.Common.Gif;
using Zhoubin.Infrastructure.Common.Properties;
using Zhoubin.Infrastructure.Common.Tools;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using System.IO.Compression;
using System.Security;
using com.google.zxing;
using com.google.zxing.qrcode.decoder;
using System.Globalization;

namespace Zhoubin.Infrastructure.Common.Extent
{
    /// <summary>
    /// 字符串扩展方法
    /// </summary>
    public static class StringExtent
    {
        /// <summary>
        /// 转换字符串的长度（Int）为字节流
        /// 当字字符昨晚上为null或空时，返回-1
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>返回字节流</returns>
        public static byte[] ToIntSizedBytes(this string value)
        {
            if (string.IsNullOrEmpty(value)) return (-1).ToBytes();

            return value.Length.ToBytes()
                        .Concat(value.ToBytes())
                        .ToArray();
        }
        /// <summary>
        /// 转换字符串的长度（Int16）为字节流
        /// 当字符串为null或空时，返回-1
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>返回字节流</returns>

        public static byte[] ToInt16SizedBytes(this string value)
        {
            if (string.IsNullOrEmpty(value)) return (-1).ToBytes();

            return ((Int16)value.Length).ToBytes()
                        .Concat(value.ToBytes())
                        .ToArray();
        }
        /// <summary>
        /// 转换字节流的长度（Int32）为字节流
        /// 当字节流为null或空时，返回-1
        /// </summary>
        /// <param name="value">字节流</param>
        /// <returns>返回字节流</returns>
        public static byte[] ToInt32PrefixedBytes(this byte[] value)
        {
            if (value == null) return (-1).ToBytes();

            return value.Length.ToBytes()
                        .Concat(value)
                        .ToArray();
        }
        /// <summary>
        /// 转换字节流为utf-8字符串
        /// 当字节流为null时，返回空
        /// </summary>
        /// <param name="value">字节流</param>
        /// <returns>返回字符串</returns>
        public static string ToUtf8String(this byte[] value)
        {
            if (value == null) return string.Empty;

            return Encoding.UTF8.GetString(value);
        }
        /// <summary>
        /// 转移字符串为utf-8字节流
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>字节流，当字符串为空时，返回-1的字节流</returns>
        public static byte[] ToBytes(this string value)
        {
            if (string.IsNullOrEmpty(value)) return (-1).ToBytes();

            //UTF8 is array of bytes, no endianness
            return Encoding.UTF8.GetBytes(value);
        }
        /// <summary>
        /// 转移Int16为字节流
        /// </summary>
        /// <param name="value">待转换参数</param>
        /// <returns>字节流</returns>
        public static byte[] ToBytes(this Int16 value)
        {
            return BitConverter.GetBytes(value).Reverse().ToArray();
        }
        /// <summary>
        /// 转移Int32为字节流
        /// </summary>
        /// <param name="value">待转换参数</param>
        /// <returns>字节流</returns>
        public static byte[] ToBytes(this Int32 value)
        {
            return BitConverter.GetBytes(value).Reverse().ToArray();
        }
        /// <summary>
        /// 转移Int64为字节流
        /// </summary>
        /// <param name="value">待转换参数</param>
        /// <returns>字节流</returns>
        public static byte[] ToBytes(this Int64 value)
        {
            return BitConverter.GetBytes(value).Reverse().ToArray();
        }
        /// <summary>
        /// 转移float为字节流
        /// </summary>
        /// <param name="value">待转换参数</param>
        /// <returns>字节流</returns>
        public static byte[] ToBytes(this float value)
        {
            return BitConverter.GetBytes(value).Reverse().ToArray();
        }
        /// <summary>
        /// 转移double为字节流
        /// </summary>
        /// <param name="value">待转换参数</param>
        /// <returns>字节流</returns>
        public static byte[] ToBytes(this double value)
        {
            return BitConverter.GetBytes(value).Reverse().ToArray();
        }
        /// <summary>
        /// 转移char为字节流
        /// </summary>
        /// <param name="value">待转换参数</param>
        /// <returns>字节流</returns>
        public static byte[] ToBytes(this char value)
        {
            return BitConverter.GetBytes(value).Reverse().ToArray();
        }
        /// <summary>
        /// 转移bool为字节流
        /// </summary>
        /// <param name="value">待转换参数</param>
        /// <returns>字节流</returns>
        public static byte[] ToBytes(this bool value)
        {
            return BitConverter.GetBytes(value).Reverse().ToArray();
        }
        /// <summary>
        /// 转移字节流为Int32
        /// </summary>
        /// <param name="value">待转换参数</param>
        /// <returns>Int32变量</returns>
        public static Int32 ToInt32(this byte[] value)
        {
            return BitConverter.ToInt32(value.Reverse().ToArray(), 0);
        }
        /// <summary>
        /// 转移字符串为整型
        /// 当字符串为空或转移失败时返回null
        /// </summary>
        /// <param name="value">待转移字符串</param>
        /// <returns>当字符串为空或转移失败时返回null</returns>
        public static int? ToInt(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            int target;
            if (int.TryParse(value, out target))
            {
                return target;
            }
            return null;
        }
        /// <summary>
        /// 转移字符串为decimal
        /// 当字符串为空或转移失败时返回null
        /// </summary>
        /// <param name="value">待转移字符串</param>
        /// <returns>当字符串为空或转移失败时返回null</returns>
        public static decimal? ToDecimal(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            decimal target;
            if (decimal.TryParse(value, out target))
            {
                return target;
            }
            return null;
        }
        /// <summary>
        /// 转移字符串为浮点型
        /// 当字符串为空或转移失败时返回null
        /// </summary>
        /// <param name="value">待转移字符串</param>
        /// <returns>当字符串为空或转移失败时返回null</returns>
        public static float? ToFloat(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            float target;
            if (float.TryParse(value, out target))
            {
                return target;
            }
            return null;
        }

        /// <summary>
        /// Execute an await task while monitoring a given cancellation token.  Use with non-cancelable async operations.
        /// </summary>
        /// <param name="task">上下文</param>
        /// <param name="cancellationToken">取消token</param>
        /// <returns>返回等待上下文</returns>
        /// <remarks>
        /// This extension method will only cancel the await and not the actual IO operation.  The status of the IO opperation will still
        /// need to be considered after the operation is cancelled.
        /// See http://blogs.msdn.com/b/pfxteam/archive/2012/10/05/how-do-i-cancel-non-cancelable-async-operations.aspx
        /// </remarks>
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();

            var cancelRegistration = cancellationToken.Register(source => ((TaskCompletionSource<bool>)source).TrySetResult(true), tcs);

            using (cancelRegistration)
            {
                if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(false))
                {
                    throw new OperationCanceledException(cancellationToken);
                }
            }

            return await task.ConfigureAwait(false);
        }

        /// <summary>
        /// Execute an await task while monitoring a given cancellation token.  Use with non-cancelable async operations.
        /// </summary>
        /// <remarks>
        /// This extension method will only cancel the await and not the actual IO operation.  The status of the IO opperation will still
        /// need to be considered after the operation is cancelled.
        /// See http://blogs.msdn.com/b/pfxteam/archive/2012/10/05/how-do-i-cancel-non-cancelable-async-operations.aspx
        /// </remarks>
        /// <param name="task">上下文</param>
        /// <param name="cancellationToken">取消token</param>
        /// <returns>返回等待上下文</returns>
        public static async Task WithCancellation(this Task task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();

            var cancelRegistration = cancellationToken.Register(source => ((TaskCompletionSource<bool>)source).TrySetResult(true), tcs);

            using (cancelRegistration)
            {
                if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(false))
                {
                    throw new OperationCanceledException(cancellationToken);
                }
            }
        }


        /// <summary>
        /// Returns true if <see cref="WaitHandle"/> before timeout expires./>
        /// </summary>
        /// <param name="handle">The handle whose signal triggers the task to be completed.</param>
        /// <param name="timeout">The timespan to wait before returning false</param>
        /// <returns>The task returns true if the handle is signaled before the timeout has expired.</returns>
        /// <remarks>
        /// Original code from: http://blog.nerdbank.net/2011/07/c-await-for-waithandle.html
        /// There is a (brief) time delay between when the handle is signaled and when the task is marked as completed.
        /// </remarks>
        public static Task<bool> WaitAsync(this WaitHandle handle, TimeSpan timeout)
        {
            Contract.Requires<ArgumentNullException>(handle != null);
            Contract.Ensures(Contract.Result<Task>() != null);

            var tcs = new TaskCompletionSource<bool>();
            var localVariableInitLock = new object();
            RegisteredWaitHandle callbackHandle = null;
            lock (localVariableInitLock)
            {
                callbackHandle = ThreadPool.RegisterWaitForSingleObject(
                    handle,
                    (state, timedOut) =>
                    {
                        tcs.TrySetResult(!timedOut);

                        // We take a lock here to make sure the outer method has completed setting the local variable callbackHandle.
                        lock (localVariableInitLock)
                        {
                            if (callbackHandle != null) callbackHandle.Unregister(null);
                        }
                    },
                    state: null,
                    millisecondsTimeOutInterval: (long)timeout.TotalMilliseconds,
                    executeOnlyOnce: true);
            }

            return tcs.Task;
        }

        /// <summary>
        /// Mainly used for testing, allows waiting on a single task without throwing exceptions.
        /// </summary>
        /// <param name="source">上下文</param>
        /// <param name="timeout">溢出时间</param>
        public static void SafeWait(this Task source, TimeSpan timeout)
        {
            try
            {
                source.Wait(timeout);
            }
            catch
            {
                //ignore an exception that happens in this source
            }
        }

        /// <summary>
        /// Splits a collection into given batch sizes and returns as an enumerable of batches.
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="collection">待处理对象</param>
        /// <param name="batchSize">分割大小</param>
        /// <returns>返回分割好的对象</returns>
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> collection, int batchSize)
        {
            var nextbatch = new List<T>(batchSize);
            foreach (T item in collection)
            {
                nextbatch.Add(item);
                if (nextbatch.Count == batchSize)
                {
                    yield return nextbatch;
                    nextbatch = new List<T>(batchSize);
                }
            }
            if (nextbatch.Count > 0)
                yield return nextbatch;
        }

        /// <summary>
        /// Extracts a concrete exception out of a Continue with result.
        /// </summary>
        /// <param name="task">上下文</param>
        /// <exception cref="ApplicationException">当任务成功完成时，抛出此异常</exception>
        /// <returns>返回异常对象</returns>
        public static Exception ExtractException(this Task task)
        {
            if (task.IsFaulted == false) return null;
            if (task.Exception != null)
                return task.Exception.Flatten();

            return new ApplicationException("Unknown exception occured.");
        }
        /// <summary>
        /// 根据字符串创建类型实例
        /// </summary>
        /// <param name="typeString">字符串类型字符串，如：Ben.NetUtility.Web.UI.Validate.ValidateConfigSection,Ben.NetUtility</param>
        /// <param name="paramsObjects">创建类型时，要输入的参数</param>
        /// <returns>返回创建好的实例，如果失败返回null</returns>
        public static object CreateInstance(this string typeString, params object[] paramsObjects)
        {
            string[] strs;
            if (typeString.Contains("[[") && typeString.Contains("]]"))
            {
                if (typeString.EndsWith("]]"))
                {
                    strs = new[] { typeString };
                }
                else
                {
                    var index = typeString.LastIndexOf(',');
                    strs = new[] { typeString.Substring(0, index), typeString.Substring(index + 1) };
                }
            }
            else
            {
                strs = typeString.Split(",".ToArray(), 2);
            }

            if (paramsObjects == null || paramsObjects.Length == 0)
            {
                return Assembly.Load(strs[1].Trim()).CreateInstance(strs[0].Trim());
            }

            return Assembly.Load(strs[1].Trim()).CreateInstance(strs[0].Trim(), false, BindingFlags.CreateInstance, null, paramsObjects, null, null);
        }

        /// <summary>
        /// 根据字符串创建类型实例T
        /// </summary>
        /// <param name="typeString">字符串类型字符串，如：Ben.NetUtility.Web.UI.Validate.ValidateConfigSection,Ben.NetUtility</param>
        /// <param name="paramsObjects">创建类型时，要输入的参数</param>
        /// <typeparam name="T">类型参数</typeparam>
        /// <returns>返回创建好的实例，如果失败抛出异常</returns>
        public static T CreateInstance<T>(this string typeString, params object[] paramsObjects)
        {
            return (T)CreateInstance(typeString, paramsObjects);
        }
        /// <summary>
        /// 忽略大小写比较两个字符串是否相等
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="tagert">目标字符串</param>
        /// <returns>true:相等，false:不等</returns>
        public static bool EqualsIgnoreCase(this string source, string tagert)
        {
            return String.Compare(source, tagert, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// 忽略大小写判断源字符串是否包含目标字符串
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="target">目标字符串</param>
        /// <returns>true:包含，false:不包含</returns>
        public static bool ContainsIgnoreCase(this string source, string target)
        {
            return source.ToUpperInvariant().Contains(target.ToUpperInvariant());
        }
        /// <summary>
        /// 用参数填充源字符串
        /// 功能是对string.Format的扩展
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="formats">参数</param>
        /// <returns>返回填充后的字符串</returns>
        public static string Fill(this string source, params object[] formats)
        {
            return string.Format(source, formats);
        }

        /// <summary>
        /// 忽略大小写源字符串是否以目标字符串结尾
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="target">目标字符串</param>
        /// <returns>true:是，false:不是</returns>
        public static bool EndWithIgnoreCase(this string source, string target)
        {
            return source.ToUpper().EndsWith(target.ToUpper());
        }

        /// <summary>
        /// 忽略大小写源字符串是否以目标字符串开始
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="target">目标字符串</param>
        /// <returns>true:是，false:不是</returns>
        public static bool StartWithIgnoreCase(this string source, string target)
        {
            return source.ToUpper().StartsWith(target.ToUpper());
        }

        /// <summary>
        /// 忽略大小写替换字符串
        /// </summary>
        /// <param name="str">待处理字符串</param>
        /// <param name="source">待替换子串</param>
        /// <param name="target">目标子串</param>
        /// <returns>返回替换好的子串，全部为小写</returns>
        public static string ReplaceIgnoreCase(this string str, string source, string target)
        {
            return str.ToLower().Replace(source.ToLower(), target.ToLower());
        }

        /// <summary>
        /// 根据IP获取其所在地区
        /// </summary>
        /// <param name="str">地区名</param>
        /// <returns>返回IP，未查询到返回null</returns>
        public static string GetLocationByIp(this string str)
        {
            var ipRead = LocatorFactory.GetLocator(Locator.QQWry);
            var data = ipRead.Query(str);
            if (data == null)
            {
                return null;
            }

            return data.Country;
        }

        /// <summary>
        /// 创建条形码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="barcodeFormat">编码格式</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns>如果输入字符串为空或null,返回null</returns>
        public static Bitmap CreateBarcode(this string str, int width, int height, BarcodeFormat barcodeFormat)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            if (width <= 0 || height <= 0)
            {
                throw new ArgumentException(Resources.WidthMustGTZero, "width");
            }

            if (height <= 0)
            {
                throw new ArgumentException(Resources.HeightMustGTZero, "height");
            }

            if (barcodeFormat.Name == BarcodeFormat.EAN_13.Name && str.Length < 13)
            {
                throw new ArgumentException(Resources.EAN13LengthMust13Bit, "barcodeFormat");
            }

            if (barcodeFormat.Name == BarcodeFormat.EAN_8.Name && str.Length < 8)
            {
                throw new ArgumentException(Resources.EAN8LengthMust8bit, "barcodeFormat");
            }

            var multiFormatWriter = new MultiFormatWriter();
            var hint = new Hashtable
            {
                {EncodeHintType.CHARACTER_SET, "UTF-8"},
                {EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H}
            };
            var byteMatrix = multiFormatWriter.encode(str, barcodeFormat, width, height, hint);
            return byteMatrix.ToBitmap();
        }

        /// <summary>
        /// 创建条形码,默认使用BarcodeFormat.EAN_13
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns>如果输入字符串为空或null,返回null</returns>
        public static Bitmap CreateBarcode(this string str, int width, int height)
        {
            return CreateBarcode(str, width, height, BarcodeFormat.EAN_13);
        }

        /// <summary>
        /// 从文件解析二位码
        /// </summary>
        /// <param name="str">文件路径</param>
        /// <returns>返回解析后的二位码</returns>
        public static string DecodeBarcode(this string str)
        {
            return Image.FromFile(str).DecodeBarcode();
        }

        /// <summary>
        /// 创建带中心图片的二位码
        /// </summary>
        /// <param name="str">二位码内容</param>
        /// <param name="width">长度</param>
        /// <param name="centerImage">中心图片</param>
        /// <returns>创建好的图片</returns>
        public static Bitmap CreateBarcode(this string str, int width, Bitmap centerImage)
        {
            return CreateBarcode(str, width, centerImage, 0.3f);
        }

        /// <summary>
        /// 创建带中心图片的二位码
        /// </summary>
        /// <param name="str">二位码内容</param>
        /// <param name="width">长度</param>
        /// <param name="centerImage">中心图片</param>
        /// <param name="zoom">中心图片的缩放比例</param>
        /// <returns>创建好的图片</returns>
        public static Bitmap CreateBarcode(this string str, int width, Bitmap centerImage, float zoom)
        {
            if (zoom < 0 && zoom > 1)
            {
                throw new ArgumentException(Resources.ImageZoom, "zoom");
            }
            var img = CreateBarcode(str, width, width, BarcodeFormat.QR_CODE);
            if (img == null)
            {
                return null;
            }

            var centerImageWidth = (int)(width * zoom);
            var zoomImage = Thumbnail.CreateThumbnail(centerImage, centerImageWidth, true);
            var bmp = new Bitmap(width, width);
            using (var g = Graphics.FromImage(bmp))
            {
                g.DrawImage(img, new Point { X = 0, Y = 0 });
                g.DrawImage(zoomImage, new Point { X = width / 2 - centerImageWidth / 2, Y = width / 2 - centerImageWidth / 2 });
            }
            return bmp;
        }

        /// <summary>
        ///  转半角的函数(SBC case)
        /// </summary>
        /// <param name="input">输入</param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public static string ToDBC(this string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }

        /// <summary>
        /// 创建图片
        /// </summary>
        /// <param name="str">待创建字符串</param>
        /// <returns>返回生成的验证码图像流</returns>
        public static Stream CreateVerificationCode(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            int iwidth = str.Length * 11;
            Bitmap image = new Bitmap(iwidth, 19);
            Graphics g = Graphics.FromImage(image);
            g.Clear(Color.White);
            //定义颜色
            Color[] c = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Chocolate, Color.Brown, Color.DarkCyan, Color.Purple };

            //输出不同字体和颜色的验证码字符
            for (int i = 0; i < str.Length; i++)
            {
                int cindex = RandomStringExtent.Next(7);
                Font f = new Font("Microsoft Sans Serif", 11);
                Brush b = new SolidBrush(c[cindex]);
                g.DrawString(str.Substring(i, 1), f, b, (i * 10) + 1, 0, StringFormat.GenericDefault);
            }
            //画一个边框
            g.DrawRectangle(new Pen(Color.Black, 0), 0, 0, image.Width - 1, image.Height - 1);


            //输出到浏览器
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            g.Dispose();
            image.Dispose();
            ms.Position = 0;
            return ms;
        }
        /// <summary>
        /// 创建校验码
        /// 如果字符串为空，返回null
        /// </summary>
        /// <param name="str">待创建字符串</param>
        /// <param name="width">图像宽度</param>
        /// <param name="height">图像高度</param>
        /// <param name="delay">动画延时</param>
        /// <exception cref="ArgumentException">当宽或高小于0时，抛出此异常</exception>
        /// <returns>返回图片对象，当字符串为空或null时返回null</returns>
        public static Stream CreateGifVerificationCode(this string str, int width, int height, int delay)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            if (width <= 0 || height <= 0)
            {
                throw new ArgumentException(Resources.WidthMustGTZero, "width");
            }
            var coder = new AnimatedGifEncoder();
            coder.SetSize(width, height);
            coder.SetRepeat(0);

            var stream = new MemoryStream();
            coder.Start(stream);
            Process(coder, str, width, height, delay);
            stream.Position = 0;
            return stream;
        }

        private static void Process(AnimatedGifEncoder coder, string code, int width, int height, int delay)
        {
            // GenerateIdentifyingCode(_defaultIdentifyingCodeLen);
            var br = Brushes.White;

            var rect = new Rectangle(0, 0, width, height);

            var f = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold);
            var f1 = new Font(FontFamily.GenericMonospace, 14, FontStyle.Italic);

            var frameCount = RandomStringExtent.Next(3, 4);
            var codeFrameIndex = RandomStringExtent.Next(1, frameCount);

            for (var i = 0; i < frameCount; i++)
            {
                Image im = new Bitmap(width, height);
                var ga = Graphics.FromImage(im);
                ga.FillRectangle(br, rect);
                var fH = height - 1 - (int)f.GetHeight();
                var fW = width - 1 - (int)ga.MeasureString(code, (i == codeFrameIndex - 1 ? f : f1)).Width;
                if (fH < 1)
                {
                    fH = 2;
                }

                if (fW < 1)
                {
                    fW = 3;
                }
                AddNoise(ga, width, height, RandomStringExtent);
                if (i == codeFrameIndex - 1)
                {

                    Brush b = new SolidBrush(Color.Black);
                    ga.DrawString(code, f, b
                        , new PointF(RandomStringExtent.Next(1, fW)
                            , RandomStringExtent.Next(1, fH)
                            )
                        );
                }
                else
                {
                    Brush b = new SolidBrush(Color.Aquamarine);
                    ga.DrawString(code.RandomString(code.Length), f1, b
                        , new PointF(RandomStringExtent.Next(1, fW)
                            , RandomStringExtent.Next(1, fH)
                            )
                        );
                }
                ga.Flush();
                coder.SetDelay(delay);
                coder.AddFrame(im);
                im.Dispose();
            }
            coder.Finish();
        }
        private static void AddNoise(Graphics ga, int width, int height, Random random)
        {
            var pen = new Pen(SystemColors.GrayText);
            var noiseCount = random.Next(8, 15);
            var ps = new Point[noiseCount];
            for (var i = 0; i < noiseCount; i++)
            {
                ps[i] = new Point(random.Next(1, width - 1), random.Next(1, height - 1));
            }

            ga.DrawLines(pen, ps);

        }

        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <param name="str">扩展对象</param>
        /// <param name="length">长度，默认为5</param>
        /// <param name="chinese">是否包含中文，默认false</param>
        /// <returns>返回生成的随机字符串</returns>
        public static string RandomString(this string str, int length = 5, bool chinese = false)
        {
            return GenerateIdentifyingCode(length, chinese ? AvailableLetters : AvailableLettersEnglish);

        }

        static readonly Random RandomStringExtent = new Random();
        private const string AvailableLetters = @"的一是在了不和有大这主中人上为们地个用工时要动国产以我到他会作来分生对于学下级就年阶义发成部民可出能方进同行面说种过命度革而多子后自社加小机也经力线本电高量长党得实家定深法表着水理化争现所二起政三好十战无农使性前等反体合斗路图把结第里正新开论之物从当两些还天资事队批如应形想制心样干都向变关点育重其思与间内去因件日利相由压员气业代全组数果期导平各基或月毛然问比展那它最及外没看治提五解系林者米群头意只明四道马认次文通但条较克又公孔领军流入接席位情运器并飞原油放立题质指建区验活众很教决特此常石强极土少已根共直团统式转别造切九你取西持总料连任志观调七么山程百报更见必真保热委手改管处己将修支识病象几先老光专什六型具示复安带每东增则完风回南广劳轮科北打积车计给节做务被整联步类集号列温装即毫知轴研单色坚据速防史拉世设达尔场织历花受求传口断况采精金界品判参层止边清至万确究书术状厂须离再目海交权且儿青才证低越际八试规斯近注办布门铁需走议县兵固除般引齿千胜细影济白格效置推空配刀叶率述今选养德话查差半敌始片施响收华觉备名红续均药标记难存测士身紧液派准斤角降维板许破述技消底床田势端感往神便贺村构照容非搞亚磨族火段算适讲按值美态黄易彪服早班麦削信排台声该击素张密害侯草何树肥继右属市严径螺检左页抗苏显苦英快称坏移约巴材省黑武培著河帝仅针怎植京助升王眼她抓含苗副杂普谈围食射源例致酸旧却充足短划剂宣环落首尺波承粉践府鱼随考刻靠够满夫失包住促枝局菌杆周护岩师举曲春元超负砂封换太模贫减阳扬江析亩木言球朝医校古呢稻宋听唯输滑站另卫字鼓刚写刘微略范供阿块某功套友限项余倒卷创律雨让骨远帮初皮播优占死毒圈伟季训控激找叫云互跟裂粮粒母练塞钢顶策双留误础吸阻故寸盾晚丝女散焊功株亲院冷彻弹错散商视艺灭版烈零室轻血倍缺厘泵察绝富城冲喷壤简否柱李望盘磁雄似困巩益洲脱投送奴侧润盖挥距触星松送获兴独官混纪依未突架宽冬章湿偏纹吃执阀矿寨责熟稳夺硬价努翻奇甲预职评读背协损棉侵灰虽矛厚罗泥辟告卵箱掌氧恩爱停曾溶营终纲孟钱待尽俄缩沙退陈讨奋械载胞幼哪剥迫旋征槽倒握担仍呀鲜吧卡粗介钻逐弱脚怕盐末阴丰编印蜂急拿扩伤飞露核缘游振操央伍域甚迅辉异序免纸夜乡久隶缸夹念兰映沟乙吗儒杀汽磷艰晶插埃燃欢铁补咱芽永瓦倾阵碳演威附牙芽永瓦斜灌欧献顺猪洋腐请透司危括脉宜笑若尾束壮暴企菜穗楚汉愈绿拖牛份染既秋遍锻玉夏疗尖殖井费州访吹荣铜沿替滚客召旱悟刺脑措贯藏敢令隙炉壳硫煤迎铸粘探临薄旬善福纵择礼愿伏残雷延烟句纯渐耕跑泽慢栽鲁赤繁境潮横掉锥希池败船假亮谓托伙哲怀割摆贡呈劲财仪沉炼麻罪祖息车穿货销齐鼠抽画饲龙库守筑房歌寒喜哥洗蚀废纳腹乎录镜妇恶脂庄擦险赞钟摇典柄辩竹谷卖乱虚桥奥伯赶垂途额壁网截野遗静谋弄挂课镇妄盛耐援扎虑键归符庆聚绕摩忙舞遇索顾胶羊湖钉仁音迹碎伸灯避泛亡答勇频皇柳哈揭甘诺概宪浓岛袭谁洪谢炮浇斑讯懂灵蛋闭孩释乳巨徒私银伊景坦累匀霉杜乐勒隔弯绩招绍胡呼痛峰零柴簧午跳居尚丁秦稍追梁折耗碱殊岗挖氏刃剧堆赫荷胸衡勤膜篇登驻案刊秧缓凸役剪川雪链渔啦脸户洛孢勃盟买杨宗焦赛旗滤硅炭股坐蒸凝竟陷枪黎救冒暗洞犯筒您宋弧爆谬涂味津臂障褐陆啊健尊豆拔莫抵桑坡缝警挑污冰柬嘴啥饭塑寄赵喊垫康遵牧遭幅园腔订香肉弟屋敏恢忘衣孙龄岭骗休借丹渡耳刨虎笔稀昆浪萨茶滴浅拥穴覆伦娘吨浸袖珠雌妈紫戏塔锤震岁貌洁剖牢锋疑霸闪埔猛诉刷狠忽灾闹乔唐漏闻沈熔氯荒茎男凡抢像浆旁玻亦忠唱蒙予纷捕锁尤乘乌智淡允叛畜俘摸锈扫毕璃宝芯爷鉴秘净蒋钙肩腾枯抛轨堂拌爸循诱祝励肯酒绳穷塘燥泡袋朗喂铝软渠颗惯贸粪综墙趋彼届墨碍启逆卸航雾冠丙街莱贝辐肠付吉渗瑞惊顿挤秒悬姆烂森糖圣凹陶词迟蚕亿矩";
        private const string AvailableLettersEnglish = "abcdefghijklmnopqrstuvwxyz0123456789";
        private static string GenerateIdentifyingCode(int codeLength, string availableLetters)
        {
            var codes = new StringBuilder(codeLength);
            for (var i = 0; i < codeLength; i++)
            {
                codes.Append(availableLetters[RandomStringExtent.Next(0, availableLetters.Length)]);
            }

            return codes.ToString();
        }

        /// <summary>
        /// 序列化对象为字符串
        /// 使用<see cref="XmlSerializer"/>进行序列化
        /// </summary>
        /// <param name="toSerialize">序列化对象</param>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回序列化后的对象</returns>
        public static string SerializeObject<T>(this T toSerialize)
        {
            return SerializeObject(toSerialize, null);
        }

        /// <summary>
        /// 序列化对象为字符串
        /// 使用<see cref="XmlSerializer"/>进行序列化
        /// </summary>
        /// <param name="toSerialize">序列化对象</param>
        /// <param name="extentTypes"></param>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回序列化后的对象</returns>
        public static string SerializeObject<T>(this T toSerialize, params Type[] extentTypes)
        {
            var textWriter = new StringWriter();
            XmlSerializer serializer =
            CreateXmlSerializer(toSerialize.GetType(), extentTypes);
            serializer.Serialize(textWriter, toSerialize);
            return textWriter.ToString();
        }
        /// <summary>
        /// 创建序列化器
        /// </summary>
        /// <param name="type"></param>
        /// <param name="extentTypes"></param>
        /// <returns></returns>
        private static XmlSerializer CreateXmlSerializer(Type type, Type[] extentTypes)
        {
            return extentTypes != null && extentTypes.Length > 0 ? new XmlSerializer(type, extentTypes) : new XmlSerializer(type);
        }

        /// <summary>
        /// 字符串反序列化为对象
        /// 使用<see cref="XmlSerializer"/>进行反序列化
        /// </summary>
        /// <param name="toDeserialize">反序列化对象</param>
        /// <param name="extentTypes"></param>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回反序列化后的对象</returns>
        public static T DeserializeObject<T>(this string toDeserialize, params Type[] extentTypes)
        {
            var textReader = new StringReader(toDeserialize);

            return (T)CreateXmlSerializer(typeof(T), extentTypes).Deserialize(textReader);
        }
        /// <summary>
        /// 字符串反序列化为对象
        /// 使用<see cref="XmlSerializer"/>进行反序列化
        /// </summary>
        /// <param name="toDeserialize">反序列化对象</param>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回反序列化后的对象</returns>
        public static T DeserializeObject<T>(this string toDeserialize)
        {
            return DeserializeObject<T>(toDeserialize, null);
        }

        /// <summary>
        /// 检验是否是有效身份证号
        /// </summary>
        /// <param name="idCard">待校验值</param>
        /// <returns>有效身份证号返回：true,其它:false</returns>
        public static bool IsIdCard(this string idCard)
        {
            string msg;
            return IsIdCard(idCard, out msg);
        }

        /// <summary>
        /// 检验是否是有效身份证号
        /// </summary>
        /// <param name="idCard">待校验值</param>
        /// <param name="msg">校验错误信息,正确返回空</param>
        /// <returns>有效身份证号返回：true,其它:false</returns>
        public static bool IsIdCard(this string idCard, out string msg)
        {
            return CheckIdCard.CheckCard(idCard, out msg);
        }

        /// <summary>
        /// 检验是否是有效组织机构代码
        /// </summary>
        /// <param name="idCard">待校验值</param>
        /// <returns>有效组织机构代码返回：true,其它:false</returns>
        public static bool IsOrganizationCode(this string idCard)
        {
            string msg;
            return IsOrganizationCode(idCard, out msg);
        }

        /// <summary>
        /// 检验是否是有效组织机构代码
        /// </summary>
        /// <param name="code">待校验值</param>
        /// <param name="msg">校验错误信息,正确返回空</param>
        /// <returns>有效组织机构代码返回：true,其它:false</returns>
        public static bool IsOrganizationCode(this string code, out string msg)
        {
            return IsValidEntpCode(code, out msg);
        }
        static readonly Regex RegCode = new Regex("^([0-9A-Z]){8}-[0-9|X]$");
        private const String BaseCode = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        internal static bool IsValidEntpCode(String code, out string msg)
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
       
        #region AppSetting配置读取扩展

        /// <summary>
        /// 读取AppSetting中键并转换成整形
        /// </summary>
        /// <param name="name">键名</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>返回读取到的值</returns>
        public static int ReadAppSettingInt(this string name, int defaultValue, int? min = null, int? max = null)
        {
            int value = defaultValue;
            if (!string.IsNullOrEmpty(name))
            {
                string value1 = ConfigurationManager.AppSettings[name];
                if (!string.IsNullOrEmpty(value1))
                {
                    if (!int.TryParse(value1, out value))
                    {
                        value = defaultValue;
                    }
                }

            }

            if (min != null && value < min.Value)
            {
                return min.Value;
            }
            if (max != null && value > max.Value)
            {
                return max.Value;
            }

            return value;
        }
        /// <summary>
        /// 读取AppSetting中键并转换成浮点型
        /// </summary>
        /// <param name="name">键名</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>返回读取到的值</returns>
        public static Decimal ReadAppSettingToDecimal(this string name, Decimal defaultValue, Decimal? min = null, Decimal? max = null)
        {
            Decimal value = defaultValue;
            if (!string.IsNullOrEmpty(name))
            {
                string value1 = ConfigurationManager.AppSettings[name];
                if (!string.IsNullOrEmpty(value1))
                {
                    if (!Decimal.TryParse(value1, out value))
                    {
                        value = defaultValue;
                    }
                }

            }

            if (min != null && value < min.Value)
            {
                return min.Value;
            }
            if (max != null && value > max.Value)
            {
                return max.Value;
            }

            return value;
        }
        /// <summary>
        /// 读取AppSetting中键
        /// </summary>
        /// <param name="name">键名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回读取到的值</returns>
        public static string ReadAppSettingToString(this string name, string defaultValue)
        {
            if (!string.IsNullOrEmpty(name))
            {
                string value1 = ConfigurationManager.AppSettings[name];
                if (!string.IsNullOrEmpty(value1))
                {
                    return value1;
                }

            }

            return defaultValue;
        }
        #endregion
        /// <summary>
        /// 单词变成单数形式
        /// </summary>
        /// <param name="word"></param>
        /// <returns>返回转换后的值</returns>
        public static string ToSingular(this string word)
        {
            foreach (var key in DictionaryPlural.Keys)
            {
                if (key.IsMatch(word))
                    return key.Replace(word, DictionaryPlural[key]);
            }

            return word;
        }
        static readonly Dictionary<Regex, string> DictionaryPlural = new Dictionary<Regex, string>()
        {
            {new Regex("(?<keep>[^aeiou])ies$"),"${keep}y"},{new Regex("(?<keep>[aeiou]y)s$"),"${keep}"},
            {new Regex("(?<keep>[sxzh])es$"),"${keep}"},{new Regex("(?<keep>[^sxzhyu])s$"),"${keep}"}
        };
        static readonly Dictionary<Regex, string> DictionarySingular = new Dictionary<Regex, string>()
        {
            {new Regex("(?<keep>[^aeiou])y$"),"${keep}ies"},{new Regex("(?<keep>[aeiou]y)$"),"${keep}s"},
            {new Regex("(?<keep>[sxzh])$"),"${keep}es"},{new Regex("(?<keep>[^sxzhy])$"),"${keep}s"}
        };
        /// <summary>
        /// 单词变成复数形式
        /// </summary>
        /// <param name="word"></param>
        /// <returns>返回转换后的值</returns>
        public static string ToPlural(this string word)
        {
            foreach (var key in DictionarySingular.Keys)
            {
                if (key.IsMatch(word))
                    return key.Replace(word, DictionarySingular[key]);
            }

            return word;
        }

        /// <summary>
        /// 运行命令
        /// </summary>
        /// <param name="appPath">命令</param>
        /// <param name="arguments">命令参数</param>
        /// <param name="workingDirectory">工作路径，默认值为应用所有路径</param>
        /// <param name="user">用户名</param>
        /// <param name="passWord">密码</param>
        /// <param name="domain">域</param>
        public static bool Run(this string appPath, string arguments = null, string workingDirectory = null, string user = null, SecureString passWord = null,
            string domain = null)
        {
            Process pc = new Process();
            ProcessStartInfo psi = CreateProcessStartInfo(appPath,arguments,workingDirectory,user,passWord,domain);
            try
            {
                pc.StartInfo = psi;
                pc.Start();
                pc.WaitForExit();
            }
            catch(Exception exception)
            {
                return false;
            }
            finally
            {
                pc.Close();
            }
            return true;
        }

        /// <summary>
        /// 运行命令
        /// </summary>
        /// <param name="appPath">命令</param>
        /// <param name="arguments">命令参数</param>
        /// <param name="message"></param>
        /// <param name="workingDirectory">工作路径，默认值为应用所有路径</param>
        /// <param name="user">用户名</param>
        /// <param name="passWord">密码</param>
        /// <param name="domain">域</param>
        public static bool Run(this string appPath, string arguments,out string message, string workingDirectory = null, string user = null, SecureString passWord = null,
            string domain = null)
        {
            Process pc = new Process();
            ProcessStartInfo psi = CreateProcessStartInfo(appPath, arguments, workingDirectory, user, passWord, domain);
            message = "";
            try
            {
                pc.StartInfo = psi;
                pc.Start();
                var message1 = pc.StandardOutput.ReadToEnd();
                pc.WaitForExit();
                message = message1;
            }
            catch(Exception ex)
            {
                message = ex.ToString();
                return false;
            }
            finally
            {
                pc.Close();
            }
            return true;
        }
        private static ProcessStartInfo CreateProcessStartInfo(string appPath, string arguments, string workingDirectory, 
            string user, SecureString passWord,
            string domain)
        {
            if (string.IsNullOrEmpty(workingDirectory) || Directory.Exists(workingDirectory))
            {
                workingDirectory = Path.GetDirectoryName(Path.GetFullPath(appPath));
            }
            ProcessStartInfo psi = string.IsNullOrEmpty(arguments) ? new ProcessStartInfo("\"" + appPath + "\"") : new ProcessStartInfo("\"" + appPath + "\"", "\"" + arguments + "\"");
            if (!string.IsNullOrEmpty(workingDirectory))
            {
                psi.WorkingDirectory = workingDirectory;
            }
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = false;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.CreateNoWindow = true;

            if (!string.IsNullOrEmpty(user))
            {
                psi.UserName = user;
                psi.Password = passWord;
                if (!string.IsNullOrEmpty(domain))
                {
                    psi.Domain = domain;
                }
            }
            return psi;
        }
        /// <summary>
        /// GZip压缩字符串
        /// </summary>
        /// <param name="src">源字符串</param>
        /// <returns>压缩后字符串</returns>
        public static string GZipCompress(this string src)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(src);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }
        /// <summary>
        /// GZip解压缩字符串
        /// </summary>
        /// <param name="src">待解压字符串</param>
        /// <returns>返回解压后字符串</returns>
        public static string GZipDecompress(this string src)
        {
            byte[] gZipBuffer = Convert.FromBase64String(src);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }
        /// <summary>
        /// 读取环境变更
        /// </summary>
        /// <param name="name">变量名称</param>
        /// <returns>返回读取到的环境变更，如果读取为空，表示变量不存在</returns>
        public static string EnvironmentGet(this string name, EnvironmentVariableTarget scope = EnvironmentVariableTarget.Machine)
        {
            return Environment.GetEnvironmentVariable(name, scope);
        }
        /// <summary>
        /// 检查环境变量
        /// </summary>
        /// <param name="name">变量名称</param>
        /// <returns>返回读取到的环境变更，如果读取为空，表示变量不存在</returns>
        public static bool EnvironmentExist(this string name, EnvironmentVariableTarget scope = EnvironmentVariableTarget.Machine)
        {
            return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name, scope));
        }

        /// <summary>
        /// 设置环境变量
        /// </summary>
        /// <param name="name">变量名称</param>
        /// <param name="value">环境变量值，当设置空时，表示删除见财环境变量</param>
        /// <param name="scope">范围</param>
        public static bool EnvironmentSet(this string name, string value, EnvironmentVariableTarget scope = EnvironmentVariableTarget.Machine)
        {
            Environment.SetEnvironmentVariable(name, value, scope);
            return true;
        }
        

        private const string PathName = "PATH";
        /// <summary>
        /// 添加到PATH环境变量（会检测路径是否存在，存在就不重复）
        /// </summary>
        /// <param name="path"></param>
        /// <param name="before">路径加在前或后</param>
        public static bool EnvironmentPathSet(this string path, bool before)
        {
            string pathlist = Environment.GetEnvironmentVariable(PathName,EnvironmentVariableTarget.Machine);
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrEmpty(pathlist))
            {
                sb.Append(path + ";");
            }
            else
            {
                string[] list = pathlist.Split(';');
                if (IsPathExist(list, path))
                {
                    return true;
                }
                if (before)
                {
                    sb.AppendFormat("{0};", path);
                    sb.Append(pathlist);
                    if (!pathlist.EndsWith(";"))
                    {
                        sb.Append(";");
                    }
                }
                else
                {
                    sb.Append(pathlist);
                    if (!pathlist.EndsWith(";"))
                    {
                        sb.Append(";");
                    }
                    sb.AppendFormat("{0};", path);
                }
            }
            Environment.SetEnvironmentVariable(PathName, sb.ToString(), EnvironmentVariableTarget.Machine);
            return true;
        }
        private static bool IsPathExist(string[] list, string path)
        {
            bool isPathExist = false;

            foreach (string item in list)
            {
                if (item == path)
                {
                    isPathExist = true;
                    break;
                }
            }
            return isPathExist;
        }
        /// <summary>
        /// 增加PATH环境变量
        /// </summary>
        /// <param name="path">路径值</param>
        public static bool EnvironmentPathRemove(this string path)
        {
            string pathlist = Environment.GetEnvironmentVariable(PathName);
            if (string.IsNullOrEmpty(pathlist))
            {
                return false;
            }
            StringBuilder sb = new StringBuilder();

            string[] list = pathlist.Split(';');
            bool isPathExist = false;

            foreach (string item in list)
            {
                if (item == path)
                {
                    isPathExist = true;
                }
                sb.AppendFormat("{0};", item);
            }
            if (isPathExist)
            {
                Environment.SetEnvironmentVariable(PathName, sb.ToString(), EnvironmentVariableTarget.Machine);
            }
            return true;
        }
    }
}

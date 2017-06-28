using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Zhoubin.Infrastructure.Common.Tools
{
    /// <summary>
    /// ͼ������
    /// </summary>
    public static class Thumbnail
    {
        /// <summary>
        /// ʹ��ѹ���㷨����
        /// </summary>
        /// <param name="strPath">ԴͼƬ</param>
        /// <param name="thumbWi">ͼƬ���</param>
        /// <param name="maintainAspect">�Ƿ񰴱�������</param>
        /// <returns>�����ֽ���</returns>
        public static byte[] CreateThumbnailEx(string strPath, int thumbWi, bool maintainAspect)
        {
            var buffer = File.ReadAllBytes(strPath);
            var ms = new MemoryStream();
            ms.Write(buffer, 0, buffer.Length);
            var source = new Bitmap(ms);
            return CreateThumbnailEx(source, thumbWi, thumbWi, maintainAspect);
        }
        /// <summary>
        /// ʹ��ѹ���㷨����
        /// </summary>
        /// <param name="source">ԴͼƬ</param>
        /// <param name="thumbWi">ͼƬ��</param>
        /// <param name="maintainAspect">�Ƿ񰴱�������</param>
        /// <returns>�����ֽ���</returns>
        public static byte[] CreateThumbnailEx(Bitmap source, int thumbWi, bool maintainAspect)
        {
            return CreateThumbnailEx(source, thumbWi, thumbWi, maintainAspect);
        }
        /// <summary>
        /// ʹ��ѹ���㷨����
        /// </summary>
        /// <param name="strPath">ԴͼƬ</param>
        /// <param name="thumbWi">ͼƬ��</param>
        /// <param name="thumbHi">ͼƬ��</param>
        /// <param name="maintainAspect">�Ƿ񰴱�������</param>
        /// <returns>�����ֽ���</returns>
        public static byte[] CreateThumbnailEx(string strPath, int thumbWi, int thumbHi, bool maintainAspect)
        {
            var buffer = File.ReadAllBytes(strPath);
            var ms = new MemoryStream();
            ms.Write(buffer, 0, buffer.Length);
            var source = new Bitmap(ms);
            return CreateThumbnailEx(source, thumbWi, thumbHi, maintainAspect);
        }
        /// <summary>
        /// ʹ��ѹ���㷨����
        /// </summary>
        /// <param name="buffer">ԴͼƬ</param>
        /// <param name="thumbWi">ͼƬ��</param>
        /// <param name="thumbHi">ͼƬ��</param>
        /// <param name="maintainAspect">�Ƿ񰴱�������</param>
        /// <returns>�����ֽ���</returns>
        public static byte[] CreateThumbnailEx(byte[] buffer, int thumbWi, int thumbHi, bool maintainAspect)
        {
            var ms = new MemoryStream();
            ms.Write(buffer, 0, buffer.Length);
            var source = new Bitmap(ms);
            return CreateThumbnailEx(source, thumbWi, thumbHi, maintainAspect);
        }
        /// <summary>
        /// ʹ��ѹ���㷨����
        /// </summary>
        /// <param name="ms">ԴͼƬ</param>
        /// <param name="thumbWi">ͼƬ��</param>
        /// <param name="thumbHi">ͼƬ��</param>
        /// <param name="maintainAspect">�Ƿ񰴱�������</param>
        /// <returns>�����ֽ���</returns>
        public static byte[] CreateThumbnailEx(Stream ms, int thumbWi, int thumbHi, bool maintainAspect)
        {
            var source = new Bitmap(ms);
            return CreateThumbnailEx(source, thumbWi, thumbHi, maintainAspect);
        }
        /// <summary>
        /// ʹ��ѹ���㷨����
        /// </summary>
        /// <param name="source">ԴͼƬ</param>
        /// <param name="thumbWi">ͼƬ��</param>
        /// <param name="thumbHi">ͼƬ��</param>
        /// <param name="maintainAspect">�Ƿ񰴱�������</param>
        /// <returns>�����ֽ���</returns>
        public static byte[] CreateThumbnailEx(Bitmap source, int thumbWi, int thumbHi, bool maintainAspect)
        {
            byte[] buffer;
            using (var ms = new MemoryStream())
            {
                using (Image myThumbnail = CreateThumbnail(source, thumbWi, thumbHi, maintainAspect))
                {
                    //Configure JPEG Compression Engine
                    var encoderParams = new System.Drawing.Imaging.EncoderParameters();
                    var quality = new long[1];
                    quality[0] = 75;
                    var encoderParam = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                    encoderParams.Param[0] = encoderParam;

                    var arrayIci = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
                    System.Drawing.Imaging.ImageCodecInfo jpegIci = arrayIci.FirstOrDefault(t => t.FormatDescription.Equals("JPEG"));
                    if (jpegIci == null)
                        throw new Exception("jpegIciΪ�ա�");
                    myThumbnail.Save(ms, jpegIci, encoderParams);
                }
                ms.Seek(0, SeekOrigin.Begin);
                buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, (int)ms.Length);
            }
            return buffer;
        }


        /// <summary>
        /// ʹ��ѹ���㷨����
        /// </summary>
        /// <param name="source">ԴͼƬ</param>
        /// <param name="thumbWi">ͼƬ��</param>
        /// <param name="maintainAspect">�Ƿ񰴱�������</param>
        /// <returns>�����ֽ���</returns>
        public static Bitmap CreateThumbnail(Bitmap source, int thumbWi, bool maintainAspect)
        {
            return CreateThumbnail(source, thumbWi, thumbWi, maintainAspect);
        }
        /// <summary>
        /// ��������ͼ
        /// </summary>
        /// <param name="strPath">ԴͼƬ</param>
        /// <param name="thumbWi">ͼƬ��</param>
        /// <param name="thumbHi">ͼƬ��</param>
        /// <param name="maintainAspect">�Ƿ񰴱�������</param>
        /// <returns>�����ֽ���</returns>
        public static Bitmap CreateThumbnail(string strPath, int thumbWi, int thumbHi, bool maintainAspect)
        {
            var buffer = File.ReadAllBytes(strPath);
            var ms = new MemoryStream();
            ms.Write(buffer, 0, buffer.Length);
            var source = new Bitmap(ms);
            return CreateThumbnail(source, thumbWi, thumbHi, maintainAspect);
        }
        /// <summary>
        /// ��������ͼ
        /// </summary>
        /// <param name="strPath">ԴͼƬ</param>
        /// <param name="thumbWi">ͼƬ��</param>
        /// <param name="maintainAspect">�Ƿ񰴱�������</param>
        /// <returns>�����ֽ���</returns>
        public static Bitmap CreateThumbnail(string strPath, int thumbWi, bool maintainAspect)
        {
            var buffer = File.ReadAllBytes(strPath);
            var ms = new MemoryStream();
            ms.Write(buffer, 0, buffer.Length);
            var source = new Bitmap(ms);
            return CreateThumbnail(source, thumbWi, maintainAspect);
        }
        /// <summary>
        /// A better alternative to Image.GetThumbnail. Higher quality but slightly slower
        /// </summary>
        /// <param name="source">ԴͼƬ</param>
        /// <param name="thumbWi">���</param>
        /// <param name="thumbHi">�߶�</param>
        /// <param name="maintainAspect">�Ƿ񱣳ֳ����</param>
        /// <returns>����ͼ</returns>
        public static Bitmap CreateThumbnail(Bitmap source, int thumbWi, int thumbHi, bool maintainAspect)
        {
            // return the source image if it's smaller than the designated thumbnail
            if (source.Width < thumbWi && source.Height < thumbHi)
                return source;
            Bitmap ret;
            try
            {
                var wi = thumbWi;
                var hi = thumbHi;
                if (maintainAspect)
                {
                    // maintain the aspect ratio despite the thumbnail size parameters
                    if (source.Width > source.Height)
                    {
                        wi = thumbWi;
                        hi = (int)(source.Height * ((decimal)thumbWi / source.Width));
                    }
                    else
                    {
                        hi = thumbHi;
                        wi = (int)(source.Width * ((decimal)thumbHi / source.Height));
                    }
                }

                // original code that creates lousy thumbnails
                // System.Drawing.Image ret = source.GetThumbnailImage(wi,hi,null,IntPtr.Zero);
                ret = new Bitmap(wi, hi);
                using (var g = Graphics.FromImage(ret))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.FillRectangle(Brushes.White, 0, 0, wi, hi);
                    g.DrawImage(source, 0, 0, wi, hi);
                }
            }
            catch
            {
                ret = null;
            }

            return ret;
        }
    }
}

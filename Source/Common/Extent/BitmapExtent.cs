using System.Collections;
using System.Drawing;
using com.google.zxing;
using com.google.zxing.common;

namespace Zhoubin.Infrastructure.Common.Extent
{
    /// <summary>
    /// Bitmapr扩展方法
    /// </summary>
    public static class BitmapExtent
    {
        /// <summary>
        /// Barcode解码
        /// </summary>
        /// <param name="bitmap">输入Bitmap</param>
        /// <returns>返回解码后的字符串</returns>
        public static string DecodeBarcode(this Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return null;
            }

            var multiFormatReader = new MultiFormatReader();
            LuminanceSource source = new RGBLuminanceSource(bitmap, bitmap.Width, bitmap.Height);
            var image = new BinaryBitmap(new HybridBinarizer(source));
            var result = multiFormatReader.decode(image
                , new Hashtable { { EncodeHintType.CHARACTER_SET, "UTF-8" } });
            
            return result.Text;
        }

        /// <summary>
        /// Barcode解码
        /// </summary>
        /// <param name="image">输入image</param>
        /// <returns>返回解码后的字符串</returns>
        public static string DecodeBarcode(this Image image)
        {
            return DecodeBarcode(image as Bitmap);
        }
    }
}

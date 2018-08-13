using System.IO;
using iText.IO.Image;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace Zhoubin.Infrastructure.Common.Document.Pdf
{
    /// <summary>
    /// 图片内容
    /// </summary>
    public class PictureContent : ContentEntity<byte[]>,IPDFLeafElement
    {
        /// <summary>
        /// 图片宽度
        /// </summary>
        public float? Width { get; set; }
        /// <summary>
        /// 图片高度
        /// </summary>
        public float? Height { get; set; }
        /// <summary>
        /// 对齐方式
        /// </summary>
        public int? Alignment { get; set; }

        public ILeafElement CreateLeaf()
        {
            var image = new Image(ImageDataFactory.Create(Content));
            if (Alignment != null)
            {
                image.SetTextAlignment((TextAlignment)Alignment.Value);
            }

            if (Width != null && Height != null)
            {
                image.ScaleToFit(Width.Value, Height.Value);
            }
            else
            {
                if (Width != null)
                {
                    image.SetWidth(Width.Value);
                    image.SetAutoScaleWidth(true);
                }

                if (Height != null)
                {
                    image.SetHeight(Height.Value);
                    image.SetAutoScaleHeight(true);
                }
            }
            //image.ScalePercent();
            return image;
        }
    }
}
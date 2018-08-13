using iText.Kernel.Colors;

namespace Zhoubin.Infrastructure.Common.Document.Pdf
{
    /// <summary>
    /// 字体类型
    /// </summary>
    public class ContentFont
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 字体大小
        /// </summary>
        public float? Size { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public Color Color { get; set; }
        /// <summary>
        /// 样式
        /// </summary>
        public int? Style { get; set; }
        /// <summary>
        /// 是否嵌入字体
        /// </summary>
        public bool Embedded { get; set; }
    }
}
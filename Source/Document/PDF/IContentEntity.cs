using iText.Layout.Element;

namespace Zhoubin.Infrastructure.Common.Document.Pdf
{
    /// <summary>
    /// 内容接口
    /// </summary>
    public interface IContentEntity
    {
        /// <summary>
        /// 追加空行
        /// </summary>
        bool AppendNewLine { get; set; }
    }

    /// <summary>
    /// PDFBlock对象接口
    /// </summary>
    public interface IPDFBlockElement
    {
        /// <summary>
        /// 创建PDF对象
        /// </summary>
        /// <returns></returns>
        IBlockElement CreateBlock();
    }
    /// <summary>
    /// PDFLeaf对象接口
    /// </summary>
    public interface IPDFLeafElement
    {
        /// <summary>
        /// 创建叶子节点对象
        /// </summary>
        /// <returns></returns>
        ILeafElement CreateLeaf();
    }
}
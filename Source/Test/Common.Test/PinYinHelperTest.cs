
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Zhoubin.Infrastructure.Common.Tools;

namespace Common.Test
{
    [TestClass]
    public partial class PinYinHelperTest
    {
        static PinYinHelperTest()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        [TestMethod]
        public void TestGetInitials()
        {
            var result = PinYinHelper.GetInitials("周王");
            Assert.AreEqual("ZW", result);
        }
        [TestMethod]
        public void TestGetInitials_1()
        {
            Encoding encoding = Encoding.GetEncoding("gb2312");
            var result = PinYinHelper.GetInitials(ConvertEncoding("周王",Encoding.UTF8,encoding), encoding);
            Assert.AreEqual("ZW", result);
            Assert.AreEqual(ConvertEncoding("ZW", Encoding.UTF8, encoding), result);
        }
        [TestMethod]
        public void TestGetPinyin()
        {
            var result = PinYinHelper.GetPinyin("周王");
            Assert.AreEqual("zhou wang", result);
        }
        [TestMethod]
        public void TestGetPinyin_1()
        {
            Encoding encoding = Encoding.GetEncoding("gb2312");
            var result = PinYinHelper.GetPinyin(ConvertEncoding("周王", Encoding.UTF8, encoding), encoding);
            Assert.AreEqual("zhou wang", result);
            Assert.AreEqual(ConvertEncoding("zhou wang", Encoding.UTF8, encoding), result);
        }

        private static string ConvertEncoding(string text, Encoding srcEncoding, Encoding dstEncoding)
        {
            byte[] srcBytes = srcEncoding.GetBytes(text);
            byte[] dstBytes = Encoding.Convert(srcEncoding, dstEncoding, srcBytes);
            return dstEncoding.GetString(dstBytes);
        }
    }
}
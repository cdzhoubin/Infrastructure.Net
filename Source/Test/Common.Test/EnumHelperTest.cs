using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zhoubin.Infrastructure.Common.Tools;

namespace Common.Test
{
    [TestClass]
    public class EnumHelperTest
    {
        [TestMethod]
        public void TestGetEnumDescriptionDictionary()
        {
            var result = EnumHelper.GetEnumDescriptionDictionary(typeof(TestEnum),TestEnum.Admin);
            Assert.AreEqual("管理员", result);
        }
        [TestMethod]
        public void TestGetEnumDescriptionDictionary_1()
        {
            var result = EnumHelper.GetEnumDescriptionDictionary<TestEnum>(TestEnum.Admin);
            Assert.AreEqual("管理员", result);
        }
        [TestMethod]
        public void TestGetEnumDictionary()
        {
            var result = EnumHelper.GetEnumDictionary(typeof(TestEnum));
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("未选择", result[1]);
            Assert.AreEqual("管理员", result[2]);
            Assert.AreEqual("成员", result[3]);
        }
        [TestMethod]
        public void TestGetEnumDictionary_1()
        {
            var result = EnumHelper.GetEnumDictionary<TestEnum>();
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("未选择", result[1]);
            Assert.AreEqual("管理员", result[2]);
            Assert.AreEqual("成员", result[3]);
        }

        [TestMethod]
        public void TestGetEnumDictionary_2()
        {
            var result = EnumHelper.GetEnumDictionary<TestEnum>("全部");
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("全部", result[-1]);
            Assert.AreEqual("未选择", result[1]);
            Assert.AreEqual("管理员", result[2]);
            Assert.AreEqual("成员", result[3]);
        }
        [TestMethod]
        public void TestGetEnumDefaultDictionary_1()
        {
            var result = EnumHelper.GetEnumDefaultDictionary<TestEnum>();
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("None", result[1]);
            Assert.AreEqual("Admin", result[2]);
            Assert.AreEqual("Member", result[3]);
        }
    }
}

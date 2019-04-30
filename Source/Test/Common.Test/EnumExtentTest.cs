using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zhoubin.Infrastructure.Common.Extent;

namespace Common.Test
{
    [TestClass]
    public class EnumExtentTest
    {
        [TestMethod]
        public void TestExtentMethod()
        {
            var result = TestEnum.Admin.ToDescription();
            Assert.AreEqual("����Ա", result);
        }
    }

    public enum TestEnum
    {
        [System.ComponentModel.DescriptionAttribute("δѡ��")]
        None=1,
        [System.ComponentModel.DescriptionAttribute("����Ա")]
        Admin,
        [System.ComponentModel.DescriptionAttribute("��Ա")]
        Member
    }
}

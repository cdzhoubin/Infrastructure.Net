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
            Assert.AreEqual("管理员", result);
        }
    }

    public enum TestEnum
    {
        [System.ComponentModel.DescriptionAttribute("未选择")]
        None=1,
        [System.ComponentModel.DescriptionAttribute("管理员")]
        Admin,
        [System.ComponentModel.DescriptionAttribute("成员")]
        Member
    }
}

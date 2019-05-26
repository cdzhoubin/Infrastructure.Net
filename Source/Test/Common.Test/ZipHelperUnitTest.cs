using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zhoubin.Infrastructure.Common.Tools;

namespace Zhoubin.Infrastructure.Common.Test
{
    [TestClass]
    public class ZipHelperUnitTest
    {
        [TestMethod]
        public void TestZipContent()
        {
            const string str = "这是一个单元测试。abcdefg";
            var result = ZipHelper.Zip(str);
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void TestUnZipContent()
        {
            const string str = "这是一个单元测试。abcdefg";
            var result = ZipHelper.UnZip("UEsDBBQAAAAAAG6JNUNqGYq9IgAAACIAAAAHACQAY29udGVudAoAIAAAAAAAAQAYAN8b842qts4B3xvzjaq2zgHfG/ONqrbOAei/meaYr+S4gOS4quWNleWFg+a1i+ivleOAgmFiY2RlZmdQSwECLQAUAAAAAABuiTVDahmKvSIAAAAiAAAABwAkAAAAAAAAAAAAAAAAAAAAY29udGVudAoAIAAAAAAAAQAYAN8b842qts4B3xvzjaq2zgHfG/ONqrbOAVBLBQYAAAAAAQABAFkAAABrAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==");
            Assert.AreEqual(str, result);
        }

        [TestMethod]
        public void TestZipContentByPassword()
        {
            const string str = "这是一个单元测试。abcdefg";
            const string password = "!@#$%^ddsweeew";
            var result = ZipHelper.Zip(str, password);
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void TestUnZipContentByPassword()
        {
            const string str = "这是一个单元测试。abcdefg";
            const string password = "!@#$%^ddsweeew";
            var result = ZipHelper.UnZip("UEsDBBQAAQAAAJqTNUNqGYq9LgAAACIAAAAHACQAY29udGVudAoAIAAAAAAAAQAYAOJD3161ts4B4kPfXrW2zgHiQ99etbbOAY1f7cVPFbwSvi6jLBVPNRMv3Qx7Y4CYQUn02h0W3JLQudLRikkrC3RE89EwmFVQSwECLQAUAAEAAACakzVDahmKvS4AAAAiAAAABwAkAAAAAAAAAAAAAAAAAAAAY29udGVudAoAIAAAAAAAAQAYAOJD3161ts4B4kPfXrW2zgHiQ99etbbOAVBLBQYAAAAAAQABAFkAAAB3AAAAAAA="
                , password);
            Assert.AreEqual(str, result);
        }

        [TestMethod]
        public void TestZipStreams()
        {
            var zipStreams = new Dictionary<string, Stream>
            {
                {"1.txt", new MemoryStream(Encoding.UTF8.GetBytes("这是一个测试文件1"))},
                {"2.txt", new MemoryStream(Encoding.UTF8.GetBytes("这是一个测试文件2"))},
                {"3.txt", new MemoryStream(Encoding.UTF8.GetBytes("这是一个测试文件3"))}
            };

            var stream = ZipHelper.Zip(zipStreams);
            Assert.IsNotNull(stream);
        }

        //[TestMethod]
        //public void TestUnZipStreams()
        //{
        //    var zipStreams = ZipHelper.UnZip(new MemoryStream(Resource.dic));
        //    Assert.IsNotNull(zipStreams);
        //    Assert.AreEqual(3,zipStreams.Count);
        //}


        [TestMethod]
        public void TestZipStreamsByPassword()
        {
            var zipStreams = new Dictionary<string, Stream>
            {
                {"1.txt", new MemoryStream(Encoding.UTF8.GetBytes("这是一个测试文件1"))},
                {"2.txt", new MemoryStream(Encoding.UTF8.GetBytes("这是一个测试文件2"))},
                {"3.txt", new MemoryStream(Encoding.UTF8.GetBytes("这是一个测试文件3"))}
            };

            var stream = ZipHelper.Zip(zipStreams,"adbdde!@#$%");
            Assert.IsNotNull(stream);
           // File.WriteAllBytes("D:\\dicPassword.zip",((MemoryStream)stream).GetBuffer());
        }

        //[TestMethod]
        //public void TestUnZipStreamsByPassword()
        //{
        //    var zipStreams = ZipHelper.UnZip(new MemoryStream(Resource.dicPassword), "adbdde!@#$%");
        //    Assert.IsNotNull(zipStreams);
        //    Assert.AreEqual(3, zipStreams.Count);
        //}

        static string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string tempFolder = baseDir + "\\temp\\";
        string resourceFolder = baseDir+"\\..\\..\\..\\..\\..\\Common\\Tools\\";
        [TestMethod]
        public void TestUnZip()
        {
            ZipHelper.UnZipFolder(baseDir + "\\Resources\\UnZipTest.zip", tempFolder);
            Assert.IsTrue(File.Exists(tempFolder + "1.txt"));
            Assert.IsTrue(File.Exists(tempFolder + "2\\2.txt"));
            Assert.IsTrue(File.Exists(tempFolder + "2\\3\\3.txt"));
        }

        [TestMethod]
        public void TestUnZipByPassword()
        {
            ZipHelper.UnZipFolder(baseDir + "\\Resources\\UnZipTestPassoword.zip", tempFolder, "12345678");
            Assert.IsTrue(File.Exists(tempFolder + "1.txt"));
            Assert.IsTrue(File.Exists(tempFolder + "2\\2.txt"));
            Assert.IsTrue(File.Exists(tempFolder + "2\\3\\3.txt"));
        }

        [TestMethod]
        public void TestZip()
        {
            ZipHelper.ZipFolder(resourceFolder,tempFolder+"1.zip",true);
            Assert.IsTrue(File.Exists(tempFolder+"1.zip"));
        }

        [TestMethod]
        public void TestZip1()
        {
            ZipHelper.ZipFolder(resourceFolder, tempFolder + "2.zip");
            Assert.IsTrue(File.Exists(tempFolder + "2.zip"));
        }

        [TestMethod]
        public void TestZipByPassword()
        {
            ZipHelper.ZipFolder(resourceFolder, tempFolder + "3.zip",false,"1234567890");
            Assert.IsTrue(File.Exists(tempFolder + "3.zip"));
        }

        [TestMethod]
        public void TestZipByPassword1()
        {
            ZipHelper.ZipFolder(resourceFolder, tempFolder + "4.zip", true, "1234567890");
            Assert.IsTrue(File.Exists(tempFolder + "4.zip"));
        }

        [TestInitialize]
        public void Init()
        {
            if (Directory.Exists(tempFolder))
            {
                Directory.Delete(tempFolder, true);
            }
        }

        [TestCleanup]
        public void Init1()
        {
            Init();
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using System.IO;
using Zhoubin.Infrastructure.Common.MongoDb;
using Zhoubin.Infrastructure.Common.MongoDb.Entity;

namespace Mongodb20.Test
{
    [TestClass]
    public class FileDocStorageTest
    {
        [TestMethod]
        public void TestInsert_1()
        {
            using(var db = Factory.CreateFileStorage())
            {
                FileDocTest file = new FileDocTest();
                file.Name = "testfile";
                file.FileName = "testfile.jpg";
                file.ContentType = "jepg";
                using (var stream = new MemoryStream(File.ReadAllBytes("F:\\temp\\1.jpg")))
                {
                  var  id =   db.Insert(stream, file);
                    Assert.AreNotEqual(ObjectId.Empty, id);
                }
            }
        }
    }
    public class FileDocTest : FileEntity
    {
        public override string CollectionName => "FileDocTests";
    }
}

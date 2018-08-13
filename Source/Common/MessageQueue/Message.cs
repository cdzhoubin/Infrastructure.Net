using System;
using System.Xml.Serialization;

namespace Zhoubin.Infrastructure.Common.MessageQueue
{
    /// <summary>
    /// 消息队列信息
    /// </summary>
    /// <typeparam name="T">消息内容类型</typeparam>
    [Serializable]
    public class Message<T>
    {
        /// <summary>
        /// 用于反序列化使用
        /// </summary>
        public Message()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容，此项必须为可序列化类型</param>
        public Message(string title = "", T content = default(T))
        {
            Title = title;
            Content = content;
            MessageType = 0;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="messageType">消息类型</param>
        public Message(string title = "", int messageType = 0)
        {
            Title = title;
            MessageType = messageType;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容，此项必须为可序列化类型</param>
        /// <param name="messageType">消息类型，默认为0</param>
        public Message(string title, T content, int messageType = 0)
        {
            Title = title;
            Content = content;
            MessageType = messageType;
        }

        /// <summary>
        /// 标题
        /// </summary>
        [XmlElement]
        public string Title { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        [XmlElement]
        public int MessageType { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        [XmlElement]
        public T Content { get; set; }

        //public System.Xml.Schema.XmlSchema GetSchema()
        //{
        //    throw new NotImplementedException();
        //}

        //public void ReadXml(System.Xml.XmlReader reader)
        //{
        //    throw new NotImplementedException();
        //}

        //public void WriteXml(System.Xml.XmlWriter writer)
        //{
        //    var keySerializer = new XmlSerializer(Content.GetType());


        //    writer.WriteStartElement("Title");
        //    keySerializer.Serialize(writer, Title);
        //    writer.WriteEndElement();



        //    writer.WriteStartElement("MessageType");
        //    keySerializer.Serialize(writer, MessageType);
        //    writer.WriteEndElement();

        //    writer.WriteStartElement("MessageType");
        //    keySerializer.Serialize(writer, MessageType);
        //    writer.WriteEndElement();


        //}
    }
}
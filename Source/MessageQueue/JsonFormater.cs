using System;
using System.IO;
using System.Messaging;
using System.Text;
using Newtonsoft.Json;

namespace Zhoubin.Infrastructure.MessageQueue
{
    /// <summary>
    /// Json序列化器
    /// </summary>
    public class JsonFormater : IMessageFormatter
    {
        /// <summary>
        /// 在类中实现时，确定格式化程序是否可以反序列化消息的内容。
        /// </summary>
        /// <returns>
        /// 如果格式化程序可以反序列化消息，则为 true；否则为 false。
        /// </returns>
        /// <param name="message">要检查的 <see cref="T:System.Messaging.Message"/>。</param>
        public bool CanRead(Message message)
        {
            return message.BodyStream != null && message.BodyStream.Length > 0;
        }

        [ThreadStatic]
        private static byte[] _mBuffer;

        /// <summary>
        /// 在类中实现时，读取给定消息中的内容并创建包含该消息中的数据的对象。
        /// </summary>
        /// <returns>
        /// 反序列化的消息。
        /// </returns>
        /// <param name="message">The <see cref="T:System.Messaging.Message"/> to deserialize.</param>
        public object Read(Message message)
        {
            if (_mBuffer == null)
                _mBuffer = new byte[4096];
            var count = (int)message.BodyStream.Length;
            message.BodyStream.Read(_mBuffer, 0, count);
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(_mBuffer, 0, count), typeof(object));

        }

        [ThreadStatic]
        private static MemoryStream _mStream;

        /// <summary>
        /// 在类中实现时，将对象序列化到消息体中。
        /// </summary>
        /// <param name="message"><see cref="T:System.Messaging.Message"/>，它将包含序列化的对象。</param><param name="obj">要序列化到消息中的对象。</param>
        public void Write(Message message, object obj)
        {
            if (_mStream == null)
                _mStream = new MemoryStream(4096);
            _mStream.Position = 0;
            _mStream.SetLength(4095);
            string value = JsonConvert.SerializeObject(obj);
            int count = Encoding.UTF8.GetBytes(value, 0, value.Length, _mStream.GetBuffer(), 0);
            _mStream.SetLength(count);
            message.BodyStream = _mStream;
        }

        /// <summary>
        /// 创建作为当前实例副本的新对象。
        /// </summary>
        /// <returns>
        /// 作为此实例副本的新对象。
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            return this;
        }
    }
}

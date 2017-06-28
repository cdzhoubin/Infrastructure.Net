using System;
using System.IO;
using System.Web;

namespace Zhoubin.Infrastructure.Web.FilterExtent
{
   /// <summary>
   /// 自定义过滤流
   /// </summary>
   public class FilterStream:Stream
    {
       #region Fields
        private readonly HttpResponse _response;
        private readonly Stream _stream;
        #endregion

        #region Initialization
        /// <summary>
        /// 初始化 <see cref="FilterStream"/> 实例.
        /// </summary>
        /// <param name="response">The response.</param>
        public FilterStream(HttpResponse response)
        {
            _response = response;
            _stream = response.OutputStream;

            // Likely a bug in the framework: You have to get the value of the filter before you can use it.
            var x = _response.Filter; // Go figure.
        }
        #endregion

        #region Events
        /// <summary>
        /// 过虑执行发送此事件.
        /// </summary>
        public event EventHandler<FilterEventArgs> Filtering;
        #endregion

        #region Methods
        /// <summary>
        /// Writes a block of bytes to the current stream using data read from buffer.
        /// </summary>
        /// <param name="buffer">The buffer to write data from.</param>
        /// <param name="offset">The byte offset in <paramref name="buffer"/> at which to begin writing from.</param>
        /// <param name="count">The maximum number of bytes to write.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="buffer"/> is null.
        ///   </exception>
        ///   
        /// <exception cref="T:System.NotSupportedException">
        /// The stream does not support writing. For additional information see <see cref="P:System.IO.Stream.CanWrite"/>.
        /// -or-
        /// The current position is closer than <paramref name="count"/> bytes to the end of the stream, and the capacity cannot be modified.
        ///   </exception>
        ///   
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="offset"/> subtracted from the buffer length is less than <paramref name="count"/>.
        ///   </exception>
        ///   
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="offset"/> or <paramref name="count"/> are negative.
        ///   </exception>
        ///   
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        ///   </exception>
        ///   
        /// <exception cref="T:System.ObjectDisposedException">
        /// The current stream instance is closed.
        ///   </exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            // We can glean the encoding from the content encoding header.
            var encoding = _response.ContentEncoding;
            // Convert the bytes to/from a string using the correct Encoding before/after filtering.
            var decoded = encoding.GetString(buffer);
            var value = OnFilter(decoded);
            var encoded = encoding.GetBytes(value);
            // Write out the filtered string to the response's OutputStream.
            _response.OutputStream.Write(encoded, 0, encoded.Length);
        }

        /// <summary>
        /// Filters the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The filtered value.</returns>
        protected virtual string OnFilter(string value)
        {
            // Invoke events.
            var args = new FilterEventArgs(value);
            if(Filtering != null)
                Filtering(this, args);
            return args.Value;
        }
        #endregion

        #region Implement Stream
        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <returns>true if the stream supports reading; otherwise, false.
        ///   </returns>
        public override bool CanRead
        {
            get { return _stream.CanRead; }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <returns>true if the stream supports seeking; otherwise, false.
        ///   </returns>
        public override bool CanSeek
        {
            get { return _stream.CanSeek; }
        }

        /// <summary>
        /// When overridden in a derived class, gets or sets the position within the current stream.
        /// </summary>
        /// <returns>
        /// The current position within the stream.
        ///   </returns>
        ///   
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        ///   </exception>
        ///   
        /// <exception cref="T:System.NotSupportedException">
        /// The stream does not support seeking.
        ///   </exception>
        ///   
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        ///   </exception>
        public override long Position
        {
            get { return _stream.Position; }
            set { _stream.Position = value; }
        }

        /// <summary>
        /// When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        /// <returns>
        /// A long value representing the length of the stream in bytes.
        ///   </returns>
        ///   
        /// <exception cref="T:System.NotSupportedException">
        /// A class derived from Stream does not support seeking.
        ///   </exception>
        ///   
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        ///   </exception>
        public override long Length
        {
            get { return _stream.Length; }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <returns>true if the stream supports writing; otherwise, false.
        ///   </returns>
        public override bool CanWrite
        {
            get { return _stream.CanWrite; }
        }

        /// <summary>
        /// When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// The sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length.
        ///   </exception>
        ///   
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="buffer"/> is null.
        ///   </exception>
        ///   
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="offset"/> or <paramref name="count"/> is negative.
        ///   </exception>
        ///   
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        ///   </exception>
        ///   
        /// <exception cref="T:System.NotSupportedException">
        /// The stream does not support reading.
        ///   </exception>
        ///   
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        ///   </exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        /// <summary>
        /// When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        ///   </exception>
        public override void Flush()
        {
            _stream.Flush();
        }

        /// <summary>
        /// When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        ///   </exception>
        ///   
        /// <exception cref="T:System.NotSupportedException">
        /// The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output.
        ///   </exception>
        ///   
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        ///   </exception>
        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
        /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        /// <returns>
        /// The new position within the current stream.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        ///   </exception>
        ///   
        /// <exception cref="T:System.NotSupportedException">
        /// The stream does not support seeking, such as if the stream is constructed from a pipe or console output.
        ///   </exception>
        ///   
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        ///   </exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }
        #endregion
    }
}

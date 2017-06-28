#region .NET Disclaimer/Info
//===============================================================================
//
// gOODiDEA, uland.com
//===============================================================================
//
// $Header :		$  
// $Author :		$
// $Date   :		$
// $Revision:		$
// $History:		$  
//  
//===============================================================================
#endregion

using System;
using System.Collections;
using System.Drawing;
using System.IO;

namespace Zhoubin.Infrastructure.Common.Gif
{
    /// <summary>
    ///  Class GifDecoder - Decodes a GIF file into one or more frames.
    /// * No copyright asserted on the source code of this class.  May be used for
    /// * any purpose, however, refer to the Unisys LZW patent for any additional
    /// * restrictions.  Please forward any corrections to kweiner@fmsware.com.
    /// *
    /// * @author Kevin Weiner, FM Software; LZW decoder adapted from John Cristy's 
    /// 
    /// ImageMagick.
    ///* @version 1.03 November 2003
    /// </summary>
    class GifDecoder
    {

        /**
         * File read status: No errors.
         */
        /// <summary>
        /// 
        /// </summary>
        public static readonly int StatusOk = 0;

        /**
         * File read status: Error decoding file (may be partially decoded)
         */
        /// <summary>
        /// 
        /// </summary>
        public static readonly int StatusFormatError = 1;

        /**
         * File read status: Unable to open source.
         */
        /// <summary>
        /// 
        /// </summary>
        public static readonly int StatusOpenError = 2;

        Stream _inStream;
        int _status;

        int _width; // full image width
        int _height; // full image height
        bool _gctFlag; // global color table used
        int _gctSize; // size of global color table
        int _loopCount = 1; // iterations; 0 = repeat forever

        int[] _gct; // global color table
        int[] _lct; // local color table
        int[] _act; // active color table

        int _bgIndex; // background color index
        int _bgColor; // background color
        int _lastBgColor; // previous bg color

        bool _lctFlag; // local color table flag
        bool _interlace; // interlace flag
        int _lctSize; // local color table size

        int _ix, _iy, _iw, _ih; // current image rectangle
        Rectangle _lastRect; // last image rect
        Image _image; // current frame
        Bitmap _bitmap;
        Image _lastImage; // previous frame

        readonly byte[] _block = new byte[256]; // current data block
        int _blockSize; // block size

        // last graphic control extension info
        int _dispose;
        // 0=no action; 1=leave in place; 2=restore to bg; 3=restore to prev
        int _lastDispose;
        bool _transparency; // use transparent color
        int _delay; // delay in milliseconds
        int _transIndex; // transparent color index

        private const int MaxStackSize = 4096;
        // max decoder pixel stack size

        // LZW decoder working arrays
        short[] _prefix;
        byte[] _suffix;
        byte[] _pixelStack;
        byte[] _pixels;

        ArrayList _frames; // frames read from current file
        int _frameCount;

        /// <summary>
        /// 
        /// </summary>
        public class GifFrame
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="im"></param>
            /// <param name="del"></param>
            public GifFrame(Image im, int del)
            {
                Image = im;
                Delay = del;
            }
            /// <summary>
            /// 
            /// </summary>
            public Image Image;
            /// <summary>
            /// 
            /// </summary>
            public int Delay;
        }

        /**
         * Gets display duration for specified frame.
         *
         * @param n int index of frame
         * @return delay in milliseconds
         */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public int GetDelay(int n)
        {
            //
            _delay = -1;
            if ((n >= 0) && (n < _frameCount))
            {
                _delay = ((GifFrame)_frames[n]).Delay;
            }
            return _delay;
        }

        /**
         * Gets the number of frames read from file.
         * @return frame count
         */
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetFrameCount()
        {
            return _frameCount;
        }

        /**
         * Gets the first (or only) image read.
         *
         * @return BufferedImage containing first frame, or null if none.
         */
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Image GetImage()
        {
            return GetFrame(0);
        }

        /**
         * Gets the "Netscape" iteration count, if any.
         * A count of 0 means repeat indefinitiely.
         *
         * @return iteration count if one was specified, else 1.
         */
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetLoopCount()
        {
            return _loopCount;
        }

        /**
         * Creates new frame image from current data (and previous
         * frames as specified by their disposition codes).
         */
        int[] GetPixels(Bitmap bitmap)
        {
            int[] pixels = new int[3 * _image.Width * _image.Height];
            int count = 0;
            for (int th = 0; th < _image.Height; th++)
            {
                for (int tw = 0; tw < _image.Width; tw++)
                {
                    Color color = bitmap.GetPixel(tw, th);
                    pixels[count] = color.R;
                    count++;
                    pixels[count] = color.G;
                    count++;
                    pixels[count] = color.B;
                    count++;
                }
            }
            return pixels;
        }

        void SetPixels(int[] pixels)
        {
            int count = 0;
            for (int th = 0; th < _image.Height; th++)
            {
                for (int tw = 0; tw < _image.Width; tw++)
                {
                    Color color = Color.FromArgb(pixels[count++]);
                    _bitmap.SetPixel(tw, th, color);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void SetPixels()
        {
            // expose destination image's pixels as int array
            //		int[] dest =
            //			(( int ) image.getRaster().getDataBuffer()).getData();
            int[] dest = GetPixels(_bitmap);

            // fill in starting image contents based on last image's dispose code
            if (_lastDispose > 0)
            {
                if (_lastDispose == 3)
                {
                    // use image before last
                    int n = _frameCount - 2;
                    if (n > 0)
                    {
                        _lastImage = GetFrame(n - 1);
                    }
                    else
                    {
                        _lastImage = null;
                    }
                }

                if (_lastImage != null)
                {
                    //				int[] prev =
                    //					((DataBufferInt) lastImage.getRaster().getDataBuffer()).getData();
                    int[] prev = GetPixels(new Bitmap(_lastImage));
                    Array.Copy(prev, 0, dest, 0, _width * _height);
                    // copy pixels

                    if (_lastDispose == 2)
                    {
                        // fill last image rect area with background color
                        Graphics g = Graphics.FromImage(_image);
                        Color c;
                        if (_transparency)
                        {
                            c = Color.FromArgb(0, 0, 0, 0); 	// assume background is transparent
                        }
                        else
                        {
                            c = Color.FromArgb(_lastBgColor);
                            //						c = new Color(lastBgColor); // use given background color
                        }
                        Brush brush = new SolidBrush(c);
                        g.FillRectangle(brush, _lastRect);
                        brush.Dispose();
                        g.Dispose();
                    }
                }
            }

            // copy each source line to the appropriate place in the destination
            int pass = 1;
            int inc = 8;
            int iline = 0;
            for (int i = 0; i < _ih; i++)
            {
                int line = i;
                if (_interlace)
                {
                    if (iline >= _ih)
                    {
                        pass++;
                        switch (pass)
                        {
                            case 2:
                                iline = 4;
                                break;
                            case 3:
                                iline = 2;
                                inc = 4;
                                break;
                            case 4:
                                iline = 1;
                                inc = 2;
                                break;
                        }
                    }
                    line = iline;
                    iline += inc;
                }
                line += _iy;
                if (line < _height)
                {
                    int k = line * _width;
                    int dx = k + _ix; // start of line in dest
                    int dlim = dx + _iw; // end of dest line
                    if ((k + _width) < dlim)
                    {
                        dlim = k + _width; // past dest edge
                    }
                    int sx = i * _iw; // start of line in source
                    while (dx < dlim)
                    {
                        // map color and insert in destination
                        int index = _pixels[sx++] & 0xff;
                        int c = _act[index];
                        if (c != 0)
                        {
                            dest[dx] = c;
                        }
                        dx++;
                    }
                }
            }
            SetPixels(dest);
        }

        /**
         * Gets the image contents of frame n.
         *
         * @return BufferedImage representation of frame, or null if n is invalid.
         */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public Image GetFrame(int n)
        {
            Image im = null;
            if ((n >= 0) && (n < _frameCount))
            {
                im = ((GifFrame)_frames[n]).Image;
            }
            return im;
        }

        /**
         * Gets image size.
         *
         * @return GIF image dimensions
         */
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Size GetFrameSize()
        {
            return new Size(_width, _height);
        }

        /**
         * Reads GIF image from stream
         *
         * @param BufferedInputStream containing GIF file.
         * @return read status code (0 = no errors)
         */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inStream"></param>
        /// <returns></returns>
        public int Read(Stream inStream)
        {
            Init();
            if (inStream != null)
            {
                _inStream = inStream;
                ReadHeader();
                if (!Error())
                {
                    ReadContents();
                    if (_frameCount < 0)
                    {
                        _status = StatusFormatError;
                    }
                }
                inStream.Close();
            }
            else
            {
                _status = StatusOpenError;
            }
            return _status;
        }

        /**
         * Reads GIF file from specified file/URL source  
         * (URL assumed if name contains ":/" or "file:")
         *
         * @param name String containing source
         * @return read status code (0 = no errors)
         */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int Read(String name)
        {
            _status = StatusOk;
            try
            {
                name = name.Trim().ToLower();
                _status = Read(new FileInfo(name).OpenRead());
            }
            catch (IOException)
            {
                _status = StatusOpenError;
            }

            return _status;
        }

        /**
         * Decodes LZW image data into pixel array.
         * Adapted from John Cristy's ImageMagick.
         */
        /// <summary>
        /// 
        /// </summary>
        protected void DecodeImageData()
        {
            int NullCode = -1;
            int npix = _iw * _ih;
            int available,
                clear, code_size,
                end_of_information,
                in_code,
                old_code,
                bits,
                code,
                count,
                i,
                datum,
                data_size,
                first,
                top,
                bi,
                pi;

            if ((_pixels == null) || (_pixels.Length < npix))
            {
                _pixels = new byte[npix]; // allocate new pixel array
            }
            if (_prefix == null) _prefix = new short[MaxStackSize];
            if (_suffix == null) _suffix = new byte[MaxStackSize];
            if (_pixelStack == null) _pixelStack = new byte[MaxStackSize + 1];

            //  Initialize GIF data stream decoder.

            data_size = Read();
            clear = 1 << data_size;
            end_of_information = clear + 1;
            available = clear + 2;
            old_code = NullCode;
            code_size = data_size + 1;
            int codeMask = (1 << code_size) - 1;
            for (code = 0; code < clear; code++)
            {
                _prefix[code] = 0;
                _suffix[code] = (byte)code;
            }

            //  Decode GIF pixel stream.

            datum = bits = count = first = top = pi = bi = 0;

            for (i = 0; i < npix; )
            {
                if (top == 0)
                {
                    if (bits < code_size)
                    {
                        //  Load bytes until there are enough bits for a code.
                        if (count == 0)
                        {
                            // Read a new data block.
                            count = ReadBlock();
                            if (count <= 0)
                                break;
                            bi = 0;
                        }
                        datum += (_block[bi] & 0xff) << bits;
                        bits += 8;
                        bi++;
                        count--;
                        continue;
                    }

                    //  Get the next code.

                    code = datum & codeMask;
                    datum >>= code_size;
                    bits -= code_size;

                    //  Interpret the code

                    if ((code > available) || (code == end_of_information))
                        break;
                    if (code == clear)
                    {
                        //  Reset decoder.
                        code_size = data_size + 1;
                        codeMask = (1 << code_size) - 1;
                        available = clear + 2;
                        old_code = NullCode;
                        continue;
                    }
                    if (old_code == NullCode)
                    {
                        _pixelStack[top++] = _suffix[code];
                        old_code = code;
                        first = code;
                        continue;
                    }
                    in_code = code;
                    if (code == available)
                    {
                        _pixelStack[top++] = (byte)first;
                        code = old_code;
                    }
                    while (code > clear)
                    {
                        _pixelStack[top++] = _suffix[code];
                        code = _prefix[code];
                    }
                    first = _suffix[code] & 0xff;

                    //  Add a new string to the string table,

                    if (available >= MaxStackSize)
                        break;
                    _pixelStack[top++] = (byte)first;
                    _prefix[available] = (short)old_code;
                    _suffix[available] = (byte)first;
                    available++;
                    if (((available & codeMask) == 0)
                        && (available < MaxStackSize))
                    {
                        code_size++;
                        codeMask += available;
                    }
                    old_code = in_code;
                }

                //  Pop a pixel off the pixel stack.

                top--;
                _pixels[pi++] = _pixelStack[top];
                i++;
            }

            for (i = pi; i < npix; i++)
            {
                _pixels[i] = 0; // clear missing pixels
            }

        }

        /**
         * Returns true if an error was encountered during reading/decoding
         */
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool Error()
        {
            return _status != StatusOk;
        }

        /**
         * Initializes or re-initializes reader
         */
        /// <summary>
        /// 
        /// </summary>
        protected void Init()
        {
            _status = StatusOk;
            _frameCount = 0;
            _frames = new ArrayList();
            _gct = null;
            _lct = null;
        }

        /**
         * Reads a single byte from the input stream.
         */
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected int Read()
        {
            int curByte = 0;
            try
            {
                curByte = _inStream.ReadByte();
            }
            catch (IOException)
            {
                _status = StatusFormatError;
            }
            return curByte;
        }

        /**
         * Reads next variable length block from input.
         *
         * @return number of bytes stored in "buffer"
         */
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected int ReadBlock()
        {
            _blockSize = Read();
            int n = 0;
            if (_blockSize > 0)
            {
                try
                {
                    int count;
                    while (n < _blockSize)
                    {
                        count = _inStream.Read(_block, n, _blockSize - n);
                        if (count == -1)
                            break;
                        n += count;
                    }
                }
                catch (IOException)
                {
                }

                if (n < _blockSize)
                {
                    _status = StatusFormatError;
                }
            }
            return n;
        }

        /**
         * Reads color table as 256 RGB integer values
         *
         * @param ncolors int number of colors to read
         * @return int array containing 256 colors (packed ARGB with full alpha)
         */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ncolors"></param>
        /// <returns></returns>
        protected int[] ReadColorTable(int ncolors)
        {
            int nbytes = 3 * ncolors;
            int[] tab = null;
            byte[] c = new byte[nbytes];
            int n = 0;
            try
            {
                n = _inStream.Read(c, 0, c.Length);
            }
            catch (IOException)
            {
            }
            if (n < nbytes)
            {
                _status = StatusFormatError;
            }
            else
            {
                tab = new int[256]; // max size to avoid bounds checks
                int i = 0;
                int j = 0;
                while (i < ncolors)
                {
                    int r = c[j++] & 0xff;
                    int g = c[j++] & 0xff;
                    int b = c[j++] & 0xff;
                    tab[i++] = (int)(0xff000000 | (r << 16) | (g << 8) | b);
                }
            }
            return tab;
        }

        /**
         * Main file parser.  Reads GIF content blocks.
         */
        /// <summary>
        /// 
        /// </summary>
        protected void ReadContents()
        {
            // read GIF file content blocks
            bool done = false;
            while (!(done || Error()))
            {
                int code = Read();
                switch (code)
                {

                    case 0x2C: // image separator
                        ReadImage();
                        break;

                    case 0x21: // extension
                        code = Read();
                        switch (code)
                        {
                            case 0xf9: // graphics control extension
                                ReadGraphicControlExt();
                                break;

                            case 0xff: // application extension
                                ReadBlock();
                                String app = "";
                                for (int i = 0; i < 11; i++)
                                {
                                    app += (char)_block[i];
                                }
                                if (app.Equals("NETSCAPE2.0"))
                                {
                                    ReadNetscapeExt();
                                }
                                else
                                    Skip(); // don't care
                                break;

                            default: // uninteresting extension
                                Skip();
                                break;
                        }
                        break;

                    case 0x3b: // terminator
                        done = true;
                        break;

                    case 0x00: // bad byte, but keep going and see what happens
                        break;

                    default:
                        _status = StatusFormatError;
                        break;
                }
            }
        }

        /**
         * Reads Graphics Control Extension values
         */
        /// <summary>
        /// 
        /// </summary>
        protected void ReadGraphicControlExt()
        {
            Read(); // block size
            int packed = Read(); // packed fields
            _dispose = (packed & 0x1c) >> 2; // disposal method
            if (_dispose == 0)
            {
                _dispose = 1; // elect to keep old image if discretionary
            }
            _transparency = (packed & 1) != 0;
            _delay = ReadShort() * 10; // delay in milliseconds
            _transIndex = Read(); // transparent color index
            Read(); // block terminator
        }

        /**
         * Reads GIF file header information.
         */
        /// <summary>
        /// 
        /// </summary>
        protected void ReadHeader()
        {
            String id = "";
            for (int i = 0; i < 6; i++)
            {
                id += (char)Read();
            }
            if (!id.StartsWith("GIF"))
            {
                _status = StatusFormatError;
                return;
            }

            ReadLSD();
            if (_gctFlag && !Error())
            {
                _gct = ReadColorTable(_gctSize);
                _bgColor = _gct[_bgIndex];
            }
        }

        /**
         * Reads next frame image
         */
        /// <summary>
        /// 
        /// </summary>
        protected void ReadImage()
        {
            _ix = ReadShort(); // (sub)image position & size
            _iy = ReadShort();
            _iw = ReadShort();
            _ih = ReadShort();

            int packed = Read();
            _lctFlag = (packed & 0x80) != 0; // 1 - local color table flag
            _interlace = (packed & 0x40) != 0; // 2 - interlace flag
            // 3 - sort flag
            // 4-5 - reserved
            _lctSize = 2 << (packed & 7); // 6-8 - local color table size

            if (_lctFlag)
            {
                _lct = ReadColorTable(_lctSize); // read table
                _act = _lct; // make local table active
            }
            else
            {
                _act = _gct; // make global table active
                if (_bgIndex == _transIndex)
                    _bgColor = 0;
            }
            int save = 0;
            if (_transparency)
            {
                save = _act[_transIndex];
                _act[_transIndex] = 0; // set transparent color if specified
            }

            if (_act == null)
            {
                _status = StatusFormatError; // no color table defined
            }

            if (Error()) return;

            DecodeImageData(); // decode pixel data
            Skip();

            if (Error()) return;

            _frameCount++;

            // create new image to receive frame data
            //		image =
            //			new BufferedImage(width, height, BufferedImage.TYPE_INT_ARGB_PRE);

            _bitmap = new Bitmap(_width, _height);
            _image = _bitmap;
            SetPixels(); // transfer pixel data to image

            _frames.Add(new GifFrame(_bitmap, _delay)); // add image to frame list

            if (_transparency)
            {
                _act[_transIndex] = save;
            }
            ResetFrame();

        }

        /**
         * Reads Logical Screen Descriptor
         */
        /// <summary>
        /// 
        /// </summary>
        protected void ReadLSD()
        {

            // logical screen size
            _width = ReadShort();
            _height = ReadShort();

            // packed fields
            int packed = Read();
            _gctFlag = (packed & 0x80) != 0; // 1   : global color table flag
            // 2-4 : color resolution
            // 5   : gct sort flag
            _gctSize = 2 << (packed & 7); // 6-8 : gct size

            _bgIndex = Read(); // background color index
            Read(); // pixel aspect ratio
        }

        /**
         * Reads Netscape extenstion to obtain iteration count
         */
        protected void ReadNetscapeExt()
        {
            do
            {
                ReadBlock();
                if (_block[0] == 1)
                {
                    // loop count sub-block
                    int b1 = _block[1] & 0xff;
                    int b2 = _block[2] & 0xff;
                    _loopCount = (b2 << 8) | b1;
                }
            } while ((_blockSize > 0) && !Error());
        }

        /**
         * Reads next 16-bit value, LSB first
         */
        protected int ReadShort()
        {
            // read 16-bit value, LSB first
            return Read() | (Read() << 8);
        }

        /**
         * Resets frame state for reading next image.
         */
        protected void ResetFrame()
        {
            _lastDispose = _dispose;
            _lastRect = new Rectangle(_ix, _iy, _iw, _ih);
            _lastImage = _image;
            _lastBgColor = _bgColor;
            //		int dispose = 0;
            _lct = null;
        }

        /**
         * Skips variable length blocks up to and including
         * next zero length block.
         */
        /// <summary>
        /// 
        /// </summary>
        protected void Skip()
        {
            do
            {
                ReadBlock();
            } while ((_blockSize > 0) && !Error());
        }
    }
}

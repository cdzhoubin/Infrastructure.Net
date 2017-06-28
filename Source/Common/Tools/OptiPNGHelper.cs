using System.Diagnostics;
using System.IO;
using System.Management.Instrumentation;

namespace Zhoubin.Infrastructure.Common.Tools
{
    /// <summary>
    /// PNG图片优化
    /// </summary>
    public static class OptiPNGHelper
    {
        /// <summary>
        /// 优化PNG
        /// </summary>
        /// <param name="inputStream">待优化流</param>
        /// <param name="toolFile">优化工具</param>
        /// <returns>返回优化文件流</returns>
        public static Stream Optimization(Stream inputStream, string toolFile = ".\\optipng.exe")
        {
            var inputFile = Path.GetTempFileName();
            var buffer = new byte[1024];
            try
            {
                using (var fs = new FileStream(inputFile, FileMode.OpenOrCreate))
                {
                    long total = 0;
                    var count = inputStream.Read(buffer, 0, 1024);
                    while (count > 0 && total < inputStream.Length)
                    {
                        total += count;
                        fs.Write(buffer, 0, count);
                        count = inputStream.Read(buffer, 0, 1024);
                    }
                }

                Optimization(inputFile, null, toolFile);
                return new MemoryStream(File.ReadAllBytes(inputFile));
            }
            finally
            {
                try
                {
                    if (File.Exists(inputFile))
                    {
                        File.Delete(inputFile);
                    }
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch //clear not rethrow
                {
                }

            }


        }

        /// <summary>
        /// 优化PNG
        /// </summary>
        /// <param name="inputFile">输入文件</param>
        /// <param name="outputFile">输出文件</param>
        /// <param name="toolFile">优化工具</param>
        /// <returns></returns>
        public static void Optimization(string inputFile, string outputFile, string toolFile = ".\\optipng.exe")
        {
            var fi = new FileInfo(toolFile);
            if (!fi.Exists)
            {
                throw new InstrumentationException("转换程序optipng.exe路径不正确。");
            }


            var info = new ProcessStartInfo
            {
                FileName = fi.FullName,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = string.Format("\"{0}\" -out \"{1}\" -o 7 −quiet",inputFile,outputFile),
                UseShellExecute = true,
            };

            info.Arguments = string.IsNullOrEmpty(outputFile) ? string.Format("\"{0}\" -o7 -quiet", inputFile) : string.Format("\"{0}\" -out=\"{1}\" -o7 -quiet", inputFile, outputFile);
            if (fi.DirectoryName != null)
                info.WorkingDirectory = fi.DirectoryName;


            // Use Process for the application.
            using (Process exe = Process.Start(info))
            {
                if (exe == null)
                {
                    return;
                }

                exe.WaitForExit();
            }

        }
    }
}

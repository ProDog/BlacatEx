using System;
using System.IO;

namespace CES
{
    public class Logger : IDisposable
    {
        private FileStream stream;
        private StreamWriter writer;

        public Logger(string path)
        {
            stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.None);
            writer = new StreamWriter(stream);
            writer.AutoFlush = true;
        }

        public void Dispose()
        {
            writer.Dispose();
            stream.Dispose();
        }

        public void Log(string message)
        {
            DateTime now = DateTime.Now;
            string line = $"[{now.TimeOfDay:hh\\:mm\\:ss\\.fff}] {message}";
            Console.WriteLine(line);
            writer.WriteLine(line);
        }
    }
}

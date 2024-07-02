using System;
using System.IO;
using UnityEngine;

namespace TicTacToe
{
    public class TextLog : IDisposable
    {
        private readonly StreamWriter _streamWriter;

        public TextLog(string name )
        {
            var logStarted = DateTime.Now;
            var fileName = $"{name}_" + logStarted.ToString("yyyy-M-d-hh-mm-ss") + ".txt";
            var path = Application.dataPath;
            var fullPath = Path.Combine(path, fileName);
            try
            {
                _streamWriter = !File.Exists(fullPath) ? File.CreateText(fullPath) : new StreamWriter(File.Open(fullPath, FileMode.Append, FileAccess.Write));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void AppendLine(string line)
        {
            _streamWriter.WriteLine(line);
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        public void Close()
        {
            _streamWriter?.Close();
            _streamWriter?.Dispose();
        }
    }
}
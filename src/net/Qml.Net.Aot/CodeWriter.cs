using System;
using System.IO;

namespace Qml.Net.Aot
{
    public class CodeWriter : IDisposable
    {
        private readonly Stream _stream;
        private readonly StreamWriter _streamWriter;
        private int _tabCount;
        
        public CodeWriter(Stream stream)
        {
            _streamWriter = new StreamWriter(stream);    
        }

        public CodeWriter(string file)
        {
            _stream = File.OpenWrite(file);
            _streamWriter = new StreamWriter(_stream);
        }
        
        public void Indent()
        {
            _tabCount++;
        }
        
        public void Outdent()
        {
            _tabCount--;
        }

        public IDisposable BeginIndent()
        {
            return new IndentSession(this);
        }

        public void WriteLine(string value)
        {
            for (var x = 0; x < _tabCount; x++)
            {
                _streamWriter.Write("\t");
            }
            _streamWriter.WriteLine(value);
        }
        
        private class IndentSession : IDisposable
        {
            private readonly CodeWriter _writer;

            public IndentSession(CodeWriter writer)
            {
                _writer = writer;
                _writer._tabCount++;
            }
            
            public void Dispose()
            {
                _writer._tabCount--;
            }
        }

        public void Dispose()
        {
            _streamWriter.Dispose();
            _stream?.Dispose();
        }
    }
}
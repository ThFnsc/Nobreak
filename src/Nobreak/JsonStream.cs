using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nobreak
{
    public class JsonStream : Stream, IDisposable
    {
        private readonly IEnumerator<object> _enumerator;
        private bool _disposed = false;
        private byte[] overflow;
        private int items = 0;
        private bool _finalized = false;
        private bool _initialized = false;

        public JsonStream(IEnumerable<object> enumerable)
        {
            _enumerator = enumerable.GetEnumerator();
        }

        public override bool CanRead => !_disposed;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_disposed)
                throw new ObjectDisposedException("JsonStream disposed");
            int counter = 0;
            while (true)
            {
                byte[] item;
                if (overflow != null)
                    item = overflow;
                else if (!_initialized)
                {
                    item = "[".ToByteArray();
                    _initialized = true;
                }
                else if (_enumerator.MoveNext())
                {
                    var json = JsonConvert.SerializeObject(_enumerator.Current, Formatting.None);
                    item = (items == 0 ? json : ',' + json).ToByteArray();
                    items++;
                }
                else if (_finalized)
                    break;
                else
                {
                    _finalized = true;
                    item = "]".ToByteArray();
                }
                var destination = offset + counter;
                var available = count - destination;
                var remainder = item.Length - available;
                Array.ConstrainedCopy(item, 0, buffer, destination, remainder <= 0 ? item.Length : available);
                if (remainder <= 0 && overflow != null)
                    overflow = null;
                counter += item.Length;
                if (remainder > 0)
                {
                    overflow = new byte[remainder];
                    Array.ConstrainedCopy(item, available, overflow, 0, remainder);
                    return count;
                }
            }
            return counter;
        }

        public override long Length => throw new InvalidOperationException();

        public override long Position { get => throw new InvalidOperationException(); set => throw new InvalidOperationException(); }

        public override void Flush() =>
            throw new InvalidOperationException();

        public override long Seek(long offset, SeekOrigin origin) =>
            throw new InvalidOperationException();

        public override void SetLength(long value) =>
            throw new InvalidOperationException();

        public override void Write(byte[] buffer, int offset, int count) =>
            throw new InvalidOperationException();

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                _enumerator?.Dispose();
            }
        }
    }
}
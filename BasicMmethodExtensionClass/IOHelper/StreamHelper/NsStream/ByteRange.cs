using System;
using System.Collections.Generic;
using System.Diagnostics;
using Helper.Byte;

namespace BasicMmethodExtensionClass.IOHelper.StreamHelper.NsStream
{
    /// <summary>
    /// 表示字节数组范围   
    /// </summary>
    [DebuggerDisplay("Offset = {Offset}, Count = {Count}")]
    [DebuggerTypeProxy(typeof(DebugView))]
    public sealed class ByteRange : IByteRange
    {
        /// <summary>
        /// 获取偏移量
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// 获取字节数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 获取字节数组
        /// </summary>
        public byte[] Buffer { get; set; }

        /// <summary>
        /// 表示字节数组范围
        /// </summary>
        /// <param name="buffer">字节数组</param>   
        /// <exception cref="ArgumentNullException"></exception>
        public ByteRange(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }

            Buffer = buffer;
            Count = buffer.Length;
            Offset = 0;
        }

        /// <summary>
        /// 表示字节数组范围
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <param name="offset">偏移量</param>
        /// <param name="count">字节数</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ByteRange(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }

            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset", "offset值无效");
            }

            if (count < 0 || (offset + count) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", "count值无效");
            }
            Buffer = buffer;
            Offset = offset;
            Count = count;
        }

        /// <summary>
        /// 分割为大小相等的ByteRange集合
        /// </summary>
        /// <param name="size">新的ByteRange大小</param>
        /// <returns></returns>
        public IEnumerable<ByteRange> SplitBySize(int size)
        {
            if (size >= Count)
            {
                yield return this;
                yield break;
            }

            var remain = Count % size;
            var count = Count - remain;

            var offset = 0;
            while (offset < count)
            {
                yield return new ByteRange(Buffer, Offset + offset, size);
                offset = offset + size;
            }

            if (remain > 0)
            {
                yield return new ByteRange(Buffer, offset, remain);
            }
        }

        /// <summary>
        /// 从byte[]隐式转换
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static implicit operator ByteRange(byte[] buffer)
        {
            return new ByteRange(buffer);
        }

        /// <summary>
        /// 从ArraySegment隐式转换
        /// </summary>
        /// <param name="arraySegment"></param>
        /// <returns></returns>
        public static implicit operator ByteRange(ArraySegment<byte> arraySegment)
        {
            return new ByteRange(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
        }

        /// <summary>
        /// 调试视图
        /// </summary>
        private class DebugView
        {
            /// <summary>
            /// 查看的对象
            /// </summary>
            private readonly ByteRange _view;

            /// <summary>
            /// 调试视图
            /// </summary>
            /// <param name="view">查看的对象</param>
            public DebugView(ByteRange view)
            {
                _view = view;
            }

            /// <summary>
            /// 查看的内容
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public byte[] Values
            {
                get
                {
                    var byteArray = new byte[_view.Count];
                    System.Buffer.BlockCopy(_view.Buffer, _view.Offset, byteArray, 0, _view.Count);
                    return byteArray;
                }
            }
        }
    }
}

using System;

namespace BasicMmethodExtensionClass.EncryptHelper
{
    /// <summary>
    /// 密码学描述。
    /// </summary>
    public class Rc4CryptHelper : IDisposable
    {
        byte[] _s;
        byte[] _;
        public byte[] K { get; private set; }
        public Rc4CryptHelper() { }
        public Rc4CryptHelper(byte[] key)
        {
            Key = key;
        }
        public byte[] Key { get; set; }

        //初始化状态向量S和临时向量T，供keyStream方法调用
        void Initial()
        {
            if (_s == null || _ == null)
            {
                _s = new byte[256];
                _ = new byte[256];
            }
            for (int i = 0; i < 256; ++i)
            {
                _s[i] = (byte)i;
                _[i] = Key[i % Key.Length];
            }
        }
        //初始排列状态向量S，供keyStream方法调用
        void Ranges()
        {
            int j = 0;
            for (int i = 0; i < 256; ++i)
            {
                j = (j + _s[i] + _[i]) & 0xff;
                _s[i] = (byte)((_s[i] + _s[j]) & 0xff);
                _s[j] = (byte)((_s[i] - _s[j]) & 0xff);
                _s[i] = (byte)((_s[i] - _s[j]) & 0xff);
            }
        }
        //生成密钥流
        //len:明文为len个字节
        void KeyStream(int len)
        {
            Initial();
            Ranges();
            int i = 0, j = 0, t = 0;
            K = new byte[len];
            for (int r = 0; r < len; r++)
            {
                i = (i + 1) & 0xff;
                j = (j + _s[i]) & 0xff;

                _s[i] = (byte)((_s[i] + _s[j]) & 0xff);
                _s[j] = (byte)((_s[i] - _s[j]) & 0xff);
                _s[i] = (byte)((_s[i] - _s[j]) & 0xff);

                t = (_s[i] + _s[j]) & 0xff;
                K[r] = _s[t];
            }
        }

        public byte[] EncryptByte(byte[] data)
        {
            //生产密匙流
            KeyStream(data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                K[i] = (byte)(data[i] ^ K[i]);
            }
            return K;
        }

        public byte[] DecryptByte(byte[] data)
        {
            return EncryptByte(data);
        }

        //是否回收完毕
        bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~Rc4CryptHelper()
        {
            Dispose(false);
        }
        //这里的参数表示示是否需要释放那些实现IDisposable接口的托管对象
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;//如果已经被回收，就中断执行
            if (disposing)
            {
                //TODO:释放那些实现IDisposable接口的托管对象

            }
            //TODO:释放非托管资源，设置对象为null
            _s = null;
            _ = null;
            Key = null;
            K = null;
            _disposed = true;
        }
    }
}
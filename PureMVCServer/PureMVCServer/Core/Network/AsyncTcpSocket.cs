using System;
using System.Net.Sockets;

namespace PureNet.Network
{
    /// <summary>
    /// 异步Tcp套接字类
    /// </summary>
    public abstract class AsyncTcpSocket
    {
        // 数据缓冲区
        protected byte[] _buffer;

        // 数据缓冲区大小
        protected int _bufferLength;

        // 套接字
        protected Socket _socket;

        public Socket _Socket { get { return _socket; } }

        public AsyncTcpSocket(Socket socket, int bufferLength = 1024)
        {
            _bufferLength = bufferLength;
            _buffer = new byte[_bufferLength];
            _socket = socket;
            _socket.ReceiveBufferSize = bufferLength;
        }

        /// <summary>
        /// 异步发送
        /// </summary>
        /// <param name="bytes"></param>
        public virtual void AsyncSend(byte[] bytes)
        {
            if (_socket.Connected)
                _socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, SendCallBack, _socket);
        }

        /// <summary>
        /// 异步接收
        /// </summary>
        protected virtual void AsyncReceive()
        {
            if (_socket.Connected)
                _socket.BeginReceive(_buffer, 0, _bufferLength, SocketFlags.None, ReceiveCallBack, null);
        }

        /// <summary>
        /// 异步接收回调
        /// </summary>
        /// <param name="ar"></param>
        protected virtual void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                int count = _socket.EndReceive(ar);
                if (count <= 0)
                {
                    Close();
                    return;
                }

                ReceiveProcess(count);
                AsyncReceive();
            }
            catch (Exception)
            {
                Close();
            }
        }

        /// <summary>
        /// 异步发送回调
        /// </summary>
        /// <param name="ar"></param>
        protected virtual void SendCallBack(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndSend(ar);
            }
            catch (Exception e)
            {
                Console.WriteLine("发送失败:{0}", e.Message);
            }
        }

        public abstract void Close();

        protected abstract void ReceiveProcess(int receiveCount);
    }
}
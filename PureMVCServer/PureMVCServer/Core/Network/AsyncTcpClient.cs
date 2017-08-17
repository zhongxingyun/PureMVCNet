using System;
using System.Linq;
using System.Net.Sockets;
using PureMVC.Patterns.Facade;
using PureMVC.Interfaces;
using PureNet.Data;
using PureNet.Command;

namespace PureNet.Network
{
    /// <summary>
    /// 异步客户端类
    /// </summary>
    public class AsyncTcpClient : AsyncTcpSocket
    {
        private DataBuffer _databuffer = new DataBuffer();
        private SocketData _socketData = new SocketData();
        private CommandHandler _commandHandler;

        /// <summary>
        /// PureMVC控制接口
        /// </summary>
        public IFacade _Facade { get; private set; }

        public AsyncTcpClient(Socket socket, int bufferLength = 1024) : base(socket, bufferLength)
        {
            _Facade = new NetFacade(socket.RemoteEndPoint.ToString());
            _commandHandler = new CommandHandler(this);
            AsyncReceive();
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public override void Close()
        {
            Console.WriteLine("{0} 连接断开", _Socket.RemoteEndPoint);
            AsyncClientPool.RemoveClient(this);
            Facade.RemoveCore(_Socket.RemoteEndPoint.ToString());
            _Socket.Close();
            if (_commandHandler != null) _commandHandler.Close();
        }

        /// <summary>
        /// 以二进制方式发送
        /// </summary>
        /// <param name="_commandType"></param>
        /// <param name="_byteStreamBuffer"></param>
        public void SendMessage(int _commandType, ByteStreamBuffer _byteStreamBuffer)
        {
            SendMessageBase(_commandType, _byteStreamBuffer.ToArray());
        }

        /// <summary>
        /// 接收处理过程
        /// </summary>
        /// <param name="receiveCount"></param>
        protected override void ReceiveProcess(int receiveCount)
        {
            _databuffer.AddBuffer(_buffer, receiveCount);//将收到的数据添加到缓存器中
            while (_databuffer.GetData(out _socketData))//取出一条完整数据
            {
                _commandHandler.AddCommand(_socketData);
            }
        }

        /// <summary>
        /// 发送消息基本方法
        /// </summary>
        /// <param name="_commandType"></param>
        /// <param name="_data"></param>
        private void SendMessageBase(int _commandType, byte[] _data)
        {
            if (_Socket == null || !_Socket.Connected)
            {
                return;
            }

            byte[] _messagedata = DataToBytes(_commandType, _data);
            AsyncSend(_messagedata);
        }

        /// <summary>
        /// 数据转网络结构
        /// </summary>
        /// <param name="_commandType"></param>
        /// <param name="_data"></param>
        /// <returns></returns>
        private SocketData BytesToSocketData(int _commandType, byte[] _data)
        {
            SocketData tempSocketData = new SocketData();
            tempSocketData._buffLength = Constants.HEAD_LEN + _data.Length;
            tempSocketData._dataLength = _data.Length;
            tempSocketData._commandType = _commandType;
            tempSocketData._data = _data;
            return tempSocketData;
        }

        /// <summary>
        /// 网络结构转数据
        /// </summary>
        /// <param name="tempSocketData"></param>
        /// <returns></returns>
        private byte[] SocketDataToBytes(SocketData tempSocketData)
        {
            byte[] _tempBuff = new byte[tempSocketData._buffLength];
            byte[] _tempBuffLength = BitConverter.GetBytes(tempSocketData._buffLength);
            byte[] _tempDataLenght = BitConverter.GetBytes(tempSocketData._commandType);

            Array.Copy(_tempBuffLength, 0, _tempBuff, 0, Constants.HEAD_DATA_LEN);//缓存总长度
            Array.Copy(_tempDataLenght, 0, _tempBuff, Constants.HEAD_DATA_LEN, Constants.HEAD_TYPE_LEN);//协议类型
            Array.Copy(tempSocketData._data, 0, _tempBuff, Constants.HEAD_LEN, tempSocketData._dataLength);//协议数据

            return _tempBuff;
        }

        /// <summary>
        /// 合并协议，数据
        /// </summary>
        /// <param name="_commandType"></param>
        /// <param name="_data"></param>
        /// <returns></returns>
        private byte[] DataToBytes(int _commandType, byte[] _data)
        {
            return SocketDataToBytes(BytesToSocketData(_commandType, _data));
        }
    }
}
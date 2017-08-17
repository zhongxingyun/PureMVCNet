using System;
using System.Collections.Concurrent;
using System.Threading;
using PureNet.Data;
using PureNet.Network;

namespace PureNet.Command
{
    /// <summary>
    /// 命令处理类
    /// </summary>
    public class CommandHandler
    {
        private AsyncTcpClient _client;
        private Thread _handlerThread;
        private ConcurrentQueue<SocketData> _commandQueue = new ConcurrentQueue<SocketData>();
        private ManualResetEvent _mre = new ManualResetEvent(false);
        private bool isClose = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_client"></param>
        public CommandHandler(AsyncTcpClient _client)
        {
            this._client = _client;
            _handlerThread = new Thread(HandlerThread);
            _handlerThread.IsBackground = true;
            _handlerThread.Start();
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            if (_handlerThread != null)
            {
                isClose = true;
                _mre.Set();
                if (_handlerThread.ThreadState != ThreadState.Stopped)
                    _handlerThread.Abort();
            }
        }

        /// <summary>
        /// 添加处理命令
        /// </summary>
        /// <param name="data"></param>
        public void AddCommand(SocketData data)
        {
            _commandQueue.Enqueue(data);
            _mre.Set();
        }

        /// <summary>
        /// 命令处理线程
        /// </summary>
        private void HandlerThread()
        {
            try
            {
                do
                {
                    SocketData result;
                    while (_commandQueue.TryDequeue(out result))
                    {
                        NetCommandData _data = new NetCommandData();
                        _data.Client = _client;
                        _data.Data = new ByteStreamBuffer(result._data);
                        _client._Facade.SendNotification(((PureNet.Command.NetCommandType)result._commandType).ToString(), _data);
                    }
                    _mre.Reset();
                }
                while (!isClose);
            }
            catch (Exception)
            {

            }
        }
    }
}

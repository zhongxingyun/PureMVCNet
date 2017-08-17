using System;
using System.Net;
using System.Net.Sockets;

namespace PureNet.Network
{
    /// <summary>
    /// 服务器类
    /// </summary>
    public class AsncTcpServer
    {
        /// <summary>
        /// 监听套接字
        /// </summary>
        public Socket listen;

        /// <summary>
        /// 启动服务器
        /// </summary>
        public void Start(string host, int port)
        {
            listen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse(host);
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
            listen.Bind(ipEndPoint);
            listen.Listen(100);
            listen.BeginAccept(AcceptCallBack, null);
            Console.WriteLine("服务器启动成功！");
        }

        /// <summary>
        /// 异步建立客户端连接回调
        /// </summary>
        private void AcceptCallBack(IAsyncResult ar)
        {
            try
            {
                Socket _socket = listen.EndAccept(ar);

                AsyncClientPool.AddClient(new AsyncTcpClient(_socket));
                Console.WriteLine("客户端连接 [{0}]", _socket.RemoteEndPoint.ToString());

                listen.BeginAccept(AcceptCallBack, null);
            }
            catch (Exception e)
            {
                Console.WriteLine("异步建立客户端连接失败：" + e.Message);
            }
        }
    }
}

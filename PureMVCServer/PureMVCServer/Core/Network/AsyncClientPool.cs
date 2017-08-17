using System.Collections.Generic;
using System.Collections.Concurrent;

namespace PureNet.Network
{
    /// <summary>
    /// 异步客户端对象池
    /// </summary>
    public class AsyncClientPool
    {
        private static BlockingCollection<AsyncTcpClient> Pool = new BlockingCollection<AsyncTcpClient>();

        /// <summary>
        /// 添加客户端
        /// </summary>
        /// <param name="client"></param>
        public static void AddClient(AsyncTcpClient client)
        {
            Pool.Add(client);
        }

        /// <summary>
        /// 移除客户端
        /// </summary>
        /// <param name="client"></param>
        public static void RemoveClient(AsyncTcpClient client)
        {
            Pool.TryTake(out client);
        }

        /// <summary>
        /// 获取客户端
        /// </summary>
        /// <param name="clientAddress">客户端地址</param>
        /// <returns></returns>
        public static AsyncTcpClient GetClient(string clientAddress)
        {
            foreach (var _client in GetEnumerable())
            {
                if (_client._Socket.Connected && _client._Socket.RemoteEndPoint.ToString() == clientAddress)
                {
                    return _client;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取对象池枚举器
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<AsyncTcpClient> GetEnumerable()
        {
            return Pool;
        }
    }
}

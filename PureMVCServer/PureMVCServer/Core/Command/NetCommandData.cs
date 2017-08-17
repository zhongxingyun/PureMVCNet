using PureNet.Data;
using PureNet.Network;

namespace PureNet.Command
{
    /// <summary>
    /// 网络命令数据结构
    /// </summary>
    public struct NetCommandData
    {
        public AsyncTcpClient Client;
        public ByteStreamBuffer Data;
    }
}

namespace PureNet.Data
{
    /// <summary>
    /// 套接字数据
    /// </summary>
    [System.Serializable]
    public struct SocketData
    {
        public byte[] _data;
        public int _commandType;
        public int _buffLength;
        public int _dataLength;
    }
}

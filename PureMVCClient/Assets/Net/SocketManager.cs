using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System;
using System.IO;
using System.Reflection;
using PureMVC.Core;
using PureMVC.Patterns;
using PureMVC.Interfaces;

public class SocketManager
{
    private static SocketManager _instance;
    public static SocketManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SocketManager();
            }
            return _instance;
        }
    }
    private string _currentIP;
    private int _currentPort;

    private bool _isConnected = false;
    public bool IsConnceted { get { return _isConnected; } }
    private Socket clientSocket = null;
    private Thread receiveThread = null;

    private DataBuffer _databuffer = new DataBuffer();

    byte[] _tempReceiveBuff = new byte[4096];
    private SocketData _socketData = new SocketData();

    public SocketManager()
    {
        InitializeNetCommand();
    }

    /// <summary>
    /// 初始化网络命令
    /// </summary>
    private void InitializeNetCommand()
    {
        Type _netCommand = typeof(PureNet.Command.NetCommand);
        Assembly assembly = Assembly.GetExecutingAssembly();
        foreach (var type in assembly.GetTypes())
        {
            if (type.IsSubclassOf(_netCommand))
            {
                string _commandName = type.Name;
                PropertyInfo _property = type.GetProperty("CommandType");
                int _commandType = (int)_property.GetValue(type, null);
                ApplicationFacade.Instance.RegisterCommand(_commandName, type);
                MessageCenter.Instance.AddCommandLisitener(_commandType, _commandName);
            }
        }
    }

    /// <summary>
    /// 断开
    /// </summary>
    private void _close()
    {
        if (!_isConnected)
            return;

        _isConnected = false;

        if (receiveThread != null)
        {
            receiveThread.Abort();
            receiveThread = null;
        }

        if (clientSocket != null && clientSocket.Connected)
        {
            clientSocket.Close();
            clientSocket = null;
        }
    }

    private void _ReConnect()
    {
    }

    /// <summary>
    /// 连接
    /// </summary>
    private void _onConnet()
    {
        try
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建套接字
            IPAddress ipAddress = IPAddress.Parse(_currentIP);//解析IP地址
            IPEndPoint ipEndpoint = new IPEndPoint(ipAddress, _currentPort);
            IAsyncResult result = clientSocket.BeginConnect(ipEndpoint, new AsyncCallback(_onConnect_Sucess), clientSocket);//异步连接
            Debug.Log("开始连接....");
            bool success = result.AsyncWaitHandle.WaitOne(5000, true);
            if (!success) //超时
            {
                _onConnect_Outtime();
            }
            Debug.Log(success);
        }
        catch (System.Exception)
        {
            _onConnect_Fail();
        }
    }

    private void _onConnect_Sucess(IAsyncResult iar)
    {
        try
        {
            Socket client = (Socket)iar.AsyncState;
            client.EndConnect(iar);

            receiveThread = new Thread(new ThreadStart(_onReceiveSocket));
            receiveThread.IsBackground = true;
            receiveThread.Start();
            _isConnected = true;
            Debug.Log("连接成功");
        }
        catch (Exception _e)
        {
            Debug.Log("连接异常：" + _e.Message);
            Close();
        }
    }

    private void _onConnect_Outtime()
    {
        Debug.Log("连接超时");
        _close();
    }

    private void _onConnect_Fail()
    {
        Debug.Log("连接错误");
        _close();
    }

    /// <summary>
    /// 发送消息结果回掉，可判断当前网络状态
    /// </summary>
    /// <param name="asyncSend"></param>
    private void _onSendMsg(IAsyncResult asyncSend)
    {
        try
        {
            Socket client = (Socket)asyncSend.AsyncState;
            client.EndSend(asyncSend);
        }
        catch (Exception e)
        {
            Debug.Log("send msg exception:" + e.StackTrace);
        }
    }

    /// <summary>
    /// 接受网络数据
    /// </summary>
    private void _onReceiveSocket()
    {
        while (true)
        {
            if (!clientSocket.Connected)
            {
                _isConnected = false;
                _ReConnect();
                break;
            }
            try
            {
                int receiveLength = clientSocket.Receive(_tempReceiveBuff);
                if (receiveLength > 0)
                {
                    _databuffer.AddBuffer(_tempReceiveBuff, receiveLength);//将收到的数据添加到缓存器中
                    while (_databuffer.GetData(out _socketData))//取出一条完整数据
                    {
                        Debug.Log("取出一条数据");
                        Event_NetCommand tempNetMessageData = new Event_NetCommand();
                        tempNetMessageData._commandType = _socketData._commandType;
                        tempNetMessageData._commandData = new ByteStreamBuffer(_socketData._data);

                        //锁死消息中心消息队列，并添加数据
                        lock (MessageCenter.Instance.NetMessageDataQueue)
                        {
                            MessageCenter.Instance.NetMessageDataQueue.Enqueue(tempNetMessageData);
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                clientSocket.Disconnect(true);
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
                break;
            }
        }
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


    /// <summary>
    /// ProtoBuf序列化
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /*public static byte[] ProtoBuf_Serializer(ProtoBuf.IExtensible data)
    {
        using (MemoryStream m = new MemoryStream())
        {
            byte[] buffer = null;
            Serializer.Serialize(m, data);
            m.Position = 0;
            int length = (int)m.Length;
            buffer = new byte[length];
            m.Read(buffer, 0, length);
            return buffer;
        }
    }*/

    /// <summary>
    /// ProtoBuf反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_data"></param>
    /// <returns></returns>
    /*public static T ProtoBuf_Deserialize<T>(byte[] _data)
    {
        using (MemoryStream m = new MemoryStream(_data))
        {
            return Serializer.Deserialize<T>(m);
        }
    }*/



    /// <summary>
    /// 连接服务器
    /// </summary>
    /// <param name="_currIP"></param>
    /// <param name="_currPort"></param>
    public void Connect(string _currIP, int _currPort)
    {
        if (!IsConnceted)
        {
            this._currentIP = _currIP;
            this._currentPort = _currPort;
            _onConnet();
        }
    }

    /// <summary>
    /// 发送消息基本方法
    /// </summary>
    /// <param name="_commandType"></param>
    /// <param name="_data"></param>
    private void SendMsgBase(int _commandType, byte[] _data)
    {
        if (clientSocket == null || !clientSocket.Connected)
        {
            _ReConnect();
            return;
        }

        byte[] _msgdata = DataToBytes(_commandType, _data);
        clientSocket.BeginSend(_msgdata, 0, _msgdata.Length, SocketFlags.None, new AsyncCallback(_onSendMsg), clientSocket);
    }

    /// <summary>
    /// 以二进制方式发送
    /// </summary>
    /// <param name="_commandType"></param>
    /// <param name="_byteStreamBuffer"></param>
    public void SendMsg(int _commandType, ByteStreamBuffer _byteStreamBuffer)
    {
        SendMsgBase(_commandType, _byteStreamBuffer.ToArray());
    }

    /// <summary>
    /// 以ProtoBuf方式发送
    /// </summary>
    /// <param name="_commandType"></param>
    /// <param name="data"></param>
    /*public void SendMsg(int _commandType, ProtoBuf.IExtensible data)
    {
        SendMsgBase(_commandType, ProtoBuf_Serializer(data));
    }*/

    public void Close()
    {
        _close();
    }

}
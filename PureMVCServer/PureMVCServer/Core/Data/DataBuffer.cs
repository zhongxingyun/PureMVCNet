using System;

namespace PureNet.Data
{
    /// <summary>
    /// 网络数据缓存器，自动大小
    /// </summary>
    public class DataBuffer
    {
        //缓冲区
        private byte[] _buffer;

        //最小缓冲区长度
        private int _minBufferLength;

        //当前缓冲区位置
        private int _currentBufferPosition;

        //缓冲区总长度
        private int _bufferLength;

        //数据长度
        private int _dataLength;

        //命令类型
        private int _commandType;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_minBufferLength">最小缓冲区大小</param>
        public DataBuffer(int _minBufferLength = 1024)
        {
            if (_minBufferLength <= 0)
            {
                this._minBufferLength = 1024;
            }
            else
            {
                this._minBufferLength = _minBufferLength;
            }
            _buffer = new byte[this._minBufferLength];
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="_data">缓存数据</param>
        /// <param name="_count">缓存长度</param>
        public void AddBuffer(byte[] _data, int _count)
        {
            if (_count > _buffer.Length - _currentBufferPosition) //超过当前缓存
            {
                byte[] _tempBuffer = new byte[_currentBufferPosition + _count];
                Array.Copy(_buffer, 0, _tempBuffer, 0, _currentBufferPosition);
                Array.Copy(_data, 0, _tempBuffer, _currentBufferPosition, _count);
                _buffer = _tempBuffer;
                _tempBuffer = null;
            }
            else
            {
                Array.Copy(_data, 0, _buffer, _currentBufferPosition, _count);
            }
            _currentBufferPosition += _count;
        }

        /// <summary>
        /// 更新数据长度
        /// </summary>
        public void UpdateDataLength()
        {
            if (_dataLength == 0 && _currentBufferPosition >= Constants.HEAD_LEN)
            {
                byte[] _tempDataLength = new byte[Constants.HEAD_DATA_LEN];
                Array.Copy(_buffer, 0, _tempDataLength, 0, Constants.HEAD_DATA_LEN);

                byte[] _tempCommandType = new byte[Constants.HEAD_TYPE_LEN];
                Array.Copy(_buffer, Constants.HEAD_DATA_LEN, _tempCommandType, 0, Constants.HEAD_TYPE_LEN);

                _bufferLength = BitConverter.ToInt32(_tempDataLength, 0);
                _commandType = BitConverter.ToInt32(_tempCommandType, 0);

                _dataLength = _bufferLength - Constants.HEAD_LEN;
            }
        }

        /// <summary>
        /// 获取一条可用数据
        /// </summary>
        /// <param name="_tmpSocketData">可用数据</param>
        /// <returns>是否有数据</returns>
        public bool GetData(out SocketData _tempSocketData)
        {
            _tempSocketData = new SocketData();

            if (_bufferLength <= 0)
            {
                UpdateDataLength();
            }

            if (_bufferLength > 0 && _currentBufferPosition >= _dataLength)
            {
                _tempSocketData._buffLength = _bufferLength;
                _tempSocketData._dataLength = _dataLength;
                _tempSocketData._commandType = _commandType;
                _tempSocketData._data = new byte[_dataLength];
                Array.Copy(_buffer, Constants.HEAD_LEN, _tempSocketData._data, 0, _dataLength);

                _currentBufferPosition -= _bufferLength;
                byte[] _tempBuffer = new byte[_currentBufferPosition < _minBufferLength ? _minBufferLength : _currentBufferPosition];
                Array.Copy(_buffer, _bufferLength, _tempBuffer, 0, _currentBufferPosition);
                _buffer = _tempBuffer;

                _bufferLength = 0;
                _dataLength = 0;
                _commandType = 0;
                return true;
            }
            return false;
        }
    }
}

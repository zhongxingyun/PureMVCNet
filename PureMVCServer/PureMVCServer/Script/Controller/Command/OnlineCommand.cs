using System.Collections.Generic;
using PureMVC.Interfaces;
using PureNet.Network;
using PureNet.Data;

namespace PureNet.Command
{
    public class OnlineCommand : NetCommand, ICommand
    {
        public override int CommandType
        {
            get
            {
                return (int)NetCommandType.OnlineCommand;
            }
        }

        protected override void Process(NetCommandData data)
        {
            List<string> onlineList = new List<string>();
            foreach (var client in AsyncClientPool.GetEnumerable())
            {
                onlineList.Add(client._Socket.RemoteEndPoint.ToString());
            }
            ByteStreamBuffer _steamBuffer = new ByteStreamBuffer();
            _steamBuffer.WriteObject(onlineList);
            data.Client.SendMessage(100, _steamBuffer);
        }
    }
}

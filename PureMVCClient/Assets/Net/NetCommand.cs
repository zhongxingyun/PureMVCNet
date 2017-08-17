using UnityEngine;
using PureMVC.Patterns;
using PureMVC.Interfaces;

namespace PureNet.Command
{
    public abstract class NetCommand : SimpleCommand, ICommand
    {
        public static int CommandType { get { return 0; } }

        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            Process((ByteStreamBuffer)notification.Body);
        }

        protected abstract void Process(ByteStreamBuffer steamBuffer);
    }
}

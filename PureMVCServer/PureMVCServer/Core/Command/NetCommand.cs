using PureMVC.Patterns.Command;
using PureMVC.Interfaces;

namespace PureNet.Command
{
    /// <summary>
    /// 网络命令基类
    /// </summary>
    public abstract class NetCommand : SimpleCommand, ICommand
    {
        /// <summary>
        /// 命令ID
        /// </summary>
        public abstract int CommandType { get; }

        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            Process((NetCommandData)notification.Body);
        }

        /// <summary>
        /// 命令处理过程
        /// </summary>
        /// <param name="data"></param>
        protected abstract void Process(NetCommandData data);
    }
}

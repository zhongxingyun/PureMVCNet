using System;
using PureMVC.Patterns.Command;
using PureMVC.Interfaces;
using System.Reflection;

namespace PureNet.Command
{
    /// <summary>
    /// 初始化命令
    /// </summary>
    public class InitializeCommand : SimpleCommand, ICommand
    {
        public override void Execute(INotification notification)
        {
            //客户端初始化添加所有网络命令
            IFacade _facade = Facade;
            Type _netCommand = typeof(PureNet.Command.NetCommand);
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(_netCommand))
                {
                    string _commandName = type.Name;
                    _facade.RegisterCommand(_commandName, () => (ICommand)Activator.CreateInstance(type));
                }
            }
        }
    }
}

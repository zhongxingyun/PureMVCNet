using PureMVC.Interfaces;
using PureMVC.Patterns.Facade;

namespace PureNet
{
    /// <summary>
    /// 网络PureMVC接口类
    /// </summary>
    public class NetFacade : Facade, IFacade
    {
        public NetFacade(string key) : base(key)
        {

        }

        protected override void InitializeFacade()
        {
            base.InitializeFacade();
            SendNotification("InitializeCommand");
        }

        protected override void InitializeController()
        {
            base.InitializeController();
            RegisterCommand("InitializeCommand", () => new PureNet.Command.InitializeCommand());
        }
    }
}

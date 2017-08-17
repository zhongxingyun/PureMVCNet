using UnityEngine;
using System.Collections;
using PureMVC.Interfaces;
using PureMVC.Patterns;

public class StartupCommand : SimpleCommand, ICommand
{
    public override void Execute(INotification notification)
    {
        base.Execute(notification);
        MainUI mainUI = (MainUI)notification.Body;
        Facade.RegisterMediator(new OnlineListMediator(mainUI.onlineUser));
    }
}

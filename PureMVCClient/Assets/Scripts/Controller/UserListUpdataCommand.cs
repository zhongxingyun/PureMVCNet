using UnityEngine;
using System.Collections.Generic;
using PureMVC.Patterns;
using PureMVC.Interfaces;

public class UserListUpdataCommand : SimpleCommand, ICommand
{
    public override void Execute(INotification notification)
    {
        IList<string> users = (IList<string>)notification.Body;
    }
}

using UnityEngine;
using System.Collections;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using System.Collections.Generic;

public class OnlineListMediator : Mediator, IMediator
{
    public new const string NAME = "OnlineListMediator";

    public OnlineUserComponent View { get { return (OnlineUserComponent)base.ViewComponent; } }

    public OnlineListMediator(OnlineUserComponent component) : base(NAME, component)
    {

    }

    public override IList<string> ListNotificationInterests()
    {
        IList<string> list = new List<string>();
        list.Add(EventsEnum.ONLINELISTUPDATE);
        return list;
    }

    public override void HandleNotification(INotification notification)
    {
        IList<string> userList = (IList<string>)notification.Body;
        View.UpdateView(userList);
    }
}

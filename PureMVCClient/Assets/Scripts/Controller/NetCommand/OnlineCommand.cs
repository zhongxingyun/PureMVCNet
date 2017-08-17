using UnityEngine;
using System.Collections.Generic;
using PureMVC.Interfaces;
using System;

namespace PureNet.Command
{
    public class OnlineCommand : NetCommand, ICommand
    {
        public new static int CommandType { get { return (int)NetCommandType.OnlineCommand; } }

        protected override void Process(ByteStreamBuffer steamBuffer)
        {
            List<string> users = steamBuffer.ReadObject<List<string>>();
            if (users != null)
            {
                Debug.Log("OnlineCommand Send Notification");
                Facade.SendNotification(EventsEnum.ONLINELISTUPDATE, users);
            }
        }
    }
}

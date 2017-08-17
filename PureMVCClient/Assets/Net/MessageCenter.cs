using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PureMVC.Core;
using PureMVC.Patterns;
using PureMVC.Interfaces;

public struct Event_NetCommand
{
    public int _commandType;
    public ByteStreamBuffer _commandData;
}

public class MessageCenter : SingletonMonoBehaviour<MessageCenter>
{
    private IFacade facade;

    public Dictionary<int, string> NetCommandList = new Dictionary<int, string>();
    public Queue<Event_NetCommand> NetMessageDataQueue = new Queue<Event_NetCommand>();

    protected override void Init()
    {
        base.Init();
        facade = ApplicationFacade.Instance;
    }

    private void Update()
    {
        while (NetMessageDataQueue.Count > 0)
        {
            Event_NetCommand _event = NetMessageDataQueue.Dequeue();
            if (NetCommandList.ContainsKey(_event._commandType))
            {
                facade.SendNotification(NetCommandList[_event._commandType], _event._commandData);
            }
        }
    }

    public void AddCommandLisitener(int _commandType, string _commandName)
    {
        if (!NetCommandList.ContainsKey(_commandType))
        {
            NetCommandList[_commandType] = _commandName;
        }
    }
}

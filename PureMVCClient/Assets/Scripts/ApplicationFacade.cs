using UnityEngine;
using System.Collections;
using PureMVC.Core;
using PureMVC.Patterns;
using PureMVC.Interfaces;

public class ApplicationFacade : Facade, IFacade
{
    public new static IFacade Instance
    {
        get
        {
            if (m_instance == null)
            {
                lock (m_staticSyncRoot)
                {
                    m_instance = new ApplicationFacade();
                }
            }
            return m_instance;
        }
    }

    public void StartUI(MainUI mainUI)
    {
        SendNotification(EventsEnum.STARTUP, mainUI);
    }

    protected override void InitializeController()
    {
        base.InitializeController();
        //RegisterCommand(EventsEnum.STARTUP, typeof(StartupCommand));
    }
}

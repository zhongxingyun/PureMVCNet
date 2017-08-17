using UnityEngine;
using System.Collections;

public class MainUI : MonoBehaviour
{
    public OnlineUserComponent onlineUser;
    private ByteStreamBuffer stream;

    private void Start()
    {
        ApplicationFacade f = ApplicationFacade.Instance as ApplicationFacade;
        f.StartUI(this);
        SocketManager s = SocketManager.Instance;
        s.Connect("127.0.0.1", 25565);
        stream = new ByteStreamBuffer();
    }

    private void Update()
    {
        if (SocketManager.Instance.IsConnceted)
        {
            SocketManager.Instance.SendMsg(100, stream);
        }
    }

    private void OnApplicationQuit()
    {
        SocketManager.Instance.Close();
    }
}

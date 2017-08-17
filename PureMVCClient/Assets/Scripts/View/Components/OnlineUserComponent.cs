using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OnlineUserComponent : MonoBehaviour
{
    public Text onlineListUI;

    public void UpdateView(IList<string> users)
    {
        onlineListUI.text = users[0];
        for (int i = 1; i < users.Count; i++)
        {
            onlineListUI.text += "\r\n" + users[i];
        }

        RectTransform onlineRect = onlineListUI.rectTransform;
        onlineRect.sizeDelta = new Vector2(onlineRect.sizeDelta.x, onlineListUI.preferredHeight);
    }
}

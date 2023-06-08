using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonUIEvents : MonoBehaviour
{
    public void OpenUIPanel(GameObject gameObject)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    public void CloseUIPanel(GameObject gameObject)
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
}

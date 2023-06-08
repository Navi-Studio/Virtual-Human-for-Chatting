using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeftEvents : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private FreeChat freeChat;

    private void Start() {

    }

    public void ExitGame(){
        #if UNITY_EDITOR    // 调试时
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    public void OpenSettingsPanel(){
        if(!settingsPanel.activeSelf){
            settingsPanel.SetActive(true);
        }
    }
    
    public void CloseSettingsPanel(){
        if (settingsPanel.activeSelf)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void ChatModeOpen()
    {
        GameObject openBTN = GameObject.Find("Canvas/Left/Chat/OpenBtn");
        GameObject closeBTN = GameObject.Find("Canvas/Left/Chat/CloseBtn");
        GameObject inputPlane = GameObject.Find("Canvas/Bottom/Input");
        if (openBTN.activeSelf)
        {
            openBTN.SetActive(false);
            inputPlane.SetActive(false);
            closeBTN.SetActive(true);
            // 开启智能对话模式
            freeChat.InitFreeChat();
            freeChat.isFreeChatMode = true;
        }
    }
    public void ChatModeClose()
    {
        GameObject openBTN = GameObject.Find("Canvas/Left/Chat/OpenBtn");
        GameObject closeBTN = GameObject.Find("Canvas/Left/Chat/CloseBtn");
        GameObject inputPlane = GameObject.Find("Canvas/Bottom/Input");
        if (closeBTN.activeSelf)
        {
            closeBTN.SetActive(false);
            inputPlane.SetActive(true);
            openBTN.SetActive(true);
            // 关闭智能对话模式
            freeChat.InitFreeChat();
        }
    }

}

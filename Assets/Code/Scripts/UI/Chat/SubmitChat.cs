using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class SubmitChat : MonoBehaviour
{
    private ChatScript chatScript;
    private AzureSpeech azureSpeech;
    [SerializeField]private TMP_InputField inputText;

    private void Start() {
        chatScript = GameObject.FindGameObjectWithTag("LM").GetComponent<ChatScript>();
        azureSpeech = GameObject.FindGameObjectWithTag("TTS").GetComponentInChildren<AzureSpeech>();
    }

    public void submit(){
        if(inputText.text.Equals("")){
            return;
        }
        string msg=inputText.text;
        // Debug.Log(msg);
        chatScript.SendData(msg); // LLM
        inputText.text="";    // 清空输入框
    }
    public void StartSTT(){
        StartSTTAsync();
    }
    public async Task StartSTTAsync(){
        await azureSpeech.TurnSpeechToText();
        string sttResult = azureSpeech.m_STTResult;
        if (sttResult.Equals(""))
        {
            return;
        }
        inputText.text += sttResult;
    }

}

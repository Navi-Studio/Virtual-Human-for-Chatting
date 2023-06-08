using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;
using System.Threading.Tasks;

public class FreeChat : MonoBehaviour
{
    private ChatScript chatScript;
    private AzureSpeech azureSpeech;
    private AudioSource audioSource;
    [HideInInspector] public bool isFreeChatMode = false;
    [HideInInspector] public bool isTriggerKeyWord = false;
    [HideInInspector] public bool isFirstTriggerKeyWord = true;
    [HideInInspector] public bool isAnswerFinish = true;


    private void Awake() {
        // 订阅事件
        AzureSpeech.OnSpeechEnd += EndThisRoundOfChat;

        chatScript = GameObject.FindGameObjectWithTag("LM").GetComponent<ChatScript>();
        azureSpeech = GameObject.FindGameObjectWithTag("TTS").GetComponentInChildren<AzureSpeech>();
        audioSource = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>();
        InitFreeChat();
    }

    private void FixedUpdate() {
        if(isFreeChatMode)
        {
            if (isTriggerKeyWord)
            {
                if (isAnswerFinish)
                {
                    isAnswerFinish = false;
                    ReceiveSTTResult();
                }
            }
            else
            {
                ReceiveKWRResult();
            }
        }  
    }

    public void ReceiveKWRResult(){
        if(isFirstTriggerKeyWord){
            isFirstTriggerKeyWord = false;
            ReceiveKWRResultAsync();
        }
    }
    async Task ReceiveKWRResultAsync()
    {
        await azureSpeech.KeyWordRecognition();
        bool result = azureSpeech.m_KeyWordResult;
        if(result){
            TTSBuilder tTSBuilder = new BasicTTSBuilder();
            TTSEntity tTSEntity = tTSBuilder.build("我在呢！");
            azureSpeech.TurnTextToSpeech(tTSEntity);
            StartCoroutine(OnKWRAudioPlayEnd());
        }
    }

    IEnumerator OnKWRAudioPlayEnd()
    {
        yield return new WaitForSeconds(3); // 等待3秒，待优化
        yield return new WaitUntil(() => audioSource.isPlaying == false);
        isTriggerKeyWord = true;
    }

    public void ReceiveSTTResult()
    {
        ReceiveSTTResultAsync(); // STT -> LLM
    }
    async Task ReceiveSTTResultAsync()
    {
        await azureSpeech.TurnSpeechToText();
        // yield return new WaitUntil(() => azureSpeech.m_STTResult != "");
        string msg = azureSpeech.m_STTResult;
        GeneratedAnswer(msg);
    }

    public void GeneratedAnswer(string _msg)
    {
        if (_msg.Equals(""))
        {
            isAnswerFinish = true;
            return;
        }
        chatScript.SendData(_msg); // LLM -> TTS -> Live2D
        azureSpeech.m_STTResult = string.Empty;
        // StartCoroutine(OnAudioPlayEnd());
    }


    // IEnumerator OnAudioPlayEnd()
    // {
    //     yield return new WaitForSeconds(5); // 等待5秒，待优化
    //     yield return new WaitUntil(() => audioSource.isPlaying == false);
    //     isAnswerFinish = true;
    // }

    private void EndThisRoundOfChat(){
        isAnswerFinish = true;
    }

    public void InitFreeChat(){
        isFreeChatMode = false;
        isTriggerKeyWord = false;
        isFirstTriggerKeyWord = true;
        isAnswerFinish = true;
        azureSpeech.m_KeyWordResult = false;
    }

}

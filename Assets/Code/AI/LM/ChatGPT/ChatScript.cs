using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Live2D.Cubism.Framework.Expression;

public class ChatScript : MonoBehaviour
{
    // API key
    [SerializeField]private string m_OpenAI_Key;
	
    // 定义Chat API的URL
	private string m_ApiUrl = "https://api.openai-proxy.com/v1/completions";

    // 配置参数
    // [SerializeField]private GetOpenAI.PostData m_PostDataSetting;

    // 微软Azure语音
    [SerializeField]private AzureSpeech m_AzurePlayer;

    // gpt-3.5-turbo
    [SerializeField]public GptTurboScript m_GptTurboScript;

    // 语言
    [SerializeField]private string m_lan="使用中文回答";

    // 气泡
    [SerializeField]private GameObject bubbleUI;

    // 接收回答的UI文本框
    [SerializeField]private TMP_Text answerText;

    // 模型表情列表
    private CubismExpressionController expressionController;

    // 回答文本
    private string m_Msg = null;

    private void Start() {
        // 订阅事件
        GameSettingsEvent.OnGameSettingChanged += SetGhatGPTSettings;
        AzureSpeech.OnSpeechStart += ShowText;

        m_OpenAI_Key = GameSettingsEntity.Instance.ChatGPTAPI;
        expressionController = GameObject.FindGameObjectWithTag("Model").GetComponent<CubismExpressionController>();
        
    }
    public void SetGhatGPTSettings(){
        m_OpenAI_Key = GameSettingsEntity.Instance.ChatGPTAPI;
    }

    private void switchExpression(){
        TTSEntity tTSEntity = new TTSEntity();
        string style = tTSEntity.Style[0];
        int expression = -1;
        switch (style)
        {
            case "customerservice": expression = -1; break;
            case "cheerful": expression = 2; break;
            case "sad": expression = 1; break;
            case "angry": expression = 13; break;
            case "fearful": expression = 0; break;
            case "disgruntled": expression = 13; break;
            case "serious": expression = 10; break;
            default: expression = -1; break;
        }
        expressionController.CurrentExpressionIndex = expression;
        // 等待音频播放完毕(协程)
        // TODO
        expressionController.CurrentExpressionIndex = -1;
    }

    // 发送信息
    public void SendData(string _postData)
    {
        if (_postData.Equals("")){
            return;
        }
        string _msg = m_GptTurboScript.m_Prompt + m_lan + " " + _postData;
        //发送数据
        StartCoroutine(m_GptTurboScript.GetPostData(_msg, m_OpenAI_Key, CallBack));
    }


    // AI回复的信息
    private void CallBack(string _callback){
        m_Msg = _callback.Trim();
        // 语音功能
        StartCoroutine(Speek(m_Msg));
    }


    private IEnumerator Speek(string _msg){
        yield return new WaitForEndOfFrame();
        // 识别情绪
        List<string> styleList = new List<string>();
        SAService sAService = new SAService();
        SAEntity sAEntity = sAService.sentimentAnalysis(_msg);
        TTSBuilder tTSBuilder = new BasicTTSBuilder();
        TTSEntity tTSEntity = tTSBuilder.build(_msg);
        tTSEntity.Style[0] = sAEntity.Style;

        // 播放合成并播放音频
        m_AzurePlayer.TurnTextToSpeech(tTSEntity);
    }

    private void ShowText(){
        if(m_Msg != null){
            bubbleUI.SetActive(true);   // 开启气泡
            answerText.text = "";
            //开始逐个显示返回的文本
            m_WriteState = true;
            StartCoroutine(SetTextPerWord(m_Msg));
            m_Msg = null;
        }
    }

    //逐字显示的时间间隔
    private float m_WordWaitTime=0.2f;
    //是否显示完成
    private bool m_WriteState=false;
    private IEnumerator SetTextPerWord(string _msg){
        // yield return new WaitUntil(() => m_AzurePlayer. == false);
        int currentPos=0;
        while(m_WriteState){
            yield return new WaitForSeconds(m_WordWaitTime);
            currentPos++;
            //更新显示的内容
            answerText.text =_msg.Substring(0,currentPos);

            m_WriteState=currentPos<_msg.Length;

        }
        yield return new WaitForSeconds(3f);
        bubbleUI.SetActive(false);
    }



}

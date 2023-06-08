using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using Assets.Code.Scripts.UI.Chat;
using System.Runtime.InteropServices;

public class ReadEvents : MonoBehaviour
{
    private ReadService readService = ReadService.Instance;
    [SerializeField] private List<GameObject> gameObjects;
    [SerializeField] private GameObject settingPanel;

    // 微软Azure语音
    [SerializeField] private AzureSpeech m_AzurePlayer;

    public void UnfoldPanel(GameObject advancedPanel)
    {
        advancedPanel.SetActive(true);
    }

    public void FoldPanel(GameObject advancedPanel)
    {
        advancedPanel.SetActive(false);
    }

    public void OnEndEdit(int number)
    {
        TMP_InputField inputText = gameObjects[number].GetComponentInChildren<TMP_InputField>();
        readService._TTSEntity.Content[number] = inputText.text;
    }


    public void addEntity()
    {
        if (!checkInput(readService.Number - 1))
        {
            return;
        }
        // TORE 写的有点乱，不想改了
        if (readService.Number < 4)
        {
            readService.addEntity();
            gameObjects[readService.Number - 1].SetActive(true);
        }
        else
        {
            Debug.Log("error: 超出上限");
        }
    }

    public void removeEntity()
    {
        if (readService.Number > 1)
        {
            readService.removeEntity();
            cleanInput(readService.Number);
            gameObjects[readService.Number].SetActive(false);

        }
        else
        {
            Debug.Log("error: 最少为1");
        }
    }
    public void removeAllEntity()
    {
        for (int i = 1; i < gameObjects.Count; i++)
        {
            cleanInput(i);
            gameObjects[i].SetActive(false);
        }
        cleanInput(0);
    }

    public void UploadSSML(GameObject advancedPanel)
    {
        try
        {

#if UNITY_EDITOR
            string file = EditorUtility.OpenFilePanel("Choose SSML File...", "D:\\", "xml");
#elif UNITY_STANDALONE_WIN  
            string file = loadFile();
#elif UNITY_STANDALONE_OSX
            string file = loadFile();
#endif

            if (string.IsNullOrEmpty(file))
            {
                return;
            }
            else
            {
                StreamReader sr = new StreamReader(file);
                string ssml = sr.ReadToEnd();
                if (ssml.Length > 0)
                {
                    m_AzurePlayer.TurnTextToSpeechFromSSML(ssml);
                }
                sr.Close();
            }
        }
        catch (Exception ex)
        {
            // error
            Console.WriteLine(ex.Message);
        }

        // 调用TTS
        //TurnTextToSpeechForSSML();

        // 关闭面板
        clearPanel();
        FoldPanel(advancedPanel);
    }

    public void OpenSettingPanel(int number)
    {
        if (!checkInput(number))
        {
            return;
        }
        double ProsodyVolume = readService._TTSEntity.ProsodyVolume[number];
        double ProsodyRate = readService._TTSEntity.ProsodyRate[number];
        double StyleDegree = readService._TTSEntity.StyleDegree[number];
        string Break = readService._TTSEntity.Break[number];
        string Style = readService._TTSEntity.Style[number];
        // 读取已有配置
        InputSettingsEntity inputSettingsEntity = settingPanel.GetComponent<InputSettingsEntity>();
        inputSettingsEntity.Number = number;
        inputSettingsEntity.openPanel(ProsodyVolume, ProsodyRate, StyleDegree, Break, Style);

        // 打开界面
        if (!settingPanel.activeSelf)
        {
            settingPanel.SetActive(true);
        }
    }

    public void saveEntity()
    {
        InputSettingsEntity inputSettingsEntity = settingPanel.GetComponent<InputSettingsEntity>();
        TTSEntity tempTTSEntity = inputSettingsEntity.savePanel();
        int number = inputSettingsEntity.Number;
        readService._TTSEntity.Break[number] = tempTTSEntity.Break[0];
        readService._TTSEntity.Style[number] = tempTTSEntity.Style[0];
        readService._TTSEntity.ProsodyVolume[number] = tempTTSEntity.ProsodyVolume[0];
        readService._TTSEntity.ProsodyRate[number] = tempTTSEntity.ProsodyRate[0];
        readService._TTSEntity.StyleDegree[number] = tempTTSEntity.StyleDegree[0];
        // 关闭界面
        if (settingPanel.activeSelf)
        {
            settingPanel.SetActive(false);
        }
    }

    public void sentimentAnalysis()
    {
        InputSettingsEntity inputSettingsEntity = settingPanel.GetComponent<InputSettingsEntity>();
        int number = inputSettingsEntity.Number;
        string msg = readService._TTSEntity.Content[number];
        // 情感分析
        SAService sAService = new SAService();
        SAEntity sAEntity = sAService.sentimentAnalysis(msg);
        // Debug.Log($"msg:{msg} sa:{sAEntity.Style}");
        inputSettingsEntity.setStyle(sAEntity.Style);
    }

    public void clearPanel()
    {
        // TODO
        readService.init();
        removeAllEntity();

    }

    public void submit(GameObject advancedPane)
    {
        if (!checkInput(readService.Number - 1))
        {
            return;
        }

        // 调用TTS
        // StartCoroutine(ReadIE(readService._TTSEntity));
        m_AzurePlayer.TurnTextToSpeech(readService._TTSEntity); // TODO public void TurnTextToSpeech(TTSEntity tTSEntity);
        clearPanel();
        FoldPanel(advancedPane);
    }

    // 暂时不做
    private IEnumerator ReadIE(TTSEntity _TTSEntity)
    {
        Debug.Log(_TTSEntity.Content.Count);
        yield return new WaitForEndOfFrame();
        Debug.Log("content : " + _TTSEntity.Content[0]);
        // m_AzurePlayer.TurnTextToSpeech(_TTSEntity);   // TORE
    }


    public bool checkInput(int number)
    {
        TMP_InputField inputText = gameObjects[number].GetComponentInChildren<TMP_InputField>();
        if (inputText.text.Length == 0)
        {
            Debug.Log("输入框为空!");
            return false;
        }
        return true;
    }

    public void cleanInput(int number)
    {
        TMP_InputField inputText = gameObjects[number].GetComponentInChildren<TMP_InputField>();
        inputText.text = "";
    }

    // 打开文件浏览器
    string loadFile()
    {
        OpenFileName openFileName = new OpenFileName();
        openFileName.structSize = Marshal.SizeOf(openFileName);
        openFileName.filter = "xml(*.xml)";
        openFileName.file = new string(new char[256]);
        openFileName.maxFile = openFileName.file.Length;
        openFileName.fileTitle = new string(new char[64]);
        openFileName.maxFileTitle = openFileName.fileTitle.Length;
        openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
        openFileName.title = "Choose SSML File...";
        openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;

        if (LocalDialog.GetSaveFileName(openFileName))
        {
            return openFileName.file;

        }
        else
        {
            return null;
        }
    }
}

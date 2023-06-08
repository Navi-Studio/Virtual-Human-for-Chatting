using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System;

public class GameSettingsEvent : MonoBehaviour
{
    [SerializeField] private List<GameObject> gameObjects;
    private GameSettingsEntity gameSettingsEntity;
    [HideInInspector]public static event Action OnGameSettingChanged;

    private void Start() {
        gameSettingsEntity = GameSettingsEntity.Instance;
    }
    public void SaveGameSettings(){
        bool isGameSettingChanged = false;
        if(gameObjects[0].GetComponent<Toggle>().isOn){
            if(gameSettingsEntity.LookTargetMode != 0){
                gameSettingsEntity.LookTargetMode = 0;
                isGameSettingChanged = true;
            }
        }else{
            if (gameSettingsEntity.LookTargetMode != 1)
            {
                gameSettingsEntity.LookTargetMode = 1;
                isGameSettingChanged = true;
            }
        }
        
        if(gameObjects[1].GetComponent<TMP_Dropdown>().value == 0){
            if(!gameSettingsEntity.Speaker.Equals("zh-CN-XiaoxiaoNeural")){
                gameSettingsEntity.Speaker = "zh-CN-XiaoxiaoNeural";
                isGameSettingChanged = true;
            }
        }
        else if(gameObjects[1].GetComponent<TMP_Dropdown>().value == 1){
            if (!gameSettingsEntity.Speaker.Equals("zh-CN-XiaoyiNeural"))
            {
                gameSettingsEntity.Speaker = "zh-CN-XiaoyiNeural";
                isGameSettingChanged = true;
            }
        }
        if(!gameSettingsEntity.ChatGPTAPI.Equals(gameObjects[2].GetComponent<TMP_InputField>().text)){
            gameSettingsEntity.ChatGPTAPI = gameObjects[2].GetComponent<TMP_InputField>().text;
            isGameSettingChanged = true;
        }
        if (!gameSettingsEntity.AzureAPI.Equals(gameObjects[3].GetComponent<TMP_InputField>().text))
        {
            gameSettingsEntity.AzureAPI = gameObjects[3].GetComponent<TMP_InputField>().text;
            isGameSettingChanged = true;
        }
        if (!gameSettingsEntity.APISpaceAPI.Equals(gameObjects[4].GetComponent<TMP_InputField>().text))
        {
            gameSettingsEntity.APISpaceAPI = gameObjects[4].GetComponent<TMP_InputField>().text;
            isGameSettingChanged = true;
        }
        if (!gameSettingsEntity.Persona.Equals(gameObjects[5].GetComponent<TMP_InputField>().text))
        {
            gameSettingsEntity.Persona = gameObjects[5].GetComponent<TMP_InputField>().text;
            isGameSettingChanged = true;
        }

        if(isGameSettingChanged){
            // 将 GameSettingsEntity 数据保存到 JSON 做持久化
            string js = JsonConvert.SerializeObject(gameSettingsEntity);
            string fileUrl = Application.persistentDataPath + "\\GameSettings.json";
            // Debug.Log(fileUrl);
            using (StreamWriter sw = new StreamWriter(fileUrl))
            {
                sw.Write(js);
                sw.Close();
                sw.Dispose();
            }
            OnGameSettingChanged?.Invoke();
        }
    }

    public void InitGameSettings(){
        gameSettingsEntity = GameSettingsEntity.Instance;
        if (gameSettingsEntity.LookTargetMode == 0)
        {
            gameObjects[0].GetComponent<Toggle>().isOn = true;
        }
        else
        {
            gameObjects[0].GetComponent<Toggle>().isOn = false;
        }
        if(gameSettingsEntity.Speaker == "zh-CN-XiaoxiaoNeural"){
            gameObjects[1].GetComponent<TMP_Dropdown>().value = 0;
        }else if(gameSettingsEntity.Speaker == "zh-CN-XiaoyiNeural"){
            gameObjects[1].GetComponent<TMP_Dropdown>().value = 1;
        }
        gameObjects[2].GetComponent<TMP_InputField>().text = gameSettingsEntity.ChatGPTAPI;
        gameObjects[3].GetComponent<TMP_InputField>().text = gameSettingsEntity.AzureAPI;
        gameObjects[4].GetComponent<TMP_InputField>().text = gameSettingsEntity.APISpaceAPI;
        gameObjects[5].GetComponent<TMP_InputField>().text = gameSettingsEntity.Persona;
    }

}

using System.Collections.Generic;
using UnityEngine;

public abstract class TTSBuilder
{
    public TTSEntity ttsEntity;

    public TTSBuilder(){
        this.ttsEntity = new TTSEntity();
        ttsEntity.Speaker = "zh-CN-XiaoxiaoNeural";
        // ttsEntity.SsmlFile = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/Audios/ssml.xml";
        ttsEntity.SsmlFile = "ssml.xml";
        ttsEntity.Content = new List<string>();
        ttsEntity.Style = new List<string>();
        ttsEntity.StyleDegree = new List<double>();
        ttsEntity.ProsodyRate = new List<double>();
        ttsEntity.ProsodyVolume = new List<double>();
        ttsEntity.Break = new List<string>();
    }
    public TTSEntity rebuild(){
        this.ttsEntity = new TTSEntity();
        ttsEntity.Speaker = "zh-CN-XiaoxiaoNeural";
        ttsEntity.SsmlFile = "ssml.xml";
        ttsEntity.Content = new List<string>();
        ttsEntity.Style = new List<string>();
        ttsEntity.StyleDegree = new List<double>();
        ttsEntity.ProsodyRate = new List<double>();
        ttsEntity.ProsodyVolume = new List<double>();
        ttsEntity.Break = new List<string>();
        return ttsEntity;
    }
    public abstract TTSEntity build(string content);
    public abstract TTSEntity build(string content, string style, double styleDegree, double prosodyRate, double prosodyVolume, string _break);
    public abstract TTSEntity add();
    public abstract TTSEntity remove();

}

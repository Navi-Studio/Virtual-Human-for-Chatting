using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputSettingsEntity : MonoBehaviour
{
    // 音量
    // 0.0 - 100 默认 100
    public Slider ProsodyVolume;

    // 语速
    // 0.5 - 2 默认 1
    public Slider ProsodyRate;

    // 咬字轻重
    // 0.01 - 2 默认 1
    public Slider StyleDegree;

    // 停顿
    // x-weak weak Medium Strong x-strong
    public TMP_Dropdown Break;

    // 情绪
    public TMP_Dropdown Style;

    public int Number { get; set; }

    public void openPanel(double prosodyVolume, double prosodyRate, double styleDegree, string _Break, string style)
    {
        ProsodyVolume.value = (float)prosodyVolume;
        ProsodyRate.value = (float)prosodyRate;
        StyleDegree.value = (float)styleDegree;
        setBreak(_Break);
        setStyle(style);
    }
    public TTSEntity savePanel()
    {
        TTSBuilder builder = new BasicTTSBuilder();
        TTSEntity tTSEntity = builder.build("");
        tTSEntity.ProsodyVolume[0] = getProsodyVolume();
        tTSEntity.ProsodyRate[0] = getProsodyRate();
        tTSEntity.StyleDegree[0] = getStyleDegree();
        tTSEntity.Break[0] = getBreak();
        tTSEntity.Style[0] = getStyle();
        return tTSEntity;
    }
    public double getProsodyVolume(){
        return ProsodyVolume.value;
    }

    public void setProsodyVolume(double val)
    {
        ProsodyVolume.value = (float)val;
    }

    public double getProsodyRate()
    {
        return ProsodyRate.value;
    }

    public void setProsodyRate(double val)
    {
        ProsodyRate.value = (float)val;
    }

    public double getStyleDegree()
    {
        return StyleDegree.value;
    }

    public void setStyleDegree(double val)
    {
        StyleDegree.value = (float)val;
    }

    public string getBreak(){
        string _Break;
        switch(Break.value){
            case 0 : _Break = "x-weak";break;
            case 1: _Break = "weak"; break;
            case 2: _Break = "medium"; break;
            case 3: _Break = "strong"; break;
            case 4: _Break = "x-strong"; break;
            default : _Break = "";break;
        }
        return _Break;
    }
    public void setBreak(string _Break)
    {
        switch (_Break)
        {
            case "x-weak": Break.value = 0; break;
            case "weak": Break.value = 1; break;
            case "medium": Break.value = 2; break;
            case "strong": Break.value = 3; break;
            case "x-strong": Break.value = 4; break;
            default: Break.value = 2; break;
        }
    }

    public string getStyle()
    {
        string style;
        switch (Style.value)
        {
            case 0: style = "customerservice"; break;
            case 1: style = "cheerful"; break;
            case 2: style = "sad"; break;
            case 3: style = "angry"; break;
            case 4: style = "fearful"; break;
            case 5: style = "disgruntled"; break;
            case 6: style = "serious"; break;
            default: style = ""; break;
        }
        return style;
    }
    public void setStyle(string style)
    {
        switch (style)
        {
            case "customerservice": Style.value = 0; break;
            case "cheerful": Style.value = 1; break;
            case "sad": Style.value = 2; break;
            case "angry": Style.value = 3; break;
            case "fearful": Style.value = 4; break;
            case "disgruntled": Style.value = 5; break;
            case "serious": Style.value = 6; break;
            default: Style.value = 0; break;
        }
    }


}

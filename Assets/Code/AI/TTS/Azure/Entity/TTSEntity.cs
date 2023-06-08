using System.Collections.Generic;

public class TTSEntity
{
    // 语言-声源
    public string Speaker { get; set; } 
    // ssml文件生成路径
    public string SsmlFile { get; set; }
    // 实际文本
    public List<string> Content { get; set; }
    // 情绪
    public List<string> Style { get; set; }
    // 咬字轻重
    // 0.01 - 2 默认 1
    public List<double> StyleDegree { get; set; }
    // 语速
    // 0.5 - 2 默认 1
    public List<double> ProsodyRate { get; set; }
    // 音量
    // 0.0 - 100 默认 100
    public List<double> ProsodyVolume { get; set; }
    // 停顿
    // x-weak weak Medium Strong x-strong
    public List<string> Break { get; set; }

}

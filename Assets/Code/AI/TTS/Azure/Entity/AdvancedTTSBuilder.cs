
public class AdvancedTTSBuilder : TTSBuilder
{
    public override TTSEntity build(string content)
    {
        ttsEntity.Content.Add(content);
        ttsEntity.Break.Add("medium");
        ttsEntity.Style.Add("customerservice");
        ttsEntity.StyleDegree.Add(1);
        ttsEntity.ProsodyRate.Add(1);
        ttsEntity.ProsodyVolume.Add(100);
        return this.ttsEntity;
    }



    public override TTSEntity build(string content, string style, double styleDegree, double prosodyRate, double prosodyVolume, string _break)
    {
        ttsEntity.Content.Add(content);
        ttsEntity.Break.Add(_break);
        ttsEntity.Style.Add(style);
        ttsEntity.StyleDegree.Add(styleDegree);
        ttsEntity.ProsodyRate.Add(prosodyRate);
        ttsEntity.ProsodyVolume.Add(prosodyVolume);
        return this.ttsEntity;
    }

    public override TTSEntity add()
    {
        ttsEntity.Content.Add("");
        ttsEntity.Break.Add("medium");
        ttsEntity.Style.Add("customerservice");
        ttsEntity.StyleDegree.Add(1);
        ttsEntity.ProsodyRate.Add(1);
        ttsEntity.ProsodyVolume.Add(100);
        return this.ttsEntity;
    }

    public override TTSEntity remove()
    {
        int lastIndex = ttsEntity.Content.Count - 1;
        ttsEntity.Content.RemoveAt(lastIndex);
        ttsEntity.Break.RemoveAt(lastIndex);
        ttsEntity.Style.RemoveAt(lastIndex);
        ttsEntity.StyleDegree.RemoveAt(lastIndex);
        ttsEntity.ProsodyRate.RemoveAt(lastIndex);
        ttsEntity.ProsodyVolume.RemoveAt(lastIndex);
        return this.ttsEntity;
    }
}

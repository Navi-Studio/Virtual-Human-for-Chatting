
public class BasicTTSBuilder : TTSBuilder
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
        // none
        throw new System.NotImplementedException();
    }
    
    public override TTSEntity add(){
        // none
        throw new System.NotImplementedException();
    }

    public override TTSEntity remove()
    {
        // NONE
        throw new System.NotImplementedException();
    }
}

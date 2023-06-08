using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadService : Singleton<ReadService>
{
    public TTSEntity _TTSEntity { get; set; }
    public TTSBuilder _TTSBuilder { get; set; }
    public int Number { get; set; }

    public ReadService(){
        _TTSBuilder = new AdvancedTTSBuilder();
        _TTSEntity = _TTSBuilder.build("");
        Number = 1;
    }
    public void init(){
        _TTSBuilder = new AdvancedTTSBuilder();
        _TTSEntity = _TTSBuilder.rebuild();
        _TTSEntity = _TTSBuilder.build("");
        Number = 1;
    }

    public void addEntity(){
        _TTSEntity = _TTSBuilder.add();
        Number++;
    }
    public void removeEntity()
    {
        _TTSEntity = _TTSBuilder.remove();
        Number--;
    }


}

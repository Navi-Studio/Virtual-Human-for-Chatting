using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFPS : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Application.targetFrameRate = 60;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;
using System.Threading.Tasks;

public class Audio2Face : MonoBehaviour
{
    private CubismModel cubismModel;
    private AzureSpeech azureSpeech;
    private AudioSource audioSource;

    // Live2D BlendShape
    private CubismParameter mouthOpenYParam;
    private CubismParameter mouthFormParam;
    private CubismParameter cheekPuffParam;
    private CubismParameter mouthZParam;
    private CubismParameter mouthFunnelParam;
    private bool isFirstPlayAudio = true;



    [HideInInspector]public static bool f_IsAudioPlaying { get; set; }

    private void Awake()
    {
        f_IsAudioPlaying = false;
        cubismModel = this.FindCubismModel();
        azureSpeech = GameObject.FindGameObjectWithTag("TTS").GetComponentInChildren<AzureSpeech>();
        audioSource = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>();

        mouthOpenYParam = cubismModel.Parameters[38];
        mouthFormParam = cubismModel.Parameters[37];
        mouthZParam = cubismModel.Parameters[17];
        cheekPuffParam = cubismModel.Parameters[18];    
        mouthFunnelParam = cubismModel.Parameters[19];
    }

    private void LateUpdate() {
        float rate = 1.5f;
        if(f_IsAudioPlaying){
            if(azureSpeech.blendShapeQueue.Count>0){
                var blendShapeList = azureSpeech.blendShapeQueue.Dequeue();
                // 参数映射处理
                float jawOpenAzure = blendShapeList[17] * rate;
                float mouthPuckerAzure = blendShapeList[20] * rate;
                float mouthFunnelAzure = -(blendShapeList[19] * 2f - 1) * rate; // 0~1 映射到 1~-1

                // BlendShape Azure -> Live2D 对齐
                if(jawOpenAzure > 0.2f){ // 阈值
                    mouthOpenYParam.Value = jawOpenAzure;
                }else{
                    mouthOpenYParam.Value = 0;
                }
                cheekPuffParam.Value = mouthPuckerAzure;
                mouthFormParam.Value = mouthFunnelAzure;
            }else{
                ResetParam();
            }
        }else{
            ResetParam();
        }   
    }

    private void ResetParam(){
        mouthOpenYParam.Value = 0;
        mouthFormParam.Value = 0;
        cheekPuffParam.Value = 0;
    }

    IEnumerator PlayAudioIE(){
        yield return new WaitForSeconds(0.6f);
        audioSource.clip = azureSpeech.m_AudioClip;
        audioSource.Play();
    }
}
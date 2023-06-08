using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchAnimationEvent : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;
    private AudioClip touch218Clip;


    private void Start() {
        animator = GetComponentInChildren<Animator>();
        audioSource = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>();
        touch218Clip = Resources.Load<AudioClip>("Audios/Animation/touchBodyClip");
    }

    // 触碰胳膊动画事件
    public void OnTouch218Start(){
        animator.SetInteger("idleState",-1);
        animator.SetInteger("touchState",0);
        audioSource.clip = touch218Clip;
        audioSource.Play();
    }

    public void OnTouch218End(){
        animator.SetInteger("idleState",0);
    }
}

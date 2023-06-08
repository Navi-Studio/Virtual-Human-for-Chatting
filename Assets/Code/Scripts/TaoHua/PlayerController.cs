using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;


public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private bool canSwitch = true;
    private void Awake() {
        animator = GetComponentInChildren<Animator>();
        
    }

    void Update()
    {
        int idleState = animator.GetInteger("idleState");
        
        if(idleState == 0 && canSwitch){
            StartCoroutine(RandomIdleState());
        }
    }


    IEnumerator RandomIdleState(){
        canSwitch = false;
        int random = Random.Range(0, 2);
        animator.SetInteger("idleState",random);
        yield return new WaitForSeconds(4);
        animator.SetInteger("idleState",0);
        yield return new WaitForSeconds(5);
        canSwitch = true;
    }

}

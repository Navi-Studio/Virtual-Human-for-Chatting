using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Framework.Raycasting;
using Live2D.Cubism.Framework.Motion;

public class TouchController : MonoBehaviour
{
    private CubismRaycaster cubismRaycaster;
    private CubismRaycastHit[] cubismRaycastHits;
    private Animator animator;
    private CubismMotionController cubismMotionController;

    private void Start() {
        animator = GetComponentInChildren<Animator>();
        cubismRaycaster = GetComponentInChildren<CubismRaycaster>();
        cubismMotionController = GetComponentInChildren<CubismMotionController>();
    }


    void Update()
    {
        int idleState = animator.GetInteger("idleState");
        if(!Input.GetMouseButtonDown(0) || idleState!=0){
            return;
        }

        cubismRaycastHits = new CubismRaycastHit[4];    // 一般射线击中不会超过4个图层
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int hitCount = cubismRaycaster.Raycast(ray,cubismRaycastHits);  // 发射射线
        for(int i=0;i<hitCount;i++){
            String name = cubismRaycastHits[i].Drawable.name;
            switch(name){
                case "ArtMesh218" : // 手部
                    TouchResponse(i);
                    break;
                default : 
                    break;
            }
        }
    }

    public void TouchResponse(int i){
        animator.SetInteger("touchState",218);
        // cubismMotionController.PlayAnimation(ani)
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Framework.LookAt;
using OpenCvSharp;
using System.IO;
using UnityEditor;

public class LookTargetController : MonoBehaviour, ICubismLookTarget
{
    private Animator animator;

    private FrameSource video;
    // model for differ
    private CascadeClassifier cascade = null;
    // cube start cordinate x
    private float centerX = 850; // -0.025f;
    // cube start cordinate y
    private float centerY = 600; // 1.42f;
    // user start cordinate x
    private int centerRX = 200; // 240;
    // user start cordinate y
    private int centerRY = 200; // 145;
    // proportion:from user move pixel to sight target move pixel
    private float proportion = 1.0f; // 0.00065f;
    // realtime user cordinate x
    private int posX = 200; // 240;
    // realtime user cordinate y
    private int posY = 200; // 145;

    public TextAsset faceFile;

    private void Start() {
        animator = GameObject.FindGameObjectWithTag("Model").GetComponent<Animator>();
        if(GameSettingsEntity.Instance.LookTargetMode == 1)
        {
            InitCamera();
        }
    }

    public void InitCamera()
    {
        video = Cv2.CreateFrameSource_Camera(0);
        //load model 
        FileStorage storageFaces = new FileStorage(faceFile.text, FileStorage.Mode.Read | FileStorage.Mode.Memory);
        cascade = new CascadeClassifier();
        if (!cascade.Read(storageFaces.GetFirstTopLevelNode()))
            throw new System.Exception("FaceProcessor.Initialize: Failed to load faces cascade classifier");
    }

    public Vector3 GetPosition()
    {
        if(animator.GetInteger("idleState") == -1){
            return Vector3.zero;
        }else{
            Vector3 targetPosition;
            if (GameSettingsEntity.Instance.LookTargetMode == 1){
                if (cascade == null){
                    InitCamera();
                }
                Mat frame = new Mat();
                video.NextFrame(frame);
                targetPosition = findNewFace(frame);
            }else{
                targetPosition = Input.mousePosition;
                OnCameraQuit();
            }
            targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(targetPosition.x
                                    ,targetPosition.y,0-Camera.main.transform.position.z));
            return targetPosition;
        }
    }

    public Vector3 findNewFace(Mat frame)
    {
        var faces = cascade.DetectMultiScale(frame);
        Vector3 pos = Input.mousePosition;
        if (faces.Length >= 1)
        {
            posX = faces[0].X - faces[0].Width / 2;
            posY = faces[0].Y - faces[0].Height / 2;
            //update sight target cordinates by proportion
            pos.x = centerX - (posX - centerRX) * proportion;
            pos.y = centerY - (posY - centerRY) * proportion * 0.6f;
            // Debug.Log(faces[0].Location + "pos.X: " + pos.x + " pos.Y: " + pos.y);
        }
        return pos;
    }

    void OnCameraQuit()
    {
        if (video != null)
        {
            video.Dispose();
            video = null;
            cascade = null;
        }
    }

    public bool IsActive()
    {
        return gameObject.activeInHierarchy;
    }

}

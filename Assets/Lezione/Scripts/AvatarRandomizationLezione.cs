using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AvatarRandomizationLezione : MonoBehaviour
{
    private Object[] animationsList;
    private Object[] modelsList;
    private GameObject tempModel;
    private float startingTime;
    private bool initFlag;
    private Animator myAnimator;
    private List<byte[]> frames;
    private CameraControllerLezione myCamera;
    private string clipName;

    public int fps;
    public string screensFolderName;

    // Start is called before the first frame update
    void Start()
    {
        frames = new List<byte[]>();

        if(fps == 0)
            fps = 30;
        Time.captureDeltaTime = 1f / fps;
        
        animationsList = Resources.LoadAll("AnimationsLezione", typeof(AnimationClip));
        modelsList = Resources.LoadAll("PrefabsLezione", typeof(GameObject));
        initFlag = true;

        GameObject tempCamera = GameObject.Find("Main Camera");
        myCamera = tempCamera.GetComponent<CameraControllerLezione>();
    }

    // Update is called once per frame
    void Update()
    {
        if(initFlag)
        {
            tempModel = Instantiate((GameObject)modelsList[Random.Range(0, modelsList.Length)]);
            myAnimator = tempModel.GetComponent<Animator>();

            RuntimeAnimatorController tempCont = Resources.Load("AnimationControllersLezione/baseController") as RuntimeAnimatorController;
            AnimatorOverrideController tempOverrideCont = new AnimatorOverrideController(tempCont);

            myAnimator.runtimeAnimatorController = tempOverrideCont;

            AnimationClip tempClip = (AnimationClip)animationsList[Random.Range(0, animationsList.Length)];
            tempOverrideCont["Ginga Variation 3"] = tempClip;
            clipName = tempClip.name;

            if (Random.Range(0f,1f) > 0.5f)
                myAnimator.SetBool("Mirroring", true);
            //myAnimator.SetBool("Mirroring", true);

            myAnimator.speed = Random.Range(0.5f, 3f);

            startingTime = Time.time;
            initFlag = false;
        }
        //codice
        //Debug.Log((Time.time - startingTime) * 30);
        byte[] tempFrame = myCamera.CamCapture();
        frames.Add(tempFrame);

        if(myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            Destroy(tempModel);
            initFlag = true;
            saveOnDisk();
            frames.Clear();
        }
    }

    void saveOnDisk()
    {
        string folderName = screensFolderName + "/" + tempModel.name + "/";
        if(Directory.Exists(folderName) == false)
        {
            Directory.CreateDirectory(folderName);
        }
        folderName += clipName + "/";
        if(Directory.Exists(folderName) == false)
        {
            Directory.CreateDirectory(folderName);
        }

        int dirCounter = 0;
        string tempFolderName = folderName + dirCounter++.ToString() + "/";
        while(Directory.Exists(tempFolderName))
            tempFolderName = folderName + dirCounter++.ToString() + "/";
        Directory.CreateDirectory(tempFolderName);

        string tempPath;
        for (int frameCounter = 0; frameCounter < frames.Count; frameCounter++)
        {
            tempPath = tempFolderName + frameCounter.ToString();
            File.WriteAllBytes(tempPath + ".png", frames[frameCounter]);
        }
    }


}

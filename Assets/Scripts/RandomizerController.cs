using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class RandomizerController : MonoBehaviour
{
    private Object[] modelList;
    private Object[] animationList;
    private bool restartFlag;
    private Animator tempAnim;
    private GameObject tempAvatar;
    private AnimationClip tempClip;
    private List<byte[]> frames;
    private CameraController mainCameraController;

    public int fps;
    public bool addMirrors;
    public Vector2 speedRange;
    public string screensFolderName = "";

    // Start is called before the first frame update
    void Start()
    {
        Time.captureDeltaTime = 1f/(float)fps;

        frames = new List<byte[]>();

        GameObject temp_camera = GameObject.Find("Main Camera");
        mainCameraController = temp_camera.GetComponent<CameraController>();

        modelList = Resources.LoadAll("Prefabs", typeof(GameObject));
        animationList = Resources.LoadAll("Animations", typeof(AnimationClip));
        restartFlag = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(restartFlag)
        {
            tempAvatar = Instantiate((GameObject)modelList[Random.Range(0, modelList.Length)]);
            
            tempAnim = tempAvatar.GetComponent<Animator>();
            AnimatorOverrideController tempOverride = tempAnim.runtimeAnimatorController as AnimatorOverrideController;

            tempAnim.speed = Random.Range(speedRange[0], speedRange[1]);

            if (addMirrors && Random.Range(0.0f, 1.0f) >= 0.5f)
                tempAnim.SetBool("Mirror", true);

            tempClip = (AnimationClip)animationList[Random.Range(0, animationList.Length)]; 
            tempOverride["ginga variation 1"] = tempClip;


            restartFlag = false;
        }
        byte[] Bytes = mainCameraController.CamCapture();
        frames.Add(Bytes);

        if (tempAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        { 
            saveOnDisk();
            restartFlag = true;
            Destroy(tempAvatar);
            frames.Clear();
        }
    }

    void saveOnDisk()
    {
        string folderName = screensFolderName + "/" + tempAvatar.name + "/";
        if(Directory.Exists(folderName) == false)
        {
            Directory.CreateDirectory(folderName);
        }
        folderName += tempClip.name + "/";
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

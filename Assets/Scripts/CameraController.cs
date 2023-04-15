using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector2Int resolution;
    // Start is called before the first frame update
    void Start()
    {
        setCameraRes(new Vector2Int(resolution[0], resolution[1])); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void setCameraRes(Vector2Int newResolution)
    {
        Camera Cam = GetComponent<Camera>();
        RenderTexture rt = new RenderTexture(newResolution[0], newResolution[1], 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        Cam.targetTexture = rt;
    }

    public byte[] CamCapture()
    {
        Camera Cam = GetComponent<Camera>();
 
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = Cam.targetTexture;
 
        Cam.Render();
 
        Texture2D Image = new Texture2D(Cam.targetTexture.width, Cam.targetTexture.height, TextureFormat.RGBA64, false);
        Image.ReadPixels(new Rect(0, 0, Cam.targetTexture.width, Cam.targetTexture.height), 0, 0);
        Image.Apply();
        RenderTexture.active = currentRT;
 
        var Bytes = Image.EncodeToPNG();
        DestroyImmediate(Image);
 
        return Bytes;
    }
}

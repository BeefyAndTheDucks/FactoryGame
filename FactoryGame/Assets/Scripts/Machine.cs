using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : MonoBehaviour
{
    public Camera imageCamera;
    public Vector3 cameraOffset;

    RenderTexture texture = new RenderTexture(500,500,32);

    private void Start()
    {
        texture.Create();
    }

    public void TakePicture()
    {
        imageCamera.targetTexture = texture;
        imageCamera.transform.position = transform.position + cameraOffset;
        imageCamera.targetTexture = null;
    }
}

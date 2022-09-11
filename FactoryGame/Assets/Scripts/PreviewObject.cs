using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    public Material previewMaterial;
    public string ignoreTag;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ignoreTag))
            return;
        PlayerBuildManager.canPlace = false;
        previewMaterial.color = PlayerBuildManager.red;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(ignoreTag))
            return;
        PlayerBuildManager.canPlace = true;
        previewMaterial.color = PlayerBuildManager.green;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    public Material previewMaterial;
    public string ignoreTag;
    public bool canPlace = true;

    List<Collider> collidedWith = new List<Collider>();

    void FixedUpdate()
    {
        previewMaterial.color = canPlace ? PlayerBuildManager.red : PlayerBuildManager.green;
        Debug.Log(canPlace);
        collidedWith.Clear();
        canPlace = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(ignoreTag))
            return;
        collidedWith.Add(other);
        canPlace = true;
    }

}

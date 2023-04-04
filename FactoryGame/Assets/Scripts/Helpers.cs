using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public static class Helpers {

    public static float Map(this float x, float x1, float x2, float y1, float y2) {
        return Mathf.Lerp(y1, y2, Mathf.InverseLerp(x1, x2, x));
    }
}

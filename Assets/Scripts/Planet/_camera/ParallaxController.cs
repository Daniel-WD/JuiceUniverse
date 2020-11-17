using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour {

    public float fraction = 1;

    public void onCamYPosChanged(float dy) {
        transform.position = transform.position + new Vector3(0, fraction * dy, 0);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableSpriteEvent : MonoBehaviour {

    public void disable() {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

}

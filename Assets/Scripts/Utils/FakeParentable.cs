using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeParentable : MonoBehaviour {
    
    public Vector3 fakeParentPosition;
    
    void OnEnable() {
        fakeParentPosition = new Vector3();
    }
    
    void LateUpdate() {
        Vector3 newPos = new Vector3(transform.localPosition.x+fakeParentPosition.x,
                                        transform.localPosition.y+fakeParentPosition.y,
                                        transform.localPosition.z+fakeParentPosition.z);
        transform.localPosition = newPos;
    }
    
}

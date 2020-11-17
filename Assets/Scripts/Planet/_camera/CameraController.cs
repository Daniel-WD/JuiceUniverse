using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform topBound;
    public float speedReduction;
    
    public ParallaxController[] parallaxControllers;

    private Camera mCam;

    private float mOriginY;

    private bool mMouseEnter = false;
    private float mLastMouseY;

    private float mDelta = 0;
    
    private bool mEnabled = true;

    void Start() {
        mCam = GetComponent<Camera>();
        mOriginY = transform.position.y;
    }

    void Update() {
        if(!mEnabled) {
            mDelta = 0;
            return;
        }
        
        mDelta = Mathf.MoveTowards(mDelta, 0, Time.deltaTime * speedReduction);

        if (Input.GetMouseButton(0) && mMouseEnter) {

            float currentMouseY = mCam.ScreenToWorldPoint(Input.mousePosition).y;
            mDelta = mLastMouseY - currentMouseY;


        } else if (Input.GetMouseButtonDown(0)) {
            mMouseEnter = true;
            mLastMouseY = mCam.ScreenToWorldPoint(Input.mousePosition).y;

        } else {
            mMouseEnter = false;
        }

        float newY = Mathf.Clamp(transform.position.y + mDelta, mOriginY, topBound.transform.position.y - mCam.orthographicSize);
        
        float realDelta = newY - transform.position.y;
        foreach(ParallaxController c in parallaxControllers) c.onCamYPosChanged(realDelta);

        Vector3 newPos = new Vector3(transform.position.x, newY, transform.position.z);
        transform.position = newPos;
    }
    
    public void setEnabled(bool enabled) {
        mEnabled = enabled;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MySlider : MonoBehaviour {

    public RectTransform bounds;
    
    private RectTransform mContent;
    private float mProgress = 0.1f;

    void Start() {
        mContent = GetComponent<RectTransform>();
        refresh();
    }
    
    public void setProgress(float p) {
        mProgress = Mathf.Clamp(p, 0, 1);
        refresh();
    }
    
    public float getProgress() {
        return mProgress;
    }
    
    private void refresh() {
        if(!mContent) return;
        
        float startX = bounds.anchoredPosition.x;
        
        float negDist = mContent.rect.width * (1-mProgress);
        
        mContent.anchoredPosition = new Vector2(startX - negDist, mContent.anchoredPosition.y);
        
    }
    
}

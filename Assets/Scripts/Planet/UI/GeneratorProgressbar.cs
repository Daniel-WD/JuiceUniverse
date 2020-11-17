using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorProgressbar : MonoBehaviour {
    
    public Slider slider;
    
    private float fillDelta = 0;
    
    void Start() {
        gameObject.SetActive(false);
    }
    
    void Update() {
        
        if(!gameObject.activeSelf) return;
        
        slider.value = Mathf.MoveTowards(slider.value, 1, fillDelta * Time.deltaTime);
        
        if(slider.value >= 1) {
            gameObject.SetActive(false);
        }
        
    }
    
    public void start(float time) {
        slider.value = 0;
        gameObject.SetActive(true);
        fillDelta = 1f/time;
    }
    
    public void cancel() {
        slider.value = 1;
    }
    
}

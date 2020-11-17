using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Zygote : MonoBehaviour {
    
    public string triggerString;
    public float triggerInterval;
    
    private List<Animator> animators = new List<Animator>();
    
    void Start() {
        StartCoroutine(trigger());
    }
    
    private IEnumerator trigger() {
        while(true) {
            foreach(var a in animators) {
              if(a) a.SetTrigger(triggerString);  
            }
            yield return new WaitForSeconds(triggerInterval);
        }
    }
    
    public void addAnimator(Animator anim) {
        if(animators.Contains(anim)) return;
        animators.Add(anim);
    }
    
    public void removeAnimator(Animator anim) {
        animators.Remove(anim);
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLoopSpawner : MonoBehaviour {

    public GameObject spawnObject;
    public string animationTrigger;
    public float clipDuration, clipDelay;

    public bool isEndElevator = true;
    public bool spawnInParent = false;
    public bool spawnInFakeParent = false;
    public bool ignoreClipDuration = false;
    
    private bool killed = false;
    
    private Animator lastStartedAnimator;

    public IEnumerator spawnWithDelay(float duration, float delay) {
        yield return new WaitForSeconds(delay);
        spawn(duration);
    }

    public void spawn(float duration) {
        // int count = (int)((spawnRate * duration - (!ignoreClipDuration ? (isEndElevator ? 2 : 1) : 0) * spawnRate * clipDuration) / clipDuration);
        int count = (int) (duration/(clipDelay));
        if(count <= 0) count = 1;
        StartCoroutine(spawnRoutine(count));
        Invoke("kill", duration);
    }

    private IEnumerator spawnRoutine(int count) {
        killed = false;
        for (int i = 0; i < count; i++) {
            if(killed) break;
            GameObject obj;
            if (spawnInParent) {
                obj = ObjectPooler.inst.giveMe(spawnObject, transform);
                
            } else {
                obj = ObjectPooler.inst.giveMe(spawnObject, transform.position, Quaternion.identity);
            }
            
            if(spawnInFakeParent) {
                FakeParentable fakeParentable = obj.GetComponent<FakeParentable>();
                if(fakeParentable != null) {
                    fakeParentable.fakeParentPosition = transform.position;
                }
            }

            if (animationTrigger != "") {
                Animator anim = obj.GetComponent<Animator>();
                lastStartedAnimator = anim;
                anim.SetTrigger(animationTrigger);
            }
            yield return new WaitForSeconds(clipDelay);
        }
    }
    
    private void kill() { // only for this class --> see invoke call
        killed = true;
    }
    
    public void cancel() {
        kill();
        if(lastStartedAnimator) lastStartedAnimator.SetTrigger("cancel");
    }

}

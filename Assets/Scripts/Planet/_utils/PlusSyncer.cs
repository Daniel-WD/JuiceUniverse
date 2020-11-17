using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlusSyncer : MonoBehaviour {

    private static readonly string TRIGGER = "startMove";

    private static bool oneAnimatorExists = false;
    private static List<Animator> sNextAnimators = new List<Animator>();

    private static void animationStarted() {
        foreach (var anim in sNextAnimators) anim.SetTrigger(TRIGGER);
        sNextAnimators.Clear();
    }

    void Start() {
        Animator anim = GetComponent<Animator>();
        if (oneAnimatorExists) {
            sNextAnimators.Add(anim);
        } else {
            anim.SetTrigger(TRIGGER);
            oneAnimatorExists = true;
        }
    }

    public void animStart() {
        animationStarted();
    }

}

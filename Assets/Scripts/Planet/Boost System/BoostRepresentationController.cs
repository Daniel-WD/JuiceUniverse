using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostRepresentationController : MonoBehaviour, BoostManagerListener {

    public static BoostRepresentationController inst;

    public SpriteRenderer slotOne, slotTwo, slotThree;

    void Awake() {
        inst = this;
    }

    void Start() {
        BoostManager.inst.addListener(this);
        refresh();
    }

    public void refresh() {
        slotOne.sprite = null;
        slotTwo.sprite = null;
        slotThree.sprite = null;
        List<BoostInstance> boostInsts = BoostManager.inst.getActiveBoosts();
        for (int i = 0; i < boostInsts.Count; i++) {
            if (i % 3 == 0) {
                slotOne.sprite = boostInsts[i].boost.littleGfx;
                slotOne.sortingOrder = 22;
                slotTwo.sortingOrder = 20;
                slotThree.sortingOrder = 21;
            } else if (i % 3 == 1) {
                slotTwo.sprite = boostInsts[i].boost.littleGfx;
                slotOne.sortingOrder = 21;
                slotTwo.sortingOrder = 22;
                slotThree.sortingOrder = 20;
            } else {
                slotThree.sprite = boostInsts[i].boost.littleGfx;
                slotOne.sortingOrder = 20;
                slotTwo.sortingOrder = 21;
                slotThree.sortingOrder = 22;
            }
        }
        
    }

    public void onBoostActivated(BoostInstance boost) {
        refresh();
    }

    public void onBoostDied(BoostInstance boost) {
        refresh();
    }

    public void onBoostUsable(BoostInstance boost) {
        refresh();
    }
}

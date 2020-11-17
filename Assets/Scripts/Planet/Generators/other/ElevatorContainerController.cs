using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class ElevatorContainerController : MonoBehaviour {

    public ElevatorController elevator;
    
    public GameObject band;
    public SpriteMask bandMask;
    public float bandOvershoot;

    private Vector3 mHatStartPos;

    void Update() {
        notifyNewElevatorPosition();

    }

    public void notifyProducerCountChanged() {
        if (elevator != null) {
            elevator.updateGeneratorData();
        }
    }

    public void notifyNewElevatorPosition() {
        if (elevator != null && elevator.gameObject != null
                && elevator.gameObject.activeSelf) {
            band.SetActive(true);
        } else {
            band.SetActive(false);
            return;
        }
        float edge = elevator.getTopEdge();
        float targetY = edge + bandMask.bounds.size.y / 2 - bandOvershoot;
        Vector3 pos = new Vector3(bandMask.transform.position.x, targetY, bandMask.transform.position.z);
        bandMask.transform.position = pos;
    }

}

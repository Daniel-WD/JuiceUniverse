using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClickBehaviour : MonoBehaviour {

    private bool mMouseDowned = false;

    void OnMouseDown() {
        if(PlanetUiController.inst.uiActiveLvl0()) return;
        mMouseDowned = true;
    }

    void OnMouseUp() {
        if(PlanetUiController.inst.uiActiveLvl0()) return;
        if (mMouseDowned) {
            mMouseDowned = false;
            onClick();
        }
    }
    
    protected abstract void onClick();

}

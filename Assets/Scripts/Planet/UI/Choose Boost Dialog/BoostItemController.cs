using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum BoostItemState {
    EMPTY, NORMAL, ACTIVE
}

public class BoostItemController : MonoBehaviour {

    public Button btnNormal;
    public Image imgEmpty, imgBoost, imgActive;

    public TextMeshProUGUI txtTimer, txtInfo;

    private BoostItemState mState;
    private BoostInstance mBoost;

    public void set(BoostInstance boostInst, BoostItemState state) {

        mState = state;
        mBoost = boostInst;

        switch (state) {
            case BoostItemState.NORMAL:
                btnNormal.gameObject.SetActive(true);
                txtInfo.gameObject.SetActive(true);
                imgBoost.gameObject.SetActive(true);
                imgEmpty.gameObject.SetActive(false);
                imgActive.gameObject.SetActive(false);
                txtTimer.gameObject.SetActive(false);

                imgBoost.sprite = boostInst.boost.gfx;
                imgBoost.SetNativeSize();
                txtInfo.text = boostInst.boost.multiplier + "x " + Formatter.formatTime(boostInst.activeTime, true);
                break;

            case BoostItemState.ACTIVE:
                imgActive.gameObject.SetActive(true);
                txtTimer.gameObject.SetActive(true);
                imgBoost.gameObject.SetActive(true);
                btnNormal.gameObject.SetActive(false);
                txtInfo.gameObject.SetActive(false);
                imgEmpty.gameObject.SetActive(false);

                imgBoost.sprite = boostInst.boost.gfx;
                imgBoost.SetNativeSize();
                txtTimer.text = Formatter.formatTime(boostInst.activeTimeLeft, true);
                imgActive.sprite = boostInst.boost.bgGfx;
                break;

            case BoostItemState.EMPTY:
                imgEmpty.gameObject.SetActive(true);
                imgActive.gameObject.SetActive(false);
                txtTimer.gameObject.SetActive(false);
                imgBoost.gameObject.SetActive(false);
                btnNormal.gameObject.SetActive(false);
                txtInfo.gameObject.SetActive(false);
                break;
        }

    }

    public void updateTimer() {
        if (mState == BoostItemState.ACTIVE) {
            txtTimer.text = Formatter.formatTime(mBoost.activeTimeLeft, true);
        }
    }
    
    public void onBtnNormalClicked() {
        UseBoostDialog.inst.show(mBoost);
    }

}

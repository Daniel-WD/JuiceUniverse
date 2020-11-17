using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UseBoostDialog : MonoBehaviour {

    public static UseBoostDialog inst;

    public TextMeshProUGUI txtInfo;
    public Image imgBoost;

    private BoostInstance mBoostInst;

    void Awake() {
        inst = this;
    }

    void Start() {
        hide();
    }

    public void show(BoostInstance boostInst) {
        if (PlanetUiController.inst.uiActiveLvl1()) return;
        gameObject.SetActive(true);
        PlanetUiController.inst.enableUiModeLvl1();

        mBoostInst = boostInst;
        
        imgBoost.sprite = mBoostInst.boost.gfx;
        // imgPerkBoostGlow.sprite = glows[(int)mPerk.strenght];
        // imgPerkBoostGlow.SetNativeSize();

        txtInfo.text = "Increase income by " + boostInst.boost.multiplier + "x for " + Formatter.formatTime(mBoostInst.activeTime, true);
    }

    public void hide() {
        gameObject.SetActive(false);
        PlanetUiController.inst.disableUiModeLvl1();
        mBoostInst = null;
    }

    public void onButtonUseClick() {
        BoostManager.inst.activateBoost(mBoostInst);
        mBoostInst = null;
        hide();
    }

    public void onButtonCloseClick() {
        hide();
    }

}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UsePerkDialog : MonoBehaviour {

    public static UsePerkDialog inst;

    public Sprite[] glows;

    public TextMeshProUGUI txtInfo;
    public Image imgPerk, imgPerkGlow;

    private PerkInstance mPerk;

    void Awake() {
        inst = this;
    }

    void Start() {
        hide();
    }

    public void show(PerkInstance perk) {
        if (PlanetUiController.inst.uiActiveLvl1()) return;
        gameObject.SetActive(true);
        PlanetUiController.inst.enableUiModeLvl1();

        mPerk = perk;
        
        imgPerk.sprite = mPerk.perk.gfx;
        imgPerkGlow.sprite = glows[(int)mPerk.perk.strenght];
        imgPerkGlow.SetNativeSize();

        txtInfo.text = "Use +" + (Perk.STRENGTHS[(int)mPerk.perk.strenght] * 100) + "% " + mPerk.perk.getPropertyString() + " for " + Formatter.formatTime(mPerk.activeTime, true);
    }
    
    public void hide() {
        gameObject.SetActive(false);
        PlanetUiController.inst.disableUiModeLvl1();
        mPerk = null;
    }

    public void onButtonUseClick() {
        ChoosePerkDialog.inst.requestActivatePerk(mPerk);
        mPerk = null;
        hide();
    }

    public void onButtonCloseClick() {
        hide();
    }

}
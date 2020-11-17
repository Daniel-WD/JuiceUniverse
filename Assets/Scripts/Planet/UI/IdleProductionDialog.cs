using System.Collections;
using System.Collections.Generic;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IdleProductionDialog : MonoBehaviour {

    public static IdleProductionDialog inst;

    public TextMeshProUGUI txtInfo, txtIncome;
    public RectTransform incomeContainer;

    private BigDecimal mFakeIncome = 1000;
    private long mFakeTime = 2342;

    void Awake() {
        inst = this;
    }

    void Start() {
        hide();
    }

    public void show() {
        if (PlanetUiController.inst.uiActiveLvl0()) return;
        gameObject.SetActive(true);
        PlanetUiController.inst.enableUiModeLvl0();
        
        txtIncome.text = Formatter.formatNumber(mFakeIncome);
        LayoutRebuilder.ForceRebuildLayoutImmediate(incomeContainer);
        
        txtInfo.text = "We worked hard in the last " + Formatter.formatTime(mFakeTime, true) + " since you were gone!";
    }

    public void hide() {
        gameObject.SetActive(false);
        PlanetUiController.inst.disableUiModeLvl0();

    }

    public void onButtonCloseClick() {
        hide();
    }
    
    public void onBtnVideoClicked() {
        
    }
    
    public void onBtnCollectClicked() {
        Database.inst.addPrimaryMoney(mFakeIncome);
        hide();
    }

}

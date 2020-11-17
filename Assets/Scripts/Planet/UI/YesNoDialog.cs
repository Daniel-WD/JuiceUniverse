using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface YesNoListener {

    void onYes();
    void onNo();
    void onClose();

}

public class YesNoDialog : MonoBehaviour {

    public static YesNoDialog inst;

    public Image imgBg;
    public TextMeshProUGUI txtTitle, txtInfoOne, txtInfoThree, txtBtnNo, txtBtnYes;
    public Sprite spriteBgOne, spriteBgThree; 

    private Action mActionNo, mActionYes, mActionClose;

    void Awake() {
        inst = this;
    }

    void Start() {
        hide();
    }

    public void show(string title, string txtYes, string txtNo, string msg, Action actionNo, Action actionYes, Action actionClose) {
        if (PlanetUiController.inst.uiActiveLvl1()) return;
        gameObject.SetActive(true);
        PlanetUiController.inst.enableUiModeLvl1();

        if (msg.Length > 35) { //3 liner
            txtInfoThree.gameObject.SetActive(true);
            txtInfoOne.gameObject.SetActive(false);

            imgBg.sprite = spriteBgThree;
            txtInfoThree.text = msg;
            
        } else { // 1 liner
            txtInfoThree.gameObject.SetActive(false);
            txtInfoOne.gameObject.SetActive(true);

            imgBg.sprite = spriteBgOne;
            txtInfoOne.text = msg;
        }

        txtTitle.text = title;
        txtBtnNo.text = txtNo;
        txtBtnYes.text = txtYes;
        
        mActionNo = actionNo;
        mActionYes = actionYes;
        mActionClose = actionClose;
        
    }

    public void hide() {
        gameObject.SetActive(false);
        PlanetUiController.inst.disableUiModeLvl1();
    }

    public void onBtnYesClick() {
        hide();
        if(mActionYes != null) mActionYes();
    }

    public void onBtnNoClick() {
        hide();
        if(mActionNo != null) mActionNo();
    }

    public void onBtnCloseClick() {
        hide();
        if(mActionClose != null) mActionClose();
    }

}
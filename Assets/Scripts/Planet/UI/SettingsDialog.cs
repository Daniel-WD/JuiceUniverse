using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsDialog : MonoBehaviour {

    public static SettingsDialog inst;

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
        
        
    }

    public void hide() {
        gameObject.SetActive(false);
        PlanetUiController.inst.disableUiModeLvl0();
    }
    
    //Sound
    
    public void onBtnBgMusicClick() {
        
    }
    
    public void onBtnSoundEffectsClick() {
        
    }
    
    //General
    
    public void onBtnPrivacyPolicyClicked() {
        
    }
    
    public void onBtnLanguageClicked() {
        ChooseLanguageDialog.inst.show();
    }
    
    public void onBtnShopClicked() {
        
    }
    
    public void onBtnRenewClicked() {
        string title = "Renew Game";
        string msg = "Do you really want to renew the game? Everything will be irrevocably reset!";
        string no = "No";
        string yes = "Yes";
        Action actionYes = delegate() {
            //TODO renew game
        };
        YesNoDialog.inst.show(title, yes, no, msg, null, actionYes, null);
    }
    
    //Game Services
    
    public void onBtnGameServicesClicked() {
        
    }
    
    //Social Media
    
    public void onBtnFacebookClicked() {
        
    }
    
    public void onBtnInstagramClicked() {
        
    }
    
    public void onBtnWhatsappClicked() {
        
    }
    
    public void onBtnSnapchatClicked() {
        
    }
    
    public void onBtnTwitterClicked() {
        
    }
    
    //Others
    
    public void onBtnFeedbackClicked() {
        
    }
    
    public void onBtnSupportClicked() {
        
    }
    
    public void onBtnRateClicked() {
        
    }

    public void onButtonCloseClick() {
        hide();
    }

}
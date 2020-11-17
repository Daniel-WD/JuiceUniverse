using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActivePerkItemController : MonoBehaviour {
    
    public Sprite[] sprBgs, sprPerkGlows;
    public Image imgBg, imgPerk, imgPerkGlow;
    public TextMeshProUGUI txtTimer;
    
    private PerkInstance mPerk;
    
    public void setPerk(PerkInstance perkInst) {
        mPerk = perkInst;
        
        imgBg.sprite = sprBgs[(int)perkInst.perk.strenght];
        imgPerk.sprite = perkInst.perk.gfx;
        imgPerkGlow.sprite = sprPerkGlows[(int)perkInst.perk.strenght];
        txtTimer.text = Formatter.formatTime(perkInst.activeTimeLeft, true);
        
        imgPerkGlow.SetNativeSize();
        
    }
    
    public PerkInstance getPerk() {
        return mPerk;
    }
    
    public void updateTimer() {
        txtTimer.text = Formatter.formatTime(mPerk.activeTimeLeft, true);
    }

}

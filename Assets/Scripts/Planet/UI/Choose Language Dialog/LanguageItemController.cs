using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageItemController : MonoBehaviour {
    
    public TextMeshProUGUI txtLanguage;
    public Sprite spriteLight, spriteDark;
    
    public void set(string language, bool dark) {
        txtLanguage.text = language;
        GetComponent<Image>().sprite = dark ? spriteDark : spriteLight;
        
    }
    
    public void onBtnChooseClick() {
        
    }
    
}

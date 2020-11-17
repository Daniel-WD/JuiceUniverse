using System.Collections;
using System.Collections.Generic;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FillStationItem : MonoBehaviour {
    
    public Sprite gfxLight, gfxDark;
    
    public TextMeshProUGUI txtLevel, txtRate, txtTitle;
    public RectTransform rateContainer;
    
    public void set(int lvl, int number, BigDecimal rate, bool dark) {
        txtTitle.text = "Fill Station " + number;
        txtLevel.text = "Level " + lvl;
        txtRate.text = Formatter.formatNumber(rate) + "/s";
        LayoutRebuilder.ForceRebuildLayoutImmediate(rateContainer);
        GetComponent<Image>().sprite = dark ? gfxDark : gfxLight;
    }
    
}

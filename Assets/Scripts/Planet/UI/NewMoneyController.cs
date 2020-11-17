using System.Collections;
using System.Collections.Generic;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewMoneyController : MonoBehaviour {
    
    public TextMeshProUGUI text;
    public RectTransform horizontalLayout;
    
    public void destroyMe() {
        Destroy(gameObject);
    }
    
    public void setValue(BigDecimal value) {
        text.text = "+" + Formatter.formatNumber(value);
        LayoutRebuilder.ForceRebuildLayoutImmediate(horizontalLayout);
    }
    
}

using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ValueHolder : MonoBehaviour {

    public TextMeshProUGUI text;
    public RectTransform horizontalLayout;
    private BigDecimal savedValue = new BigDecimal(0, 0);

    void Start() {
        updateText();
    }

    private void updateText() {
        text.text = Formatter.formatNumber(savedValue);
        LayoutRebuilder.ForceRebuildLayoutImmediate(horizontalLayout);
    }

    public void saveValue(BigDecimal value) {
        savedValue += value;
        updateText();
    }

    public BigDecimal removeValue(BigDecimal value) {
        BigDecimal removed;
        if (savedValue < value) {
            removed = savedValue;
            savedValue = 0;
        } else {
            savedValue -= value;
            removed = value;
        }
        updateText();
        return removed;
    }

    public BigDecimal getValue() {
        return savedValue;
    }
    
    public void setValue(BigDecimal value) {
        savedValue = value;
        updateText();
    }

}

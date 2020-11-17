using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapUiController : MonoBehaviour {

    public static MapUiController inst;

    public GameObject overlayLvl0;

    public TextMeshProUGUI primaryMoneyText, secondaryMoneyText;

    void Awake() {
        inst = this;
    }

    void Start() {
        disableUiModeLvl0();
        updateMoneyText();
    }

    public void enableUiModeLvl0() {
        overlayLvl0.SetActive(true);
    }

    public void disableUiModeLvl0() {
        overlayLvl0.SetActive(false);
    }

    public bool uiActiveLvl0() {
        return overlayLvl0.activeSelf;
    }

    public void updateMoneyText() {
        primaryMoneyText.text = Formatter.formatNumber(Database.inst.getPrimaryMoney());
        secondaryMoneyText.text = Formatter.formatNumber(Database.inst.getSecondaryMoney());
    }
}

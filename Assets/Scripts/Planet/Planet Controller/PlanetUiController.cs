using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlanetUiController : MonoBehaviour {

    public static PlanetUiController inst;

    public GameObject overlayLvl0, overlayLvl1, planetUi;
    public CameraController cameraController;

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
        cameraController.setEnabled(false);
    }
    
    public void disableUiModeLvl0() {
        overlayLvl0.SetActive(false);
        cameraController.setEnabled(true);
    }
    
    public bool uiActiveLvl0() {
        return overlayLvl0.activeSelf;
    }
    
    public void enableUiModeLvl1() {
        overlayLvl1.SetActive(true);
    }
    
    public void disableUiModeLvl1() {
        overlayLvl1.SetActive(false);
    }
    
    public bool uiActiveLvl1() {
        return overlayLvl1.activeSelf;
    }

    public void updateMoneyText() {
        primaryMoneyText.text = Formatter.formatNumber(Database.inst.getPrimaryMoney());
        secondaryMoneyText.text = Formatter.formatNumber(Database.inst.getSecondaryMoney());
    }

}

using System.Collections;
using System.Collections.Generic;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyPlanetDialog : MonoBehaviour, MoneyListener {

    public static BuyPlanetDialog inst;
    
    public Button btnBuy;
    public Image imgBtnBuy, imgCostCoin;
    public TextMeshProUGUI txtCost, txtBuy;
    public RectTransform costContainer;

    private PlanetReprController mPlanetRepr;

    void Awake() {
        inst = this;
    }

    void Start() {
        hide();
    }

    public void show(PlanetReprController planetRepr) {
        if (MapUiController.inst.uiActiveLvl0()) return;
        gameObject.SetActive(true);
        MapUiController.inst.enableUiModeLvl0();
        
        mPlanetRepr = planetRepr;
        
        txtCost.text = Formatter.formatNumber(planetRepr.getCost());
        LayoutRebuilder.ForceRebuildLayoutImmediate(costContainer);
        
        checkCanPay();
    }
    
    public void hide() {
        gameObject.SetActive(false);
        MapUiController.inst.disableUiModeLvl0();
        mPlanetRepr = null;
    }

    private void checkCanPay() {
        if (Database.inst.getPrimaryMoney() >= mPlanetRepr.getCost()) { // can pay
            btnBuy.interactable = true;
            txtCost.color = Color.white;
            txtBuy.color = Color.white;
            imgBtnBuy.color = Color.white;
            imgCostCoin.color = Color.white;
        } else { // cannot pay
            btnBuy.interactable = false;
            Color red = Database.inst.noMoneyRedColor;
            red.a = 0.5f;
            txtCost.color = red;
            txtBuy.color = red;
            Color semiWhite = new Color(1, 1, 1, 0.5f);
            imgBtnBuy.color = semiWhite;
            imgCostCoin.color = semiWhite;
        }
    }

    public void onBtnBuyClick() {
        Database.inst.removePrimaryMoney(mPlanetRepr.getCost());
        int dex = MapController.inst.findIndex(mPlanetRepr);
        PlanetSaveData newData = new PlanetSaveData();
        newData.production = -1;
        Database.inst.planets[dex] = newData;
        MapController.inst.refresh();
        hide();
    }

    public void onButtonCloseClick() {
        hide();
    }

    public void onPrimaryMoneyChanged(BigDecimal newMoney) {
        checkCanPay();
    }

    public void onSecondaryMoneyChanged(BigDecimal newMoney) {
    }
}
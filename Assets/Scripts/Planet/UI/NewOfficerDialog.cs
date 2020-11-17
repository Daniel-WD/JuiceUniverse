using System.Collections;
using System.Collections.Generic;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewOfficerDialog : MonoBehaviour, MoneyListener {

    public static BigDecimal employeeCost() {
        return 10 * BigDecimal.Pow(2, PlanetController.inst.employeeCount);
    }

    public static NewOfficerDialog inst;

    public Image imgBtnBuy, imgCoin;
    public Button btnBuy;
    public TextMeshProUGUI txtCost, txtBuy;
    public RectTransform contCost;

    private GeneratorBehaviour mGenerator = null;

    void Awake() {
        inst = this;
    }

    void Start() {
        hide();
        Database.inst.moneyListeners.Add(this);
        
        
    }

    public void show(GeneratorBehaviour gen) {
        if(PlanetUiController.inst.uiActiveLvl0()) return;
        mGenerator = gen;
        gameObject.SetActive(true);
        PlanetUiController.inst.enableUiModeLvl0();

        updateText();
        checkCanPay();
    }

    public void hide() {
        mGenerator = null;
        gameObject.SetActive(false);
        PlanetUiController.inst.disableUiModeLvl0();

    }

    private void checkCanPay() {
        if(mGenerator == null) return;
        if (Database.inst.getPrimaryMoney() >= employeeCost()) { // can pay
            btnBuy.interactable = true;
            txtCost.color = Color.white;
            txtBuy.color = Color.white;
            imgBtnBuy.color = Color.white;
            imgCoin.color = Color.white;
        } else { // cannot pay
            btnBuy.interactable = false;
            Color red = Database.inst.noMoneyRedColor;
            red.a = 0.5f;
            txtCost.color = red;
            txtBuy.color = red;
            Color semiWhite = new Color(1, 1, 1, 0.5f);
            imgBtnBuy.color = semiWhite;
            imgCoin.color = semiWhite;
        }
    }

    private void updateText() {
        txtCost.text = Formatter.formatNumber(employeeCost());
        LayoutRebuilder.ForceRebuildLayoutImmediate(contCost);
    }

    public void onButtonBuyClick() {
        Database.inst.removePrimaryMoney(employeeCost());
        if (mGenerator != null) mGenerator.setAuto(true);
        hide();
        PlanetController.inst.employeeCount++;
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

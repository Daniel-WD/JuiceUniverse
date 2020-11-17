using System.Collections;
using System.Collections.Generic;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlanetReprController : ClickBehaviour, MoneyListener {

    public int costMantissa, costExponent;

    public SpriteRenderer spritePlanet, spriteLocketPlanet, spriteProductionBg, spriteCostBg;
    public TextMeshProUGUI txtCost, txtProduction;
    public RectTransform costContainer, productionContainer;

    public Color cantPayColor;

    private BigDecimal mCost;

    void Start() {
        // Database.inst.moneyListeners.Add(this);
        mCost = new BigDecimal(costMantissa, costExponent);
    }

    public void set(BigDecimal production) {
        spritePlanet.enabled = production >= 0;
        spriteLocketPlanet.enabled = production < 0;
        spriteProductionBg.gameObject.SetActive(production >= 0);
        spriteCostBg.gameObject.SetActive(production < 0);

        txtProduction.text = Formatter.formatNumber(production) + "/s";
        txtCost.text = Formatter.formatNumber(mCost);

        LayoutRebuilder.ForceRebuildLayoutImmediate(costContainer);
        LayoutRebuilder.ForceRebuildLayoutImmediate(productionContainer);

        checkCanPay();
    }

    private void checkCanPay() {
        txtCost.color = Database.inst.getPrimaryMoney() >= new BigDecimal(costMantissa, costExponent) ? Color.white : cantPayColor;
    }

    protected override void onClick() {
        if (Database.inst.planets[MapController.inst.findIndex(this)] == null) {
            BuyPlanetDialog.inst.show(this);
        } else {
            SceneManager.UnloadSceneAsync("Map");
            PlanetController.inst.savePlanet();
            int myDex = MapController.inst.findIndex(this);
            PlanetController.inst.loadPlanet(myDex);
            PlanetUiController.inst.planetUi.SetActive(true);
        }
    }

    public BigDecimal getCost() {
        return mCost;
    }

    public void onPrimaryMoneyChanged(BigDecimal newMoney) {
        checkCanPay();
    }

    public void onSecondaryMoneyChanged(BigDecimal newMoney) {
    }

}

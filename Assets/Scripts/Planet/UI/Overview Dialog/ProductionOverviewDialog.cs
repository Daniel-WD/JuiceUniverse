using System.Collections;
using System.Collections.Generic;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductionOverviewDialog : MonoBehaviour, PerkManagerListener {

    public static ProductionOverviewDialog inst;

    public GameObject fillStationItemInst, lastFillStationItemInst;

    public TextMeshProUGUI txtBottleDistributerRate, txtBottleDistributerLvl,
                            txtMoneyCollectorRate, txtMoneyCollectorLvl,
                            txtFillStationsRateSum;

    public RectTransform containerBottleDistributerRate, containerMoneyCollectorRate, containerFillStationsRateSum;

    public VerticalLayoutGroup fillStationsContainer;

    private List<FillStationItem> mFillStationItems = new List<FillStationItem>();

    void Awake() {
        inst = this;
    }

    void Start() {
        hide();
    }

    public void show() {
        if (PlanetUiController.inst.uiActiveLvl0()) return;
        PerkManager.inst.addListener(this);
        gameObject.SetActive(true);
        PlanetUiController.inst.enableUiModeLvl0();
        updateData();
    }

    public void hide() {
        PerkManager.inst.removeListener(this);
        gameObject.SetActive(false);
        PlanetUiController.inst.disableUiModeLvl0();

    }

    private void updateData() { //ON PERK CHANGED

        //bottle distributer
        txtBottleDistributerLvl.text = "Level " + BottleDistributerController.inst.getLevel();
        txtBottleDistributerRate.text = Formatter.formatNumber(((ElevatorGeneratorData)BottleDistributerController.inst.getGeneratorData()).realRate) + "/s";
        LayoutRebuilder.ForceRebuildLayoutImmediate(containerBottleDistributerRate);

        //money collector
        txtMoneyCollectorLvl.text = "Level " + MoneyCollectorController.inst.getLevel();
        txtMoneyCollectorRate.text = Formatter.formatNumber(((ElevatorGeneratorData)MoneyCollectorController.inst.getGeneratorData()).realRate) + "/s";
        LayoutRebuilder.ForceRebuildLayoutImmediate(containerMoneyCollectorRate);

        //fill station
        //destroy old
        foreach (FillStationItem item in mFillStationItems) {
            ObjectPooler.inst.addToPool(item.gameObject);
        }

        //fill
        BigDecimal fillStationsRateSum = 0;
        for (int i = 0; i < PlanetController.inst.bottleFillers.Count; i++) {
            BottleFillerController contr = PlanetController.inst.bottleFillers[i];
            FillStationItem item = ObjectPooler.inst.giveMe(i == PlanetController.inst.bottleFillers.Count - 1 ? lastFillStationItemInst : fillStationItemInst, fillStationsContainer.transform).GetComponent<FillStationItem>();
            BigDecimal rate = ((BottleFillerGeneratorData)contr.getGeneratorData()).realRate;
            item.set(contr.getLevel(), PlanetController.inst.bottleFillers.IndexOf(contr) + 1, rate, i % 2 == 0);
            mFillStationItems.Add(item);
            fillStationsRateSum += rate;
        }

        txtFillStationsRateSum.text = Formatter.formatNumber(fillStationsRateSum) + "/s";
        LayoutRebuilder.ForceRebuildLayoutImmediate(containerFillStationsRateSum);

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)fillStationsContainer.transform);

    }

    public void onButtonCloseClick() {
        hide();
    }

    public void onPerkUsable(PerkInstance perk) {
    }

    public void onPerkActivated(PerkInstance perk) {
        if (gameObject.activeSelf) updateData();
    }

    public void onPerkSleep(PerkInstance perk) {
        if (gameObject.activeSelf) updateData();
    }
}

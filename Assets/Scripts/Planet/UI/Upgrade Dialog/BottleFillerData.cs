using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BottleFillerData : DataContainer {

    public TextMeshProUGUI title;
    public TextMeshProUGUI productionRate, productionRatePlus;
    public TextMeshProUGUI bottleCapacity, bottleCapacityPlus;
    public TextMeshProUGUI bottleFillSpeed, bottleFillSpeedPlus;
    public TextMeshProUGUI fillRate, fillRatePlus;

    public override void updateData(int previewLevelCount) {
        int genLvl = generatorBehaviour.getLevel();

        int contrIndex = PlanetController.inst.bottleFillers.IndexOf((BottleFillerController)generatorBehaviour);

        title.text = "Fill Station " + (contrIndex+1);

        BottleFillerGeneratorData currentData = (BottleFillerGeneratorData)generatorBehaviour.getGeneratorData();
        BottleFillerGeneratorData nextData = BottleFillerController.applyPerksToData(BottleFillerController.calcGeneratorData(genLvl + previewLevelCount, contrIndex), generatorBehaviour.getPerks());

        productionRate.text = Formatter.formatNumber(currentData.realRate) + "/s";
        bottleCapacity.text = Formatter.formatNumber(currentData.bottleCapacity);
        bottleFillSpeed.text = Formatter.formatNumber(currentData.bottleFillPs) + "/s";
        fillRate.text = currentData.productionRate.ToString();

        productionRatePlus.text = "+" + Formatter.formatNumber(nextData.realRate - currentData.realRate);
        bottleCapacityPlus.text = "+" + Formatter.formatNumber(nextData.bottleCapacity - currentData.bottleCapacity);
        bottleFillSpeedPlus.text = "+" + Formatter.formatNumber(nextData.bottleFillPs - currentData.bottleFillPs);
        fillRatePlus.text = "+" + (nextData.productionRate - currentData.productionRate).ToString();

        updateCoinLayouts();
    }

}

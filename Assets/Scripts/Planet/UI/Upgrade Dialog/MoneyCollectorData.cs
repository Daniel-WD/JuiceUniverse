using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyCollectorData : DataContainer {

    public TextMeshProUGUI productionRate, productionRatePlus;
    public TextMeshProUGUI moneyCapacity, moneyCapacityPlus;
    public TextMeshProUGUI fillSpeed, fillSpeedPlus;
    public TextMeshProUGUI moveSpeed, moveSpeedPlus;

    public override void updateData(int previewLevelCount) {
        int genLvl = generatorBehaviour.getLevel();

        ElevatorGeneratorData currentData = (ElevatorGeneratorData)generatorBehaviour.getGeneratorData();
        ElevatorGeneratorData nextData = ElevatorController.applyPerksToData(ElevatorController.calcGeneratorData(genLvl + previewLevelCount, true), generatorBehaviour.getPerks());

        productionRate.text = Formatter.formatNumber(currentData.realRate) + "/s";
        moneyCapacity.text = Formatter.formatNumber(currentData.maxAmount);
        fillSpeed.text = Formatter.formatNumber(currentData.fillPs) + "/s";
        moveSpeed.text = Formatter.formatNumber(3f / currentData.stationMovingTime);

        productionRatePlus.text = "+" + Formatter.formatNumber(nextData.realRate - currentData.realRate);
        moneyCapacityPlus.text = "+" + Formatter.formatNumber(nextData.maxAmount - currentData.maxAmount);
        fillSpeedPlus.text = "+" + Formatter.formatNumber(nextData.fillPs - currentData.fillPs);
        moveSpeedPlus.text = "+" + Formatter.formatNumber(3f / nextData.stationMovingTime - 3f / currentData.stationMovingTime);

        updateCoinLayouts();
    }

}

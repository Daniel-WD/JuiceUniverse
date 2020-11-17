using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BottleDistributerData : DataContainer {

    public TextMeshProUGUI productionRate, productionRatePlus;
    public TextMeshProUGUI juiceCapacity, juiceCapacityPlus;
    public TextMeshProUGUI fillSpeed, fillSpeedPlus;
    public TextMeshProUGUI moveSpeed, moveSpeedPlus;

    public override void updateData(int previewLevelCount) {
        int genLvl = generatorBehaviour.getLevel();
        ElevatorGeneratorData currentData = (ElevatorGeneratorData)generatorBehaviour.getGeneratorData();
        
        ElevatorGeneratorData nextData = ElevatorController.applyPerksToData(ElevatorController.calcGeneratorData(genLvl + previewLevelCount, false), generatorBehaviour.getPerks());
        // ElevatorGeneratorData nextData = ElevatorController.calcGeneratorData(genLvl + previewLevelCount, false);

        productionRate.text = Formatter.formatNumber(currentData.realRate) + "/s";
        juiceCapacity.text = Formatter.formatNumber(currentData.maxAmount);
        fillSpeed.text = Formatter.formatNumber(currentData.fillPs) + "/s";
        moveSpeed.text = Formatter.formatNumber(3f / currentData.stationMovingTime);
        // moveSpeed.text = (3f / currentData.stationMovingTime).ToString();

        productionRatePlus.text = "+" + Formatter.formatNumber(nextData.realRate - currentData.realRate);
        juiceCapacityPlus.text = "+" + Formatter.formatNumber(nextData.maxAmount - currentData.maxAmount);
        fillSpeedPlus.text = "+" + Formatter.formatNumber(nextData.fillPs - currentData.fillPs);
        moveSpeedPlus.text = "+" + Formatter.formatNumber(3f / nextData.stationMovingTime - 3f / currentData.stationMovingTime);

        updateCoinLayouts();
    }

}

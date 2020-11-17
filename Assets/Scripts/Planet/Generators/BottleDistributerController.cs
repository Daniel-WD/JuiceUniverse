using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.Playables;

public class BottleDistributerController : ElevatorController {

    public static BottleDistributerController inst;

    public ObjectLoopSpawner fillAnimationSpawner, unloadAnimationSpawner;

    public Animator firstReusedBox, secondReusedBox;

    private BigDecimal[] mDistributionShare;
    private BigDecimal mCurrentMaxAmount;

    private bool mCancelCycle = false;

    protected override void Awake() {
        base.Awake();
        inst = this;
    }

    protected override void Update() {
        base.Update();

        // after docking on a station
        if (docked && !inProgress && PlanetController.inst.bottleFillers.Count > 0) {
            if (currentStationDex > -1) { // on station
                startUnloading();

            } else { // on base
                if (auto) startCycle();
                else {

                    startHighlight();

                    // HighlightZygote.inst.addAnimator(highlightAnimator);
                }

            }
        }

    }

    public override void startCycle() {
        if (!docked || inProgress || PlanetController.inst.bottleFillers.Count == 0 || mCancelCycle) {
            mCancelCycle = false;
            return;
        }

        stopHighlight();

        // HighlightZygote.inst.removeAnimator(highlightAnimator);

        autoControllerAnimator.SetTrigger("send_elevator_left");
        startFill();
    }

    public override void stopCycle() {
        mCancelCycle = true;
        fillAnimationSpawner.cancel();
        unloadAnimationSpawner.cancel();
    }

    private void startFill() {
        if (mCancelCycle) {
            mCancelCycle = false;
            return;
        }
        inProgress = true;
        float time = (float)(generatorData.maxAmount / generatorData.fillPs);
        progressbar.start(time);
        Invoke("applyFill", time);
        startFillAnimation(time);
    }

    private void startFillAnimation(float time) {

        firstReusedBox.SetTrigger("first_box_insert");
        secondReusedBox.SetTrigger("second_box_hide");

        float realFillDuration = time - 0.183f - 0.133f;

        StartCoroutine(fillAnimationSpawner.spawnWithDelay(realFillDuration, 0.183f));
        Invoke("startFillFinishedAnimation1", realFillDuration + 0.133f);
        Invoke("startFillFinishedAnimation2", realFillDuration + 2 * 0.133f);
    }

    private void startFillFinishedAnimation1() {
        firstReusedBox.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        firstReusedBox.SetTrigger("last_first_box_stop");
    }

    private void startFillFinishedAnimation2() {
        secondReusedBox.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        secondReusedBox.SetTrigger("last_second_box_stop");
    }

    private void applyFill() {
        if(mCancelCycle) {
            mCancelCycle = false;
            return;
        }
        valueHolder.saveValue(generatorData.maxAmount);

        calcDistributionShare();
        mCurrentMaxAmount = generatorData.maxAmount;

        moveToNextStation();
    }

    private void calcDistributionShare() {
        mDistributionShare = new BigDecimal[PlanetController.inst.bottleFillers.Count];
        BigDecimal sumProdRate = 0;
        foreach (var prod in PlanetController.inst.bottleFillers) {
            sumProdRate += ((BottleFillerGeneratorData)prod.getGeneratorData()).realRate;
        }
        for (int i = 0; i < mDistributionShare.Length; i++) {
            var prod = PlanetController.inst.bottleFillers[i];
            mDistributionShare[i] = ((BottleFillerGeneratorData)prod.getGeneratorData()).realRate / sumProdRate;
        }
    }

    private void startUnloading() {
        if(mCancelCycle) {
            mCancelCycle = false;
            return;
        }
        inProgress = true;
        BigDecimal changeAmount = mCurrentMaxAmount * mDistributionShare[currentStationDex];

        float time = (float)(changeAmount / generatorData.fillPs);
        progressbar.start(time);
        Invoke("applyUnloading", time);
        unloadAnimationSpawner.spawn(time);
    }

    private void applyUnloading() {
        if(mCancelCycle) {
            mCancelCycle = false;
            return;
        }
        BigDecimal changedAmount = mCurrentMaxAmount * mDistributionShare[currentStationDex];

        if (currentStationDex == PlanetController.inst.bottleFillers.Count - 1 || currentStationDex == mDistributionShare.Length - 1) {
            changedAmount = valueHolder.getValue();
        }

        BottleFillerController prod = PlanetController.inst.bottleFillers[currentStationDex];
        BigDecimal realValue = valueHolder.removeValue(changedAmount);
        prod.inputValueHolder.saveValue(realValue);
        moveToNextStation(mDistributionShare.Length - 1);
    }

    public override void updateGeneratorData() {

        generatorData = ElevatorController.applyPerksToData(calcGeneratorData(level, false), getPerks());
        // generatorData = calcGeneratorData(level, false);

    }

    public override int possibleUpgradeCount(int maxCount) {
        return GeneratorValues.bottleDistributerUpgradeCountForCurrentPrimMoney(level, maxCount);
    }

    public override bool canUpgrade() {
        return GeneratorValues.bottleDistributerCost(level) <= Database.inst.getPrimaryMoney();
    }
    
    public override void reset() {
        base.reset();
        firstReusedBox.SetTrigger("cancel");
        secondReusedBox.SetTrigger("cancel");
        PlanetController.inst.destroyChildren(fillAnimationSpawner.transform);
    }
}

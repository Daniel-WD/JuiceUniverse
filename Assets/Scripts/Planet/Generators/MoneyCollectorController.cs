using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class MoneyCollectorController : ElevatorController {

    public static MoneyCollectorController inst;

    public GameObject newMoneyInst;
    public Transform newMoneyInstSpawn;
    public float newMoneyInstSpawnRadius;

    public ObjectLoopSpawner deloadAnimSpawner, insertAnimSpawner;

    private bool mCancelCycle = false;
    private GameObject mLastNewMoneyInstance;
    
    protected override void Awake() {
        base.Awake();        
        inst = this;
    }

    protected override void Update() {
        base.Update();

        // after docking on a station
        if (docked && !inProgress && PlanetController.inst.bottleFillers.Count > 0) {
            if (currentStationDex > -1) { // on station
                startInsert();

            } else { // on base
                if (valueHolder.getValue() > 0) {
                    startDeload();
                } else if (isThereAStationWithMoney()) {
                    if (auto) startCycle();
                    else {
                        
                        startHighlight();
                        
                        // HighlightZygote.inst.addAnimator(highlightAnimator);
                    }
                }

            }
        }

    }

    private bool isThereAStationWithMoney() {
        foreach (var filler in PlanetController.inst.bottleFillers) {
            if (filler.outputValueHolder.getValue() > 0) return true;
        }
        return false;
    }

    public override void startCycle() {
        if (!docked || inProgress || PlanetController.inst.bottleFillers.Count == 0 || mCancelCycle) {
            mCancelCycle = false;
            return;
        }
        
        stopHighlight();
        
        // HighlightZygote.inst.removeAnimator(highlightAnimator);
        
        autoControllerAnimator.SetTrigger("send_elevator_right");
        moveToNextStationWithMoney();
    }
    
    public override void stopCycle() {
        mCancelCycle = true;
        deloadAnimSpawner.cancel();
        insertAnimSpawner.cancel();
    }

    private void startInsert() {
        if(mCancelCycle) {
            mCancelCycle = false;
            return;
        }
        inProgress = true;
        BottleFillerController contr = PlanetController.inst.bottleFillers[currentStationDex];
        BigDecimal floatingValue = freeAmount() > contr.outputValueHolder.getValue() ? contr.outputValueHolder.getValue() : freeAmount();
        float time = (float)(floatingValue / generatorData.fillPs);
        progressbar.start(time);
        StartCoroutine(applyInsert(floatingValue, time));
        insertAnimSpawner.spawn(time);
    }

    private IEnumerator applyInsert(BigDecimal floatingValue, float delay) {
        yield return new WaitForSeconds(delay);
        if(mCancelCycle) {
            mCancelCycle = false;
            yield break;
        }
        BottleFillerController contr = PlanetController.inst.bottleFillers[currentStationDex];
        BigDecimal reducedValue = contr.outputValueHolder.removeValue(floatingValue);
        valueHolder.saveValue(reducedValue);
        if (freeAmount() > 0) {
            moveToNextStationWithMoney();

        } else {
            currentStationDex = -1;
            inProgress = false;
        }
    }

    private void startDeload() {
        if(mCancelCycle) {
            mCancelCycle = false;
            return;
        }
        inProgress = true;
        float time = (float)(valueHolder.getValue() / generatorData.fillPs);
        progressbar.start(time);
        Invoke("applyDeload", time);
        deloadAnimSpawner.spawn(time);
    }

    private void applyDeload() {
        if(mCancelCycle) {
            mCancelCycle = false;
            return;
        }
        BigDecimal changedAmount = valueHolder.getValue();
        BigDecimal reduced = valueHolder.removeValue(changedAmount);
        
        //apply multiplier
        reduced *= BoostManager.inst.calcFinalMultiplier();
        
        Database.inst.addPrimaryMoney(reduced);
        inProgress = false;

        //spawn new money inst
        GameObject newMoney = Instantiate(newMoneyInst, newMoneyInstSpawn.position + (Vector3)Random.insideUnitCircle * newMoneyInstSpawnRadius, Quaternion.identity);
        NewMoneyController contr = newMoney.GetComponent<NewMoneyController>();
        contr.setValue(reduced);
        mLastNewMoneyInstance = newMoney;
    }

    private BigDecimal freeAmount() {
        return generatorData.maxAmount - valueHolder.getValue();
    }

    public override void updateGeneratorData() {
        generatorData = ElevatorController.applyPerksToData(calcGeneratorData(level, true), getPerks());
    }

    public override int possibleUpgradeCount(int maxCount) {
        return GeneratorValues.moneyCollectorUpgradeCountForCurrentPrimMoney(level, maxCount);
    }
    
    public override bool canUpgrade() {
        return GeneratorValues.moneyCollectorCost(level) <= Database.inst.getPrimaryMoney();
    }
    
    public override void reset() {
        base.reset();
        Destroy(mLastNewMoneyInstance);
        PlanetController.inst.destroyChildren(insertAnimSpawner.transform);
    }
}

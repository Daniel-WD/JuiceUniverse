using System.Collections;
using System.Collections.Generic;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct BottleFillerGeneratorData {

    public BigDecimal realRate;
    public BigDecimal bottleCapacity;
    public BigDecimal bottleFillPs;
    public float bottleMovingTime;
    public float productionRate;
    
    public BottleFillerGeneratorData clone() {
        BottleFillerGeneratorData clone = new BottleFillerGeneratorData();
        clone.realRate = realRate;
        clone.bottleCapacity = bottleCapacity;
        clone.bottleFillPs = bottleFillPs;
        clone.bottleMovingTime = bottleMovingTime;
        clone.productionRate = productionRate;
        return clone;
    }

}

public class BottleFillerController : GeneratorBehaviour {

    public static BottleFillerGeneratorData calcGeneratorData(int level, int dex) {
        int jumpedLevels = GeneratorValues.firstResultJumpedLevels(dex);

        BigDecimal result = GeneratorValues.bottleFillerRate(level, dex);

        float boostMultiplier = Mathf.Pow(2, GeneratorValues.bottleFillerBoostCount(level));
        result *= boostMultiplier;

        BottleFillerGeneratorData data = new BottleFillerGeneratorData();
        data.productionRate = 1f * boostMultiplier;
        data.bottleMovingTime = 1.5f;

        double exponent = jumpedLevels + level + 11.96, expIncrement = 0.001;
        double increment = GeneratorValues.BOTTLE_FILLER_RESULT_INCREMENT;
        double fillPsInit = (double)(4 * BigDecimal.Pow(increment, -10));
        double capacityInit = (double)(8 * BigDecimal.Pow(increment, -10));

        int c = 0;

        BigDecimal rr;

        do {
            c++;
            data.bottleFillPs = fillPsInit * BigDecimal.Pow(increment, exponent);
            data.bottleCapacity = capacityInit * BigDecimal.Pow(increment, exponent);
            exponent += expIncrement;
            rr = realRate(data);
        } while (rr < result);
        
        data.realRate = rr;

        // Debug.Log(c + " --- " + Formatter.bigDecToString(rr) + "  goal: " + Formatter.bigDecToString(result));

        return data;
    }

    public static BigDecimal realRate(BottleFillerGeneratorData data) {
        return (data.bottleCapacity / (data.bottleCapacity / data.bottleFillPs + data.bottleMovingTime)) * data.productionRate;
    }

    public static BottleFillerGeneratorData applyPerksToData(BottleFillerGeneratorData data, List<PerkInstance> perks) {
        BottleFillerGeneratorData newData = data.clone();
        foreach (PerkInstance perkInst in perks) {
            switch (perkInst.perk.property) {
                case PerkProperty.FILL_STATION_CAPACITY:
                    newData.bottleCapacity *= 1 + Perk.STRENGTHS[(int)perkInst.perk.strenght];
                    break;

                case PerkProperty.FILL_STATION_FILL_RATE:
                    newData.productionRate *= 1 + Perk.STRENGTHS[(int)perkInst.perk.strenght];
                    break;

                case PerkProperty.FILL_STATION_FILL_SPEED:
                    newData.bottleFillPs *= 1 + Perk.STRENGTHS[(int)perkInst.perk.strenght];
                    break;
            }
        }
        newData.realRate = realRate(newData);
        return newData;
    }

    //----------------------------------------------------------------

    public ValueHolder inputValueHolder, outputValueHolder; //save
    public GeneratorUpgrader upgrader;
    public SpriteRenderer gfx, etageConnection;
    public TextMeshPro numberText;
    public Transform dockPos;

    public ObjectLoopSpawner fillAnimSpawner;
    public Animator bottlesEmptyAnimator;

    private BottleFillerGeneratorData mGeneratorData;

    private bool mInProgress = false;
    private bool wasActiveOnce = false, bottlesEmptyActive = false;

    private readonly int mCycleTime = 3;

    private bool mCancelCycle = false;

    protected override void Start() {
        base.Start();
        
        updateGeneratorData();

        upgrader.setGeneratorBehaviour(this);

        int indexInPool = PlanetController.inst.bottleFillers.IndexOf(this);
        setNumberText(indexInPool + 1);

        etageConnection.enabled = indexInPool > 0;

    }

    protected override void Update() {
        base.Start();

        if (auto) startCycle();
        else if (inputValueHolder.getValue() > 0 && !mInProgress) {

            startHighlight();

            // HighlightZygote.inst.addAnimator(highlightAnimator);

        }

    }

    public override void startCycle() {
        if (mInProgress || mCancelCycle) {
            mCancelCycle = false;
            return;   
        }
        if (inputValueHolder.getValue() == 0) {
            if (wasActiveOnce && !bottlesEmptyActive) {
                bottlesEmptyAnimator.SetTrigger("show");
                bottlesEmptyActive = true;
            }
            return;
        }
        if (wasActiveOnce && bottlesEmptyActive) bottlesEmptyAnimator.SetTrigger("hide");
        wasActiveOnce = true;
        bottlesEmptyActive = false;
        startProduce();

        stopHighlight();

        // HighlightZygote.inst.removeAnimator(highlightAnimator);

        autoControllerAnimator.SetTrigger("start_fill");
    }
    
    public override void stopCycle() {
        mCancelCycle = true;
        fillAnimSpawner.cancel();
    }

    public override void updateGeneratorData() {
        mGeneratorData = applyPerksToData(calcGeneratorData(level, PlanetController.inst.bottleFillers.IndexOf(this)), getPerks());
    }

    private void startProduce() {
        if(mCancelCycle) {
            mCancelCycle = false;
            return;
        }
        progressbar.start(mCycleTime);
        Invoke("applyProduce", mCycleTime);
        mInProgress = true;
        fillAnimSpawner.spawn(mCycleTime);
    }

    private void applyProduce() {
        if(mCancelCycle) {
            mCancelCycle = false;
            return;
        }
        BigDecimal changedValue = mCycleTime * mGeneratorData.realRate;
        BigDecimal reducedValue = inputValueHolder.removeValue(changedValue);
        outputValueHolder.saveValue(reducedValue);
        mInProgress = false;
    }


    // private IEnumerator spawn() {
    //     while (true) {
    //         spawnThing();
    //         yield return new WaitForSeconds((float)(mThingMovingTime + (mThingCapacity / mThingFillPs)) / mProductionRate);
    //     }
    // }

    // private void spawnThing() {
    //     GameObject thingObj = Instantiate(thingInst, thingSpawn);
    //     ThingController contr = thingObj.GetComponent<ThingController>();
    //     contr.setLookLeft(true);
    //     contr.setLayer(mThingInFrontLayer);
    //     contr.setProductionData(this, (float)(mThingCapacity / mThingFillPs), mThingMovingTime);
    // }

    private void setNumberText(int number) {
        numberText.text = number.ToString();
    }

    public float getHeight() {
        if (gfx == null) return 0;
        return gfx.bounds.size.y;
    }

    public override System.Object getGeneratorData() {
        return mGeneratorData;
    }

    public override int possibleUpgradeCount(int maxCount) {
        return GeneratorValues.bottleFillerUpgradeCountForCurrentPrimMoney(level, maxCount, PlanetController.inst.bottleFillers.IndexOf(this));
    }

    public override bool canUpgrade() {
        return GeneratorValues.bottleFillerCost(level, PlanetController.inst.bottleFillers.IndexOf(this)) <= Database.inst.getPrimaryMoney();
    }
    
    public override void reset() {
        base.reset();
        inputValueHolder.setValue(0);
        outputValueHolder.setValue(0);
    }

}

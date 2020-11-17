using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public struct ElevatorGeneratorData {
    
    public BigDecimal realRate;
    public BigDecimal maxAmount;
    public BigDecimal fillPs;
    public float stationMovingTime;
    
    public ElevatorGeneratorData clone() {
        ElevatorGeneratorData clone = new ElevatorGeneratorData();
        clone.realRate = realRate;
        clone.maxAmount = maxAmount;
        clone.fillPs = fillPs;
        clone.stationMovingTime = stationMovingTime;
        return clone;
    }
    
    public void print() {
        Debug.Log($"realRate: {realRate}, maxAmount: {maxAmount}, fillPs: {fillPs}, stationMovingTime: {stationMovingTime}");
    }
    
}

public abstract class ElevatorController : GeneratorBehaviour {

    public static ElevatorGeneratorData calcGeneratorData(int level, bool moneyCollector) {
        BigDecimal result = moneyCollector ? GeneratorValues.moneyCollectorRate(level) : GeneratorValues.bottleDistributerRate(level);
        
        float boostMultiplier = Mathf.Pow(2, moneyCollector ? GeneratorValues.moneyCollectorBoostCount(level) : GeneratorValues.bottleDistributerBoostCount(level));
        result *= boostMultiplier;
        
        ElevatorGeneratorData data = new ElevatorGeneratorData();
        data.stationMovingTime = 1f / (Mathf.Pow(boostMultiplier, 0.5f));

        float div = PlanetController.inst.bottleFillers.Count;
        if(div <= 0) div = 1;

        float partBoostMultiplier = Mathf.Pow((float)boostMultiplier, 1f/2f) * 1.5f;
        
        // double exponent = (double)level + 17.53 + Mathf.Pow(div, 1f/2.3f) * 0.01, expIncrement = 0.001;
        double expIncrement = 0.01;
        double exponent = (double) level + level * boostMultiplier * expIncrement + 16.35 + Mathf.Pow(div, 1f/2.3f);
        double increment = 1.4;
        double fillPsInit = (double)(3 * BigDecimal.Pow(increment, -10));
        double maxAmountInit = (double)(9 * BigDecimal.Pow(increment, -10));

        BigDecimal rr;
        
        int c = 0; 

        do {
            c++;
            data.fillPs = fillPsInit * BigDecimal.Pow(increment, exponent);
            data.maxAmount = maxAmountInit * BigDecimal.Pow(increment, exponent);
            exponent += expIncrement;
            rr = realRate(data);
        } while (rr < result && PlanetController.inst.bottleFillers.Count > 0);
        
        data.realRate = rr;
        
        return data;
    }

    private static BigDecimal realRate(ElevatorGeneratorData data) {
        int producerCount = PlanetController.inst.bottleFillers.Count;
        return (data.maxAmount) / (2 * producerCount * data.stationMovingTime + 2 * data.maxAmount / data.fillPs);
    }
    
    public static ElevatorGeneratorData applyPerksToData(ElevatorGeneratorData data, List<PerkInstance> perks) {
        ElevatorGeneratorData newData = data.clone();
        foreach (PerkInstance perkInst in perks) {
            switch (perkInst.perk.property) {
                case PerkProperty.ELEVATOR_CAPACITY:
                    newData.maxAmount *= 1 + Perk.STRENGTHS[(int)perkInst.perk.strenght];
                    break;

                case PerkProperty.ELEVATOR_FILL_SPEED:
                    newData.fillPs *= 1 + Perk.STRENGTHS[(int)perkInst.perk.strenght];
                    break;

                case PerkProperty.ELEVATOR_MOVE_SPEED:
                    newData.stationMovingTime *= 1 - Perk.STRENGTHS[(int)perkInst.perk.strenght];
                    break;
            }
        }
        newData.realRate = realRate(newData);
        return newData;
    }
    
    //----------------------------------------------------

    public ValueHolder valueHolder; // save
    public GeneratorUpgrader upgrader;
    public SpriteRenderer gfx;
    public Transform dockPos;

    protected Vector3 origin;

    protected bool docked = true;

    protected int currentStationDex = -1;

    protected ElevatorGeneratorData generatorData;

    protected float movingDelta = 0;

    protected bool inProgress;
    
    protected override void Start() {
        base.Start();
        
        origin = transform.position;

        upgrader.setGeneratorBehaviour(this);
        
        updateGeneratorData();
    }

    protected override void Update() {
        base.Update();

        // movement
        Vector3 targetPos = transform.position;

        if (currentStationDex == -1) {
            targetPos.y = origin.y;
        } else {
            BottleFillerController producer = PlanetController.inst.bottleFillers[currentStationDex];
            Vector3 dockDelta = producer.dockPos.position - dockPos.position;
            targetPos.y = producer.dockPos.position.y + (transform.position.y - dockPos.position.y);
        }

        if (transform.position == targetPos) {
            docked = true;
        } else {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, movingDelta * Time.deltaTime);
            docked = false;
        }

    }
    
    private void calcMovingDelta(int lastStationDex) {
        if (currentStationDex > -1) {
            Vector3 targetPos = PlanetController.inst.bottleFillers[currentStationDex].transform.position;
            movingDelta = Mathf.Abs(targetPos.y - transform.position.y) / generatorData.stationMovingTime;
        } else {
            Vector3 targetPos = origin;
            movingDelta = Mathf.Abs(targetPos.y - transform.position.y) / (generatorData.stationMovingTime * (lastStationDex + 1));
        }

    }

    protected void moveToNextStation(int maxDex = -1) {
        int last = currentStationDex;
        currentStationDex++;
        if (currentStationDex >= PlanetController.inst.bottleFillers.Count || (maxDex >= 0 && currentStationDex > maxDex)) {
            currentStationDex = -1;
        }
        calcMovingDelta(last);
        inProgress = false;
    }

    protected bool moveToNextStationWithMoney() {
        int last = currentStationDex;
        do {
            currentStationDex++;
            if (currentStationDex >= PlanetController.inst.bottleFillers.Count) currentStationDex = -1;
        } while (currentStationDex > -1 && PlanetController.inst.bottleFillers[currentStationDex].outputValueHolder.getValue() <= 0);
        calcMovingDelta(last);
        inProgress = false;
        return currentStationDex > -1;
    }

    public float getTopEdge() {
        return gfx.bounds.max.y;
    }
    
    public override System.Object getGeneratorData() {
        return generatorData;
    }
    
    public override void reset() {
        base.reset();
        
        transform.position = origin;
        movingDelta = 0;
        docked = true;
        inProgress = false;
        
        valueHolder.setValue(0);
        
        currentStationDex = -1;
        
    }

}
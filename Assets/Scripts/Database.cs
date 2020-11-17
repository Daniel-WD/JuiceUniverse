using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using Common;
using UnityEngine;

public interface MoneyListener {

    void onPrimaryMoneyChanged(BigDecimal newMoney);
    void onSecondaryMoneyChanged(BigDecimal newMoney);

}

public class Database : MonoBehaviour {

    private string mSavePath;

    public GameItem[] dailyRewards;
    // [HideInInspector][NonSerialized]
    public bool[] dailyRewardUsed;

    // [HideInInspector][NonSerialized]
    public Color noMoneyRedColor, btnDisabledWhiteTextColor, btnDisabledColor;

    private BigDecimal mPrimaryMoney = 1000000000, mSecondaryMoney;

    [HideInInspector][NonSerialized]
    public List<MoneyListener> moneyListeners = new List<MoneyListener>();

    [HideInInspector][NonSerialized]
    public PlanetSaveData[] planets = new PlanetSaveData[5];
    [HideInInspector][NonSerialized]
    public int currentPlanetIndex = -1;

    public static Database inst;

    void Awake() {
        inst = this;
        
        mSavePath = Application.persistentDataPath + "/save.me";
        
        //load saved data
        loadGameData();

        //daily reward stuff
        dailyRewardUsed = new bool[dailyRewards.Length];
        for (int i = 0; i < dailyRewardUsed.Length; i++) {
            dailyRewardUsed[i] = UnityEngine.Random.Range(0, 2) == 0;
        }
        
    }
    
    void OnApplicationQuit() {
        saveGameData();
    }

    void OnApplicationPause(bool pause) {
        if (pause) saveGameData();
    }
    
    void Start() {
        //load current planet
        Invoke("loadPlanet", 1);
    }
    
    private void loadPlanet() {
        PlanetController.inst.loadPlanet(currentPlanetIndex < 0 ? 0 : currentPlanetIndex, true);
    }

    public void saveGameData() {

        // if(true) return;
        PlanetController.inst.savePlanet();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(mSavePath);
        
        GameSaveData saveData = generateGameSaveData();
        bf.Serialize(file, saveData);
        file.Close();

    }

    public void loadGameData() {

        File.Delete(mSavePath);  

        if (File.Exists(mSavePath)) { 
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(mSavePath, FileMode.Open);
            applyGameSaveData((GameSaveData)bf.Deserialize(file));
            file.Close();
        } else {
            // firstStart = true;
        }

    }

    public void addPrimaryMoney(BigDecimal money) {
        mPrimaryMoney += money;
        PlanetUiController.inst.updateMoneyText();
        foreach (var l in moneyListeners) l.onPrimaryMoneyChanged(mPrimaryMoney);
    }

    public void addSecondaryMoney(BigDecimal money) {
        mSecondaryMoney += money;
        PlanetUiController.inst.updateMoneyText();
        foreach (var l in moneyListeners) l.onSecondaryMoneyChanged(mSecondaryMoney);
    }

    public void removePrimaryMoney(BigDecimal money) {
        mPrimaryMoney -= money;
        PlanetUiController.inst.updateMoneyText();
        foreach (var l in moneyListeners) l.onPrimaryMoneyChanged(mPrimaryMoney);
    }

    public void removeSecondaryMoney(BigDecimal money) {
        mSecondaryMoney -= money;
        PlanetUiController.inst.updateMoneyText();
        foreach (var l in moneyListeners) l.onSecondaryMoneyChanged(mSecondaryMoney);
    }

    public BigDecimal getPrimaryMoney() {
        return mPrimaryMoney;
    }

    public BigDecimal getSecondaryMoney() {
        return mSecondaryMoney;
    }
    
    private GameSaveData generateGameSaveData() {
        GameSaveData data = new GameSaveData();
        data.planets = planets;
        data.primMoney = new BigDecimalSaveData(mPrimaryMoney.Mantissa.ToString(), mPrimaryMoney.Exponent);
        data.secMoney = new BigDecimalSaveData(mSecondaryMoney.Mantissa.ToString(), mSecondaryMoney.Exponent);
        data.dailyRewardUsed = dailyRewardUsed;
        data.curPlanetIndex = currentPlanetIndex;
        return data;
    }
    
    private void applyGameSaveData(GameSaveData data) {
        for(int i = 0; i < Mathf.Min(planets.Length, data.planets.Length); i++) {
            planets[i] = data.planets[i];
        }
        mPrimaryMoney = data.primMoney.value();
        mSecondaryMoney = data.secMoney.value();
        for(int i = 0; i < Mathf.Min(dailyRewardUsed.Length, data.dailyRewardUsed.Length); i++) {
            dailyRewardUsed[i] = data.dailyRewardUsed[i];
        }
        currentPlanetIndex = data.curPlanetIndex;
    }

}

[Serializable]
public class GameSaveData {
    
    [SerializeField]
    public PlanetSaveData[] planets;
    [SerializeField]
    public BigDecimalSaveData secMoney, primMoney;
    [SerializeField]
    public bool[] dailyRewardUsed;
    public int curPlanetIndex;
    
}

[Serializable]
public class PlanetSaveData {
    
    [NonSerialized]
    public BigDecimal production;
    
    public GeneratorSaveData bottleDistributerData;
    public GeneratorSaveData moneyCollectorData;
    [SerializeField]
    public GeneratorSaveData[] fillStationDatas;
    
    [SerializeField]
    public PerkInstSaveData[] usablePerkInstances;
    [SerializeField]
    public PerkInstSaveData[] sleepingPerkInstances;
    
    [SerializeField]
    public BoostInstSaveData[] usableBoostInstances;
    [SerializeField]
    public BoostInstSaveData[] activeBoostInstances;
    
    public int employeeCount;
    
    public PlanetSaveData() {}
    
    public PlanetSaveData(GeneratorSaveData bottleDistrData, GeneratorSaveData moneyCollData, 
                            GeneratorSaveData[] fillStatDatas, PerkInstSaveData[] usablePerkInsts,
                            PerkInstSaveData[] sleepingPerkInsts, BoostInstSaveData[] usableBoostInsts,
                            BoostInstSaveData[] activeBoostInsts, int emplCount, BigDecimal prod) {
        bottleDistributerData = bottleDistrData;
        moneyCollectorData = moneyCollData;
        fillStationDatas = fillStatDatas;
        usablePerkInstances = usablePerkInsts;
        sleepingPerkInstances = sleepingPerkInsts;
        usableBoostInstances = usableBoostInsts;
        activeBoostInstances = activeBoostInsts;
        production = prod;
        employeeCount = emplCount;
    }
    
}

[Serializable]
public class GeneratorSaveData {
    
    public bool auto;
    public int level;
    
    [SerializeField]
    public PerkInstSaveData[] perks;
    
    public BigDecimalSaveData valueHolderValueOne;
    public BigDecimalSaveData valueHolderValueTwo;
    
    public GeneratorSaveData() {}
    
    public GeneratorSaveData(bool auto, int level, PerkInstSaveData[] perks, BigDecimalSaveData valueHolderOne, BigDecimalSaveData valueHolderTwo) {
        this.auto = auto;
        this.level = level;
        this.perks = perks;
        valueHolderValueOne = valueHolderOne;
        valueHolderValueTwo = valueHolderTwo;
    }
    
}

[Serializable]
public class PerkInstSaveData {
    
    public int perkHash;
    
    public long activeTimeLeft;
    public long activeTime;

    public long sleepTimeLeft;
    public long sleepTime;
    
    public PerkInstSaveData() {}
    
    public PerkInstSaveData(int perkHash, long activeTimeLeft, long activeTime, long sleepTimeLeft, long sleepTime) {
        this.perkHash = perkHash;
        this.activeTime = activeTime;
        this.activeTimeLeft = activeTimeLeft;
        this.sleepTime = sleepTime;
        this.sleepTimeLeft = sleepTimeLeft;
    }
    
    public PerkInstance createPerkInstance() {
        PerkInstance instance = new PerkInstance();
        instance.perk = PerkManager.inst.findPerkByHash(perkHash);
        instance.activeTimeLeft = activeTimeLeft;
        instance.activeTime = activeTime;
        instance.sleepTimeLeft = sleepTimeLeft;
        instance.sleepTime = sleepTime;
        return instance;
    }
    
}

[Serializable]
public class BoostInstSaveData {
    
    public int boostHash;
    
    public long activeTimeLeft;
    public long activeTime;
    
    public BoostInstSaveData() {}
    
    public BoostInstSaveData(int boostHash, long activeTime, long activeTimeLeft) {
        this.boostHash = boostHash;
        this.activeTime = activeTime;
        this.activeTimeLeft = activeTimeLeft;
    }
    
    public BoostInstance createBoostInstance() {
        BoostInstance instance = new BoostInstance();
        instance.boost = BoostManager.inst.findBoostByHash(boostHash);
        instance.activeTime = activeTime;
        instance.activeTimeLeft = activeTimeLeft;
        return instance;
    }

}

[Serializable]
public class BigDecimalSaveData {
    
    public string mantissa;
    public int exponent;
    
    public BigDecimalSaveData() {}
    
    public BigDecimalSaveData(string mantissa, int expontent) {
        this.mantissa = mantissa;
        this.exponent = expontent;
    }
    
    public BigDecimal value() {
        return new BigDecimal(BigInteger.Parse(mantissa), exponent);
    }
    
}

// [Serializable]
// class BottleFillerSaveData : GeneratorSaveData {
    
//     public BigDecimalSaveData valueHolderValue;
    
// }

// [Serializable]
// class ElevatorSaveData : GeneratorSaveData {
    
//     public BigDecimalSaveData valueHolderValue;
//     public BigDecimalSaveData valueHolderValue;
    
// }

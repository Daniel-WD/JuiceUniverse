using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class PlanetController : MonoBehaviour {

    public static PlanetController inst;

    void Awake() {
        inst = this;
    }

    public List<BottleFillerController> bottleFillers = new List<BottleFillerController>(); //ship
    public int employeeCount = 0; //ship

    public ElevatorContainerController[] elevatorContainerControllers;
    public NewProducerController[] newProducerControllers;
    public GameObject newEtageBtnsParent;
    public Transform firstProducerSpawn;
    public BottleFillerController producerInst;
    public Sprite etageConnectionInst;

    public GameObject topBuilding;
    private Vector3 mTopBuildingStartPosition;

    public SpriteRenderer leftBand, rightBand;
    private Vector3 mLeftBandStartPos, mRightBandStartPos;

    private float mProdHeight, mEtageConnectionHeight;
    private Vector3 mNewProdLayoutOrigin;

    void Start() {
        mNewProdLayoutOrigin = newEtageBtnsParent.transform.position;
        mProdHeight = producerInst.GetComponent<BottleFillerController>().getHeight();
        mEtageConnectionHeight = etageConnectionInst.bounds.size.y;
        mTopBuildingStartPosition = topBuilding.transform.position;
        mLeftBandStartPos = leftBand.transform.position;
        mRightBandStartPos = rightBand.transform.position;

        // Invoke("askForRating", 2);
        // StartCoroutine(doSmt());
    }

    // PlanetSaveData saveData;
    // int a = 60;
    // public IEnumerator doSmt() {

    //     while (a > 0) {
    //         print(a--);
    //         yield return new WaitForSeconds(1);
    //     }

    //     saveData = generateSaveData();
    //     yield return new WaitForSeconds(5);

    //     resetPlanet();
    //     yield return new WaitForSeconds(5);

    //     loadPlanetData(saveData);
    // }

    public BottleFillerController newProducer() {
        Vector3 lastEtagesHeight = currentEtagesHeight();
        Vector3 pos = firstProducerSpawn.position + lastEtagesHeight;
        BottleFillerController controller = Instantiate(producerInst, pos, Quaternion.identity);
        bottleFillers.Add(controller);

        //reposition newProdLayout
        Vector3 posss = mNewProdLayoutOrigin + currentEtagesHeight();
        newEtageBtnsParent.transform.position = posss;

        //reposition top building part
        Vector3 topBuildingPos = mTopBuildingStartPosition + lastEtagesHeight + (Vector3)(Vector2.up * mProdHeight);
        topBuilding.transform.position = topBuildingPos;

        //reposition bands
        //left
        Vector3 leftBandPos = mLeftBandStartPos + lastEtagesHeight + (Vector3)(Vector2.up * mProdHeight);
        leftBand.transform.position = leftBandPos;

        //right
        Vector3 rightBandPos = mRightBandStartPos + lastEtagesHeight + (Vector3)(Vector2.up * mProdHeight);
        rightBand.transform.position = rightBandPos;

        foreach (var c in elevatorContainerControllers) c.notifyProducerCountChanged();
        foreach (var c in newProducerControllers) c.notifyProducerCountChanged();

        return controller;
    }

    public void resetPlanet() {
        foreach (BottleFillerController contr in bottleFillers) {
            contr.reset();
            Destroy(contr.gameObject);
        }
        bottleFillers.Clear();

        BottleDistributerController.inst.reset();
        MoneyCollectorController.inst.reset();

        newEtageBtnsParent.transform.position = mNewProdLayoutOrigin;
        topBuilding.transform.position = mTopBuildingStartPosition;
        leftBand.transform.position = mLeftBandStartPos;
        rightBand.transform.position = mRightBandStartPos;

        foreach (var c in elevatorContainerControllers) c.notifyProducerCountChanged();
        foreach (var c in newProducerControllers) c.notifyProducerCountChanged();

        BoostManager.inst.deleteAllBoosts();
        PerkManager.inst.deleteAllPerks();

        BoostRepresentationController.inst.refresh();

    }

    public Vector3 currentEtagesHeight() {
        return (Vector3)(Vector2.up * (bottleFillers.Count * mProdHeight + bottleFillers.Count * mEtageConnectionHeight));
    }

    public void destroyChildren(Transform trans) {
        for (int i = 0; i < trans.childCount; i++) {
            Destroy(trans.GetChild(i).gameObject);
        }
    }

    public void askForRating() {
        //Ask
        string titleAsk = "What do you think?";
        string msgAsk = "Hi, do you like this game?";
        string noAsk = "No, I don't like it.";
        string yesAsk = "Yeah, I love it.";

        //Feedback
        string titleFeedback = "Give us Feedback!";
        string msgFeedback = "Would you like to tell us, what we can do to, make our game better for you?";
        string noFeedback = "No";
        string yesFeedback = "Sure!";

        //Rate
        string titleRate = "Rate us!";
        string msgRate = "Would you like to rate our game and help us to make it even better?";
        string noRate = "No";
        string yesRate = "Sure!";

        Action askActionYes = delegate () {

            Action rateActionYes = delegate () {
                print("TODO Open Store");
                //TODO Open Store
            };
            YesNoDialog.inst.show(titleRate, yesRate, noRate, msgRate, null, rateActionYes, null);

        };
        Action askActionNo = delegate () {

            Action feedbackActionYes = delegate () {
                print("TODO Open E-Mail thing...");
                //TODO Open E-Mail thing...
            };
            YesNoDialog.inst.show(titleFeedback, yesFeedback, noFeedback, msgFeedback, null, feedbackActionYes, null);

        };

        YesNoDialog.inst.show(titleAsk, yesAsk, noAsk, msgAsk, askActionNo, askActionYes, null);
    }

    public void savePlanet() {
        
        BigDecimal production = 0;
        production += ((ElevatorGeneratorData)BottleDistributerController.inst.getGeneratorData()).realRate;
        production += ((ElevatorGeneratorData)MoneyCollectorController.inst.getGeneratorData()).realRate;
        foreach(BottleFillerController filler in bottleFillers) {
            production += ((BottleFillerGeneratorData)filler.getGeneratorData()).realRate;
        }
        if(bottleFillers.Count == 0) production = 0;
        
        //bottle distributer save data
        GeneratorSaveData bottleDistrData = new GeneratorSaveData();
        bottleDistrData.auto = BottleDistributerController.inst.isAuto();
        bottleDistrData.level = BottleDistributerController.inst.getLevel();

        List<PerkInstance> bottleDistrPerks = BottleDistributerController.inst.getPerks();
        PerkInstSaveData[] bottleDistrPerksData = new PerkInstSaveData[bottleDistrPerks.Count];
        for (int i = 0; i < bottleDistrPerksData.Length; i++) {
            PerkInstance perkInst = bottleDistrPerks[i];
            PerkInstSaveData perkData = new PerkInstSaveData();
            perkData.perkHash = perkInst.perk.getHash();
            perkData.activeTimeLeft = perkInst.activeTimeLeft;
            perkData.activeTime = perkInst.activeTime;
            perkData.sleepTimeLeft = perkInst.sleepTimeLeft;
            perkData.sleepTime = perkInst.sleepTime;
            bottleDistrPerksData[i] = perkData;
        }
        bottleDistrData.perks = bottleDistrPerksData;

        // BigDecimal bottleDistrValue = BottleDistributerController.inst.valueHolder.getValue();
        // bottleDistrData.valueHolderValueOne = new BigDecimalSaveData(bottleDistrValue.Mantissa.ToString(), bottleDistrValue.Exponent);

        //money collector save data
        GeneratorSaveData moneyCollData = new GeneratorSaveData();
        moneyCollData.auto = MoneyCollectorController.inst.isAuto();
        moneyCollData.level = MoneyCollectorController.inst.getLevel();

        List<PerkInstance> moneyCollPerks = MoneyCollectorController.inst.getPerks();
        PerkInstSaveData[] moneyCollPerksData = new PerkInstSaveData[moneyCollPerks.Count];
        for (int i = 0; i < moneyCollPerksData.Length; i++) {
            PerkInstance perkInst = moneyCollPerks[i];
            PerkInstSaveData perkData = new PerkInstSaveData();
            perkData.perkHash = perkInst.perk.getHash();
            perkData.activeTimeLeft = perkInst.activeTimeLeft;
            perkData.activeTime = perkInst.activeTime;
            perkData.sleepTimeLeft = perkInst.sleepTimeLeft;
            perkData.sleepTime = perkInst.sleepTime;
            moneyCollPerksData[i] = perkData;
        }
        moneyCollData.perks = moneyCollPerksData;

        // BigDecimal moneyCollValue = MoneyCollectorController.inst.valueHolder.getValue();
        // moneyCollData.valueHolderValueOne = new BigDecimalSaveData(moneyCollValue.Mantissa.ToString(), moneyCollValue.Exponent);

        //add current moneycollector value to database
        Database.inst.addPrimaryMoney(MoneyCollectorController.inst.valueHolder.getValue());

        //bottle fillers save data

        GeneratorSaveData[] fillStationsData = new GeneratorSaveData[bottleFillers.Count];
        for (int i = 0; i < fillStationsData.Length; i++) {
            GeneratorSaveData fillStation = new GeneratorSaveData();
            fillStation.auto = bottleFillers[i].isAuto();
            fillStation.level = bottleFillers[i].getLevel();

            List<PerkInstance> bottleFillerPerks = bottleFillers[i].getPerks();
            PerkInstSaveData[] bottleFillerPerksData = new PerkInstSaveData[bottleFillerPerks.Count];
            for (int j = 0; j < bottleFillerPerksData.Length; j++) {
                PerkInstance perkInst = bottleFillerPerks[j];
                PerkInstSaveData perkData = new PerkInstSaveData();
                perkData.perkHash = perkInst.perk.getHash();
                perkData.activeTimeLeft = perkInst.activeTimeLeft;
                perkData.activeTime = perkInst.activeTime;
                perkData.sleepTimeLeft = perkInst.sleepTimeLeft;
                perkData.sleepTime = perkInst.sleepTime;
                bottleFillerPerksData[j] = perkData;
            }
            fillStation.perks = bottleFillerPerksData;

            BigDecimal bottleFillerValueOne = bottleFillers[i].inputValueHolder.getValue();
            fillStation.valueHolderValueOne = new BigDecimalSaveData(bottleFillerValueOne.Mantissa.ToString(), bottleFillerValueOne.Exponent);

            BigDecimal bottleFillerValueTwo = bottleFillers[i].outputValueHolder.getValue();
            fillStation.valueHolderValueTwo = new BigDecimalSaveData(bottleFillerValueTwo.Mantissa.ToString(), bottleFillerValueTwo.Exponent);

            fillStationsData[i] = fillStation;
        }

        //usable perk instances
        List<PerkInstance> usablePerkInstances = PerkManager.inst.getUsablePerks();
        PerkInstSaveData[] usablePerksInstData = new PerkInstSaveData[usablePerkInstances.Count];
        for (int j = 0; j < usablePerksInstData.Length; j++) {
            PerkInstance perkInst = usablePerkInstances[j];
            PerkInstSaveData perkData = new PerkInstSaveData();
            perkData.perkHash = perkInst.perk.getHash();
            perkData.activeTimeLeft = perkInst.activeTimeLeft;
            perkData.activeTime = perkInst.activeTime;
            perkData.sleepTimeLeft = perkInst.sleepTimeLeft;
            perkData.sleepTime = perkInst.sleepTime;
            usablePerksInstData[j] = perkData;
        }

        //sleeping perk instances
        List<PerkInstance> sleepingPerkInstances = PerkManager.inst.getSleepingPerks();
        PerkInstSaveData[] sleepingPerksInstData = new PerkInstSaveData[sleepingPerkInstances.Count];
        for (int j = 0; j < sleepingPerksInstData.Length; j++) {
            PerkInstance perkInst = sleepingPerkInstances[j];
            PerkInstSaveData perkData = new PerkInstSaveData();
            perkData.perkHash = perkInst.perk.getHash();
            perkData.activeTimeLeft = perkInst.activeTimeLeft;
            perkData.activeTime = perkInst.activeTime;
            perkData.sleepTimeLeft = perkInst.sleepTimeLeft;
            perkData.sleepTime = perkInst.sleepTime;
            sleepingPerksInstData[j] = perkData;
        }

        //usable boost instances
        List<BoostInstance> usableBoostInstances = BoostManager.inst.getUsableBoosts();
        BoostInstSaveData[] usableBoostInstData = new BoostInstSaveData[usableBoostInstances.Count];
        for (int j = 0; j < usableBoostInstData.Length; j++) {
            BoostInstance boostInst = usableBoostInstances[j];
            BoostInstSaveData boostData = new BoostInstSaveData();
            boostData.boostHash = boostInst.boost.getHash();
            boostData.activeTimeLeft = boostInst.activeTimeLeft;
            boostData.activeTime = boostInst.activeTime;
            usableBoostInstData[j] = boostData;
        }

        //active boost instances
        List<BoostInstance> activeBoostInstances = BoostManager.inst.getActiveBoosts();
        BoostInstSaveData[] activeBoostInstData = new BoostInstSaveData[activeBoostInstances.Count];
        for (int j = 0; j < activeBoostInstData.Length; j++) {
            BoostInstance boostInst = activeBoostInstances[j];
            BoostInstSaveData boostData = new BoostInstSaveData();
            boostData.boostHash = boostInst.boost.getHash();
            boostData.activeTimeLeft = boostInst.activeTimeLeft;
            boostData.activeTime = boostInst.activeTime;
            activeBoostInstData[j] = boostData;
        }

        Database.inst.planets[Database.inst.currentPlanetIndex] = 
            new PlanetSaveData(bottleDistrData, moneyCollData, fillStationsData, usablePerksInstData,
                                sleepingPerksInstData, usableBoostInstData, activeBoostInstData, employeeCount, production);

    }

    public void loadPlanet(int index, bool firstStart = false) {
        if(index == Database.inst.currentPlanetIndex && !firstStart) return;
        Database.inst.currentPlanetIndex = index;
        
        resetPlanet();
        
        PlanetSaveData data = Database.inst.planets[index];
        
        if(data == null || data.production < 0) return;

        //bottle distributer save data

        BottleDistributerController.inst.setAuto(data.bottleDistributerData.auto);
        BottleDistributerController.inst.setLevel(data.bottleDistributerData.level);
        // BottleDistributerController.inst.valueHolder.setValue(data.bottleDistributerData.valueHolderValueOne.value());

        PerkInstance[] bdPerks = new PerkInstance[data.bottleDistributerData.perks.Length];
        for (int i = 0; i < data.bottleDistributerData.perks.Length; i++) {
            bdPerks[i] = data.bottleDistributerData.perks[i].createPerkInstance();
        }
        BottleDistributerController.inst.addPerks(bdPerks);
        PerkManager.inst.loadActivePerks(bdPerks);

        //money collector save data
        MoneyCollectorController.inst.setAuto(data.moneyCollectorData.auto);
        MoneyCollectorController.inst.setLevel(data.moneyCollectorData.level);
        // MoneyCollectorController.inst.valueHolder.setValue(data.moneyCollectorData.valueHolderValueOne.value());

        PerkInstance[] mcPerks = new PerkInstance[data.moneyCollectorData.perks.Length];
        for (int i = 0; i < data.moneyCollectorData.perks.Length; i++) {
            mcPerks[i] = data.moneyCollectorData.perks[i].createPerkInstance();
        }
        MoneyCollectorController.inst.addPerks(mcPerks);
        PerkManager.inst.loadActivePerks(mcPerks);

        //bottle fillers save data

        foreach (GeneratorSaveData fillStationData in data.fillStationDatas) {
            BottleFillerController fillStation = newProducer();

            fillStation.setAuto(fillStationData.auto);
            fillStation.setLevel(fillStationData.level);
            fillStation.inputValueHolder.setValue(fillStationData.valueHolderValueOne.value());
            fillStation.outputValueHolder.setValue(fillStationData.valueHolderValueTwo.value());

            PerkInstance[] bfPerks = new PerkInstance[fillStationData.perks.Length];
            for (int i = 0; i < fillStationData.perks.Length; i++) {
                bfPerks[i] = fillStationData.perks[i].createPerkInstance();
            }
            fillStation.addPerks(bfPerks);
            PerkManager.inst.loadActivePerks(bfPerks);

        }

        //usable perk instances
        PerkInstance[] usablePerks = new PerkInstance[data.usablePerkInstances.Length];
        for (int i = 0; i < data.usablePerkInstances.Length; i++) {
            usablePerks[i] = data.usablePerkInstances[i].createPerkInstance();
        }
        PerkManager.inst.loadUsablePerks(usablePerks);

        //sleeping perk instances
        PerkInstance[] sleepingPerks = new PerkInstance[data.sleepingPerkInstances.Length];
        for (int i = 0; i < data.sleepingPerkInstances.Length; i++) {
            sleepingPerks[i] = data.sleepingPerkInstances[i].createPerkInstance();
        }
        PerkManager.inst.loadSleepingPerks(sleepingPerks);

        //usable boost instances
        BoostInstance[] usableBoosts = new BoostInstance[data.usableBoostInstances.Length];
        for (int i = 0; i < data.usableBoostInstances.Length; i++) {
            usableBoosts[i] = data.usableBoostInstances[i].createBoostInstance();
        }
        BoostManager.inst.loadUsableBoosts(usableBoosts);

        //active boost instances
        BoostInstance[] activeBoosts = new BoostInstance[data.activeBoostInstances.Length];
        for (int i = 0; i < data.activeBoostInstances.Length; i++) {
            activeBoosts[i] = data.activeBoostInstances[i].createBoostInstance();
        }
        BoostManager.inst.loadActiveBoosts(activeBoosts);
        BoostRepresentationController.inst.refresh();
        
        employeeCount = data.employeeCount;
        
    }
    
}

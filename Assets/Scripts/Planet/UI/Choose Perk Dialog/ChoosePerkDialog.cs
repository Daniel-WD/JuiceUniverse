using System.Collections;
using System.Collections.Generic;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoosePerkDialog : MonoBehaviour, MoneyListener, PerkManagerListener {

    public static ChoosePerkDialog inst;

    public PerkItemController perkItemInst;
    public ActivePerkItemController activePerkItemInst;

    public HorizontalLayoutGroup activePerkList;
    public GridLayoutGroup normalPerkList;

    private GeneratorBehaviour mGenerator;

    private List<PerkItemController> mPerkItems = new List<PerkItemController>();
    private List<ActivePerkItemController> mActivePerkItems = new List<ActivePerkItemController>();

    private bool mPerkTimeUpdaterActive = false;

    void Awake() {
        inst = this;
    }

    void Start() {
        hide();
        Database.inst.moneyListeners.Add(this);
        PerkManager.inst.addListener(this);
    }

    private IEnumerator perkTimeUpdater() {
        mPerkTimeUpdaterActive = true;
        while (mPerkTimeUpdaterActive) {
            foreach (PerkItemController contr in mPerkItems) contr.updateTimes();
            foreach (ActivePerkItemController contr in mActivePerkItems) contr.updateTimer();
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void show(GeneratorBehaviour generator) {
        if (PlanetUiController.inst.uiActiveLvl0()) return;
        gameObject.SetActive(true);
        PlanetUiController.inst.enableUiModeLvl0();

        mGenerator = generator;

        refreshLists();
        StartCoroutine(perkTimeUpdater());

    }

    public void hide() {
        gameObject.SetActive(false);
        PlanetUiController.inst.disableUiModeLvl0();

        mGenerator = null;
        mPerkTimeUpdaterActive = false;
    }

    public void refreshLists() {

        //----> normal list
        mPerkItems.Clear();
        PlanetController.inst.destroyChildren(normalPerkList.transform);
        //usable perks
        foreach (PerkInstance perkInst in PerkManager.inst.getUsablePerks()) {
            if (perkInst.perk.generatorType != mGenerator.type) continue;
            PerkItemController perkItem = Instantiate(perkItemInst, normalPerkList.transform);
            perkItem.set(PerkItemState.NORMAL, perkInst);
            mPerkItems.Add(perkItem);
        }
        //sleeping perks
        foreach (PerkInstance perkInst in PerkManager.inst.getSleepingPerks()) {
            if (perkInst.perk.generatorType != mGenerator.type) continue;
            PerkItemController perkItem = Instantiate(perkItemInst, normalPerkList.transform);
            perkItem.set(PerkItemState.COUNTDOWN, perkInst);
            mPerkItems.Add(perkItem);
        }
        //plus
        PerkItemController plusItem = Instantiate(perkItemInst, normalPerkList.transform);
        plusItem.set(PerkItemState.PLUS, null);
        mPerkItems.Add(plusItem);

        int emptyFields = 0;

        if (mPerkItems.Count < 8) emptyFields += 8 - mPerkItems.Count;
        if ((mPerkItems.Count + emptyFields) % 2 != 0) emptyFields++;

        for (int i = 0; i < emptyFields; i++) {
            PerkItemController emptyItem = Instantiate(perkItemInst, normalPerkList.transform);
            emptyItem.set(PerkItemState.EMPTY, null);
            mPerkItems.Add(emptyItem);
        }

        //----> active list
        mActivePerkItems.Clear();
        PlanetController.inst.destroyChildren(activePerkList.transform);
        foreach (PerkInstance perk in mGenerator.getPerks()) {
            ActivePerkItemController activePerkItem = Instantiate(activePerkItemInst, activePerkList.transform);
            activePerkItem.setPerk(perk);
            mActivePerkItems.Add(activePerkItem);
        }


    }

    public void requestActivatePerk(PerkInstance perk) {
        if (mActivePerkItems.Count < 2) {
            PerkManager.inst.activatePerk(perk, mGenerator);
        }
    }
    
    public GeneratorBehaviour getGenerator() {
        return mGenerator;
    }

    // LayoutRebuilder.ForceRebuildLayoutImmediate(contCost);

    public void onButtonCloseClick() {
        hide();
    }

    public void onPrimaryMoneyChanged(BigDecimal newMoney) {

    }

    public void onSecondaryMoneyChanged(BigDecimal newMoney) {
    }

    public void onPerkUsable(PerkInstance perkInst) {
        if (mGenerator && mGenerator.type == perkInst.perk.generatorType) refreshLists();
    }

    public void onPerkActivated(PerkInstance perkInst) {
        if (mGenerator && mGenerator.type == perkInst.perk.generatorType) refreshLists();
    }

    public void onPerkSleep(PerkInstance perkInst) {
        if (mGenerator && mGenerator.type == perkInst.perk.generatorType) refreshLists();
    }
}

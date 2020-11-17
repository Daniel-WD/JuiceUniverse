using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseBoostDialog : MonoBehaviour, BoostManagerListener {

    public static ChooseBoostDialog inst;

    public BoostItemController boostItemInst;

    public GridLayoutGroup itemContainer;
    
    public RectTransform shopBtnContainer;

    private List<BoostItemController> mActiveBoostItems = new List<BoostItemController>();

    private bool mBoostTimeUpdaterActive = false;

    void Awake() {
        inst = this;
    }

    void Start() {
        hide();
        BoostManager.inst.addListener(this);
    }

    private IEnumerator boostTimeUpdater() {
        mBoostTimeUpdaterActive = true;
        while (mBoostTimeUpdaterActive) {
            foreach (BoostItemController contr in mActiveBoostItems) contr.updateTimer();
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void show() {
        if (PlanetUiController.inst.uiActiveLvl0()) return;
        LayoutRebuilder.ForceRebuildLayoutImmediate(shopBtnContainer);
        gameObject.SetActive(true);
        PlanetUiController.inst.enableUiModeLvl0();
        StartCoroutine(boostTimeUpdater());

        refreshList();
    }

    public void hide() {
        gameObject.SetActive(false);
        PlanetUiController.inst.disableUiModeLvl0();

        mBoostTimeUpdaterActive = false;
    }

    public void refreshList() {
        PlanetController.inst.destroyChildren(itemContainer.transform);

        int addedItems = 0;

        //active boost
        mActiveBoostItems.Clear();
        foreach (BoostInstance boost in BoostManager.inst.getActiveBoosts()) {
            BoostItemController boostItem = Instantiate(boostItemInst, itemContainer.transform);
            boostItem.set(boost, BoostItemState.ACTIVE);
            mActiveBoostItems.Add(boostItem);
            addedItems++;
        }
        //usable boost
        foreach (BoostInstance boost in BoostManager.inst.getUsableBoosts()) {
            BoostItemController boostItem = Instantiate(boostItemInst, itemContainer.transform);
            boostItem.set(boost, BoostItemState.NORMAL);
            addedItems++;
        }
        int emptyFields = 0;

        if (addedItems < 8) emptyFields += 8 - addedItems;
        if ((addedItems + emptyFields) % 2 != 0) emptyFields++;

        for (int i = 0; i < emptyFields; i++) {
            BoostItemController emptyItem = Instantiate(boostItemInst, itemContainer.transform);
            emptyItem.set(null, BoostItemState.EMPTY);
        }

    }

    public void onButtonCloseClick() {
        hide();
    }
    
    public void onBtnVideoClicked() {
        
    }
    
    public void onBtnShopClicked() {
        
    }

    public void onBoostUsable(BoostInstance boost) {
        refreshList();
    }

    public void onBoostActivated(BoostInstance boost) {
        refreshList();
    }

    public void onBoostDied(BoostInstance boost) {
        refreshList();
    }
}

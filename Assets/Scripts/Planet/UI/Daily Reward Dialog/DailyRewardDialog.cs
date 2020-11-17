using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardDialog : MonoBehaviour {

    public static DailyRewardDialog inst;

    public GameObject itemInst;

    public GridLayoutGroup layout;

    void Awake() {
        inst = this;
    }

    void Start() {
        hide();
    }

    public void show() {
        if (PlanetUiController.inst.uiActiveLvl0()) return;
        gameObject.SetActive(true);
        PlanetUiController.inst.enableUiModeLvl0();
        
        refresh();
    }
    
    private void refresh() {
        PlanetController.inst.destroyChildren(layout.transform);
        
        GameItem[] items = Database.inst.dailyRewards;
        for(int i = 0; i < items.Length; i++) {
            DailyRewardItemController newItem = ObjectPooler.inst.giveMe(itemInst, layout.transform).GetComponent<DailyRewardItemController>();
            newItem.set(items[i], i+1, Database.inst.dailyRewardUsed[i]);
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)layout.transform);
    }

    public void hide() {
        gameObject.SetActive(false);
        PlanetUiController.inst.disableUiModeLvl0();
    }
    
    public void onButtonCloseClick() {
        hide();
    }

}
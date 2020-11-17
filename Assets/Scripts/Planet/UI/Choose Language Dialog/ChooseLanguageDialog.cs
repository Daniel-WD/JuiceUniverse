using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseLanguageDialog : MonoBehaviour {

    public static ChooseLanguageDialog inst;

    public RectTransform itemContainer;
    
    public GameObject topItemInst, middleItemInst, bottomItemInst;
    
    private string[] mFakeLanguages = new string[] {
        "deutsch", "russch", "englsch", "sakjdf", "sakjdfasdf", "sakjdfasdf", "sakjdasdff",
        "deutsch", "russch", "englsch", "sakjdf", "sakjdfasdf", "sakjdfasdf", "sakjdasdff",
        "deutsch", "russch", "englsch", "sakjdf", "sakjdfasdf", "sakjdfasdf", "sakjdasdff",
        "deutsch", "russch", "englsch", "sakjdf", "sakjdfasdf", "sakjdfasdf", "sakjdasdff",
        "deutsch", "russch", "englsch", "sakjdf", "sakjdfasdf", "sakjdfasdf", "sakjdasdff"
    };

    void Awake() {
        inst = this;
    }

    void Start() {
        hide();
    }

    public void show() {
        if (PlanetUiController.inst.uiActiveLvl1()) return;
        gameObject.SetActive(true);
        PlanetUiController.inst.enableUiModeLvl1();
        
        refreshList();
    }
    
    private void refreshList() {
        PlanetController.inst.destroyChildren(itemContainer);

        for (int i = 0; i < mFakeLanguages.Length; i++) {
            LanguageItemController newItem;
            if(i == 0) {
                newItem = ObjectPooler.inst.giveMe(topItemInst, itemContainer).GetComponent<LanguageItemController>();
            } else if(i == mFakeLanguages.Length-1) {
                newItem = ObjectPooler.inst.giveMe(bottomItemInst, itemContainer).GetComponent<LanguageItemController>();
            } else {
                newItem = ObjectPooler.inst.giveMe(middleItemInst, itemContainer).GetComponent<LanguageItemController>();
            }
            newItem.set(mFakeLanguages[i], i % 2 != 0);
        }
    }

    public void hide() {
        gameObject.SetActive(false);
        PlanetUiController.inst.disableUiModeLvl1();

    }

    public void onButtonCloseClick() {
        hide();
    }
    
}
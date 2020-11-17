using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemRevealDialog : MonoBehaviour {

    public static ItemRevealDialog inst;

    public Image imgContent, imgIcon;
    public TextMeshProUGUI txtDescription;
    public RectTransform descriptionContainer;

    private GameItem mGameItem;

    void Awake() {
        inst = this;
    }

    void Start() {
        hide();
    }

    public void show(GameItem item) {
        if (PlanetUiController.inst.uiActiveLvl1()) return;
        gameObject.SetActive(true);
        PlanetUiController.inst.enableUiModeLvl1();

        mGameItem = item;

        imgContent.sprite = item.gfx;
        imgContent.SetNativeSize();
        txtDescription.text = item.info();
        if (item.icon) {
            imgIcon.gameObject.SetActive(true);
            imgIcon.sprite = item.icon;
        } else {
            imgIcon.gameObject.SetActive(false);
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(descriptionContainer);
    }

    public void hide() {
        gameObject.SetActive(false);
        PlanetUiController.inst.disableUiModeLvl1();
        mGameItem = null;
    }

    public void onBtnTakeClicked() {
        mGameItem.makeAvailable();
        hide();
    }

}

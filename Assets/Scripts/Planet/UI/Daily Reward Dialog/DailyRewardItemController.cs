using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardItemController : MonoBehaviour {

    public Button btnButton;
    public Image imgDescriptionBg, imgContent, imgIcon, imgCheck;
    public TextMeshProUGUI txtDay, txtInfo;
    public RectTransform descriptionContainer;

    private GameItem mGameItem;

    public void set(GameItem item, int number, bool used) {
        mGameItem = item;
        if (used) {
            btnButton.interactable = false;
            imgCheck.gameObject.SetActive(true);
            imgDescriptionBg.gameObject.SetActive(false);
            imgContent.gameObject.SetActive(false);
            descriptionContainer.gameObject.SetActive(false);
        } else {
            btnButton.interactable = true;
            imgCheck.gameObject.SetActive(false);
            imgDescriptionBg.gameObject.SetActive(true);
            imgContent.gameObject.SetActive(true);
            descriptionContainer.gameObject.SetActive(true);

            imgContent.sprite = item.gfx;

            if (item.icon) {
                imgIcon.gameObject.SetActive(true);
                imgIcon.sprite = item.icon;
            } else {
                imgIcon.gameObject.SetActive(false);
            }

            txtInfo.text = item.info();

            imgContent.SetNativeSize();
            imgIcon.SetNativeSize();
            LayoutRebuilder.ForceRebuildLayoutImmediate(descriptionContainer);
        }

        txtDay.text = "Day " + number;
    }

    public void onClick() {
        DailyRewardDialog.inst.hide();
        ItemRevealDialog.inst.show(mGameItem);
    }

}

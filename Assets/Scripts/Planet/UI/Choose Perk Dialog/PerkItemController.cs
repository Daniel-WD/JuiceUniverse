using System.Collections;
using System.Collections.Generic;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PerkItemState {
    EMPTY, PLUS, NORMAL, COUNTDOWN
}

public class PerkItemController : MonoBehaviour {

    public Sprite[] sprGlowInsts;

    public Image imgNotInteractable, imgPerk, imgPerkGlow;
    public Button btnPlus, btnNormal;
    public TextMeshProUGUI txtAction, txtTimer, txtCost;
    public RectTransform costContainer;

    private PerkItemState mState = PerkItemState.EMPTY;
    private PerkInstance mPerkInst;

    private readonly BigDecimal FAKE_COST = 1000;

    private void refreshState() {
        switch (mState) {
            case PerkItemState.EMPTY:
                imgNotInteractable.gameObject.SetActive(true);

                imgPerk.gameObject.SetActive(false);
                imgPerkGlow.gameObject.SetActive(false);
                btnPlus.gameObject.SetActive(false);
                btnNormal.gameObject.SetActive(false);
                txtAction.gameObject.SetActive(false);
                txtTimer.gameObject.SetActive(false);
                costContainer.gameObject.SetActive(false);
                break;

            case PerkItemState.PLUS:
                btnPlus.gameObject.SetActive(true);
                costContainer.gameObject.SetActive(true);

                //todo
                txtCost.text = Formatter.formatNumber(FAKE_COST);
                LayoutRebuilder.ForceRebuildLayoutImmediate(costContainer);

                imgNotInteractable.gameObject.SetActive(false);
                imgPerk.gameObject.SetActive(false);
                imgPerkGlow.gameObject.SetActive(false);
                btnNormal.gameObject.SetActive(false);
                txtAction.gameObject.SetActive(false);
                txtTimer.gameObject.SetActive(false);
                break;

            case PerkItemState.NORMAL:
                imgPerk.gameObject.SetActive(true);
                imgPerkGlow.gameObject.SetActive(true);
                btnNormal.gameObject.SetActive(true);
                txtAction.gameObject.SetActive(true);

                imgPerk.sprite = mPerkInst.perk.gfx;
                imgPerkGlow.sprite = sprGlowInsts[(int)mPerkInst.perk.strenght];
                txtAction.text = "+" + (Perk.STRENGTHS[(int)mPerkInst.perk.strenght] * 100) + "% " + Formatter.formatTime(mPerkInst.activeTime, true);

                btnPlus.gameObject.SetActive(false);
                costContainer.gameObject.SetActive(false);
                imgNotInteractable.gameObject.SetActive(false);
                txtTimer.gameObject.SetActive(false);
                break;

            case PerkItemState.COUNTDOWN:
                imgPerk.gameObject.SetActive(true);
                imgNotInteractable.gameObject.SetActive(true);
                txtTimer.gameObject.SetActive(true);

                imgPerk.sprite = mPerkInst.perk.gfx;
                txtTimer.text = Formatter.formatTime(mPerkInst.sleepTimeLeft, true);

                imgPerkGlow.gameObject.SetActive(false);
                btnNormal.gameObject.SetActive(false);
                txtAction.gameObject.SetActive(false);
                btnPlus.gameObject.SetActive(false);
                costContainer.gameObject.SetActive(false);
                break;
        }
        imgPerkGlow.SetNativeSize();
        updateCanPay();
    }
    
    public void set(PerkItemState state, PerkInstance perk) {
        mState = state;
        mPerkInst = perk;
        refreshState();
    }

    public void updateTimes() {
        switch (mState) {
            case PerkItemState.NORMAL:
                txtAction.text = "+" + (Perk.STRENGTHS[(int)mPerkInst.perk.strenght] * 100) + "% " + Formatter.formatTime(mPerkInst.activeTime, true);
                break;

            case PerkItemState.COUNTDOWN:
                txtTimer.text = Formatter.formatTime(mPerkInst.sleepTimeLeft, true);
                break;
        }
    }
    
    public void updateCanPay() {
        if(Database.inst.getPrimaryMoney() < FAKE_COST) {
            txtCost.color = Database.inst.noMoneyRedColor;
            btnPlus.interactable = false;
        } else {
            txtCost.color = Color.white;
            btnPlus.interactable = true;
        }
    }

    public void onBtnNormalClicked() {
        UsePerkDialog.inst.show(mPerkInst);
    }

    public void onBtnPlusClicked() {
        Database.inst.removePrimaryMoney(FAKE_COST);
        PerkManager.inst.generateRandomPerk(ChoosePerkDialog.inst.getGenerator().type);
        refreshState();
    }

}
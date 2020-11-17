using System.Collections;
using System.Collections.Generic;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class NewProducerController : MonoBehaviour, MoneyListener {

    private static bool sInCloudProcess = false;
    
    private static IEnumerator resetInCloudProcess(float delay) {
        yield return new WaitForSeconds(delay);
        sInCloudProcess = false;
    }

    public TextMeshProUGUI costText, actionText;
    public RectTransform costContainer;
    public bool primary;
    public Button button;
    public PlayableDirector cloudAnim;

    private BigDecimal mCost;
    private Vector3 mCloudsOrigin;

    void Start() {
        notifyProducerCountChanged();
        checkEnabled();
        
        mCloudsOrigin = cloudAnim.transform.position;

        Database.inst.moneyListeners.Add(this);
    }

    private void checkEnabled() {
        if (primary) {
            if (mCost <= Database.inst.getPrimaryMoney()) {
                costText.color = Color.white;
                actionText.color = Color.white;
                button.interactable = true;
            } else {
                actionText.color = Database.inst.btnDisabledWhiteTextColor;
                costText.color = Database.inst.noMoneyRedColor;
                button.interactable = false;
            }
        }
    }

    public void onBtnNewProducerClicked() {
        if(sInCloudProcess) return;
        sInCloudProcess = true;
        StartCoroutine(resetInCloudProcess(1));
        
        //reposition clouds
        Vector3 pos = mCloudsOrigin + PlanetController.inst.currentEtagesHeight();
        cloudAnim.transform.position = pos;
        
        Database.inst.removePrimaryMoney(mCost);
        cloudAnim.Play();
        PlanetController.inst.Invoke("newProducer", 25f/60f);
    }

    public void notifyProducerCountChanged() {
        mCost = GeneratorValues.calcProducerCost(PlanetController.inst.bottleFillers.Count);
        checkEnabled();
        updateText();
    }

    private void updateText() {
        costText.text = Formatter.formatNumber(mCost);
        LayoutRebuilder.ForceRebuildLayoutImmediate(costContainer);
    }

    public void onPrimaryMoneyChanged(BigDecimal newMoney) {
        checkEnabled();
    }

    public void onSecondaryMoneyChanged(BigDecimal newMoney) {
        throw new System.NotImplementedException();
    }
}
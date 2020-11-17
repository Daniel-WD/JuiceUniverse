using System.Collections;
using System.Collections.Generic;
using Common;
using TMPro;
using UnityEngine;

public class GeneratorUpgrader : MonoBehaviour, MoneyListener {

    public SpriteRenderer[] pluses;

    public TextMeshProUGUI tLevel;
    public GeneratorType type;
    public Animator blowAnimator, glowAnimator;
    public float highlightDelay = 0;

    private GeneratorBehaviour mGenerator;
    private bool wasUpgradable = false;

    void Start() {
        Database.inst.moneyListeners.Add(this);
        updateLevelText();
    }
    
    void OnDestroy() {
        Database.inst.moneyListeners.Remove(this);
    }

    public void onBtnLevelClick() {
        UpgradeDialog.inst.show(mGenerator);
    }

    public void updateLevelText() {
        if(tLevel && mGenerator) tLevel.text = "Level\n" + mGenerator.getLevel();
    }

    // private void checkPluses() {
    //     int max = GeneratorValues.plusesPossbileUpgrades[GeneratorValues.plusesPossbileUpgrades.Length-1];
    //     int plusCount = GeneratorValues.plusCountForPossibleUpgrades(mGenerator.possibleUpgradeCount(max));
    //     for (int i = 0; i < pluses.Length; i++) {
    //         pluses[i].enabled = i < plusCount;
    //     }
    // }

    private void checkHighlight() {
        bool canUpgrade = mGenerator.canUpgrade();
        glowAnimator.SetBool("highlighting", canUpgrade);
        if (canUpgrade && !wasUpgradable) {
            blowAnimator.SetTrigger("highlight");
        }
        wasUpgradable = canUpgrade;
    }

    public GeneratorBehaviour getGeneratorBehaviour() {
        return mGenerator;
    }

    public void setGeneratorBehaviour(GeneratorBehaviour behaviour) {
        mGenerator = behaviour;
        updateLevelText();
        Invoke("checkHighlight", highlightDelay);
        // checkPluses();
    }

    public void onPrimaryMoneyChanged(BigDecimal newMoney) {
        // checkPluses();
        Invoke("checkHighlight", highlightDelay);
    }

    public void onSecondaryMoneyChanged(BigDecimal newMoney) {
    }
}

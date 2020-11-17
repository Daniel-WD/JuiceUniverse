using System.Collections;
using System.Collections.Generic;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeDialog : MonoBehaviour, MoneyListener, PerkListener {

    public static UpgradeDialog inst;

    public TextMeshProUGUI levelText, costText, upgradeText;
    public TextMeshProUGUI txtBottleFillerCapacity, txtBottleFillerRate, txtBottleFillerSpeed;
    public TextMeshProUGUI txtBottleDistributerCapacity, txtBottleDistributerMoveSpeed, txtBottleDistributerFillSpeed;
    public TextMeshProUGUI txtMoneyCollectorCapacity, txtMoneyCollectorMoveSpeed, txtMoneyCollectorFillSpeed;

    public DataContainer bottleFillerLayout, moneyCollectorLayout, bottleDistributerLayout;
    public Button btnUpgrade, btnPreviousMultiplier, btnNextMultiplier;
    public Image imgBtnUpgrade, imgBtnPreviousMultiplier, imgBtnNextMultiplier;
    public RectTransform costContainer;

    public TextMeshProUGUI nextBoostLvlText, boostRewardText;
    public Image nextRewardCoin, imgUpgradeBtnCoin;
    public RectTransform boostRewardContainer;

    public MySlider mainSlider, previewSlider;

    public Sprite imgPerkGlowInstOne, imgPerkGlowInstTwo, imgPerkGlowInstThree, imgPerkGlowInstFour;
    public Sprite imgPerkSlotInstOne, imgPerkSlotInstTwo, imgPerkSlotInstThree, imgPerkSlotInstFour;
    public Sprite imgTimeGlowInstOne1, imgTimeGlowInstOne2, imgTimeGlowInstTwo1, imgTimeGlowInstTwo2,
                    imgTimeGlowInstThree1, imgTimeGlowInstThree2, imgTimeGlowInstFour1, imgTimeGlowInstFour2;
                    
    private Sprite[] mImgTimeGlowInsts1, mImgTimeGlowInsts2, mImgPerkSlotInsts, mImgPerkGlowInsts;

    public Image imgPerkSlotOne, imgPerkSlotTwo, imgPerkSlotThree;
    public Image imgPerkOne, imgPerkTwo, imgPerkThree;
    public Image imgPerkGlowOne, imgPerkGlowTwo, imgPerkGlowThree;
    public Image imgTimeGlowOne, imgTimeGlowTwo, imgTimeGlowThree;
    public TextMeshProUGUI txtPerkCountdownOne, txtPerkCountdownTwo, txtPerkCountdownThree;

    private GeneratorBehaviour mCurrentGenerator;

    private BigDecimal[] mCosts;
    private int mCostDex = 0;

    private bool mPerkTimeUpdaterActive = false;

    void Awake() {
        inst = this;
        mCosts = new BigDecimal[GeneratorValues.upgradeStages.Length];
        mImgTimeGlowInsts1 = new Sprite[] {imgTimeGlowInstOne1, imgTimeGlowInstTwo1, imgTimeGlowInstThree1, imgTimeGlowInstFour1};
        mImgTimeGlowInsts2 = new Sprite[] {imgTimeGlowInstOne2, imgTimeGlowInstTwo2, imgTimeGlowInstThree2, imgTimeGlowInstFour2};
        mImgPerkGlowInsts = new Sprite[] {imgPerkGlowInstOne, imgPerkGlowInstTwo, imgPerkGlowInstThree, imgPerkGlowInstFour};
        mImgPerkSlotInsts = new Sprite[] {imgPerkSlotInstOne, imgPerkSlotInstTwo, imgPerkSlotInstThree, imgPerkSlotInstFour};
    }

    void Start() {
        Database.inst.moneyListeners.Add(this);
        hide();
    }

    private IEnumerator perkTimeUpdater() {
        mPerkTimeUpdaterActive = true;
        while (mPerkTimeUpdaterActive) {
            List<PerkInstance> perks = mCurrentGenerator.getPerks();
            bool smtUpdated = false;
            foreach (PerkInstance perkInst in perks) {
                if (perkInst.perk.generatorType != mCurrentGenerator.type) continue;
                switch (perkInst.perk.property) {
                    case PerkProperty.FILL_STATION_CAPACITY:
                    case PerkProperty.ELEVATOR_CAPACITY:
                        txtPerkCountdownOne.text = Formatter.formatTime(perkInst.activeTimeLeft, true);
                        imgTimeGlowOne.sprite = findTimeGlow(perkInst.perk, txtPerkCountdownOne);
                        imgTimeGlowOne.SetNativeSize();
                        smtUpdated = true;
                        break;

                    case PerkProperty.FILL_STATION_FILL_RATE:
                    case PerkProperty.ELEVATOR_MOVE_SPEED:
                        txtPerkCountdownTwo.text = Formatter.formatTime(perkInst.activeTimeLeft, true);
                        imgTimeGlowTwo.sprite = findTimeGlow(perkInst.perk, txtPerkCountdownTwo);
                        imgTimeGlowTwo.SetNativeSize();
                        smtUpdated = true;
                        break;

                    case PerkProperty.FILL_STATION_FILL_SPEED:
                    case PerkProperty.ELEVATOR_FILL_SPEED:
                        txtPerkCountdownThree.text = Formatter.formatTime(perkInst.activeTimeLeft, true);
                        imgTimeGlowThree.sprite = findTimeGlow(perkInst.perk, txtPerkCountdownThree);
                        imgTimeGlowThree.SetNativeSize();
                        smtUpdated = true;
                        break;

                }
            }
            if(!smtUpdated) break;
            smtUpdated = false;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void show(GeneratorBehaviour generator) {
        if (PlanetUiController.inst.uiActiveLvl0()) return;

        generator.addPerkListener(this);

        mCurrentGenerator = generator;
        PlanetUiController.inst.enableUiModeLvl0();
        gameObject.SetActive(true);
        hideAllDatas();

        switch (mCurrentGenerator.type) {
            case GeneratorType.BOTTLE_FILLER:
                bottleFillerLayout.gameObject.SetActive(true);
                bottleFillerLayout.generatorBehaviour = generator;
                break;

            case GeneratorType.MONEY_COLLECTOR:
                moneyCollectorLayout.gameObject.SetActive(true);
                moneyCollectorLayout.generatorBehaviour = generator;
                break;

            case GeneratorType.BOTTLE_DISTRIBUTER:
                bottleDistributerLayout.gameObject.SetActive(true);
                bottleDistributerLayout.generatorBehaviour = generator;
                break;
        }

        checkPrevNextBtnsEnabled();
        calcCost();
        updateData();
        updatePerks();
        StartCoroutine(perkTimeUpdater());

    }

    private void updatePerks() {
        //reset
        imgPerkSlotOne.gameObject.SetActive(false);
        imgPerkSlotTwo.gameObject.SetActive(false);
        imgPerkSlotThree.gameObject.SetActive(false);

        txtBottleFillerCapacity.text = "Bottle Capacity";
        txtBottleFillerRate.text = "Fill Rate";
        txtBottleFillerSpeed.text = "Fill Speed";
        txtBottleDistributerCapacity.text = "Bottle Capacity";
        txtMoneyCollectorCapacity.text = "Money Capacity";
        txtBottleDistributerMoveSpeed.text = "Move Speed";
        txtMoneyCollectorMoveSpeed.text = "Move Speed";
        txtBottleDistributerFillSpeed.text = "Fill Speed";
        txtMoneyCollectorFillSpeed.text = "Fill Speed";

        //set
        List<PerkInstance> perks = mCurrentGenerator.getPerks();
        foreach (PerkInstance perkInst in perks) {
            if (perkInst.perk.generatorType != mCurrentGenerator.type) continue;
            string percantageText = "\n+" + (Perk.STRENGTHS[(int)perkInst.perk.strenght] * 100) + "%";
            switch (perkInst.perk.property) {
                case PerkProperty.FILL_STATION_CAPACITY:
                case PerkProperty.ELEVATOR_CAPACITY:
                    if (perkInst.perk.generatorType == GeneratorType.BOTTLE_DISTRIBUTER) {
                        txtBottleDistributerCapacity.text = "Bottle Capacity" + percantageText;
                    } else if (perkInst.perk.generatorType == GeneratorType.MONEY_COLLECTOR) {
                        txtMoneyCollectorCapacity.text = "Money Capacity" + percantageText;
                    } else {
                        txtBottleFillerCapacity.text = "Bottle Capacity" + percantageText;
                    }
                    imgPerkSlotOne.gameObject.SetActive(true);
                    imgPerkSlotOne.sprite = mImgPerkSlotInsts[(int)perkInst.perk.strenght];
                    imgPerkOne.sprite = perkInst.perk.gfx;
                    imgPerkGlowOne.sprite = mImgPerkGlowInsts[(int)perkInst.perk.strenght];
                    txtPerkCountdownOne.text = Formatter.formatTime(perkInst.activeTimeLeft, true);
                    imgTimeGlowOne.sprite = findTimeGlow(perkInst.perk, txtPerkCountdownOne);
                    imgTimeGlowOne.SetNativeSize();
                    break;

                case PerkProperty.FILL_STATION_FILL_RATE:
                case PerkProperty.ELEVATOR_MOVE_SPEED:
                    if (perkInst.perk.generatorType == GeneratorType.BOTTLE_DISTRIBUTER) {
                        txtBottleDistributerMoveSpeed.text = "Move Speed" + percantageText;
                    } else if (perkInst.perk.generatorType == GeneratorType.MONEY_COLLECTOR) {
                        txtMoneyCollectorMoveSpeed.text = "Move Speed" + percantageText;
                    } else {
                        txtBottleFillerRate.text = "Fill Rate" + percantageText;
                    }
                    imgPerkSlotTwo.gameObject.SetActive(true);
                    imgPerkSlotTwo.sprite = mImgPerkSlotInsts[(int)perkInst.perk.strenght];
                    imgPerkTwo.sprite = perkInst.perk.gfx;
                    imgPerkGlowTwo.sprite = mImgPerkGlowInsts[(int)perkInst.perk.strenght];
                    txtPerkCountdownTwo.text = Formatter.formatTime(perkInst.activeTimeLeft, true);
                    imgTimeGlowTwo.sprite = findTimeGlow(perkInst.perk, txtPerkCountdownTwo);
                    imgTimeGlowTwo.SetNativeSize();
                    break;

                case PerkProperty.FILL_STATION_FILL_SPEED:
                case PerkProperty.ELEVATOR_FILL_SPEED:
                    if (perkInst.perk.generatorType == GeneratorType.BOTTLE_DISTRIBUTER) {
                        txtBottleDistributerFillSpeed.text = "Fill Speed" + percantageText;
                    } else if (perkInst.perk.generatorType == GeneratorType.MONEY_COLLECTOR) {
                        txtMoneyCollectorFillSpeed.text = "Fill Speed" + percantageText;
                    } else {
                        txtBottleFillerSpeed.text = "Fill Speed" + percantageText;
                    }
                    imgPerkSlotThree.gameObject.SetActive(true);
                    imgPerkSlotThree.sprite = mImgPerkSlotInsts[(int)perkInst.perk.strenght];
                    imgPerkThree.sprite = perkInst.perk.gfx;
                    imgPerkGlowThree.sprite = mImgPerkGlowInsts[(int)perkInst.perk.strenght];
                    txtPerkCountdownThree.text = Formatter.formatTime(perkInst.activeTimeLeft, true);
                    imgTimeGlowThree.sprite = findTimeGlow(perkInst.perk, txtPerkCountdownThree);
                    imgTimeGlowThree.SetNativeSize();
                    break;
            }
        }
    }

    private Sprite findTimeGlow(Perk perk, TextMeshProUGUI text) {
        return text.text.Length <= 3 ? mImgTimeGlowInsts1[(int)perk.strenght] : mImgTimeGlowInsts2[(int)perk.strenght];
    }

    private void updateData() {
        upgradeText.text = "Upgrade " + GeneratorValues.upgradeStages[mCostDex] + "x";
        costText.text = Formatter.formatNumber(mCosts[mCostDex]);
        LayoutRebuilder.ForceRebuildLayoutImmediate(costContainer);

        levelText.text = mCurrentGenerator.getLevel().ToString();

        switch (mCurrentGenerator.type) {
            case GeneratorType.BOTTLE_FILLER:
                bottleFillerLayout.updateData(GeneratorValues.upgradeStages[mCostDex]);
                break;

            case GeneratorType.MONEY_COLLECTOR:
                moneyCollectorLayout.updateData(GeneratorValues.upgradeStages[mCostDex]);
                break;

            case GeneratorType.BOTTLE_DISTRIBUTER:
                bottleDistributerLayout.updateData(GeneratorValues.upgradeStages[mCostDex]);
                break;

        }

        updateSliders();
        checkCanPay();
    }

    private void checkCanPay() {
        if (mCurrentGenerator == null) return;
        if (Database.inst.getPrimaryMoney() >= mCosts[mCostDex]) { // can pay
            btnUpgrade.interactable = true;
            costText.color = Color.white;
            upgradeText.color = Color.white;
            imgBtnUpgrade.color = Color.white;
            imgUpgradeBtnCoin.color = Color.white;
        } else { // cannot pay
            btnUpgrade.interactable = false;
            Color red = Database.inst.noMoneyRedColor;
            red.a = 0.5f;
            costText.color = red;
            upgradeText.color = red;
            Color semiWhite = new Color(1, 1, 1, 0.5f);
            imgBtnUpgrade.color = semiWhite;
            imgUpgradeBtnCoin.color = semiWhite;
        }
    }

    private void calcCost() {
        int lvl = mCurrentGenerator.getLevel();
        switch (mCurrentGenerator.type) {
            case GeneratorType.BOTTLE_FILLER:
                BottleFillerController b = (BottleFillerController)mCurrentGenerator;
                GeneratorValues.bottleFillerStagesCost(lvl, PlanetController.inst.bottleFillers.IndexOf(b), ref mCosts);
                break;

            case GeneratorType.MONEY_COLLECTOR:
                GeneratorValues.moneyCollectorStagesCost(lvl, ref mCosts);
                break;

            case GeneratorType.BOTTLE_DISTRIBUTER:
                GeneratorValues.bottleDistributerStagesCost(lvl, ref mCosts);
                break;
        }
    }

    private void hideAllDatas() {
        bottleFillerLayout.gameObject.SetActive(false);
        moneyCollectorLayout.gameObject.SetActive(false);
        bottleDistributerLayout.gameObject.SetActive(false);
    }

    private void updateSliders() {
        int genLvl = mCurrentGenerator.getLevel();
        int minValue = 0, maxValue = 0, nextReward = 0;
        switch (mCurrentGenerator.type) {
            case GeneratorType.BOTTLE_FILLER:
                maxValue = GeneratorValues.bottleFillerNextBoostLvl(genLvl);
                minValue = GeneratorValues.bottleFillerLastBoostLvl(genLvl);
                nextReward = GeneratorValues.bottleFillerNextBoostReward(genLvl);
                break;

            case GeneratorType.MONEY_COLLECTOR:
                maxValue = GeneratorValues.moneyCollectorNextBoostLvl(genLvl);
                minValue = GeneratorValues.moneyCollectorLastBoostLvl(genLvl);
                nextReward = GeneratorValues.moneyCollectorNextBoostReward(genLvl);
                break;

            case GeneratorType.BOTTLE_DISTRIBUTER:
                maxValue = GeneratorValues.bottleDistributerNextBoostLvl(genLvl);
                minValue = GeneratorValues.bottleDistributerLastBoostLvl(genLvl);
                nextReward = GeneratorValues.bottleDistributerNextBoostReward(genLvl);
                break;

        }

        mainSlider.setProgress((float)(genLvl - minValue) / (float)(maxValue - minValue));
        // mainSlider.minValue = minValue;
        // mainSlider.maxValue = maxValue;
        // mainSlider.value = genLvl;

        previewSlider.setProgress((float)(genLvl + GeneratorValues.upgradeStages[mCostDex] - minValue) / (float)(maxValue - minValue));
        // previewSlider.minValue = minValue;
        // previewSlider.maxValue = maxValue;
        // previewSlider.value = genLvl + 1;

        if (nextReward > 0) {
            nextRewardCoin.enabled = true;
            nextBoostLvlText.text = "Next boost in Level " + maxValue;
            boostRewardText.text = "+" + nextReward;
        } else {
            nextRewardCoin.enabled = false;
            boostRewardText.text = "";
            nextBoostLvlText.text = "";
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(boostRewardContainer);

    }

    public void hide() {
        if (mCurrentGenerator) mCurrentGenerator.removePerkListener(this);
        PlanetUiController.inst.disableUiModeLvl0();
        mCurrentGenerator = null;
        gameObject.SetActive(false);
        mPerkTimeUpdaterActive = false;
    }

    private void checkPrevNextBtnsEnabled() {
        bool prevEnabled = mCostDex > 0;
        bool nextEnabled = mCostDex < mCosts.Length - 1;

        btnPreviousMultiplier.interactable = prevEnabled;
        btnNextMultiplier.interactable = nextEnabled;

        Color semiWhite = new Color(1, 1, 1, 0.5f);
        imgBtnPreviousMultiplier.color = prevEnabled ? Color.white : semiWhite;
        imgBtnNextMultiplier.color = nextEnabled ? Color.white : semiWhite;

    }

    public void onBtnUpgradeClicked() {
        Database.inst.removePrimaryMoney(mCosts[mCostDex]);

        mCurrentGenerator.increaseLevel(GeneratorValues.upgradeStages[mCostDex]);

        calcCost();
        updateData();
    }

    public void onBtnCloseClicked() {
        hide();
    }

    public void onBtnPreviousMultiplierClicked() {
        mCostDex--;
        updateData();
        checkPrevNextBtnsEnabled();
    }

    public void onBtnNextMultiplierClicked() {
        mCostDex++;
        updateData();
        checkPrevNextBtnsEnabled();
    }

    public void onPrimaryMoneyChanged(BigDecimal newMoney) {
        checkCanPay();
    }

    public void onSecondaryMoneyChanged(BigDecimal newMoney) {
    }

    public void onPerksChanged() {
        updatePerks();
        updateData();
    }
}

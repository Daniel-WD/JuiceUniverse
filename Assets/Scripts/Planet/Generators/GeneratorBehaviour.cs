using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PerkListener {

    void onPerksChanged();

}

public enum GeneratorType {
    BOTTLE_FILLER = 0, MONEY_COLLECTOR = 1, BOTTLE_DISTRIBUTER = 2, ALL = 3
}

public abstract class GeneratorBehaviour : MonoBehaviour, PerkManagerListener {

    public GeneratorType type;

    public GeneratorProgressbar progressbar;
    
    public Animator highlightAnimator, autoControllerAnimator;
    
    public ParticleSystem highlightParticleSystem;

    public GeneratorUpgrader generatorUpgrader;
    public AutoController autoController;

    public GameObject bannerInst;
    public Transform perkRepLeft, perkRepRight, perkRepCenter;

    private List<PerkBanner> mPerkBanners = new List<PerkBanner>();
    

    protected bool isHighlighting = false;

    protected bool auto = false; // save
    protected int level = 1; //save

    private List<PerkInstance> mPerks = new List<PerkInstance>(); //save
    private List<PerkListener> mPerkListeners = new List<PerkListener>();

    public abstract void startCycle();
    public abstract void stopCycle();
    public abstract void updateGeneratorData();
    public abstract int possibleUpgradeCount(int maxCount);
    public abstract bool canUpgrade();
    public abstract System.Object getGeneratorData();

    protected virtual void Awake() {
    }

    protected virtual void Start() {
        PerkManager.inst.addListener(this);
        // StartCoroutine(decreasePerkTime());
    }

    protected virtual void Update() {

    }

    // private IEnumerator decreasePerkTime() {
    //     List<Perk> removablePerks = new List<Perk>();
    //     bool smtRemoved = false;
    //     while (true) {
    //         foreach (Perk perk in mPerks) {
    //             perk.activeTimeLeft--;
    //             if (perk.activeTimeLeft <= 0) {
    //                 removablePerks.Add(perk);
    //                 smtRemoved = true;
    //             }
    //         }
    //         foreach (Perk perk in removablePerks) mPerks.Remove(perk);
    //         removablePerks.Clear();
    //         if (smtRemoved) {
    //             foreach (PerkListener listener in mPerkListeners) listener.onPerksChanged();
    //             updateGeneratorData();
    //             smtRemoved = false;
    //         }

    //         yield return new WaitForSeconds(1);
    //     }
    // }

    public void setAuto(bool a) {
        auto = a;
        autoController.updateGfx();
    }

    public bool isAuto() {
        return auto;
    }

    public void setLevel(int lvl) {
        level = lvl;
        updateGeneratorData();
        generatorUpgrader.updateLevelText();
    }

    public int getLevel() {
        return level;
    }

    public void increaseLevel(int count) {
        level += count;
        updateGeneratorData();
        generatorUpgrader.updateLevelText();
    }

    public List<PerkInstance> getPerks() {
        return mPerks;
    }

    public void addPerks(PerkInstance[] perks) {
        foreach (PerkInstance perk in perks) {
            if (mPerks.Contains(perk)) continue;
            mPerks.Add(perk);
            foreach (PerkListener listener in mPerkListeners) listener.onPerksChanged();
        }
        updateGeneratorData();
        updatePerkBanners();
    }

    public void addPerk(PerkInstance perk) {
        if (mPerks.Contains(perk)) return;
        mPerks.Add(perk);
        updateGeneratorData();
        foreach (PerkListener listener in mPerkListeners) listener.onPerksChanged();
        updatePerkBanners();
    }

    public void removePerk(PerkInstance perk) {
        mPerks.Remove(perk);
        updateGeneratorData();
        foreach (PerkListener listener in mPerkListeners) listener.onPerksChanged();
        updatePerkBanners();
    }

    private void updatePerkBanners() {
        foreach (PerkBanner banner in mPerkBanners) {
            ObjectPooler.inst.addToPool(banner.gameObject);
        }
        mPerkBanners.Clear();

        for (int i = 0; i < mPerks.Count; i++) {
            Vector3 pos = perkRepCenter.position;
            if (mPerks.Count > 1) {
                if (i == 0) pos = perkRepLeft.position;
                else pos = perkRepRight.position;
            }
            PerkBanner newBanner = ObjectPooler.inst.giveMe(bannerInst, pos, Quaternion.identity).GetComponent<PerkBanner>();
            newBanner.set(mPerks[i].perk, type);
            mPerkBanners.Add(newBanner);
        }
    }

    public void addPerkListener(PerkListener listener) {
        mPerkListeners.Add(listener);
    }

    public void removePerkListener(PerkListener listener) {
        mPerkListeners.Remove(listener);
    }

    protected void startHighlight() {
        if (highlightParticleSystem && !highlightParticleSystem.isEmitting) highlightParticleSystem.Play();
        if (!isHighlighting && highlightAnimator) highlightAnimator.SetTrigger("highlight");
        isHighlighting = true;
    }

    protected void stopHighlight() {
        if (highlightParticleSystem && highlightParticleSystem.isEmitting) highlightParticleSystem.Stop();
        if (isHighlighting && highlightAnimator) highlightAnimator.SetTrigger("stopHighlight");
        isHighlighting = false;
    }

    public void onPerkUsable(PerkInstance perk) {
    }

    public void onPerkActivated(PerkInstance perk) {
    }

    public void onPerkSleep(PerkInstance perk) {
        if (mPerks.Contains(perk)) {
            removePerk(perk);
        }
    }

    public virtual void reset() {
        stopCycle();

        setAuto(false);
        setLevel(1);

        foreach (PerkBanner banner in mPerkBanners) Destroy(banner.gameObject);
        mPerkBanners.Clear();

        stopHighlight();
        mPerks.Clear();
        
        progressbar.cancel();
        autoControllerAnimator.SetTrigger("cancel");

    }
}

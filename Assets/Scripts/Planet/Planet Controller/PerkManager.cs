using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PerkManagerListener {

    void onPerkUsable(PerkInstance perk);
    void onPerkActivated(PerkInstance perk);
    void onPerkSleep(PerkInstance perk);

}

public class PerkManager : MonoBehaviour {

    public static PerkManager inst;

    public Perk[] perkPool;

    private List<PerkInstance> mUsablePerks = new List<PerkInstance>();
    private List<PerkInstance> mSleepingPerks = new List<PerkInstance>();
    private List<PerkInstance> mActivePerks = new List<PerkInstance>();

    private List<PerkManagerListener> mListeners = new List<PerkManagerListener>();

    public Perk[] initialUsablePerks;
    public Perk[] initialSleepingPerks;

    public Perk[] p;

    void Awake() {
        inst = this;
    }

    void Start() {
        StartCoroutine(secondTicker());

        // TESTING
        foreach (Perk perk in p) {
            PerkInstance inst = new PerkInstance(perk, 100, 200);
            mUsablePerks.Add(inst);
            activatePerk(inst, BottleDistributerController.inst);
        }

        foreach (Perk p in initialUsablePerks) {
            PerkInstance inst = new PerkInstance(p, 100, 200);
            mUsablePerks.Add(inst);
        }

        foreach (Perk p in initialSleepingPerks) {
            PerkInstance inst = new PerkInstance(p, 100, 200);
            inst.sleepTimeLeft = inst.sleepTime;
            mSleepingPerks.Add(inst);
        }
    }

    private IEnumerator secondTicker() {

        List<PerkInstance> goToSleepPerks = new List<PerkInstance>();
        List<PerkInstance> makeUsablePerks = new List<PerkInstance>();
        while (true) {
            //active time
            foreach (PerkInstance perk in mActivePerks) {
                perk.activeTimeLeft--;
                if (perk.activeTimeLeft <= 0) {
                    perk.activeTimeLeft = -1;
                    goToSleepPerks.Add(perk);
                }
            }
            foreach (PerkInstance perk in goToSleepPerks) {
                mActivePerks.Remove(perk);
                mSleepingPerks.Add(perk);
                perk.sleepTimeLeft = perk.sleepTime;
                foreach (PerkManagerListener listener in mListeners) {
                    listener.onPerkSleep(perk);
                }
            }
            goToSleepPerks.Clear();

            //sleeping time
            foreach (PerkInstance perk in mSleepingPerks) {
                perk.sleepTimeLeft--;
                if (perk.sleepTimeLeft <= 0) {
                    perk.sleepTimeLeft = -1;
                    makeUsablePerks.Add(perk);
                }
            }
            foreach (PerkInstance perk in makeUsablePerks) {
                perk.sleepTimeLeft = perk.sleepTime;
                mSleepingPerks.Remove(perk);
                mUsablePerks.Add(perk);
                foreach (PerkManagerListener listener in mListeners) {
                    listener.onPerkUsable(perk);
                }
            }
            makeUsablePerks.Clear();

            yield return new WaitForSeconds(1);
        }
    }

    public void generateRandomPerk(GeneratorType type) {
        Perk perk;
        do {
            perk = perkPool[Random.Range(0, perkPool.Length)];
        } while (perk.generatorType != type);

        ///TODO!!!!!!!!!!!!!!!!!!!!!!!! SET SLEEP AND ACTIVE TIME
        PerkInstance inst = new PerkInstance(perk, 100, 200);

        addUsablePerk(inst);
    }

    public void addUsablePerk(PerkInstance perk) {
        mUsablePerks.Add(perk);
        foreach (PerkManagerListener listener in mListeners) listener.onPerkUsable(perk);
    }

    public void activatePerk(PerkInstance perkInst, GeneratorBehaviour generator) {
        foreach (PerkInstance p in generator.getPerks()) {
            if (p.perk.property == perkInst.perk.property) return;
        }
        perkInst.activeTimeLeft = perkInst.activeTime;
        mUsablePerks.Remove(perkInst);
        mActivePerks.Add(perkInst);
        generator.addPerk(perkInst);
        foreach (PerkManagerListener listener in mListeners) listener.onPerkActivated(perkInst);
    }

    // private void makePerkSleep(Perk perk) {
    //     mActivePerks.Remove(perk);
    //     mSleepingPerks.Add(perk);
    //     foreach (PerkManagerListener listener in mListeners) listener.onPerkSleep(perk);
    // }

    public Perk findPerkByHash(int hash) {
        foreach (Perk p in perkPool) {
            if (p.getHash() == hash) return p;
        }
        return null;
    }

    public void addListener(PerkManagerListener listener) {
        if (!mListeners.Contains(listener) && listener != null) mListeners.Add(listener);
    }

    public void removeListener(PerkManagerListener listener) {
        mListeners.Remove(listener);
    }

    public List<PerkInstance> getUsablePerks() {
        return mUsablePerks;
    }

    public List<PerkInstance> getSleepingPerks() {
        return mSleepingPerks;
    }

    public List<PerkInstance> getActivePerks() {
        return mActivePerks;
    }

    public void deleteAllPerks() {
        mUsablePerks.Clear();
        mSleepingPerks.Clear();
        mActivePerks.Clear();
    }

    public void loadUsablePerks(PerkInstance[] perks) {
        foreach (PerkInstance perk in perks) {
            addUsablePerk(perk);
        }
    }

    public void loadSleepingPerks(PerkInstance[] perks) {
        foreach (PerkInstance perk in perks) {
            mSleepingPerks.Add(perk);
            foreach (PerkManagerListener listener in mListeners) listener.onPerkSleep(perk);
        }
    }

    public void loadActivePerks(PerkInstance[] perks) {
        foreach (PerkInstance perk in perks) {
            mActivePerks.Add(perk);
            foreach (PerkManagerListener listener in mListeners) listener.onPerkActivated(perk);
        }
    }

}

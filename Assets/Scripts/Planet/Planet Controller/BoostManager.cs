using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface BoostManagerListener {

    void onBoostUsable(BoostInstance boost);
    void onBoostActivated(BoostInstance boost);
    void onBoostDied(BoostInstance boost);

}

public class BoostManager : MonoBehaviour {

    public static BoostManager inst;

    public Boost[] availableBoosts;

    public Boost[] initialUsableBoosts, initialActiveBoosts;

    private List<BoostInstance> mUsableBoosts = new List<BoostInstance>();
    private List<BoostInstance> mActiveBoosts = new List<BoostInstance>();

    private List<BoostManagerListener> mListeners = new List<BoostManagerListener>();

    void Awake() {
        inst = this;
    }

    void Start() {
        StartCoroutine(secondTicker());

        foreach (Boost b in initialUsableBoosts) {
            BoostInstance inst = new BoostInstance(b, 200);
            mUsableBoosts.Add(inst);
        }

        foreach (Boost b in initialActiveBoosts) {
            BoostInstance inst = new BoostInstance(b, 200);
            mActiveBoosts.Add(inst);
        }

        foreach (BoostManagerListener listener in mListeners)
            foreach (BoostInstance boost in mActiveBoosts) listener.onBoostActivated(boost);
    }

    private IEnumerator secondTicker() {

        List<BoostInstance> removableBoosts = new List<BoostInstance>();
        while (true) {
            //active time
            foreach (BoostInstance boost in mActiveBoosts) {
                boost.activeTimeLeft--;
                if (boost.activeTimeLeft <= 0) {
                    boost.activeTimeLeft = -1;
                    removableBoosts.Add(boost);
                }
            }
            foreach (BoostInstance boost in removableBoosts) {
                mActiveBoosts.Remove(boost);
                foreach (BoostManagerListener listener in mListeners) {
                    listener.onBoostDied(boost);
                }
            }
            removableBoosts.Clear();

            yield return new WaitForSeconds(1);
        }
    }

    private void addUsableBoost(BoostInstance boostInst) { //---> set active time on generate... before calling this method
        mUsableBoosts.Add(boostInst);
        foreach (BoostManagerListener listener in mListeners) listener.onBoostUsable(boostInst);
    }

    public void activateBoost(BoostInstance boost) {
        boost.activeTimeLeft = boost.activeTime;
        mUsableBoosts.Remove(boost);
        mActiveBoosts.Add(boost);
        foreach (BoostManagerListener listener in mListeners) listener.onBoostActivated(boost);
    }

    public int calcFinalMultiplier() {
        int multiplier = 1;
        foreach (BoostInstance boostInst in mActiveBoosts) {
            multiplier *= boostInst.boost.multiplier;
        }
        return multiplier;
    }

    public Boost findBoostByHash(int hash) {
        foreach (Boost b in availableBoosts) {
            if (b.getHash() == hash) return b;
        }
        return null;
    }

    public void addListener(BoostManagerListener listener) {
        mListeners.Add(listener);
    }

    public void removeListener(BoostManagerListener listener) {
        mListeners.Remove(listener);
    }

    public List<BoostInstance> getUsableBoosts() {
        return mUsableBoosts;
    }

    public List<BoostInstance> getActiveBoosts() {
        return mActiveBoosts;
    }

    public void deleteAllBoosts() {
        mUsableBoosts.Clear();
        mActiveBoosts.Clear();
    }

    public void loadUsableBoosts(BoostInstance[] boosts) {
        foreach (BoostInstance b in boosts) {
            addUsableBoost(b);
        }
    }

    public void loadActiveBoosts(BoostInstance[] boosts) {
        foreach (BoostInstance b in boosts) {
            mActiveBoosts.Add(b);
            foreach (BoostManagerListener listener in mListeners) listener.onBoostActivated(b);
        }

    }

}

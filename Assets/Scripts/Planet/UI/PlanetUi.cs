using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetUi : MonoBehaviour {

    // int i = 0;

    void Awake() {
        // print("Awake: " + i++);
    }

    public void onBtnOverviewClicked() {
        ProductionOverviewDialog.inst.show();
    }

    public void onBtnBoostClicked() {
        ChooseBoostDialog.inst.show();
    }

    public void onBtnSettingsClicked() {
        SettingsDialog.inst.show();
    }

    public void onBtnMapClicked() {
        PlanetController.inst.savePlanet();
        SceneManager.LoadSceneAsync("Map", LoadSceneMode.Additive);
        gameObject.SetActive(false);
    }

}

using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class MapController : MonoBehaviour {

    public static MapController inst;

    public PlanetReprController[] planets;

    void Awake() {
        inst = this;
    }

    void Start() {
        refresh();
    }

    public void refresh() {
        for (int i = 0; i < planets.Length; i++) {
            PlanetReprController planet = planets[i];
            PlanetSaveData data = Database.inst.planets[i];
            if (data != null) planet.set(data.production < 0 ? 0 : data.production);
            else planet.set(new BigDecimal(-123, 1));
        }
    }

    public int findIndex(PlanetReprController controller) {
        for (int i = 0; i < planets.Length; i++) {
            if (planets[i] == controller) return i;
        }
        return -1;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolAdderEvent : MonoBehaviour {
    
    public void addToPool() {
        ObjectPooler.inst.addToPool(gameObject);
    }
    
}

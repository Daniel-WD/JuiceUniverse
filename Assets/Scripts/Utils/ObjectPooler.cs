using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {

    // public interface Resetable {
        // void reset();
    // }

    public static ObjectPooler inst;

    private List<List<GameObject>> pool = new List<List<GameObject>>();
    private List<List<GameObject>> emptyLists = new List<List<GameObject>>();

    void Awake() {
        inst = this;
    }
    
    public GameObject giveMe(GameObject clone, Vector3 position, Quaternion rotation) {
        List<GameObject> instPool = findList(clone);
        if (instPool != null && instPool.Count > 0) {
            GameObject result = instPool[0];
            result.transform.position = position;
            result.transform.localRotation = rotation;
            reeneableObjectRecursivly(result.transform); 
            instPool.RemoveAt(0);
            if (instPool.Count == 0) {
                emptyLists.Add(instPool);
                pool.Remove(instPool);
            }
            return result;
        } else {
            GameObject newObject = Instantiate(clone, position, rotation);
            return newObject;
        }
    }
    //TODO...
    public GameObject giveMe(GameObject clone, Transform parent) {
        List<GameObject> instPool = findList(clone);
        if (instPool != null && instPool.Count > 0) {
            GameObject result = instPool[0];
            result.transform.parent = parent;
            result.transform.localPosition = Vector3.zero;
            result.transform.localEulerAngles = Vector3.zero;
            reeneableObjectRecursivly(result.transform); 
            instPool.RemoveAt(0);
            if (instPool.Count == 0) {
                emptyLists.Add(instPool);
                pool.Remove(instPool);
            }
            return result;
        } else {
            GameObject newObject = Instantiate(clone, parent);
            return newObject;
        }
    }

    public void addToPool(GameObject instance) {
        instance.transform.parent = null;
        instance.SetActive(false);
        List<GameObject> list = findList(instance);
        if (list == null) {

            list = giveMeAList();
            pool.Add(list);
        }
        list.Add(instance);
    }

    private List<GameObject> findList(GameObject clone) {
        if (pool.Count == 0) return null;
        foreach (var list in pool) {
            if (list.Count == 0 || list[0] == null) continue;
            if (removeClonePartOfGameObjectName(list[0].name).Equals(removeClonePartOfGameObjectName(clone.name))) {
                return list;
            }
        }
        return null;
    }

    private List<GameObject> giveMeAList() {
        if (emptyLists.Count == 0) {
            return new List<GameObject>();
        } else {
            List<GameObject> res = emptyLists[0];
            emptyLists.RemoveAt(0);
            return res;
        }
    }

    private string removeClonePartOfGameObjectName(string name) {
        if (name.Contains("(Clone)")) {
            string res = name.Substring(0, name.IndexOf("(Clone)"));
            return res;
        } else {
            return name;
        }

    }

    private void reeneableObjectRecursivly(Transform parent) {
        if (parent == null) return;
        parent.gameObject.SetActive(true);
        if (parent.childCount > 0) {
            for(int i = 0; i < parent.childCount; i++) {
                reeneableObjectRecursivly(parent.GetChild(i));
            }
        }
    }

}

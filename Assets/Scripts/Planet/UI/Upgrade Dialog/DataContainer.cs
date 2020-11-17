using UnityEngine;
using UnityEngine.UI;

public abstract class DataContainer : MonoBehaviour {

    //no singleton stuff because producer arent singletons!!!
    public GeneratorBehaviour generatorBehaviour;
    public RectTransform[] coinLayouts;

    public abstract void updateData(int previewLevelCount);

    protected void updateCoinLayouts() {
        foreach (var l in coinLayouts) {
            LayoutRebuilder.ForceRebuildLayoutImmediate(l);
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoController : ClickBehaviour {

    public GeneratorBehaviour generatorObj;

    public SpriteRenderer autoSprite, nonAutoSprite, glowSprite;

    void Start() {
        updateGfx();
    }

    protected override void onClick() {
        if (generatorObj.isAuto()) {
            ChoosePerkDialog.inst.show(generatorObj);
        } else {
            NewOfficerDialog.inst.show(generatorObj);
        }
    }

    public void updateGfx() {
        bool auto = generatorObj.isAuto();
        autoSprite.enabled = auto;
        if (glowSprite) glowSprite.enabled = auto;
        nonAutoSprite.enabled = !auto;
    }

}

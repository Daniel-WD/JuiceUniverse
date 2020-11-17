using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualCycleStarter : ClickBehaviour {
    
    public GeneratorBehaviour generatorBehaviour;
    
    protected override void onClick() {
        if(generatorBehaviour != null) generatorBehaviour.startCycle();
    }
    
}

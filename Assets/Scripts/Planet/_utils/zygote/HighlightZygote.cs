using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightZygote : Zygote {
    
    public static HighlightZygote inst;
    
    void Awake() {
        inst = this;
    }
    
}

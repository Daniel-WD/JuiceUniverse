using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Boost", menuName = "Boost")]
public class Boost : ScriptableObject {
    
    public Sprite littleGfx, gfx, bgGfx;
    public int multiplier;
    
    public Boost clone() {
        Boost boost = new Boost();
        boost.littleGfx = littleGfx;
        boost.gfx = gfx;
        boost.bgGfx = bgGfx;
        return boost;
    }
    
    public int getHash() {
        return multiplier.ToString().GetHashCode();
    }
    
}

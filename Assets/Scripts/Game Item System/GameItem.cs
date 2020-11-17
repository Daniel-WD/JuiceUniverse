using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameItem : ScriptableObject {
    
    public Sprite gfx;
    public Sprite icon;
    
    public abstract string info();
    public abstract void makeAvailable();
    
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public enum PerkStrength {
    ONE = 0, TWO = 1, THREE = 2, FOUR = 3
}

public enum PerkProperty {
    //Elevators
    ELEVATOR_FILL_SPEED = 0, ELEVATOR_MOVE_SPEED = 1, ELEVATOR_CAPACITY = 2, ELEVATOR_DISCOUNT = 3,
    //Fill Station
    FILL_STATION_FILL_SPEED = 4, FILL_STATION_FILL_RATE = 5, FILL_STATION_CAPACITY = 6, FILL_STATION_DISCOUNT = 7,
    //All
    DISCOUNT = 8
}

[CreateAssetMenu(fileName = "New Perk", menuName = "Perk")]
public class Perk : ScriptableObject {

    public static readonly float[] STRENGTHS = { 0.1f, 0.3f, 0.5f, 0.7f };

    public GeneratorType generatorType;
    public PerkStrength strenght;
    public PerkProperty property;
    public Sprite gfx, little_gfx;
    
    public string getPropertyString() {
        switch (property) {

            case PerkProperty.FILL_STATION_FILL_SPEED:
            case PerkProperty.ELEVATOR_FILL_SPEED:
                return "Fill Speed";

            case PerkProperty.ELEVATOR_MOVE_SPEED:
                return "Move Speed";

            case PerkProperty.ELEVATOR_CAPACITY:
                return generatorType == GeneratorType.BOTTLE_DISTRIBUTER ? "Bottle Capacity" : "Money Capacity";

            case PerkProperty.FILL_STATION_DISCOUNT:
            case PerkProperty.ELEVATOR_DISCOUNT:
            case PerkProperty.DISCOUNT:
                return "Discount";

            case PerkProperty.FILL_STATION_FILL_RATE:
                return "Fill Rate";

            case PerkProperty.FILL_STATION_CAPACITY:
                return "Bottle Capacity";

        }
        return "";
    }
    
    public int getHash() {
        return ("" + (int)generatorType + (int)strenght + (int)property).GetHashCode();;
    }
    
}

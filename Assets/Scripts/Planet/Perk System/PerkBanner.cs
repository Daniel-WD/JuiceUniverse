using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkBanner : MonoBehaviour {

    public Sprite[] leftBanners, rightBanners, centerBanners;

    public SpriteRenderer banner;
    public SpriteRenderer perkLeft, perkRight, perkCenter;

    public void set(Perk perk, GeneratorType type) {

        switch (type) {
            case GeneratorType.BOTTLE_FILLER: //center
                perkLeft.enabled = false;
                perkRight.enabled = false;
                perkCenter.enabled = true;

                perkCenter.sprite = perk.little_gfx;
                banner.sprite = centerBanners[(int)perk.strenght];
                break;

            case GeneratorType.BOTTLE_DISTRIBUTER: //left
                perkLeft.enabled = true;
                perkRight.enabled = false;
                perkCenter.enabled = false;

                perkLeft.sprite = perk.little_gfx;
                banner.sprite = leftBanners[(int)perk.strenght];
                break;

            case GeneratorType.MONEY_COLLECTOR: //right
                perkLeft.enabled = false;
                perkRight.enabled = true;
                perkCenter.enabled = false;

                perkRight.sprite = perk.little_gfx;
                banner.sprite = rightBanners[(int)perk.strenght];
                break;
        }

    }

}

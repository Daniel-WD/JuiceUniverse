using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour {

    public TextMeshProUGUI text;
    public string textUp, textDown;
    public Sprite spriteUp, spriteDown;

    public bool down = true;

    private Image mImage;
    private Button mButton;

    void Start() {
        mImage = GetComponent<Image>();
        mButton = GetComponent<Button>();
        setState(down);
    }

    public void onBtnUp() {
        setState(down);
    }

    public void onBtnDown() {
        down = !down; 

        text.text = down ? textDown : textUp;
    }

    public void setState(bool u) {
        down = u;

        mImage.sprite = down ? spriteDown : spriteUp;
        SpriteState btnSpriteState = mButton.spriteState;
        btnSpriteState.pressedSprite = down ? spriteUp : spriteDown;
        mButton.spriteState = btnSpriteState;
    }

}

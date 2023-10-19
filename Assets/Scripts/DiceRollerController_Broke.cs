using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRollerController_Broke : DiceRollerController
{

    bool isPressed = false;
    bool isRandoming = false;



    float randomTime = 0;
    float randomTime_interval = 0;

    public int randomCount = 4;


    public List<MeshRenderer> lightSwitch = new List<MeshRenderer>();

    public Material mat_on;
    public Material mat_off;


    bool flashLightSwitch = false;
    bool isLit = false;
    float lightTime;
    float lightTime_interval;

    public override void changeDiceFace(int faceNumber)
    {
        diceAnim.SetInteger("DiceFace", faceNumber);
        //SetButtonPressed(faceNumber);

        // important!
        currentFace = faceNumber;
    }


    public override void randomDiceFace()
    {
        if (isPressed) return;

        AudioManager.instance.PlaySound("sfx_click", AudioManager.Chanel.SFX_1);

        isPressed = true;
        randomTime = 0;
        randomTime_interval = 0;

        diceAnim.speed = 3;

        diceButton[0].localPosition = new Vector3(
            diceButton[0].localPosition.x,
            button_pressedY,
            diceButton[0].localPosition.z
            );

        if (randomCount <= 0)
        {
            // do something;
            flashLightSwitch = true;
            lightTime = 0;
            lightTime_interval = 0;

            lightSwitch[0].material = mat_on;
            isLit = true;

            return;
        }

        isRandoming = true;
        randomCount--;

        UpdateLightSwitch();

        //diceAnim.SetInteger("DiceFace", faceNumber);
        //SetButtonPressed(faceNumber);

        int randombullshit = Random.Range(1, 7);

        // important!
        currentFace = randombullshit;

    }

    // Update is called once per frame
    void Update()
    {
        var dt = Time.deltaTime;

        if (isPressed)
        {
            randomTime += dt;
            randomTime_interval += dt;

            if (randomTime_interval > 0.1f && isRandoming)
            {
                int randombullshit = Random.Range(1, 7);
                diceAnim.SetInteger("DiceFace", randombullshit);
                diceAnim.transform.GetChild(0).Rotate(360, 360, 360);
                randomTime_interval -= 0.1f;
            }

            if (randomTime > 1f)
            {
                if (isRandoming)
                {
                    diceAnim.SetInteger("DiceFace", currentFace);
                }
                isRandoming = false;
                isPressed = false;
            }

            diceButton[0].localPosition = new Vector3(
                diceButton[0].localPosition.x,
                randomTime.Remap(0, 1, button_pressedY, button_originY),
                diceButton[0].localPosition.z
                );
        }


        if (flashLightSwitch)
        {
            lightTime += dt;
            lightTime_interval += dt;

            if (lightTime_interval >= 0.5f)
            {
                isLit = !isLit;
                lightSwitch[0].material = isLit ? mat_on : mat_off;
                lightTime_interval -= 0.5f;
            }

            if (lightTime >= 3)
            {
                flashLightSwitch = false;
                lightSwitch[0].material = mat_off;
            }

        }

    }

    void UpdateLightSwitch()
    {

        foreach (var item in lightSwitch)
        {
            item.material = mat_off;
        }

        for (int i = 0; i < randomCount; i++)
        {
            lightSwitch[i].material = mat_on;
        }
    }

}

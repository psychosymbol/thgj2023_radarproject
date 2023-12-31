using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DiceRollerController : MonoBehaviour
{
    public Animator diceAnim;

    public List<Transform> diceButton = new List<Transform>();

    public float button_originY;
    public float button_pressedY;

    public int currentFace = 0;
    public int currentRotation = 0;

    public DR_STATE drState;
    public GameManager.DR_TYPE drType;

    public virtual void changeDiceFace(int faceNumber)
    {
        diceAnim.SetInteger("DiceFace", faceNumber);
        SetButtonPressed(faceNumber);

        // important!
        currentFace = faceNumber;
    }

    public virtual void randomDiceFace()
    {
        int randombullshit = Random.Range(1, 7);
        diceAnim.SetInteger("DiceFace", randombullshit);
    }

    public void toggleRotation()
    {
        diceAnim.SetBool("isRotate", !diceAnim.GetBool("isRotate"));

        // important!
        currentRotation = currentRotation == 0 ? 1 : 0;
    }


    public void SetButtonPressed(int faceNumber)
    {
        AudioManager.instance.PlaySound("sfx_click", AudioManager.Chanel.SFX_1);

        foreach (var t in diceButton)
        {
            t.localPosition = new Vector3(
            t.localPosition.x,
            button_originY,
            t.localPosition.z
            );
        }

        var index = faceNumber - 1;

        diceButton[index].localPosition = new Vector3(
            diceButton[index].localPosition.x,
            button_pressedY,
            diceButton[index].localPosition.z
            );

    }


    protected void Start()
    {
        // set default on start

        changeDiceFace(1);
    }

    public enum DR_STATE
    {
        REST,
        CAM,
        SET
    }

    public void SetDRState(DR_STATE state)
    {
        drState = state;
    }
}

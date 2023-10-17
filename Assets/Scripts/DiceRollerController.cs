using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRollerController : MonoBehaviour
{
    public Animator diceAnim;

    public void changeDiceFace(int faceNumber)
    {
        diceAnim.SetInteger("DiceFace", faceNumber);
    }

    public void randomDiceFace()
    {
        int randombullshit = Random.Range(1, 7);
        diceAnim.SetInteger("DiceFace", randombullshit);
    }

    public void toggleRotation()
    {
        diceAnim.SetBool("isRotate", !diceAnim.GetBool("isRotate"));
    }
}

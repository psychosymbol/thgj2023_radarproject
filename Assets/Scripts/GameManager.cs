using DigitalRuby.Tween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public RadarController radarController;
    public RadarGridController radarGridController;

    public DiceRollerController DiceRoller_normal;
    public DiceRollerController DiceRoller_broke;

    public Transform DR_pos_cam;
    public Transform DR_pos_rest1;
    public Transform DR_pos_rest2;
    public Transform DR_pos_set1;
    public Transform DR_pos_set2;

    public float tweenDuration = 0.3f;

    public MeshRenderer scanLight;
    public MeshRenderer distractLight;

    public Material mat_on;
    public Material mat_off;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    switch (dr1State)
        //    {
        //        case DR_STATE.REST:
        //            ToCam();
        //            break;
        //        case DR_STATE.CAM:
        //            ToRest();
        //            break;
        //        case DR_STATE.SET:
        //            break;
        //    }
        //}



    }

    public float currentDepth = 0;
    public float descendSpeed = 100;

    //public float 

    //func
    public void Descend()
    {

    }

    bool isTweening = false;

    public void ToRest()
    {
        var dr = GetSelectedDR();
        ToRest(dr);
    }

    public void ToRest(DiceRollerController diceRoller)
    {
        var restTransform = DR_pos_rest1;

        switch (diceRoller.drType)
        {
            case DR_TYPE.NORMAL:
                restTransform = DR_pos_rest1;
                break;
            case DR_TYPE.BROKE:
                restTransform = DR_pos_rest2;
                break;
        }

        TweenDR(diceRoller, restTransform, () => { diceRoller.SetDRState(DiceRollerController.DR_STATE.REST); });
    }

    public void ToCam(DiceRollerController diceRoller)
    {
        TweenDR(diceRoller, DR_pos_cam, () => { diceRoller.SetDRState(DiceRollerController.DR_STATE.CAM); });
    }
    public void ToSet1_Scan(DiceRollerController diceRoller)
    {
        TweenDR(diceRoller, DR_pos_set1, () => { diceRoller.SetDRState(DiceRollerController.DR_STATE.SET); });
    }
    public void ToSet2_Distract(DiceRollerController diceRoller)
    {
        TweenDR(diceRoller, DR_pos_set2, () => { diceRoller.SetDRState(DiceRollerController.DR_STATE.SET); });
    }

    public void TweenDR(DiceRollerController diceRoller, Transform target, System.Action callback)
    {

        if (isTweening) return;
        isTweening = true;

        var drTransform = diceRoller.transform;

        var startPos = drTransform.position;
        var startRot = drTransform.rotation;

        System.Action<ITween<float>> onUpdate = (t) =>
        {
            drTransform.position = Vector3.Lerp(startPos, target.position, t.CurrentProgress);
            drTransform.rotation = Quaternion.Lerp(startRot, target.rotation, t.CurrentProgress);
        };

        System.Action<ITween<float>> onComplete = (t) =>
        {
            isTweening = false;
            callback();
        };

        DiceRoller_normal.gameObject.Tween(
            "diceRollerTween",
            0,
            1,
            tweenDuration,
            TweenScaleFunctions.QuadraticEaseOut,
            onUpdate,
            onComplete
            );
    }



    public DR_TYPE selectedDR;
    public DR_TYPE scanDR;
    public DR_TYPE distractDR;

    public enum DR_TYPE
    {
        NONE,
        NORMAL,
        BROKE
    }

    public void SetSelectedDR(DR_TYPE state)
    {
        selectedDR = state;
    }
    public void SetScanDR(DR_TYPE state)
    {
        scanDR = state;

        switch (state)
        {
            case DR_TYPE.NONE:
                Debug.Log("Set Scan pattern to: NONE");
                scanLight.material = mat_off;
                break;
            case DR_TYPE.NORMAL:
            case DR_TYPE.BROKE:
                Debug.Log("Set Scan pattern to: " + state.ToString());

                var dr = GetDR(state);

                Debug.Log("Current Dice face: " + dr.currentFace);

                scanLight.material = mat_on;
                break;
            default:
                break;
        }


    }
    public void SetDistractDR(DR_TYPE state)
    {
        distractDR = state;

        switch (state)
        {
            case DR_TYPE.NONE:
                Debug.Log("Set Distract pattern to: NONE");

                distractLight.material = mat_off;

                break;
            case DR_TYPE.NORMAL:
            case DR_TYPE.BROKE:
                Debug.Log("Set Distract pattern to: " + state.ToString());

                var dr = GetDR(state);

                Debug.Log("Current Dice face: " + dr.currentFace);


                distractLight.material = mat_on;
                break;
            default:
                break;
        }
    }

    public void OnRest1Click()
    {
        if (selectedDR == DR_TYPE.NONE && DiceRoller_normal.drState != DiceRollerController.DR_STATE.SET)
        {
            ToCam(DiceRoller_normal);
            SetSelectedDR(DR_TYPE.NORMAL);
        }
        else if (selectedDR != DR_TYPE.NONE)
        {
            ToRest();
            SetSelectedDR(DR_TYPE.NONE);
        }
    }

    public void OnRest2Click()
    {
        if (selectedDR == DR_TYPE.NONE && DiceRoller_broke.drState != DiceRollerController.DR_STATE.SET)
        {
            ToCam(DiceRoller_broke);
            SetSelectedDR(DR_TYPE.BROKE);
        }
        else if (selectedDR != DR_TYPE.NONE)
        {
            ToRest();
            SetSelectedDR(DR_TYPE.NONE);
        }
    }

    public void OnSelectedDRClick()
    {
        if (selectedDR == DR_TYPE.NONE) return;
        ToRest();
        SetSelectedDR(DR_TYPE.NONE);
    }

    public void OnSet1Click()
    {
        if (selectedDR == DR_TYPE.NONE && scanDR != DR_TYPE.NONE)
        {
            var dr = GetDR(scanDR);
            ToCam(dr);
            SetSelectedDR(dr.drType);
            SetScanDR(DR_TYPE.NONE);
        }
        else if (selectedDR != DR_TYPE.NONE && scanDR == DR_TYPE.NONE)
        {
            var dr = GetSelectedDR();
            ToSet1_Scan(dr);
            SetScanDR(dr.drType);
            SetSelectedDR(DR_TYPE.NONE);
        }

    }
    public void OnSet2Click()
    {
        if (selectedDR == DR_TYPE.NONE && distractDR != DR_TYPE.NONE)
        {
            var dr = GetDR(distractDR);
            ToCam(dr);
            SetSelectedDR(dr.drType);
            SetDistractDR(DR_TYPE.NONE);
        }
        else if (selectedDR != DR_TYPE.NONE && distractDR == DR_TYPE.NONE)
        {
            var dr = GetSelectedDR();
            ToSet2_Distract(dr);
            SetDistractDR(dr.drType);
            SetSelectedDR(DR_TYPE.NONE);
        }


    }



    public DiceRollerController GetSelectedDR()
    {
        return GetDR(selectedDR);
    }

    public DiceRollerController GetDR(DR_TYPE type)
    {
        switch (type)
        {
            case DR_TYPE.NORMAL:
                return DiceRoller_normal;
            case DR_TYPE.BROKE:
                return DiceRoller_broke;
            default:
                return null;

        }
    }



}

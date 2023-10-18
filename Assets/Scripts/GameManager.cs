using DigitalRuby.Tween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    public Transform handleTransform;
    public Transform handle_restTransform;
    public Transform handle_PressedTransform;

    public float tweenDuration = 0.3f;

    public MeshRenderer scanLight;
    public MeshRenderer distractLight;

    public Material mat_on;
    public Material mat_off;

    public float currentDepth = 250;
    public float descendSpeed = 24; //Conventional submarines have maximum submerged speeds of 16 to 24 knots (8.x-12.x m/s)
    public float currentSpeed = 0;

    public float speedToUnit = 0.1f;

    public bool descending = false;
    public bool released = false;
    public TextMeshPro depthText;
    public TextMeshPro speedText;

    bool isDRTweening = false;
    bool isDescendTweening = false;


    public bool bothScanFlag = false;
    // dice roller flash
    bool flashLightSwitch = false;
    bool isLit = false;
    float lightTime;
    float lightTime_interval;

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

    void UpdateText()
    {

        depthText.text = "Depth: " + currentDepth.ToString("f0") + "m";
        speedText.text = "Descend rate: " + currentSpeed.ToString("f0") + "m/s";
    }

    // Update is called once per frame
    void Update()
    {

        var dt = Time.deltaTime;

        currentDepth += currentSpeed * dt;

        if (!released)
        {
            if (descending)
                Descend();
            else
                Stop();
        }
        else
            currentSpeed += dt * 10;


        //flash

        if (flashLightSwitch)
        {
            lightTime += dt;
            lightTime_interval += dt;

            if (lightTime_interval >= 0.5f)
            {
                isLit = !isLit;
                if (scanDR == DR_TYPE.NONE) scanLight.material = isLit ? mat_on : mat_off;
                if (distractDR == DR_TYPE.NONE) distractLight.material = isLit ? mat_on : mat_off;
                lightTime_interval -= 0.5f;
            }

            if (lightTime >= 3)
            {
                flashLightSwitch = false;
                if (scanDR == DR_TYPE.NONE) scanLight.material = mat_off;
                if (distractDR == DR_TYPE.NONE) distractLight.material = mat_off;
            }
        }

        UpdateText();
    }


    //func
    public void ToggleDescendingStatus()
    {
        if (isDRTweening) return;

        if (!bothScanFlag)
        {
            StartDRTerminalFlash();
            return;
        }

        TweenHandle(!descending, () => { });
    }

    public void StartDescending()
    {
        if (isDRTweening) return;
        if (!bothScanFlag)
        {
            StartDRTerminalFlash();
            return;
        }

        TweenHandle(true, () => { });

    }

    public void StopDescending()
    {
        if (isDRTweening) return;
        if (!bothScanFlag)
        {
            StartDRTerminalFlash();
            return;
        }

        TweenHandle(false, () => { });
    }

    public void Descend()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, descendSpeed, Time.deltaTime);
    }
    public void Stop()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime);
    }


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

        if (isDRTweening) return;
        isDRTweening = true;

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
            isDRTweening = false;
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

    public void TweenHandle(bool descending, System.Action callback)
    {

        if (isDescendTweening) return;
        isDescendTweening = true;

        TimCameraController.instance.Shake(1f, 0.5f, 1f, 1f);

        //var startRot = descending ? handle_originXRot : handle_newXRot;
        var startRot = handleTransform.rotation;
        var endRot = descending ? handle_PressedTransform.rotation : handle_restTransform.rotation;


        System.Action<ITween<float>> onUpdate = (t) =>
        {
            handleTransform.rotation = Quaternion.Lerp(startRot, endRot, t.CurrentProgress);
        };

        System.Action<ITween<float>> onComplete = (t) =>
        {
            handleTransform.rotation = endRot;
            isDescendTweening = false;

            this.descending = descending;

            callback();
        };

        handleTransform.gameObject.Tween(
            "handleTween",
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
                //Debug.Log("Set Scan pattern to: NONE");
                scanLight.material = mat_off;
                break;
            case DR_TYPE.NORMAL:
            case DR_TYPE.BROKE:
                var dr = GetDR(state);
                //Debug.Log("Set Scan pattern to: " + state.ToString());
                //Debug.Log("Current Dice face: " + dr.currentFace);
                scanLight.material = mat_on;
                break;
            default:
                break;
        }
        UpdateScanStatus();
    }

    public void SetDistractDR(DR_TYPE state)
    {
        distractDR = state;

        switch (state)
        {
            case DR_TYPE.NONE:
                //Debug.Log("Set Distract pattern to: NONE");
                distractLight.material = mat_off;
                break;
            case DR_TYPE.NORMAL:
            case DR_TYPE.BROKE:
                var dr = GetDR(state);
                //Debug.Log("Set Distract pattern to: " + state.ToString());
                //Debug.Log("Current Dice face: " + dr.currentFace);
                distractLight.material = mat_on;
                break;
            default:
                break;
        }
        UpdateScanStatus();
    }

    public void UpdateScanStatus()
    {
        bothScanFlag = (distractDR != DR_TYPE.NONE && scanDR != DR_TYPE.NONE);

        if (bothScanFlag)
        {
            var scanDRController = GetDR(scanDR);
            radarController.SetScanPattern(scanDRController.currentFace, scanDRController.currentRotation == 1);
            var distractDRController = GetDR(distractDR);
            radarController.SetDistractPattern(distractDRController.currentFace, distractDRController.currentRotation == 1);
            radarController.StartPing();
        }
        else
        {
            radarController.StopPing();
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
        if (descending) return;
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
        if (descending) return;
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

    public void StartDRTerminalFlash()
    {
        flashLightSwitch = true;
        lightTime = 0;
        lightTime_interval = 0;
        isLit = true;

        if (scanDR == DR_TYPE.NONE) scanLight.material = mat_on;
        if (distractDR == DR_TYPE.NONE) distractLight.material = mat_on;
    }


}

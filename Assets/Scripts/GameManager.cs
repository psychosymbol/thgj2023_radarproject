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

    public int hullDurability = 3;

    public float currentDepth = 250;
    public float descendSpeed = 24; //Conventional submarines have maximum submerged speeds of 16 to 24 knots (8.x-12.x m/s)
    public float currentSpeed = 0;

    public float depthToUnit = 0.1f;

    public bool descending = false;
    public bool released = false;
    public TextMeshPro hullCondition;
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

    //docking
    bool dockingPreparing = false;
    float dockingStartDepth = 0;
    float dockingTime = 0;
    public float dockingDuration = 5;

    bool stationStop;
    float stationTime;
    float stationTime_interval;
    bool isFlashStationIcon = false;
    public float stationStopDuration = 10;
    public GameObject stationTransferIcon;

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

        var dockingRange = 100f;

        if (!dockingPreparing)
        {
            if (!released)
            {
                if (descending)
                {
                    Descend();
                }
                else
                {
                    Stop();
                }
                currentDepth += currentSpeed * dt;

                //check for docking
                if (nextStation != null && nextStation.depth - currentDepth < dockingRange)
                {
                    StartDocking();
                }

            }
            else
            {
                currentSpeed += dt * 10;
                currentDepth += currentSpeed * dt;
            }
        }
        else
        {
            dockingTime += dt;
            currentDepth = Mathf.Lerp(dockingStartDepth, nextStation.depth, dockingTime.Remap(0, dockingDuration, 0, 1));
            if (dockingTime >= dockingDuration)
            {
                FinishDocking();
            }

        }



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

        //flash
        if (stationStop)
        {
            stationTime += dt;
            stationTime_interval += dt;

            if (stationTime_interval >= 0.5f)
            {
                isFlashStationIcon = !isFlashStationIcon;
                stationTransferIcon.SetActive(isFlashStationIcon);
                stationTime_interval -= 0.5f;
            }

            if (stationTime >= stationStopDuration)
            {
                Repaired();
                DiceRoller_broke.GetComponent<DiceRollerController_Broke>().RestoreRandomCount();
                stationTransferIcon.SetActive(false);
                AudioManager.instance.PlaySound("sfx_finishdocking", AudioManager.Chanel.SFX_2);
                stationStop = false;
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))         //delete this
        {                                           //delete this
            Damaged();                              //delete this
            testDamage = false;                     //delete this
        }

        UpdateText();
    }

    public bool testDamage = false; //delete this
    public void Damaged()
    {
        hullDurability -= 1;
        string condition = "";
        switch (hullDurability)
        {
            case 3:
                condition = "<color=green>Hull Condition: Perfect</color>";
                AudioManager.instance.PlaySound("sfx_damaged1", AudioManager.Chanel.SFX_2);
                TimCameraController.instance.Shake(.2f, 1.5f, .5f, 1);
                break;
            case 2:
                condition = "<color=yellow>Hull Condition: Damaged</color>";
                AudioManager.instance.PlaySound("sfx_damaged2", AudioManager.Chanel.SFX_2);
                TimCameraController.instance.Shake(.2f, 3f, .5f, 1);
                break;
            case 1:
                condition = "<color=red>Hull Condition: Critical</color>";
                AudioManager.instance.PlaySound("sfx_damaged3", AudioManager.Chanel.SFX_2);
                TimCameraController.instance.Shake(.2f, 5f, .5f, 1);
                break;
            case 0:
            default:
                condition = "<color=red>Hull Condition: Lost</color>";
                AudioManager.instance.PlaySound("sfx_damaged3", AudioManager.Chanel.SFX_2);
                TimCameraController.instance.Shake(.2f, 2f, .5f, 1);
                break;
        }
        Flash.instance.doflash();
        hullCondition.text = condition;
    }
    public void Repaired()
    {
        hullDurability = 3;
        hullCondition.text = "<color=green>Hull Condition: Perfect</color>";
    }

    public void StartDocking()
    {
        Debug.Log("start docking");

        dockingPreparing = true;
        dockingTime = 0;
        dockingStartDepth = currentDepth;
        StopDescending();

        // calculate speed
        currentSpeed = (nextStation.depth - currentDepth) / dockingDuration;
    }
    public void FinishDocking()
    {
        Debug.Log("finished docking");

        AudioManager.instance.PlaySound("sfx_docking", AudioManager.Chanel.SFX_1);
        AudioManager.instance.StopSound(AudioManager.Chanel.ELEVATOR_LOOP); // loop channel
        TimCameraController.instance.Shake(1f, 0.5f, 1f, 1f);

        dockingPreparing = false;
        currentSpeed = 0;
        nextStation.dockingEnable = false;
        nextStation = null;

        stationStop = true;
        isFlashStationIcon = true;
        stationTime = 0;
        stationTime_interval = 0;
        stationTransferIcon.SetActive(true);
        radarController.StopPing();
    }

    public StationController nextStation = null;

    //func
    public void ToggleDescendingStatus()
    {
        if (dockingPreparing) return;
        if (stationStop) return;
        if (isDescendTweening) return;

        if (!bothScanFlag)
        {
            StartDRTerminalFlash();
            return;
        }

        var _descending = !descending;

        if (_descending)
        {
            AudioManager.instance.PlaySound("sfx_start", AudioManager.Chanel.SFX_1);
            AudioManager.instance.PlaySound("sfx_machineloop", AudioManager.Chanel.ELEVATOR_LOOP); // loop channel

            if (!radarController.isPinging) radarController.StartPing();

            //checking for next docking station

            var nextStation = StationManager.instance.GetNextDockingable();
            this.nextStation = nextStation;
        }
        else
        {
            AudioManager.instance.PlaySound("sfx_finishdocking", AudioManager.Chanel.SFX_1);
            AudioManager.instance.StopSound(AudioManager.Chanel.ELEVATOR_LOOP); // loop channel
        }

        TweenHandle(_descending, () => { });
    }

    // public void StartDescending()
    // {
    //     if (isDescendTweening) return;
    //     if (!bothScanFlag)
    //     {
    //         StartDRTerminalFlash();
    //         return;
    //     }
    // 
    //     TweenHandle(true, () => { });
    // 
    // }
    // 
    public void StopDescending()
    {
        AudioManager.instance.PlaySound("sfx_start", AudioManager.Chanel.SFX_1);
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
        if (dockingPreparing) return;
        if (stationStop) return;
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

            AudioManager.instance.PlaySound("sfx_click", AudioManager.Chanel.SFX_1);
        }

    }
    public void OnSet2Click()
    {
        if (dockingPreparing) return;
        if (stationStop) return;
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
            AudioManager.instance.PlaySound("sfx_click", AudioManager.Chanel.SFX_1);
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

        AudioManager.instance.PlaySound("sfx_incorrect", AudioManager.Chanel.SFX_1);

        flashLightSwitch = true;
        lightTime = 0;
        lightTime_interval = 0;
        isLit = true;

        if (scanDR == DR_TYPE.NONE) scanLight.material = mat_on;
        if (distractDR == DR_TYPE.NONE) distractLight.material = mat_on;
    }


}

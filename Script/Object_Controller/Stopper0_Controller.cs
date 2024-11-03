using System;
using UnityEditor.PackageManager;
using UnityEngine;

public class Stopper0_Controller : MonoBehaviour
{
    private SystemManager systemManager;

    public bool currentValue = false; // 읽어올 특정 값

    private float speed = 120f; // 실린더 이동 속도
    private float minZ = 527f; // 실린더가 이동할 수 있는 최소 X값
    private float maxZ = 552f; // 실린더가 이동할 수 있는 최대 X값




    public Camera secondCamera;



    void Start()
    {
        systemManager = FindObjectOfType<SystemManager>();

        if (systemManager == null)
        {
            Debug.LogError("SystemManager not found in the scene.");
        }


    }


    void Update()
    {

        switch (systemManager.CurrentMode)
        {
            case OperationMode.DigitalTwinMode:
                RunDigitalTwin();
                break;

            case OperationMode.SimulationMode:
                RunSimulation();
                break;

            case OperationMode.ManualMode:
                RunManualControl();
                break;
        }

    }


    void RunDigitalTwin()
    {
        controlstopper0(systemManager.ReadModbusCoil(1110, 1));
    }



    void RunSimulation()
    {
        if (currentValue)
        {
            controlstopper0(true);
            secondCamera.enabled = true;
        }
        else {
            controlstopper0(false);
            secondCamera.enabled = false;
        }
    }



    private void RunManualControl()
    {
        controlstopper0(currentValue);
        systemManager.WriteModbusCoil(215, currentValue);
    }

    private void controlstopper0(bool moveForward)
    {
        
        Vector3 position = transform.position;

        // 전진/후진 여부에 따라 X축 방향으로 이동
        if (moveForward)
        {
            position.z += speed * Time.deltaTime;
        }
        else
        {
            position.z -= speed * Time.deltaTime;
        }

        // 이동 가능 범위 제한
        position.z = Mathf.Clamp(position.z, minZ, maxZ);

        // 새로운 위치를 적용
        transform.position = position;
    }
}

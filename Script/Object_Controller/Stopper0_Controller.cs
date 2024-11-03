using System;
using UnityEditor.PackageManager;
using UnityEngine;

public class Stopper0_Controller : MonoBehaviour
{
    private SystemManager systemManager;

    public bool currentValue = false; // �о�� Ư�� ��

    private float speed = 120f; // �Ǹ��� �̵� �ӵ�
    private float minZ = 527f; // �Ǹ����� �̵��� �� �ִ� �ּ� X��
    private float maxZ = 552f; // �Ǹ����� �̵��� �� �ִ� �ִ� X��




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

        // ����/���� ���ο� ���� X�� �������� �̵�
        if (moveForward)
        {
            position.z += speed * Time.deltaTime;
        }
        else
        {
            position.z -= speed * Time.deltaTime;
        }

        // �̵� ���� ���� ����
        position.z = Mathf.Clamp(position.z, minZ, maxZ);

        // ���ο� ��ġ�� ����
        transform.position = position;
    }
}

using UnityEngine;
using EasyModbus;
using System;

public class SystemManager : MonoBehaviour
{
    public GameObject[] controlledObjects; // ������ ������Ʈ��

    private ModbusClient modbusClient;
    private float nextActionTime = 0.0f;
    public float period = 1.0f; // 1�� ����

    // ��� ���� ���
    private const int MODE_MANUAL = 0;
    private const int MODE_AUTO = 1;
    private const int MODE_SIMULATION = 2;
    public OperationMode CurrentMode { get; private set; }

    public int[] simulation_contains = new int[9];

    public GameObject uiPanel; // ������带 ���� UI�г�

    public bool ismoving= false;
    public int incomingitem = 0;

    void Start()
    {
        try
        {
            modbusClient = new ModbusClient("192.168.0.1", 502); // ���� IP�� ��Ʈ
            //modbusClient = new ModbusClient("127.0.0.1", 502); // ���� IP�� ��Ʈ

            modbusClient.Connect(); // Modbus ������ ����
            Debug.Log("Connected to Modbus server.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Could not connect to Modbus server: {ex.Message}");
        }
    }

    void Update()
    {
        if (Time.time >= nextActionTime)
        {
            nextActionTime += period;

            try
            {
                int[] holdingRegisters = modbusClient.ReadHoldingRegisters(1, 1);
                int modeValue = holdingRegisters[0];

                // ��忡 ���� ó��
                switch (modeValue)
                {
                    case MODE_MANUAL:
                        Debug.Log("Current Mode: Manual");
                        RunManualControl();
                        SwitchMode(OperationMode.ManualMode);
                        break;
                    case MODE_AUTO:
                        Debug.Log("Current Mode: Auto");
                        RunDigitalTwin();
                        SwitchMode(OperationMode.DigitalTwinMode);

                        break;
                    case MODE_SIMULATION:
                        Debug.Log("Current Mode: Simulation");
                        SwitchMode(OperationMode.SimulationMode);
                        RunSimulation();
                        break;
                    default:
                        Debug.Log("Unknown Mode: " + modeValue);
                        break;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error reading Modbus register: {ex.Message}");
            }
        }
    }

    public void SwitchMode(OperationMode newMode)
    {
        CurrentMode = newMode;
    }

     void RunDigitalTwin()
    {
        uiPanel.SetActive(false);
    }
    void RunSimulation()
    {

        uiPanel.SetActive(false);
    }

    void RunManualControl()
    {
        uiPanel.SetActive(true);
        Debug.Log("�̰� ����Ȱ� �³�?");
    }



    void OnApplicationQuit()
    {
        if (modbusClient != null && modbusClient.Connected)
        {
            modbusClient.Disconnect();
            Debug.Log("Disconnected from Modbus server.");
        }
    }

    public bool ReadModbusCoil(int startingAddress, int quantity)
    {
        try
        {
            return modbusClient.ReadCoils(startingAddress, quantity)[0];
            
        }
        catch(System.Exception ex)
        {
            Debug.LogError($"Error reading Modbus Coils: {ex.Message}");
            return false;
        }
    }

    public bool WriteModbusCoil(int startingAddress, bool value)
    {
        try
        {
            modbusClient.WriteSingleCoil(startingAddress, value);
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error reading Modbus Coil: {ex.Message}");
            return false;
        }
    }

    public int ReadModbusRegister(int startingAddress, int quantity)
    {
        try
        {
            return modbusClient.ReadHoldingRegisters(startingAddress, quantity)[0];
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error reading Modbus registers: {ex.Message}");
            return -1;
        }
    }

    public bool WriteModbusRegisters(int startingAddress, int values)
    {
        try
        {
            modbusClient.WriteSingleRegister(startingAddress, values);
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error writing to Modbus registers: {ex.Message}");
            return false;
        }
    }
}

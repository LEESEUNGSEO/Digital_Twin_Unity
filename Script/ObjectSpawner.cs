using System;
using UnityEngine;
using static UnityEditor.Progress;

public class ObjectSpawner : MonoBehaviour
{

    private SystemManager systemManager;
    public GameObject prefabToSpawnPepper; // ������ �������� ���� ����
    public GameObject prefabToSpawnRadish; // ������ �������� ���� ����
    public GameObject prefabToSpawnKoreanCabbage; // ������ �������� ���� ����
    public GameObject prefabToSpawnGarlic; // ������ �������� ���� ����
    public GameObject prefabToSpawnOnion; // ������ �������� ���� ����

    public Vector3 spawnPosition;    // ������ ��ġ�� ���� ���� (Inspector���� ���� ����)

    public float spawnInterval = 2.0f; // ������ ���� ���� (��)
    private float lastSpawnTime = 0f;   // ������ ������ ���� �ð�

    private int outgoingitem;


    private void Start()
    {
        systemManager = FindObjectOfType<SystemManager>();

        if (systemManager == null)
        {
            Debug.LogError("SystemManager not found in the scene.");
        }
        systemManager = FindObjectOfType<SystemManager>();

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
        // ���� �ð��� ������ ���� �ð� ���Ͽ� ���� ������ Ȯ��
        if (Time.time - lastSpawnTime >= spawnInterval && systemManager.ReadModbusRegister(4, 1) == 1)
        {
            // �ߺ� ���� ���� ���� Modbus �������� ���� 0���� ����
            systemManager.WriteModbusRegisters(4, 0);

            if (systemManager.ReadModbusRegister(5, 1) == 1)
            {
                A_SpawnPrefab(prefabToSpawnRadish);
            }
            else if (systemManager.ReadModbusRegister(5, 1) == 2)
            {
                A_SpawnPrefab(prefabToSpawnKoreanCabbage);
            }
            else if (systemManager.ReadModbusRegister(5, 1) == 3)
            {
                A_SpawnPrefab(prefabToSpawnGarlic);
            }
            else if (systemManager.ReadModbusRegister(5, 1) == 4)
            {
                A_SpawnPrefab(prefabToSpawnOnion);
            }
            else if (systemManager.ReadModbusRegister(5, 1) == 5)
            {
                A_SpawnPrefab(prefabToSpawnPepper);
            }

            // �������� �����Ǿ����� ǥ���ϱ� ���� �ð��� ����
            lastSpawnTime = Time.time;


        }
    }

    void RunSimulation()
    {
        if (systemManager.ReadModbusRegister(6, 1) == 1)
        {
            SpawnPrefab(prefabToSpawnRadish); // ������ ���� �޼ҵ� ȣ��
        }
        else if((systemManager.ReadModbusRegister(6, 1) == 2))
        {
            SpawnPrefab(prefabToSpawnKoreanCabbage); // ������ ���� �޼ҵ� ȣ��
        }
        else if ((systemManager.ReadModbusRegister(6, 1) == 3))
        {
            SpawnPrefab(prefabToSpawnGarlic); // ������ ���� �޼ҵ� ȣ��
        }
        else if ((systemManager.ReadModbusRegister(6, 1) == 4))
        {
            SpawnPrefab(prefabToSpawnOnion); // ������ ���� �޼ҵ� ȣ��
        }
        else if ((systemManager.ReadModbusRegister(6, 1) == 5))
        {
            SpawnPrefab(prefabToSpawnPepper); // ������ ���� �޼ҵ� ȣ��
        }

        int finditem = FindOutgoingItem();
        if (finditem >= 0)
        {
            GameObject itemToShip = GameObject.Find($"box_{finditem}");

           
            Basket_Controller itemScript = itemToShip.GetComponent<Basket_Controller>();

                // Basket_Controller ������Ʈ�� �ִ��� Ȯ��
            if (itemScript != null)
            {
                // ���⼭ �ڷ�ƾ �Ǵ� ���� ����
                StartCoroutine(itemScript.OutgoingSequence());
                systemManager.WriteModbusRegisters(7, 0);
            }
            else
            {
                Debug.LogError("Basket_Controller ������Ʈ�� ã�� �� �����ϴ�.");
            }
           
        }
    }
    
    void RunManualControl()
    {
        // �޴��� ��忡���� ��ü���� X
    }

 
    void SpawnPrefab(GameObject prefabObject)
    {
        if (prefabObject != null) // �������� �Ҵ�Ǿ� �ִ��� Ȯ��
        {
            Instantiate(prefabObject, spawnPosition, Quaternion.identity); // ������ ����
            systemManager.incomingitem = systemManager.ReadModbusRegister(6,1);
            systemManager.WriteModbusRegisters(6, 0);
        }
        else
        {
            Debug.LogWarning("Prefab is not assigned in the Inspector"); // ��� �޽���
        }
    }

    void A_SpawnPrefab(GameObject prefabObject)
    {
        if (prefabObject != null) // �������� �Ҵ�Ǿ� �ִ��� Ȯ��
        {
            Instantiate(prefabObject, spawnPosition, Quaternion.identity); // ������ ����
            systemManager.WriteModbusCoil(104, true);
        }
        else
        {
            Debug.LogWarning("Prefab is not assigned in the Inspector"); // ��� �޽���
        }
    }


    private int FindOutgoingItem()
    {
        outgoingitem = systemManager.ReadModbusRegister(7, 1);

        Debug.Log($"�Ƕ�Ƕ�::{outgoingitem}");
        if (outgoingitem != 0)
        {
            for (int i = 0; i < systemManager.simulation_contains.Length; i++)
            {
                if (systemManager.simulation_contains[i] == outgoingitem)
                {
                    systemManager.simulation_contains[i] = 0;
                    return i;
                }
                if (i == 8)
                {
                    Debug.LogError("�ش� ��ǰ ����");
                    systemManager.WriteModbusRegisters(7, 0);
                    return -1;
                }
            }
        }
          
        return -1;

    }
}

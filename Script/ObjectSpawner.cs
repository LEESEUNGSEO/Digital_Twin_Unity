using System;
using UnityEngine;
using static UnityEditor.Progress;

public class ObjectSpawner : MonoBehaviour
{

    private SystemManager systemManager;
    public GameObject prefabToSpawnPepper; // 생성할 프리팹을 위한 변수
    public GameObject prefabToSpawnRadish; // 생성할 프리팹을 위한 변수
    public GameObject prefabToSpawnKoreanCabbage; // 생성할 프리팹을 위한 변수
    public GameObject prefabToSpawnGarlic; // 생성할 프리팹을 위한 변수
    public GameObject prefabToSpawnOnion; // 생성할 프리팹을 위한 변수

    public Vector3 spawnPosition;    // 생성할 위치를 위한 변수 (Inspector에서 설정 가능)

    public float spawnInterval = 2.0f; // 프리팹 생성 간격 (초)
    private float lastSpawnTime = 0f;   // 마지막 프리팹 생성 시간

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
        // 현재 시간과 마지막 생성 시간 비교하여 생성 간격을 확인
        if (Time.time - lastSpawnTime >= spawnInterval && systemManager.ReadModbusRegister(4, 1) == 1)
        {
            // 중복 생성 방지 위한 Modbus 레지스터 값을 0으로 변경
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

            // 프리팹이 생성되었음을 표시하기 위해 시간을 갱신
            lastSpawnTime = Time.time;


        }
    }

    void RunSimulation()
    {
        if (systemManager.ReadModbusRegister(6, 1) == 1)
        {
            SpawnPrefab(prefabToSpawnRadish); // 프리팹 생성 메소드 호출
        }
        else if((systemManager.ReadModbusRegister(6, 1) == 2))
        {
            SpawnPrefab(prefabToSpawnKoreanCabbage); // 프리팹 생성 메소드 호출
        }
        else if ((systemManager.ReadModbusRegister(6, 1) == 3))
        {
            SpawnPrefab(prefabToSpawnGarlic); // 프리팹 생성 메소드 호출
        }
        else if ((systemManager.ReadModbusRegister(6, 1) == 4))
        {
            SpawnPrefab(prefabToSpawnOnion); // 프리팹 생성 메소드 호출
        }
        else if ((systemManager.ReadModbusRegister(6, 1) == 5))
        {
            SpawnPrefab(prefabToSpawnPepper); // 프리팹 생성 메소드 호출
        }

        int finditem = FindOutgoingItem();
        if (finditem >= 0)
        {
            GameObject itemToShip = GameObject.Find($"box_{finditem}");

           
            Basket_Controller itemScript = itemToShip.GetComponent<Basket_Controller>();

                // Basket_Controller 컴포넌트가 있는지 확인
            if (itemScript != null)
            {
                // 여기서 코루틴 또는 로직 실행
                StartCoroutine(itemScript.OutgoingSequence());
                systemManager.WriteModbusRegisters(7, 0);
            }
            else
            {
                Debug.LogError("Basket_Controller 컴포넌트를 찾을 수 없습니다.");
            }
           
        }
    }
    
    void RunManualControl()
    {
        // 메뉴얼 모드에서는 객체생성 X
    }

 
    void SpawnPrefab(GameObject prefabObject)
    {
        if (prefabObject != null) // 프리팹이 할당되어 있는지 확인
        {
            Instantiate(prefabObject, spawnPosition, Quaternion.identity); // 프리팹 생성
            systemManager.incomingitem = systemManager.ReadModbusRegister(6,1);
            systemManager.WriteModbusRegisters(6, 0);
        }
        else
        {
            Debug.LogWarning("Prefab is not assigned in the Inspector"); // 경고 메시지
        }
    }

    void A_SpawnPrefab(GameObject prefabObject)
    {
        if (prefabObject != null) // 프리팹이 할당되어 있는지 확인
        {
            Instantiate(prefabObject, spawnPosition, Quaternion.identity); // 프리팹 생성
            systemManager.WriteModbusCoil(104, true);
        }
        else
        {
            Debug.LogWarning("Prefab is not assigned in the Inspector"); // 경고 메시지
        }
    }


    private int FindOutgoingItem()
    {
        outgoingitem = systemManager.ReadModbusRegister(7, 1);

        Debug.Log($"되라되라::{outgoingitem}");
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
                    Debug.LogError("해당 제품 없음");
                    systemManager.WriteModbusRegisters(7, 0);
                    return -1;
                }
            }
        }
          
        return -1;

    }
}

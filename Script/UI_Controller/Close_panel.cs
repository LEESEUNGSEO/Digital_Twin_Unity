using UnityEngine;
using UnityEngine.UI;

public class Close_panel : MonoBehaviour
{
    // ��Ȱ��ȭ�ϰ� ���� �г��� public���� �����Ͽ� Unity Editor���� �巡���Ͽ� ������ �� �ְ� ��
    public GameObject panelToDisable;
    private GameObject mainCamera;

    private void Start()
    {
        // ���� ���� mainCamera�� ���� �Ҵ�
        mainCamera = GameObject.FindWithTag("MainCamera");

        // mainCamera�� ����� �Ҵ���� �ʾ��� ��츦 ����� ���� üũ
        if (mainCamera == null)
        {
            Debug.LogError("MainCamera ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    // �� �Լ��� ��ư�� OnClick �̺�Ʈ�� �����
    public void OnButtonClicked()
    {
        Transform grandparentObject = transform.parent.parent;

        // �г��� ��Ȱ��ȭ�ϴ� �ڵ�
        panelToDisable.SetActive(false);
        Debug.Log($"{grandparentObject}�̰� ������");

        // �θ� ������Ʈ�� ��Ȱ��ȭ
        mainCamera.GetComponent<FreeCameraController>().targetObject = null;
        foreach (Transform child in grandparentObject.transform)
        {
            child.gameObject.SetActive(false);
        }


    }
}

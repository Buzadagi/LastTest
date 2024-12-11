using UnityEngine;
using TMPro;

public class DisplayLocationName : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI locationNameText; // �ν����Ϳ��� ������ TextMeshPro UI

    void Start()
    {
        // PlayerPrefs���� locationName�� �޾ƿ���, ���� ������ "Nothing"�� ���
        string locationName = PlayerPrefs.GetString("CurrentLocationName", "Nothing");
        locationNameText.text = locationName;
    }
}

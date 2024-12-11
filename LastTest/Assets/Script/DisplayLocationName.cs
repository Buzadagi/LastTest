using UnityEngine;
using TMPro;

public class DisplayLocationName : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI locationNameText; // 인스펙터에서 지정한 TextMeshPro UI

    void Start()
    {
        // PlayerPrefs에서 locationName을 받아오되, 값이 없으면 "Nothing"을 출력
        string locationName = PlayerPrefs.GetString("CurrentLocationName", "Nothing");
        locationNameText.text = locationName;
    }
}

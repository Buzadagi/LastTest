using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI speakerText;     // 화자 이름
    public TextMeshProUGUI dialogText;      // 서술문 텍스트
    public TextMeshProUGUI eventNameText;   // 이벤트 이름
    public Image backgroundImage;          // 배경 이미지

    [Header("Settings")]
    public int eventNumber = 1;             // 인스펙터에서 이벤트 번호 설정
    public string csvFileName = "DialogData.csv";  // CSV 파일명

    private List<DialogLine> dialogLines = new List<DialogLine>(); // 대사 목록
    private int currentIndex = 0;           // 현재 출력 중인 대사 인덱스

    private void Start()
    {
        LoadDialogData();
        ShowNextDialog(); // 첫 번째 대사 출력
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ShowNextDialog();
        }
    }

    private void LoadDialogData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, csvFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError("CSV 파일을 찾을 수 없습니다: " + filePath);
            return;
        }

        string[] lines = File.ReadAllLines(filePath);

        // 첫 줄은 헤더이므로 건너뛴다.
        for (int i = 1; i < lines.Length; i++)
        {
            string[] data = lines[i].Split(',');

            if (data.Length < 5) continue;

            if (int.TryParse(data[0], out int num) && num == eventNumber)
            {
                dialogLines.Add(new DialogLine
                {
                    Num = num,
                    EventName = data[1],
                    NPCName = data[2],
                    Contents = data[3],
                    ImageName = data[4]
                });
            }
        }
    }

    private void ShowNextDialog()
    {
        if (currentIndex < dialogLines.Count)
        {
            // 대사 업데이트
            var currentLine = dialogLines[currentIndex];
            eventNameText.text = currentLine.EventName;
            speakerText.text = currentLine.NPCName;
            dialogText.text = currentLine.Contents;

            // 배경 이미지 업데이트
            Sprite newSprite = Resources.Load<Sprite>($"Background/{currentLine.ImageName}");
            if (newSprite != null)
            {
                backgroundImage.sprite = newSprite;
            }
            else
            {
                Debug.LogWarning($"이미지를 찾을 수 없습니다: {currentLine.ImageName}");
            }

            currentIndex++;
        }
        else
        {
            // 대사가 끝났을 때
            Debug.Log("종료되었습니다");
        }
    }

    // 대사 데이터를 담는 클래스
    private class DialogLine
    {
        public int Num;
        public string EventName;
        public string NPCName;
        public string Contents;
        public string ImageName;
    }
}

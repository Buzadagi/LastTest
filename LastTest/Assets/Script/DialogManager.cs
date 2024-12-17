using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class DialogManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI speakerText;     // 화자 이름
    public TextMeshProUGUI dialogText;      // 서술문 텍스트
    public TextMeshProUGUI eventNameText;   // 이벤트 이름
    public Image backgroundImage;          // 배경 이미지

    [Header("Settings")]
    public string csvFileName = "Ending.csv";  // 엔딩 CSV 파일명

    private List<DialogLine> dialogLines = new List<DialogLine>(); // 대사 목록
    private int currentIndex = 0;           // 현재 출력 중인 대사 인덱스

    private void Start()
    {
        if (!PlayerPrefs.HasKey("IsInitialized"))
        {
            PlayerPrefs.SetString("CSVFileName", "Opening.csv");
            PlayerPrefs.SetInt("EndingNumber", 0);
            PlayerPrefs.SetInt("IsInitialized", 1);
            PlayerPrefs.Save();
        }

        csvFileName = PlayerPrefs.GetString("CSVFileName", "Opening.csv");
        int endingNumber = PlayerPrefs.GetInt("EndingNumber", 0);

        if (endingNumber == 0 && csvFileName.Equals("Opening.csv", System.StringComparison.OrdinalIgnoreCase))
        {
            LoadDialogData("Opening");
            ShowNextDialog();
        }
        else
        {
            string eventName = GetEventNameByEndingNumber(endingNumber);
            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogError($"유효한 EventName이 없습니다: {endingNumber}");
                return;
            }

            LoadDialogData(eventName);
            ShowNextDialog();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ShowNextDialog();
        }
    }

    private string GetEventNameByEndingNumber(int endingNumber)
    {
        return endingNumber switch
        {
            1 => "Happy Ending",
            2 => "Normal Ending",
            3 => "Sad Ending",
            4 => "NPC Ending", 
           _ => null,
        };
    }

    private void LoadDialogData(string eventName)
    {
        dialogLines.Clear();
        string filePath = Path.Combine(Application.streamingAssetsPath, csvFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"CSV 파일을 찾을 수 없습니다: {filePath}");
            return;
        }

        using (StreamReader reader = new StreamReader(filePath))
        {
            string headerLine = reader.ReadLine();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] data = ParseCsvLine(line);

                if (data.Length < 5)
                {
                    Debug.LogWarning($"잘못된 데이터 형식: {line}");
                    continue;
                }

                if (data[1].Trim() == eventName.Trim())
                {
                    dialogLines.Add(new DialogLine
                    {
                        Num = int.Parse(data[0]),
                        EventName = data[1],
                        NPCName = data[2],
                        Contents = data[3],
                        ImageName = data[4].Trim()
                    });
                }
            }
        }

        if (dialogLines.Count == 0)
        {
            Debug.LogWarning("매칭된 대사가 없습니다.");
        }
    }

    private string[] ParseCsvLine(string line)
    {
        // Updated pattern to handle escaped quotes and commas within quotes
        string pattern = @"\""(?:[^\""]|\"")*\""|[^,]+";
        MatchCollection matches = Regex.Matches(line, pattern);
        string[] result = new string[matches.Count];

        for (int i = 0; i < matches.Count; i++)
        {
            string value = matches[i].Value;

            // Remove surrounding quotes and unescape inner quotes
            if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                value = value.Substring(1, value.Length - 2).Replace("\\\"", "\"");
            }

            result[i] = value;
        }

        return result;
    }


    private void ShowNextDialog()
    {
        if (currentIndex < dialogLines.Count)
        {
            var currentLine = dialogLines[currentIndex];
            eventNameText.text = currentLine.EventName;
            speakerText.text = currentLine.NPCName;
            dialogText.text = currentLine.Contents;

            string cleanedImageName = Path.GetFileNameWithoutExtension(currentLine.ImageName);
            string imagePath = $"Background/{cleanedImageName}";

            Sprite newSprite = Resources.Load<Sprite>(imagePath);
            if (newSprite != null)
            {
                backgroundImage.sprite = newSprite;
            }
            else
            {
                Debug.LogWarning($"이미지를 찾을 수 없습니다: {imagePath}");
            }

            currentIndex++;
        }
        else
        {
            if (csvFileName.Equals("Opening.csv", System.StringComparison.OrdinalIgnoreCase))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("WorldMap");
            }
            else if (csvFileName.Equals("Ending.csv", System.StringComparison.OrdinalIgnoreCase))
            {
                // PlayerPrefs 초기화
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();  // 변경 사항 저장

                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
        }
    }

    private class DialogLine
    {
        public int Num;
        public string EventName;
        public string NPCName;
        public string Contents;
        public string ImageName;
    }
}

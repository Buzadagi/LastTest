using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI speakerText;     // ȭ�� �̸�
    public TextMeshProUGUI dialogText;      // ������ �ؽ�Ʈ
    public TextMeshProUGUI eventNameText;   // �̺�Ʈ �̸�
    public Image backgroundImage;          // ��� �̹���

    [Header("Settings")]
    public int eventNumber = 1;             // �ν����Ϳ��� �̺�Ʈ ��ȣ ����
    public string csvFileName = "DialogData.csv";  // CSV ���ϸ�

    private List<DialogLine> dialogLines = new List<DialogLine>(); // ��� ���
    private int currentIndex = 0;           // ���� ��� ���� ��� �ε���

    private void Start()
    {
        LoadDialogData();
        ShowNextDialog(); // ù ��° ��� ���
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
            Debug.LogError("CSV ������ ã�� �� �����ϴ�: " + filePath);
            return;
        }

        string[] lines = File.ReadAllLines(filePath);

        // ù ���� ����̹Ƿ� �ǳʶڴ�.
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
            // ��� ������Ʈ
            var currentLine = dialogLines[currentIndex];
            eventNameText.text = currentLine.EventName;
            speakerText.text = currentLine.NPCName;
            dialogText.text = currentLine.Contents;

            // ��� �̹��� ������Ʈ
            Sprite newSprite = Resources.Load<Sprite>($"Background/{currentLine.ImageName}");
            if (newSprite != null)
            {
                backgroundImage.sprite = newSprite;
            }
            else
            {
                Debug.LogWarning($"�̹����� ã�� �� �����ϴ�: {currentLine.ImageName}");
            }

            currentIndex++;
        }
        else
        {
            // ��簡 ������ ��
            Debug.Log("����Ǿ����ϴ�");
        }
    }

    // ��� �����͸� ��� Ŭ����
    private class DialogLine
    {
        public int Num;
        public string EventName;
        public string NPCName;
        public string Contents;
        public string ImageName;
    }
}

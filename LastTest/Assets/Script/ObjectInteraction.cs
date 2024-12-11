using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class ObjectInteraction : MonoBehaviour
{
    public event Action OnInteractionEnd; // ��ȣ�ۿ� ���� �̺�Ʈ

    public enum InteractionType { Item, NPC }
    public InteractionType interactionType;

    public string objectName; // �˻��� ������Ʈ �̸�
    private List<InteractionData> interactionDataList = new List<InteractionData>();

    public GameObject textBoxImage;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    public Image toolImage;
    public Image npcImage;

    public Canvas interactionCanvas;
    private int currentIndex;
    private bool isInteractionActive = false;

    private void Awake()
    {
        if (interactionCanvas == null)
        {
            Debug.Log($"[{gameObject.name}] Attempting to find Interaction Canvas...");
            interactionCanvas = GetComponentInChildren<Canvas>(true);
        }

        if (interactionCanvas == null)
        {
            Debug.LogError($"[{gameObject.name}] Interaction Canvas not found. Please assign it in the Inspector or ensure it exists as a child object.");
        }
        else
        {
            Debug.Log($"[{gameObject.name}] Interaction Canvas found: {interactionCanvas.name}");
        }
    }

    void Start()
    {
        if (interactionCanvas == null)
        {
            Debug.LogError($"[{gameObject.name}] Interaction Canvas is still null after Awake. Cannot proceed.");
            return;
        }

        interactionCanvas.gameObject.SetActive(false);

        string csvPath = GetCSVPath();
        if (!string.IsNullOrEmpty(csvPath))
        {
            StartCoroutine(LoadCSVData(csvPath));
        }

        currentIndex = 0;
    }

    public void Interact()
    {
        if (interactionDataList.Count == 0)
        {
            Debug.LogWarning($"No interaction data found for object {objectName}");
            EndInteraction();
            return;
        }

        if (!isInteractionActive)
        {
            StartInteraction();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            ShowNextDialogue();
        }
    }

    private void StartInteraction()
    {
        isInteractionActive = true;

        // ��ȭ ���� �� textBoxImage Ȱ��ȭ
        textBoxImage.SetActive(true);
        ShowDialogue(currentIndex);
        interactionCanvas.gameObject.SetActive(true);
    }

    private void ShowNextDialogue()
    {
        currentIndex++;

        if (currentIndex >= interactionDataList.Count)
        {
            EndInteraction();
            return;
        }

        ShowDialogue(currentIndex);
    }

    private void ShowDialogue(int index)
    {
        InteractionData data = interactionDataList[index];
        dialogueText.text = data.Dialogue;

        if (interactionType == InteractionType.Item)
        {
            speakerText.gameObject.SetActive(false);
            npcImage.gameObject.SetActive(false);
            toolImage.gameObject.SetActive(true);
            toolImage.sprite = LoadSprite("Item", data.ImageName);
        }
        else if (interactionType == InteractionType.NPC)
        {
            speakerText.gameObject.SetActive(true);
            npcImage.gameObject.SetActive(true);
            toolImage.gameObject.SetActive(false);

            speakerText.text = data.Speaker;
            npcImage.sprite = LoadSprite("NPC", data.ImageName);
        }
    }

    private string GetCSVPath()
    {
        string fileName = interactionType == InteractionType.Item ? "Item.csv" : "NPC.csv";
        return Path.Combine(Application.streamingAssetsPath, fileName);
    }

    private IEnumerator LoadCSVData(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"CSV file not found: {path}");
            yield break;
        }

        string csvText = File.ReadAllText(path);
        ParseCSVData(csvText);
    }

    private void ParseCSVData(string csvText)
    {
        // ����Ʈ ����ǥ�� �Ϲ� ����ǥ�� ��ȯ
        csvText = csvText.Replace('��', '\'').Replace('��', '\'').Replace('��', '"').Replace('��', '"');

        string[] lines = csvText.Split('\n');

        for (int i = 1; i < lines.Length; i++) // ù ��° ���� ���
        {
            string line = lines[i];

            // ���Խ��� ����� CSV �����͸� �ùٸ��� �и�
            string[] fields = ParseCsvLine(line);

            if (fields.Length < 5) // �ּ� 5�� �̻��� �ʵ尡 �ʿ�
            {
                Debug.LogWarning($"Skipping line {i}: {lines[i]}");
                continue;
            }

            string csvObjectName = "";

            // ObjectName ��ġ�� CSV ���Ͽ� ���� �ٸ��� ó��
            if (interactionType == InteractionType.Item)
            {
                csvObjectName = fields[4].Trim(); // Item.csv������ ObjectName�� 5��° �� (0���� �����ϸ� 4��°)
            }
            else if (interactionType == InteractionType.NPC)
            {
                csvObjectName = fields[5].Trim(); // NPC.csv������ ObjectName�� 6��° �� (0���� �����ϸ� 5��°)
            }

            if (csvObjectName.Trim() != objectName.Trim()) continue; // Object Name �� �� ���� ����

            if (interactionType == InteractionType.NPC)
            {
                string speaker = fields[2];
                string dialogue = fields[3];
                string imageName = fields[4].Trim(); // ImageName�� 5��° �� (0���� �����ϸ� 4��°)
                interactionDataList.Add(new InteractionData(speaker, dialogue, imageName));
            }
            else // Item
            {
                string dialogue = fields[2];
                string imageName = fields[3].Trim(); // ImageName�� 4��° �� (0���� �����ϸ� 3��°)
                interactionDataList.Add(new InteractionData(null, dialogue, imageName));
            }
        }
    }



    // CSV ���� ���Խ����� �Ľ�
    private string[] ParseCsvLine(string line)
    {
        string pattern = @"(?:^|,)(?:""(?<value>[^""]*)""|(?<value>[^,""]*))";
        MatchCollection matches = Regex.Matches(line, pattern);
        string[] result = new string[matches.Count];

        for (int i = 0; i < matches.Count; i++)
        {
            result[i] = matches[i].Groups["value"].Value;
        }

        return result;
    }

    private Sprite LoadSprite(string folder, string imageName)
    {
        string imagePath = $"{folder}/{Path.GetFileNameWithoutExtension(imageName)}";
        Sprite sprite = Resources.Load<Sprite>(imagePath);

        if (sprite == null)
        {
            Debug.LogWarning($"Sprite not found at path: {imagePath}. Ensure the file is in the Resources folder and correctly named.");
        }
        return sprite;
    }

    private void EndInteraction()
    {
        isInteractionActive = false;

        // ��ȣ�ۿ� ĵ���� �� ��ȭ �ڽ� ��Ȱ��ȭ
        interactionCanvas.gameObject.SetActive(false);
        textBoxImage.SetActive(false);

        // ��ȭ �ε��� �ʱ�ȭ
        currentIndex = 0;

        // ��ȣ�ۿ� ���� �̺�Ʈ ȣ��
        OnInteractionEnd?.Invoke();
    }

    private class InteractionData
    {
        public string Speaker { get; }
        public string Dialogue { get; }
        public string ImageName { get; }

        public InteractionData(string speaker, string dialogue, string imageName)
        {
            Speaker = speaker;
            Dialogue = dialogue;
            ImageName = imageName;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class AnswerBoxController : MonoBehaviour
{
    public Color hoverColor = Color.green;
    public Color normalColor = Color.white;
    public GameObject hoverTextPrefab; // '정답' 텍스트 프리팹
    public Canvas uiCanvas; // UI 캔버스
    private GameObject hoverTextInstance;
    private Renderer boxRenderer;

    private void Start()
    {
        boxRenderer = GetComponent<Renderer>();
        boxRenderer.material.color = normalColor;
    }

    private void OnMouseEnter()
    {
        boxRenderer.material.color = hoverColor;
        CreateHoverText();
    }

    private void OnMouseExit()
    {
        boxRenderer.material.color = normalColor;
        DestroyHoverText();
    }

    private void OnMouseOver()
    {
        if (hoverTextInstance != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            hoverTextInstance.transform.position = mousePosition + new Vector3(50, -20, 0);
        }

        if (Input.GetMouseButtonDown(0))
        {
            AnswerManager.Instance.ShowAnswerUI();
        }
    }

    private void CreateHoverText()
    {
        hoverTextInstance = Instantiate(hoverTextPrefab, uiCanvas.transform);
    }

    private void DestroyHoverText()
    {
        if (hoverTextInstance != null)
        {
            Destroy(hoverTextInstance);
        }
    }
}

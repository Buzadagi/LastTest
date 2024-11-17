using UnityEngine;

public class HighlightOnHover : MonoBehaviour
{
    public Material outlineMaterial; // 외곽선 표시를 위한 Material
    public Material defaultMaterial; // 원래의 Material

    private Renderer objectRenderer;

    private void Start()
    {
        objectRenderer = GetComponent<Renderer>();

        // 기본 Material이 없으면 현재 Material을 저장
        if (defaultMaterial == null && objectRenderer != null)
        {
            defaultMaterial = objectRenderer.material;
        }
    }

    private void OnMouseEnter()
    {
        if (CompareTag("Struct")) // Tag가 Struct인지 확인
        {
            ApplyOutline(true); // 외곽선 적용
        }
    }

    private void OnMouseExit()
    {
        if (CompareTag("Struct")) // Tag가 Struct인지 확인
        {
            ApplyOutline(false); // 외곽선 제거
        }
    }

    private void ApplyOutline(bool apply)
    {
        if (objectRenderer != null)
        {
            Color highlightColor = Color.yellow; // 외곽선 색상 (노란색)
            objectRenderer.material.color = apply ? highlightColor : Color.white; // 기본 색상은 흰색
        }
    }

}

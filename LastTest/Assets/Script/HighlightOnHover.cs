using UnityEngine;

public class HighlightOnHover : MonoBehaviour
{
    public Material outlineMaterial; // �ܰ��� ǥ�ø� ���� Material
    public Material defaultMaterial; // ������ Material

    private Renderer objectRenderer;

    private void Start()
    {
        objectRenderer = GetComponent<Renderer>();

        // �⺻ Material�� ������ ���� Material�� ����
        if (defaultMaterial == null && objectRenderer != null)
        {
            defaultMaterial = objectRenderer.material;
        }
    }

    private void OnMouseEnter()
    {
        if (CompareTag("Struct")) // Tag�� Struct���� Ȯ��
        {
            ApplyOutline(true); // �ܰ��� ����
        }
    }

    private void OnMouseExit()
    {
        if (CompareTag("Struct")) // Tag�� Struct���� Ȯ��
        {
            ApplyOutline(false); // �ܰ��� ����
        }
    }

    private void ApplyOutline(bool apply)
    {
        if (objectRenderer != null)
        {
            Color highlightColor = Color.yellow; // �ܰ��� ���� (�����)
            objectRenderer.material.color = apply ? highlightColor : Color.white; // �⺻ ������ ���
        }
    }

}

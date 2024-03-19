using UnityEngine;
using UnityEngine.EventSystems;

public class PointerInteractableSample : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	[Space]
	[SerializeField] MeshRenderer mesh = null;
	[Space]
	[SerializeField] Color baseColor = Color.white;
	[SerializeField] Color hoverColor = Color.grey;
	[SerializeField] Color clickColor = Color.white;


	// ------------------------------------------------------

	private void Reset()
	{
		mesh = GetComponent<MeshRenderer>();
		if (mesh != null)
			baseColor = mesh.sharedMaterial.color;
	}

	Material material
	{
		get => mesh.material;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		material.color = hoverColor;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		material.color = baseColor;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		material.color = clickColor;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		material.color = baseColor;
	}
}

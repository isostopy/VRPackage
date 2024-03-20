using UnityEngine;
using UnityEngine.EventSystems;

namespace Isostopy.VR.Raycaster.Sample
{
	/// <summary>
	/// Componente para testear que el raycaster funciona correctamente. <br/>
	/// Cambia el color de un material cuando se le apunta con el rayo. </summary>
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

		private void Start()
		{
			material.color = baseColor;
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
}
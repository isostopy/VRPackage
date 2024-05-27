using UnityEngine;
using UnityEngine.EventSystems;

namespace Isostopy.VR.Raycaster
{
	/// <summary>
	/// Componente que lanza un Raycast buscando objetos con las interfaces del EventSystem. </summary>
	///
	/// Actualmente no guarda ninguna información sobre el puntero en la PointerEventData que se pasa como parámetro a las funciones
	/// y solo tiene soporte para las siguientes interfaces del EventSystem:
	///		- IPointerEnterHandler e IPointerExitHandler;
	///		- IPointerDownHandler e IPointerUpHandler;
	///		- IPointerClickHandler.
	///	
	/// Para crear un componente que pueda ser detectado por este objeto implementa alguna de las interfaces del EventSystem.
	/// Los objetos, incluidos los de la UI en world space, necesitan Collider para que este componente los encuentre.
	/// Pon Navigation en None en los elementos de la UI para que no se queden seleccionados y no pasen cosas raras.
	/// 
	/// Esto no es de este componente pero puede venir bien para hacer pruebas:
	///		Para que los eventos con el raton funcionen con GameObjects que no son de la UI la camara necesita tener asignado el componente PhysicsRaycaster.
	/// 
	[AddComponentMenu("Isostopy/VR/Raycaster/Raycaster")]
	public class VRRaycaster : MonoBehaviour
	{
		/// <summary> Transform que define el origen y la direccion del rayo. </summary>
		[Space] public Transform rayOrigin = null;
		/// <summary> Mascara de deteccion del rayo. </summary>
		public LayerMask mask = 1 << 5;
		/// <summary> Distancia maxima de deteccion del rayo. </summary>
		public float maxDistance = 10;

		/// <summary> Boton que debe pulsarse para hacer click. </summary>
		[Space] public OVRInput.Button button = OVRInput.Button.One;
		/// <summary> Trigger que debe pulsarse para hacer click. </summary>
		public OVRInput.Axis1D trigger = OVRInput.Axis1D.PrimaryIndexTrigger;
		/// <summary> Cuanto tiene que aprentar el usuario el boton para considerar que esta haciendo clic. </summary>
		[Range(0, 1)] public float inputToClick = 0.5f;
		/// Si estaba o no haciendo clic el frame anterior.
		bool wasPressing = false;


		/// <summary> Objeto que se coloca donde el rayo choca con algo. </summary>
		[Space] public Transform pointerIndicator = null;
		/// <summary> Line Renderer que muestra la direccion del rayo. </summary>
		public LineRenderer lineRenderer = null;
		/// <summary> Si esta en TRUE el rayo se esconde cuando no estemos apuntando a algo. </summary>
		public bool hideRayWhenNotPointing = true;
		/// <summary> Tamaño maximo del line renderer que añade visual al rayo. </summary>
		public float maxLineLength = 10;

		/// <summary> Objeto al que se esta apuntando con el rayo. </summary>
		Transform hoverningItem = null;
		/// <summary> Objeto sobre el que se ha pulsado el boton. </summary>
		Transform selectedItem = null;

		/// <summary> Info sobre el puntero del raton que se pasa a los eventos. </summary>
		PointerEventData pointerData = null;


		// ------------------------------------------------------
		#region Initialization

		private void Reset()
		{
			rayOrigin = transform;
		}

		private void Start()
		{
			EventSystem eventSystem = EventSystem.current;
			if (eventSystem == null)
			{
				//Debug.LogError("Hace falta un EventSystem en la escena.", this);
				this.enabled = false; return;
			}

			pointerData = new PointerEventData(eventSystem);
		}

		#endregion


		// ------------------------------------------------------

		#region Update

		private void Update()
		{
			// Lanzar raycast.
			Transform pointedItem = PointerRaycast();
			// Hacer hover sobre el objeto al que apuntamos.
			if (pointedItem != hoverningItem)
				ChangeHoveringItem(pointedItem);

			// Calcula si el gatillo se ha pulsado o soltado este frame.
			float input = OVRInput.Get(trigger);
			bool isPressing = input >= inputToClick;
			bool pressedThisFrame = isPressing == true && wasPressing == false;
			bool releasedThisFrame = isPressing == false && wasPressing == true;
			wasPressing = isPressing;

			// Seleccionar el objeto si se pulsa un boton.
			if (pressedThisFrame || OVRInput.GetDown(button) || Input.GetKeyDown(KeyCode.Space))
			{
				selectedItem = hoverningItem;
			}
			// Mantener pulsado.
			if (isPressing || OVRInput.Get(button) || Input.GetKey(KeyCode.Space))
			{
				PointerDown();
			}
			// Deseleccionarlo si se suelta.
			else if (releasedThisFrame || OVRInput.GetUp(button) || Input.GetKeyUp(KeyCode.Space))
			{
				Click();
				PointerUp();

				selectedItem = null;
			}
		}

		#endregion

		// --------------------------

		#region Raycast

		/// <summary> Lanza un Raycast y devuelve el transform del resultado. </summary>
		Transform PointerRaycast()
		{
			RaycastHit hit;

			// Si el rayo da con algo, colocar el punto y el rayo en el punto de contacto.
			if (rayOrigin.gameObject.activeInHierarchy
				&& Physics.Raycast(rayOrigin.position, rayOrigin.forward, out hit, maxDistance, mask))
			{
				PlacePointerIndicator(hit.point, hit.normal);
				PlaceLineRenderer(hit.point);

				if (lineRenderer != null)
					lineRenderer.enabled = true;

				return hit.transform;
			}

			// Si no, colcar el puntero fuera de la vista del jugador y el rayo hasta el limite de deteccion.
			else
			{
				Vector3 rayEnd = rayOrigin.position + rayOrigin.forward * maxDistance;

				PlacePointerIndicator(Vector3.up * 1000, Vector3.forward);
				PlaceLineRenderer(rayEnd);

				if (hideRayWhenNotPointing && lineRenderer != null)
					lineRenderer.enabled = false;

				return null;
			}
		}

		/// <summary> Coloca el indicador del puntero. </summary>
		void PlacePointerIndicator(Vector3 position, Vector3 normal)
		{
			if (pointerIndicator == null)
				return;

			pointerIndicator.position = position;
			pointerIndicator.LookAt(position + normal, Vector3.up);
		}

		/// <summary> Dibuja la linea llendo desde el origen del rayo a la posicion indicada. </summary>
		void PlaceLineRenderer(Vector3 targetPoint)
		{
			if (lineRenderer == null)
				return;

			Vector3 origin = rayOrigin.position;
			Vector3 dir = targetPoint - origin;
			if (dir.sqrMagnitude > maxLineLength * maxLineLength)
				dir = dir.normalized * maxLineLength;
			float positionCount = lineRenderer.positionCount;

			// Separar homogeneamente todos los puntos.
			for (int i = 0; i < lineRenderer.positionCount; i++)
			{
				float fraction = i / (positionCount - 1);
				lineRenderer.SetPosition(i, origin + dir * fraction);
			}
		}

		#endregion

		// --------------------------

		#region Hover

		/// <summary>
		/// Llamada cuando cambia el objeto al que se esta apuntado. <para></para>
		/// Guarda un nuevo "hoveringItem" haciendo OnPointerExit del anterior y OnPointerEnter del nuevo. </summary>
		void ChangeHoveringItem(Transform newItem)
		{
			// OnPointerExit del anterior item.
			if (hoverningItem != null)
			{
				IPointerExitHandler exitHandler;
				if (hoverningItem.TryGetComponent<IPointerExitHandler>(out exitHandler))
					exitHandler.OnPointerExit(pointerData);
				hoverningItem = null;
			}

			// OnPointerEnter del nuevo item.
			if (newItem != null)
			{
				IPointerEnterHandler enterHandler;
				if (newItem.TryGetComponent<IPointerEnterHandler>(out enterHandler))
				{
					enterHandler.OnPointerEnter(pointerData);
					hoverningItem = newItem;
				}
			}
		}

		#endregion

		// --------------------------

		#region Pressing

		/// <summary> Hacer OnPointerDown del objeto seleccionado. </summary>
		void PointerDown()
		{
			if (selectedItem == null)
				return;

			IPointerDownHandler downHandler = selectedItem.GetComponent<IPointerDownHandler>();
			if (downHandler != null)
				downHandler.OnPointerDown(pointerData);
		}

		/// <summary> Hacer OnPointerUp del objeto seleccionado. </summary>
		void PointerUp()
		{
			if (selectedItem == null)
				return;

			IPointerUpHandler upHandler = selectedItem.GetComponent<IPointerUpHandler>();
			if (upHandler != null)
				upHandler.OnPointerUp(pointerData);


		}

		/// <summary> Hacer OnPointerClick del objeto seleccionado. </summary>
		void Click()
		{
			if (hoverningItem != selectedItem || selectedItem == null)      /// OnClick solo funciona si se hace Up sobre el mismo objeto que se ha pulsado.
				return;

			IPointerClickHandler clickHandler = selectedItem.GetComponent<IPointerClickHandler>();
			if (clickHandler != null)
				clickHandler.OnPointerClick(pointerData);
		}

		#endregion
	}
}
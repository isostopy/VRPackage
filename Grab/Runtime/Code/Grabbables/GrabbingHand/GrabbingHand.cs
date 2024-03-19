using System.Collections.Generic;
using UnityEngine;

namespace Isostopy.VR.Grab
{
	/// <summary> Componente que permite a una mano agarrar objetos -> <see cref="Grabbable"/>. </summary>
	/* Quiza en el futuro haya que hacer una version base de este componente que no dependa de las cosas de oculus para poder usarlo en otros dispositivos. */
	public class GrabbingHand : MonoBehaviour
	{
		/// Si esta mano es derecha o izquierda.
		[Space][SerializeField] OVRInput.Hand hand = OVRInput.Hand.HandLeft;

		/// Mando de Oculus que controla esta mano.
		OVRInput.Controller controller = OVRInput.Controller.LTouch;
		/// Cuanto se tiene que apretar el gatillo para considerar que estamos agarrando.
		[SerializeField][Range(0, 1)] float inputToGrab = 0.5f;
		/// Si esta el usuario pulsando el boton de agarre.
		bool pressing = false;

		/// Lista de Grabbables sobre los que tenemos puesta la mano.
		List<Grabbable> hoveringObjects = new List<Grabbable>();
		/// El Grabbable que tenemos agarrado en este momento.
		Grabbable grabbedObject = null;

		/// El Animator del modelo de la mano.
		[Space][SerializeField] Animator handAnimator = null;
		/// Nombre de la propiedad del animator donde vamos a indicarle el input del usuario.
		[SerializeField] string animatorGrabProperty = "Grab";


		// ----------------------------------------------------------
		#region Initialization

		private void Awake()
		{
			controller = GetControllerFromHand(hand);
		}

		#endregion


		// ----------------------------------------------------------
		#region Input

		void Update()
		{
			float grabbingInput = GrabbingInput;

			// Soltar o agarrar si estamos apretando o soltando el boton.
			if (pressing == false && grabbingInput > inputToGrab)
			{
				pressing = true;
				GrabTouchingObject();
			}
			else if (pressing == true && grabbingInput < inputToGrab)
			{
				pressing = false;
				Release();
			}

			// Decirle al animator el input del usuario para que pueda animar la mano.
			if (handAnimator != null)
				handAnimator.SetFloat(animatorGrabProperty, grabbingInput);
		}

		/// <summary> Valor entre 0 y 1 que indica cuanto esta aprentando el usuario </summary>
		public virtual float GrabbingInput
		{
			get => OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller);
		}

		/// <summary> Agarra el ultimo objeto con el que la mano ha entrado en contacto. </summary>
		void GrabTouchingObject()
		{
			if (hoveringObjects.Count == 0)
				return;

			var touchingObject = hoveringObjects[hoveringObjects.Count - 1];
			if (touchingObject == null || touchingObject.gameObject.activeInHierarchy == false)
				return;

			Grab(touchingObject);
		}

		#endregion


		// ----------------------------------------------------------
		#region Trigger

		/// Mantenermos una lista con los objetos agarrables que estan en contacto con esta mano.
		private void OnTriggerEnter(Collider other)
		{
			Grabbable enteringObject = other.GetComponent<Grabbable>();
			if (enteringObject != null && hoveringObjects.Contains(enteringObject) == false)
				hoveringObjects.Add(enteringObject);
		}

		/// Cuando un objeto deja de estar en contacto, lo sacamos de la lista.
		private void OnTriggerExit(Collider other)
		{
			Grabbable exitingObject = other.GetComponent<Grabbable>();
			if (exitingObject != null && hoveringObjects.Contains(exitingObject))
				hoveringObjects.Remove(exitingObject);
		}

		#endregion


		// ----------------------------------------------------------
		#region Grab & Release

		/// <summary> Agarrar el ultimo objeto sobre el que hemos puesto la mano. </summary>
		void Grab(Grabbable item)
		{
			// Soltar el objeto anterior.
			if (grabbedObject != null)
				grabbedObject.Release();

			// Agarrar el nuevo.
			grabbedObject = item;
			item.Grab(this);
		}

		/// <summary> Soltar el objeto que tenemos agarrado. </summary>
		void Release()
		{
			if (grabbedObject == null)
				return;

			grabbedObject.Release();
			grabbedObject = null;
		}

		/// <summary> Indica a esta mano que el objeto indicado se ha soltado. </summary>
		public void ObjectRealeased(Grabbable releasedObject)
		{
			if (releasedObject == grabbedObject)
			{
				grabbedObject = null;
			}
		}

		#endregion


		// ----------------------------------------------------------
		#region Utils

		/// <summary> Devuelve el mando de Oculus que se corresponde con la mano indicada. </summary>
		public static OVRInput.Controller GetControllerFromHand(OVRInput.Hand hand)
		{
			if (hand == OVRInput.Hand.HandLeft)
				return OVRInput.Controller.LTouch;

			else if (hand == OVRInput.Hand.HandRight)
				return OVRInput.Controller.RTouch;

			return OVRInput.Controller.None;
		}

		#endregion


		// ----------------------------------------------------------
		#region Public

		/// <summary> Si esta mano es la derecha o la izquierda. </summary>
		public OVRInput.Hand Hand
		{
			get => hand;
			set
			{
				hand = value;
				controller = GetControllerFromHand(hand);
			}
		}

		/// <summary> Mando de Oculus que esta controlando esta mano. </summary>
		public OVRInput.Controller Controller => controller;

		#endregion
	}
}
using UnityEngine;
using UnityEngine.Events;

namespace Isostopy.VR.Grab
{
	/// <summary> Grabbable que se hace hijo de la mano que lo agarra. </summary>
	public class SimpleGrabbableObject : Grabbable
	{
		/// <summary> Referencia al Rigidbody de este objeto. </summary>
		protected Rigidbody rigid = null;

		/// <summary> ¿Era kinematico este objeto antes de agarrarlo? </summary>
		protected bool wasKinematic = false;
		/// <summary> Referencia al padre que tenia este objeto antes de agararrlo. </summary>
		protected Transform prevParent = null;

		/// Posicion que tenia este objeto el frame anterior.
		protected Vector3 lastFramePosition = Vector3.zero;
		/// Modificador de la velocidad a la que lanzamos este objeto.
		[Space][SerializeField] float releaseSpeedModifier = 1f;


		// ----------------------------------------------------------

		protected virtual void Start()
		{
			rigid = GetComponent<Rigidbody>();
		}


		protected virtual void LateUpdate()
		{
			if (isGrabbed)
				lastFramePosition = transform.position;
		}

		// ----------------------------------------------------------

		public override void Grab(GrabbingHand grabbingHand)
		{
			// Si no nos estaba agarrando nada, guardar los valores del rigidbody.
			if (this.grabbingHand == null)
			{
				if (rigid != null) wasKinematic = rigid.isKinematic;
				prevParent = transform.parent;
			}

			base.Grab(grabbingHand);

			// Hacemos este objeto kinematico e hijo de la mano que nos agarra.
			if (rigid != null) rigid.isKinematic = true;
			transform.parent = grabbingHand.transform;

			lastFramePosition = transform.position;
		}

		public override void Release()
		{
			// Volvemos al kinematico y padre anterior.
			if (rigid != null)
				rigid.isKinematic = wasKinematic;
			transform.parent = prevParent;
			// Le damos la velocidad a la que lo estabamos moviendo.
			var speed = (transform.position - lastFramePosition) / Time.deltaTime;
			rigid.velocity = speed * releaseSpeedModifier;

			base.Release();
		}
	}
}
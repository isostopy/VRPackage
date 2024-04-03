using UnityEngine;
using UnityEngine.Events;

namespace Isostopy.VR.Grab
{
	/// <summary> Grabbable que se hace hijo de la mano que lo agarra. </summary>

	[AddComponentMenu("Isostopy/VR/Grab/Simple Grabbable Object")]
	public class SimpleGrabbableObject : Grabbable
	{
		/// <summary> Referencia al Rigidbody de este objeto. </summary>
		protected Rigidbody rigid = null;

		/// <summary> ¿Era kinematico este objeto antes de agarrarlo? </summary>
		protected bool wasKinematic = false;
		/// <summary> Referencia al padre que tenia este objeto antes de agararrlo. </summary>
		protected Transform prevParent = null;

		/// Velocidad a la que se mueve este objeto mientras esta agarrado.
		protected Vector3 lastFramePosition = Vector3.zero;
		protected Vector3 currentSpeed = Vector3.zero;
		/// Modificador de la velocidad a la que lanzamos este objeto.
		[Space][SerializeField] float releaseSpeedModifier = 1f;


		// ---------------------------------------------------------

		protected virtual void Start()
		{
			rigid = GetComponent<Rigidbody>();
		}


		protected virtual void FixedUpdate()
		{
			if (isGrabbed)
			{
				currentSpeed = (transform.position - lastFramePosition) / Time.fixedDeltaTime;
				lastFramePosition = transform.position;
			}
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

			currentSpeed = Vector3.zero;
			lastFramePosition = transform.position;
		}

		public override void Release()
		{
			// Volvemos al kinematico y padre anterior.
			transform.parent = prevParent;
			if (rigid != null)
			{
				rigid.isKinematic = wasKinematic;
				rigid.velocity = currentSpeed * releaseSpeedModifier;	// Le damos la velocidad a la que lo estabamos moviendo.
			}

			base.Release();
		}
	}
}
using UnityEngine;
using UnityEngine.Events;

namespace Isostopy.VR.Grab
{
	/// <summary> Objeto que puede ser agarrado por una mano -> <see cref="GrabbingHand"/>. </summary>
	public abstract class Grabbable : MonoBehaviour
	{
		/// <summary> GrabbingHand que tiene actualmente agarrada este objeto. </summary>
		protected GrabbingHand grabbingHand = null;
		/// <summary> ¿Esta agarrado este objeto? </summary>
		protected bool isGrabbed = false;

		/// Eventos invocados al agarrar y soltar el objeto.
		UnityEvent grabEvent = new UnityEvent();
		UnityEvent releaseEvent = new UnityEvent();


		// ----------------------------------------------------------

		/// <summary> Agarra este objeto por la GrabbingHand indicada. </summary>
		public virtual void Grab(GrabbingHand grabbingHand)
		{
			if (this.grabbingHand != null)
			{
				this.grabbingHand.ObjectRealeased(this);
			}
			this.grabbingHand = grabbingHand;

			isGrabbed = true;
			grabEvent.Invoke();
		}

		/// <summary> Suelta este objeto. </summary>
		public virtual void Release()
		{
			if (grabbingHand != null)
			{
				grabbingHand.ObjectRealeased(this);
			}
			grabbingHand = null;

			isGrabbed = false;
			releaseEvent.Invoke();
		}


		// ----------------------------------------------------------

		/// <summary> ¿Esta agarrado este objeto? </summary>
		public bool IsGrabbed => isGrabbed;

		/// <summary> GrabbingHand que tiene agarrada este objeto. </summary>
		public GrabbingHand GrabbingHand => grabbingHand;

		/// <summary> Evento invocado al agarrar este objeto. </summary>
		public UnityEvent GrabEvent => grabEvent;

		/// <summary> Evento invocado al soltar este objeto. </summary>
		public UnityEvent ReleaseEvent => releaseEvent;
	}
}
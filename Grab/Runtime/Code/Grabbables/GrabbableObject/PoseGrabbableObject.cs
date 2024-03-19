using UnityEngine;

namespace Isostopy.VR.Grab
{
	/// <summary>
	/// Objeto que al ser agarrado pone una pose en la mano. <para></para>
	/// Para usarlo la mano tiene que tener el componente HandPoseAnimator -> <see cref="HandPoseAnimator"/>. <br/>
	/// Y este objeto necesita dos HandPose, una de la mano derecha y otra de la izquierda -> <see cref="HandPose"/> </summary>
	public class PoseGrabbableObject : SimpleGrabbableObject
	{
		/// Pose en que tenemos que poner la mano cuando se agarra este objeto.
		[Space]
		[SerializeField] HandPose handPoseL = null;
		[SerializeField] HandPose handPoseR = null;

		/// <summary> Referencia al HandPoseAnimator de la mano que tiene sujeto este objeto. </summary>
		HandPoseAnimator grabbingPoseAnimator = null;


		// ----------------------------------------------------------

		public override void Grab(GrabbingHand grabbingHand)
		{
			base.Grab(grabbingHand);

			// Si ya habia un HandPoseAnimator asignado, quitar la pose.
			if (grabbingPoseAnimator != null)
				grabbingPoseAnimator.Stop();
			// Busca el componente HandPoseAnimator en la nueva mano que nos agarra.
			grabbingPoseAnimator = grabbingHand.GetComponent<HandPoseAnimator>();
			if (grabbingPoseAnimator == null)
				return;

			// Define la pose que toca poner. Derecha o Izquierda.
			HandPose targetPose;
			if (grabbingHand.Hand == OVRInput.Hand.HandLeft)
				targetPose = (handPoseL);
			else if (grabbingHand.Hand == OVRInput.Hand.HandRight)
				targetPose = (handPoseR);
			else
				return;

			// Coloca este objeto a la distancia que toque de la mano.
			PlacePose(targetPose);
			// Pon la pose en la mano.
			grabbingPoseAnimator.PlayPose(targetPose);
		}

		public override void Release()
		{
			if (grabbingPoseAnimator != null)
				grabbingPoseAnimator.Stop();
			grabbingPoseAnimator = null;

			base.Release();
		}


		// ----------------------------------------------------------

		/// <summary> Coloca este objeto en la posicion respecto a la mano que le toca para la pose indicada. </summary>
		void PlacePose(HandPose pose)
		{
			// ¿Esto es una chapuza? Lo suyo quiza seria hacerlo con complejas operaciones matematicas.
			// Pero aqui:
			pose.transform.parent = grabbingHand.transform;         /* Hacemos la GameObject de la pose hijo de la mano y este objeto hijo de la pose. */
			this.transform.parent = pose.transform;
			pose.transform.localPosition = Vector3.zero;            /* La pose la colocamos en la posicion y rotacion de la mano. */
			pose.transform.localRotation = Quaternion.identity;
			this.transform.parent = grabbingHand.transform;         /* Luego hacemos este objeto hijo de la mano para que la siga. */
			pose.transform.parent = this.transform;                 /* Y volvemos a hacer la pose hija de este objeto. */
		}
	}
}
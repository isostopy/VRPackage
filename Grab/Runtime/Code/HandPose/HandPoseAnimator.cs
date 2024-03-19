using System.Collections.Generic;
using UnityEngine;

namespace Isostopy.VR.Grab
{
	/// <summary>
	/// Componente que permite poner poses en una mano. <para></para>
	/// Tiene que estar en la mano del jugador. </summary>
	public class HandPoseAnimator : MonoBehaviour
	{
		/// <summary> Hueso base del modelo de la mano. </summary>
		public Transform Root => root;
		[Space][SerializeField] Transform root = null;
		/// Lista con el Transform de cada hueso de la pose asignado a un nombre.
		[SerializeField] List<BoneReference> bones = new List<BoneReference>();

		/// Pose que se esta poniendo ahora mismo en la mano.
		/// Es una lista con el nombre de cada hueso asociado a una pose (rotacion y posicion).
		Dictionary<string, Pose> targetPose = new Dictionary<string, Pose>();


		// ----------------------------------------------------------

		private void LateUpdate()
		{
			AnimatePose();
		}

		/// <summary> Mantiene puesta la pose objetivo. </summary>
		void AnimatePose()
		{
			if (targetPose == null || targetPose.Count == 0)
				return;

			foreach (BoneReference bone in bones)
			{
				string boneName = bone.name;

				if (targetPose.ContainsKey(boneName) == false)
					continue;

				bone.transform.localPosition = targetPose[boneName].position;
				bone.transform.localRotation = targetPose[boneName].rotation;
			}
		}


		// ----------------------------------------------------------

		/// <summary> Pon una pose en esta mano. </summary>
		public void PlayPose(HandPose pose)
		{
			// Guarda la posicion de todos los dedos de la pose objetivo, asociadas a su nombre.
			targetPose.Clear();
			foreach (var finger in pose.BonesPoses)
				targetPose.Add(finger.Key, finger.Value);
		}

		/// <summary> Quita cualquier pose que tenga esta mano. </summary>
		public void Stop()
		{
			// Limpia la lista que define la pose objetivo.
			targetPose.Clear();
		}
	}
}
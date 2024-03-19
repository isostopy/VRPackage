using System.Collections.Generic;
using UnityEngine;

namespace Isostopy.VR.Grab.Editor
{
	/// <summary> Componente que guarda la pose de una mano cuando tocas el objeto que lo lleva. <para></para>
	/// No esta pensando para incluirse en una build, solo en el editor de Unity. </summary>
	public class HandPoseRecordButton : MonoBehaviour
	{
		[Space]
		[SerializeField] List<Collider> validColliders = new List<Collider>();
		[SerializeField] HandPoseRecorder recorder = null;


		private void Reset()
		{
			OVRHand[] hands = FindObjectsByType<OVRHand>(FindObjectsSortMode.None);
			foreach (OVRHand hand in hands)
			{
				Collider handCollider;
				if (hand.TryGetComponent<Collider>(out handCollider))
					validColliders.Add(handCollider);
			}

			recorder = FindObjectOfType<HandPoseRecorder>();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (validColliders.Contains(other))
			{
				recorder.RecordPoseWithDelay();
			}
		}
	}
}

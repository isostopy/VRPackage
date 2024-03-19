using System.Collections.Generic;
using UnityEngine;

namespace Isostopy.VR.Grab
{
	/// <summary>
	/// Componente que define la pose de todos los huesos de una mano utilizando una lista de transforms. <para></para>
	/// Tiene que estar en el objeto agarrable que se va a sujetar con una pose. </summary>
	public class HandPose : MonoBehaviour
	{
		///Modelo 3D de la mano. Solo se muestra en el editor para ver como va a quedar.
		[Space][SerializeField] GameObject handModel = null;
		/// <summary> Lista con el Transform de cada hueso asignado a su nombre. </summary>
		[SerializeField] public List<BoneReference> bones = new List<BoneReference>();
		/// <summary> Diccionario con la pose de cada dedo asignada a su nombre. </summary>
		Dictionary<string, Pose> bonePoses = new Dictionary<string, Pose>();


		// ----------------------------------------------------------
		#region Initialization

		private void Awake()
		{
			// Crear el diccionaio de FingerPose con la informacion de la lista de Transforms.
			foreach (BoneReference finger in bones)
				bonePoses.Add(finger.name, new Pose(finger.transform.localPosition, finger.transform.localRotation));

			// Ocultar el modelo de la mano.
			if (handModel != null) handModel.SetActive(false);
		}

		#endregion


		// ----------------------------------------------------------
		#region Public

		/// <summary> Diccionario con la pose de cada hueso asignada a un nombre. </summary>
		public Dictionary<string, Pose> BonesPoses
		{
			get => bonePoses;
		}

		#endregion
	}

	/// <summary> Permite guardar una referencia al hueso del dedo de una mano, asociandola a un nombre. </summary>
	[System.Serializable]
	public struct BoneReference
	{
		public string name;
		public Transform transform;
	}
}
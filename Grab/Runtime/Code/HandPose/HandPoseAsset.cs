using System.Collections.Generic;
using UnityEngine;

namespace Isostopy.VR.Grab
{
	/// <summary>
	/// Asset que contiene una lista con poses de huesos (posicion y rotacion) asiciadas a un nombre. </summary>
	public class HandPoseAsset : ScriptableObject
	{
		/// Lista con las poses de los huesos guardados en este asset.
		[Space][SerializeField] public List<BonePose> bones = new();

		// ----------

		/// <summary> Guarda la posicion de un hueso en este asset. </summary>
		public void AddPose(string boneName, Vector3 position, Quaternion rotation)
		{
			BonePose bone = new BonePose();
			bone.name = boneName;
			bone.position = position;
			bone.rotation = rotation;
			bones.Add(bone);
		}

		/// <summary> Devuelve un diccionario con la pose de cada hueso asociada a su id. </summary>
		public Dictionary<string, Pose> GetBonesDictionary()
		{
			var dictionary = new Dictionary<string, Pose>();

			foreach (BonePose bone in bones)
			{
				dictionary[bone.name] = new Pose(bone.position, bone.rotation);
			}

			return dictionary;
		}

		// ----------

		/// Clase que utilizamos para guardar la pose de cada hueso.
		[System.Serializable]
		public class BonePose
		{
			public string name;
			public Vector3 position;
			public Quaternion rotation;
		}
	}
}
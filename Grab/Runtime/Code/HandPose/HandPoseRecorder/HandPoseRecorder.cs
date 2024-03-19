#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace Isostopy.VR.Grab.Editor
{
	/// <summary> Componente que guarda la pose de una mano en un asset. <para></para>
	/// No esta pensando para incluirse en una build, solo en el editor de Unity. </summary>
	public class HandPoseRecorder : MonoBehaviour
	{
		[Space]
		/// Nombre con el que se va a crear el asset con la pose de la mano.
		public string handName = "Left";
		public string poseName = "HandPose";
		/// Lista con los huesos de la mano que tiene hand tracking.
		public List<BoneReference> playerHand = new List<BoneReference>();

		[Header("------------------------------")]
		/// Datos para guardar el delay al guardar la mano con delay.
		[Min(0)] public float delay = 3;
		[SerializeField] Text delayDidsplayText = null;
		Coroutine currentDelatRoutine = null;

		[Header("------------------------------")]
		/// Una mano que va a mostrar la misma pose que la del usuario.
		public List<BoneReference> modelHand = new();


		// ----------------------------------------------------------------------
		#region Guardar la pose de la mano en un asset

		/// <summary> Crea un asset con la pose de la mano. </summary>
		[ContextMenu("Record Pose To Asset")]
		public void RecordPose()
		{
			HandPoseAsset asset = ScriptableObject.CreateInstance<HandPoseAsset>();

			foreach (var bone in playerHand)
			{
				asset.AddPose(bone.name, bone.transform.localPosition, bone.transform.localRotation);
			}

			string path = AssetDatabase.GenerateUniqueAssetPath("Assets/" + poseName + " " + handName + ".asset");
			AssetDatabase.CreateAsset(asset, path);
		}

		#endregion


		// ----------------------------------------------------------------------
		#region Guardar pose con un delay

		/// Crear un archivo de pose pasado un delay.
		public void RecordPoseWithDelay()
		{
			if (currentDelatRoutine != null) { StopCoroutine(currentDelatRoutine); }
			currentDelatRoutine = StartCoroutine(RecordWithDelayRoutine());
		}

		/// Corrutina que espera un tiempo antes de crear el asset con la pose.
		public IEnumerator RecordWithDelayRoutine()
		{
			delayDidsplayText.text = "READY?";

			float counter = 0;
			while (counter <= delay)
			{
				yield return null;
				counter += Time.deltaTime;
			}

			RecordPose();
			delayDidsplayText.text = "DONE!";
		}

		#endregion


		// ----------------------------------------------------------------------
		#region Mantener sincronizado el modelo de la mano

		// Mantenemos la mano del modelo sincronizada con la del usuario.
		private void Update()
		{
			for (int i = 0; i < playerHand.Count; i++)
			{
				if (modelHand.Count <= i)
					break;

				var playerBone = playerHand[i].transform;
				var modelBone = modelHand[i].transform;

				if (playerBone == null || modelBone == null)
					continue;

				modelBone.SetLocalPositionAndRotation(playerBone.localPosition, playerBone.localRotation);
			}
		}

		#endregion
	}
}

#endif

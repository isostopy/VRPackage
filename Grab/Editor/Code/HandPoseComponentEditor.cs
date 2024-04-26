using UnityEngine;
using UnityEditor;

namespace Isostopy.VR.Grab.Editor
{
	[CustomEditor(typeof(HandPose))]
	[CanEditMultipleObjects]
	public class HandPoseComponentEditor : UnityEditor.Editor
	{
		int pickerId = 0;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.Space();

			// Seleccionar pose desde un asset.
			if (GUILayout.Button("Set from asset"))
			{
				OpenObjectPicker();
			}
			PoseHandAsObjectPicked();

			// Guardar la pose actual como un nuevo asset.
			if (GUILayout.Button("Save to asset"))
			{
				SaveCurrentPoseInAsset();
			}
		}

		/// <summary> Abre el object picker de Unity para seleccionar un asset. </summary>
		void OpenObjectPicker()
		{
			pickerId = EditorGUIUtility.GetObjectPickerControlID() + 1;
			EditorGUIUtility.ShowObjectPicker<HandPoseAsset>(null, false, "", pickerId);
		}

		/// <summary> Posar la mano como indica el asset seleccionado en el object picker. </summary>
		void PoseHandAsObjectPicked()
		{
			if (Event.current.commandName == "ObjectSelectorUpdated"
				&& EditorGUIUtility.GetObjectPickerControlID() == pickerId)
			{
				var selectedAsset = EditorGUIUtility.GetObjectPickerObject();
				if (selectedAsset != null)
					PoseHand(selectedAsset as HandPoseAsset);
			}
		}

		/// <summary> Coloca el modelo de la mano en la pose que esta guardada en un asset. </summary>
		void PoseHand(HandPoseAsset asset)
		{
			var target = this.target as HandPose;

			var assetBones = asset.GetBonesDictionary();
			var targetBones = target.bones;

			foreach (var bone in targetBones)
			{
				string name = bone.name;
				if (assetBones.ContainsKey(name) == false)
					continue;

				Undo.RecordObject(bone.transform, "Changed Hand Pose of " + target.transform.name);

				Pose pose = assetBones[name];
				bone.transform.SetLocalPositionAndRotation(pose.position, pose.rotation);
			}
		}

		/// <summary> Guarda la pose en la que esta la mano en un asset en el proyecto. </summary>
		void SaveCurrentPoseInAsset()
		{
			var target = this.target as HandPose;

			HandPoseAsset asset = ScriptableObject.CreateInstance<HandPoseAsset>();

			foreach (var bone in target.bones)
			{
				asset.AddPose(bone.name, bone.transform.localPosition, bone.transform.localRotation);
			}

			string path = EditorUtility.SaveFilePanel("Create Hand Pose Asset", "Assets/", "new Hand Pose", "asset");

			if (string.IsNullOrEmpty(path))
				return;

			path = System.IO.Path.GetRelativePath(Application.dataPath, path);
			path = "Assets/" + path;
			AssetDatabase.CreateAsset(asset, path);
		}
	}
}
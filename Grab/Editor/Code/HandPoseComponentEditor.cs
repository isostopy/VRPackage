using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Isostopy.VR.Grab;

[CustomEditor(typeof(HandPose))]
public class HandPoseComponentEditor : Editor
{
	int pickerId = 0;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		EditorGUILayout.Space();
		if (GUILayout.Button("Set from asset"))
		{
			pickerId = EditorGUIUtility.GetObjectPickerControlID() + 1;
			EditorGUIUtility.ShowObjectPicker<HandPoseAsset>(null, false, "", pickerId);
		}

		if (Event.current.commandName == "ObjectSelectorUpdated"
			&& EditorGUIUtility.GetObjectPickerControlID() == pickerId)
		{
			var selectedAsset = EditorGUIUtility.GetObjectPickerObject();
			if (selectedAsset != null)
				PoseHand(selectedAsset as HandPoseAsset);
		}
	}

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

			Undo.RecordObject(bone.transform, "Changed Hand Pose");

			Pose pose = assetBones[name];
			bone.transform.SetLocalPositionAndRotation(pose.position, pose.rotation);
		}

	}
}

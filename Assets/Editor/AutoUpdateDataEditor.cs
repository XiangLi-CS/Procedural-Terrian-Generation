using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AutoUpdateData), true)]
public class AutoUpdateDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AutoUpdateData data = (AutoUpdateData)target;

        if(GUILayout.Button("Update"))
        {
            data.NotifyOfUpdatedValue();
            EditorUtility.SetDirty(target);
        }
    }
}

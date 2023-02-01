using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AutoUpdateData : ScriptableObject
{
    public event System.Action OnValuesUpdated;
    public bool autoUpdate;

#if UNITY_EDITOR
    private void OnValidate() => UnityEditor.EditorApplication.delayCall += _OnValidate;

    protected virtual void _OnValidate()
    {
        UnityEditor.EditorApplication.delayCall -= _OnValidate;
        if (this == null) return;
        if (autoUpdate)
        {
            UnityEditor.EditorApplication.update += NotifyOfUpdatedValue;
        }
    }
#endif
    /*protected virtual void OnValidate()
    {
        if (autoUpdate)
        {
            NotifyOfUpdatedValue();
        }
        
    }*/

    public void NotifyOfUpdatedValue()
    {
        UnityEditor.EditorApplication.update -= NotifyOfUpdatedValue;
        if (OnValuesUpdated != null)
        {
            OnValuesUpdated();
        }
    }

}

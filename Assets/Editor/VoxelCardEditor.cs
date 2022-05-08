using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VoxelCard))]
[CanEditMultipleObjects]
public class VoxelCardEditor : Editor
{
    VoxelCard TargetComponent;

    private void OnEnable()
    {
        TargetComponent = (VoxelCard)target;
    }

    public override void OnInspectorGUI()
    {
        bool g = false;
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            if (GUILayout.Button("Generate"))
            {
                TargetComponent.Generate();
                g = true;
            }
            base.OnInspectorGUI();
            if (!g && check.changed)
            {
                TargetComponent.OnInspectorChange();
            }
        }
    }
}

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VoxelCardUser))]
[CanEditMultipleObjects]
public class VoxelCardUserEditor : Editor
{
    VoxelCardUser TargetComponent;

    private void OnEnable()
    {
        TargetComponent = (VoxelCardUser)target;
    }

    public override void OnInspectorGUI()
    {
        bool g = false;
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            if (GUILayout.Button("Generate"))
            {
                //TargetComponent.Generate();
                g = true;
            }
            base.OnInspectorGUI();
        }
    }
}

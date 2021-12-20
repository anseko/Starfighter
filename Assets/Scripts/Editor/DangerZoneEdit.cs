using System;
using Net;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(DangerZone))]
    public class DangerZoneEdit : UnityEditor.Editor
    {
        private SerializedProperty zoneConfig;

        private void OnEnable()
        {
            zoneConfig = serializedObject.FindProperty("Zone Config");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
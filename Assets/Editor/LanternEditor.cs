using UnityEditor;
using UnityEngine;

namespace Gameplay
{
    [CustomEditor(typeof(FreezingLantern))]
    public class LanternEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Change State"))
            {
                Debug.Log("Switched lantern state through inspector");
                FreezingLantern lant = (FreezingLantern)target;
                lant.ChangeLanternState(!lant.LanternOn);

            }
        }
    }
}

using UnityEditor;
using UnityEngine;

namespace Gameplay
{
    public class PlayerEditor : EditorWindow
    {
        private bool showDetails = false;

        [MenuItem("Tools/PlayerEditor")]
        public static void ShowWindow()
        {
            GetWindow<PlayerEditor>("Player Editor");
        }
        public void OnGUI()
        {
            GUILayout.Label("Working stuffs:");
            showDetails = EditorGUILayout.Toggle("Detailed options", showDetails);
            if (showDetails)
            {
                if (GUILayout.Button("Infinite Sanity") && CheckIfRunning())
                {
                    GameObject.Find("Player").GetComponent<PlayerStatusEffects>().InsanityDeath = 9999;

                }
            }
        }

        private bool CheckIfRunning()
        {
            if (Application.isPlaying)
            {
                return true;
            }
            else
            {
                Debug.Log("You cannot run this command outside of play mode");
                return false;
            }
        }
    }
}

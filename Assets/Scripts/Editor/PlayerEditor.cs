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
        int selGridInt = 0;
        string[] selStrings = { "Player", "Game", "Misc" };
        public void OnGUI()
        {
            CreateStyles();
            selGridInt = GUILayout.SelectionGrid(selGridInt, selStrings, 3);
            GUILayout.Space(20);
            switch (selStrings[selGridInt])
            {
                case "Player":
                    PlayerPage();
                    break;
                case "Game":
                    GamePage();
                    break;
                case "Misc":
                    MiscPage();
                    break;
                default:
                    Debug.Log("That page doesnt exist");
                    break;
            }
        }

        GUIStyle titleStyle;
        GUIStyle headerStyle;
        GUIStyle importantButtonStyle;
        private void CreateStyles()
        {
            titleStyle = new GUIStyle(EditorStyles.boldLabel);
            titleStyle.fontSize = 20;

            headerStyle = new GUIStyle(EditorStyles.boldLabel);
            headerStyle.fontSize = 15;

            importantButtonStyle = new GUIStyle(GUI.skin.button);
            importantButtonStyle.padding = EditorStyles.miniButton.padding;
            importantButtonStyle.fontSize = 15;
            importantButtonStyle.border = new RectOffset(0, 0, 0, 0);
            importantButtonStyle.normal.background = MakeTex(new Color32(0,0,255,255));
            importantButtonStyle.onNormal.background = MakeTex(new Color32(0, 0, 255, 255));
            
        }
        private Texture2D MakeTex(Color col)
        {
            Texture2D tex = new Texture2D(1,1);
            tex.hideFlags = HideFlags.HideAndDontSave;
            tex.SetPixel(0, 0, col);
            tex.Apply();
            return tex;
        }

        GameObject player;
        PlayerStatusEffects playerEffects;
        private void PlayerPage()
        {
            if (player == null) { player = GameObject.Find("Player"); }
            if(playerEffects == null) { playerEffects = player.GetComponent<PlayerStatusEffects>(); }
            
            GUILayout.Label("Player", titleStyle);
            showDetails = EditorGUILayout.Toggle("Detailed options", showDetails);
            GUILayout.Space(20);
            GUILayout.Label("Living", headerStyle);
            if (GUILayout.Button("Make invulnerable",importantButtonStyle) && CheckIfRunning())
            {
                playerEffects.InsanityDeath = 9999;
                playerEffects.FrostbiteDeath = 9999;
            }
            if (showDetails)
            {
                GUILayout.Space(15);
                if (GUILayout.Button("Infinite Freezing") && CheckIfRunning())
                {
                   playerEffects.FrostbiteDeath = 9999;
                }
                if (GUILayout.Button("Infinite Sanity") && CheckIfRunning())
                {
                    playerEffects.InsanityDeath = 9999;
                }
            }
        }

        private void GamePage()
        {

        }

        private void MiscPage()
        {

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

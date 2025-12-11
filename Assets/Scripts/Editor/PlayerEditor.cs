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
        int selectedTab = 0;
        string[] allTabs = { "Player", "Game", "Misc" };
        public void OnGUI()
        {
            CreateStyles();
            selectedTab = GUILayout.SelectionGrid(selectedTab, allTabs, 3);
            GUILayout.Space(20);
            switch (allTabs[selectedTab])
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
        GUIStyle header2Style;
        GUIStyle importantButtonStyle;

        GUIStyle notWorkingButton;
        private void CreateStyles()
        {
            titleStyle = new GUIStyle(EditorStyles.boldLabel);
            titleStyle.fontSize = 20;

            headerStyle = new GUIStyle(EditorStyles.boldLabel);
            headerStyle.fontSize = 15;

            header2Style = new GUIStyle(EditorStyles.boldLabel);
            header2Style.fontSize = 10;

            importantButtonStyle = new GUIStyle(EditorStyles.miniButton);
            importantButtonStyle.fontSize = 14;

            notWorkingButton = new GUIStyle(EditorStyles.miniButton);
            notWorkingButton.normal.textColor = Color.darkRed;
        }

        GameObject player;
        PlayerStatusEffects playerEffects;
        PlayerHealth playerHealth;
        PlayerController playerController;
        PlayerInventory playerInventory;
        FreezingLantern playerLantern;
        private void PlayerPage()
        {
            if(!GameObject.Find("BaseGeneration")) { return; }
            if (player == null) { player = GameObject.Find("Player"); }
            if(playerEffects == null) { playerEffects = player.GetComponent<PlayerStatusEffects>(); }
            if (playerHealth == null) { playerHealth = player.GetComponent<PlayerHealth>(); }
            if( playerController == null) {playerController = player.GetComponent<PlayerController>();}
            if( playerInventory == null) {playerInventory = player.GetComponent<PlayerInventory>();}
            if(playerLantern == null) { playerLantern = player.transform.Find("PlayerCamera").Find("Lantern").GetComponent<FreezingLantern>();}
            
            GUILayout.Label("Player", titleStyle);
            showDetails = EditorGUILayout.Toggle("Detailed options", showDetails);
            GUILayout.Space(20);
            GUILayout.Label("Living", headerStyle);

            if (GUILayout.Button("Revive", importantButtonStyle) && CheckIfRunning())
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                playerHealth.gameObject.SetActive(true);
                SerializedObject serEffects = new SerializedObject(playerEffects);

                GameObject deathUI = (GameObject)new SerializedObject(playerHealth).FindProperty("_deathUI").objectReferenceValue;
                deathUI.SetActive(false);
                Transform cam = deathUI.transform.Find("CamBrain");
                cam.parent = playerHealth.transform;
                cam.GetComponent<Interactor>().Start();

                playerHealth.HealInvincibility = 5;
                playerHealth.HealPlayer(true);

                serEffects.FindProperty("_currentInsanity").intValue = 0;
                serEffects.FindProperty("_currentFrostbite").intValue = 0;
                serEffects.ApplyModifiedProperties();
                playerEffects.Start();
                playerController.Start();
                playerInventory.Awake();
                playerLantern.Start();
            }

            if (GUILayout.Button("Make unkillable",importantButtonStyle) && CheckIfRunning())
            {
                playerHealth.HealInvincibility = 9999999;
                playerHealth.HealPlayer(true);
                playerEffects.InsanityDeath = 9999;
                playerEffects.FrostbiteDeath = 9999;
            }

            GUILayout.Space(5);
            if (GUILayout.Button("Heal Player") && CheckIfRunning())
            {
                playerHealth.HealPlayer(true);
            }

            if (GUILayout.Button("Reset effects") && CheckIfRunning())
            {
                SerializedObject serEffects = new SerializedObject(playerEffects);
                serEffects.FindProperty("_currentInsanity").intValue = 0;
                serEffects.FindProperty("_currentFrostbite").intValue = 0;
                serEffects.ApplyModifiedProperties();
            }

            if (showDetails)
            {
                GUILayout.Space(15);
                GUILayout.Label("Living: Status effects", header2Style);
                if (GUILayout.Button("Infinite Freezing") && CheckIfRunning())
                {
                   playerEffects.FrostbiteDeath = 9999;
                }
                if (GUILayout.Button("Player immune to lantern") && CheckIfRunning())
                {
                    SerializedObject serLamp = new SerializedObject(playerLantern);
                    serLamp.FindProperty("_playerEffects").objectReferenceValue = null;
                    serLamp.ApplyModifiedProperties();
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

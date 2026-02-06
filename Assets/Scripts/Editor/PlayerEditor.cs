using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gameplay
{
    public class PlayerEditor : EditorWindow
    {
        private bool showDetails = false;
        private bool showFun = false;

        [MenuItem("Tools/PlayerEditor")]
        public static void ShowWindow()
        {
            GetWindow<PlayerEditor>("Player Editor");
        }
        int selectedTab = 0;
        string[] allTabs = { "Player", "Game", "Misc" };

        private void OnHierarchyChange()
        {
            UpdateVariables();
        }

        private void OnInspectorUpdate()
        {
            switch (allTabs[selectedTab])
            {
                case "Player":
                    //get all items
                    string[] guids = AssetDatabase.FindAssets("t:InventoryItem", new[] { "Assets/ScriptableObjects/Items" });
                    List<InventoryItem> items = guids
                        .Select(guid => AssetDatabase.LoadAssetAtPath<InventoryItem>(AssetDatabase.GUIDToAssetPath(guid)))
                        .ToList();

                    itemNames = new string[items.Count];

                    for (int i = 0; i < items.Count; i++)
                    {
                        if (items[i].HoldObject == null)
                        {
                            items.RemoveAt(i);
                            i = -1;
                            itemNames = new string[items.Count];
                            continue;
                        }
                        else
                        {
                            itemNames[i] = items[i].name;
                        }
                    }
                    break;
                case "Game":
                    string[] eventGuids = AssetDatabase.FindAssets("t:EventClass", new[] { "Assets/ScriptableObjects/Events" });
                    events = eventGuids
                        .Select(guid => AssetDatabase.LoadAssetAtPath<EventClass>(AssetDatabase.GUIDToAssetPath(guid)))
                        .ToList();

                    eventNames = new string[events.Count];

                    for (int i = 0; i < events.Count; i++)
                    {
                        eventNames[i] = events[i].name;
                    }
                    break;
                case "Misc":
                    break;
                default:
                    Debug.Log("That page doesnt exist");
                    break;
            }
        }
        private void UpdateVariables()
        {
            baseGen = GameObject.Find("BaseGeneration");
            if (!baseGen) { return; }
            switch (allTabs[selectedTab])
            {
                case "Player":
                    if (player == null) { player = GameObject.Find("Player"); }
                    if (playerEffects == null) { playerEffects = player.GetComponent<PlayerStatusEffects>(); }
                    if (playerHealth == null) { playerHealth = player.GetComponent<PlayerHealth>(); }
                    if (playerController == null) { playerController = player.GetComponent<PlayerController>(); }
                    if (playerInventory == null) { playerInventory = player.GetComponent<PlayerInventory>(); }
                    if (playerLantern == null) { playerLantern = player.transform.Find("PlayerCamera").Find("Lantern").GetComponent<FreezingLantern>(); }
                    break;
                case "Game":
                    if (CheckIfRunning(true))
                    {
                        allRooms = baseGen.GetComponentsInChildren<CarriageClass>().ToList();
                    }
                    break;
                case "Misc":
                    break;
                default:
                    Debug.Log("That page doesnt exist");
                    break;
            }
        }

        Vector2 scrollPos;
        public void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
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
            EditorGUILayout.EndScrollView();
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

        int selectedItem = 0;
        public string[] itemNames;
        List<EventClass> events;
        private void PlayerPage()
        {
            if(!GameObject.Find("BaseGeneration")) { return; }
            
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
                playerHealth.DamagePlayer("Skill issue");
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
                playerController.CurrentStamina = playerController.TotalStamina;
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

                GUILayout.Space(15);
                GUILayout.Label("Living: Speed", header2Style);
                if (GUILayout.Button("Infinite Stamina") && CheckIfRunning())
                {
                    playerController.TotalStamina = 999999999;
                    playerController.CurrentStamina = 999999999;
                }
                if (GUILayout.Button("Normal speed") && CheckIfRunning())
                {
                    playerController.BaseSpeed = 4f;
                    playerController.SprintSpeed = 5.5f;
                    playerController.CrouchSpeed = 3f;
                }
                if (GUILayout.Button("High speed") && CheckIfRunning())
                {
                    playerController.BaseSpeed = 6.5f;
                    playerController.SprintSpeed = 8;
                    playerController.CrouchSpeed = 5.5f;
                }
                if (showFun)
                {
                    if (GUILayout.Button("Very High speed") && CheckIfRunning())
                    {
                        playerController.BaseSpeed = 12f;
                        playerController.SprintSpeed = 20;
                        playerController.CrouchSpeed = 10f;
                    }
                }
            }

            GUILayout.Space(20);
            GUILayout.Label("Inventory", headerStyle);
            if (GUILayout.Button("Clear Inventory",notWorkingButton) && CheckIfRunning())
            {
                //Add later
            }
            GUILayout.Space(15);
            GUILayout.Label("Inventory: Item specific", header2Style);
            selectedItem = EditorGUILayout.Popup(selectedItem, itemNames);
            if (GUILayout.Button("Give item", notWorkingButton) && CheckIfRunning())
            {
                //Add later
            }
            if (GUILayout.Button("Remove item", notWorkingButton) && CheckIfRunning())
            {
                //Add later
            }

            GUILayout.Space(20);
            GUILayout.Label("Player Location", headerStyle);

            if (GUILayout.Button("TP to start of current room", notWorkingButton) && CheckIfRunning())
            {
                //Add later
            }
            if (GUILayout.Button("TP to start", notWorkingButton) && CheckIfRunning())
            {
                //Add later
            }
            if (GUILayout.Button("TP to last room", notWorkingButton) && CheckIfRunning())
            {
                //Add later
            }
        }

        GameObject baseGen;
        List<CarriageClass> allRooms;
        int selectedEvent = 0;
        public string[] eventNames;
        bool allItemSpots;
        private void GamePage()
        {
            if (!GameObject.Find("BaseGeneration")) { return; }

            GUILayout.Label("Game", titleStyle);
            showDetails = EditorGUILayout.Toggle("Detailed options", showDetails);
            if (GUILayout.Button("Skip Cutscene but button", importantButtonStyle) && CheckIfRunning())
            {
                FindAnyObjectByType<CutsceneManager>().StopStartingCutscene();
            }
            GUILayout.Space(20);
            GUILayout.Label("Events", headerStyle);

            if (GUILayout.Button("Remove all events", importantButtonStyle) && CheckIfRunning())
            {
                for (int j = 0; j < allRooms.Count; j++)
                {
                    SerializedObject serRoom = new SerializedObject(allRooms[j]);
                    for (int i = 0; i < allRooms[j]._selectedEventClasses.Count; i++)
                    {
                        if(serRoom.FindProperty("_enterTriggered").boolValue && !serRoom.FindProperty("_exitTriggered").boolValue)
                        {
                            allRooms[j]._selectedEventClasses[i].Exited(allRooms[j]);
                        }
                        allRooms[j]._selectedEventClasses[i].CallForDeletion(allRooms[j]);
                    }
                    allRooms[j]._selectedEventClasses = new List<EventClass>(serRoom.FindProperty("_amountOfEvents").intValue);
                    serRoom.ApplyModifiedProperties();
                }
            }

            GUILayout.Space(15);
            GUILayout.Label("Events: specific", header2Style);
            selectedEvent = EditorGUILayout.Popup(selectedEvent, eventNames);
            if (GUILayout.Button("Add event to all rooms") && CheckIfRunning())
            {
                for (int j = 0; j < allRooms.Count; j++)
                {
                    allRooms[j]._selectedEventClasses.Add(events[selectedEvent]);
                    events[selectedEvent].Generated(allRooms[j]);
                }
            }
            if (showFun)
            {
                if (GUILayout.Button("Add event to all rooms 10 times") && CheckIfRunning())
                {
                    for (int j = 0; j < allRooms.Count; j++)
                    {
                        for(int i = 0; i < 10; i++)
                        {
                            allRooms[j]._selectedEventClasses.Add(events[selectedEvent]);
                            events[selectedEvent].Generated(allRooms[j]);
                        }
                    }
                }
                if (GUILayout.Button("All in one") && CheckIfRunning())
                {
                    for (int j = 0; j < allRooms.Count; j++)
                    {
                        for (int i = 0; i < events.Count; i++)
                        {
                            if (eventNames[i] == "EndingEvent") {continue;}
                            allRooms[j]._selectedEventClasses.Add(events[i]);
                            events[i].Generated(allRooms[j]);
                        }
                    }
                }
            }
            if (GUILayout.Button("Remove event from all rooms") && CheckIfRunning())
            {
                for (int j = 0; j < allRooms.Count; j++)
                {
                    for (int i = 0; i < allRooms[j]._selectedEventClasses.Count; i++)
                    {
                        if (allRooms[j]._selectedEventClasses[i] == events[selectedEvent])
                        {
                            SerializedObject serRoom = new SerializedObject(allRooms[j]);
                            if (serRoom.FindProperty("_enterTriggered").boolValue && !serRoom.FindProperty("_exitTriggered").boolValue)
                            {
                                allRooms[j]._selectedEventClasses[i].Exited(allRooms[j]);
                            }
                            allRooms[j]._selectedEventClasses[i].CallForDeletion(allRooms[j]);
                            allRooms[j]._selectedEventClasses.RemoveAt(i);
                        }
                    }
                }
            }
            GUILayout.Space(20);
            GUILayout.Label("Items", headerStyle);
            allItemSpots = EditorGUILayout.Toggle("AllSpots", allItemSpots);

            if (GUILayout.Button("RespawnRoomItems") && CheckIfRunning())
            {
                for (int j = 0; j < allRooms.Count; j++)
                {
                    if (allItemSpots)
                    {
                        allRooms[j].DespawnItems();
                        allRooms[j].SpawnItemsAllSlots();
                    }
                    else
                    {
                        allRooms[j].DespawnItems();
                        allRooms[j].SpawnItems();
                    }
                }

            }
            if (GUILayout.Button("SpawnRoomItems") && CheckIfRunning())
            {
                for (int j = 0; j < allRooms.Count; j++)
                {
                    if (allItemSpots)
                    {
                        allRooms[j].SpawnItemsAllSlots();
                    }
                    else
                    {
                        allRooms[j].SpawnItems();
                    }
                }
            }
            if (GUILayout.Button("DespawnRoomItems") && CheckIfRunning())
            {
                for (int j = 0; j < allRooms.Count; j++)
                { 
                    allRooms[j].DespawnItems();
                }
            }
        }

        private void MiscPage()
        {
            if (!GameObject.Find("BaseGeneration")) { return; }

            GUILayout.Label("Misc", titleStyle);
            showDetails = EditorGUILayout.Toggle("Detailed options", showDetails);
            GUILayout.Space(20);
            GUILayout.Label("Options", headerStyle);

            showFun = EditorGUILayout.Toggle("Show fun options", showFun);
        }

        private bool CheckIfRunning(bool shutup = false)
        {
            if (Application.isPlaying)
            {
                return true;
            }
            else
            {
                if (!shutup)
                {
                    Debug.Log("You cannot run this command outside of play mode");
                }
                return false;
            }
        }
    }
}

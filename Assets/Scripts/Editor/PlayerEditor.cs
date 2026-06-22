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
            GetWindow<PlayerEditor>("Testing Utils");
        }
        int selectedTab = 0;
        string[] allTabs = { "Player", "Game","Editor", "Misc" };

        private void OnInspectorUpdate()
        {
            switch (allTabs[selectedTab])
            {
                case "Player":
                    //get all items
                    string[] guids = AssetDatabase.FindAssets("t:InventoryItem", new[] { "Assets/ScriptableObjects/Items" });
                    items = guids
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
                    string[] eventGuids = AssetDatabase.FindAssets("t:EventClassScriptable", new[] { "Assets/ScriptableObjects/Events" });
                    events = eventGuids
                        .Select(guid => AssetDatabase.LoadAssetAtPath<EventClassScriptable>(AssetDatabase.GUIDToAssetPath(guid)))
                        .ToList();

                    eventNames = new string[events.Count];

                    for (int i = 0; i < events.Count; i++)
                    {
                        eventNames[i] = events[i].name;
                    }

                    string[] roomtypeGuids = AssetDatabase.FindAssets("t:RoomTypeScriptable", new[] { "Assets/ScriptableObjects/RoomTypes" });
                    roomtypes = roomtypeGuids
                        .Select(guid => AssetDatabase.LoadAssetAtPath<RoomTypeScriptable>(AssetDatabase.GUIDToAssetPath(guid)))
                        .ToList();

                    roomtypeNames = new string[roomtypes.Count];

                    for (int i = 0; i < roomtypes.Count; i++)
                    {
                        roomtypeNames[i] = roomtypes[i].name;
                    }

                    rooms = new();
                    for (int i = 0; i < roomtypes[selectedRoomType].AllRoomsInType.Length; i++)
                    {
                        rooms.Add(roomtypes[selectedRoomType].AllRoomsInType[i]);
                    }
                    roomNames = new string[rooms.Count];
                    for (int i = 0; i < rooms.Count; i++)
                    {
                        roomNames[i] = rooms[i].RoomName;
                    }

                    break;
                case "Editor":
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
            baseGen = Generation.mainInstance;
            if (!baseGen) { return; }
            switch (allTabs[selectedTab])
            {
                case "Player":
                    if (CheckIfRunning(true))
                    {
                        if (player == null) { player = PlrRefs.inst.gameObject; }
                        if (playerEffects == null) { playerEffects = PlrRefs.inst.PlayerStatusEffects; }
                        if (playerHealth == null) { playerHealth = PlrRefs.inst.PlayerHealth; }
                        if (playerController == null) { playerController = PlrRefs.inst.PlayerController; }
                        if (playerInventory == null) { playerInventory = PlrRefs.inst.PlayerInventory; }
                        if (playerLantern == null) { playerLantern = PlrRefs.inst.FreezingLantern; }
                    }
                    break;
                case "Game":
                    if (CheckIfRunning(true))
                    {
                        allRooms = baseGen._initializedRoomGroups[0]._initializedCarriages;
                    }
                    break;
                case "Editor":
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
            selectedTab = GUILayout.SelectionGrid(selectedTab, allTabs, 4);
            GUILayout.Space(20);
            switch (allTabs[selectedTab])
            {
                case "Player":
                    PlayerPage();
                    break;
                case "Game":
                    GamePage();
                    break;
                case "Editor":
                    EditorPage(); 
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
        List<InventoryItem> items;
        List<EventClassScriptable> events;
        List<RoomTypeScriptable> roomtypes;
        List<RoomClass> rooms;
        private void PlayerPage()
        {   
            GUILayout.Label("Player", titleStyle);
            showDetails = EditorGUILayout.Toggle("Detailed options", showDetails);
            GUILayout.Space(20);
            GUILayout.Label("Living", headerStyle);

            if (GUILayout.Button("Revive", importantButtonStyle) && CheckIfRunning())
            {
                UpdateVariables();
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
                UpdateVariables();
                playerHealth.HealInvincibility = 9999999;
                playerHealth.DamagePlayer("Skill issue");
                playerHealth.HealPlayer(true);
                playerEffects.InsanityDeath = 9999;
                playerEffects.FrostbiteDeath = 9999;
            }

            GUILayout.Space(5);
            if (GUILayout.Button("Heal Player") && CheckIfRunning())
            {
                UpdateVariables();
                playerHealth.HealPlayer(true);
            }

            if (GUILayout.Button("Reset effects") && CheckIfRunning())
            {
                UpdateVariables();
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
                    UpdateVariables();
                    playerEffects.FrostbiteDeath = 9999;
                }
                if (GUILayout.Button("Player immune to lantern") && CheckIfRunning())
                {
                    UpdateVariables();
                    SerializedObject serLamp = new SerializedObject(playerLantern);
                    serLamp.FindProperty("_playerEffects").objectReferenceValue = null;
                    serLamp.ApplyModifiedProperties();
                }
                if (GUILayout.Button("Infinite Sanity") && CheckIfRunning())
                {
                    UpdateVariables();
                    playerEffects.InsanityDeath = 9999;
                }

                GUILayout.Space(15);
                GUILayout.Label("Living: Speed", header2Style);
                if (GUILayout.Button("Infinite Stamina") && CheckIfRunning())
                {
                    UpdateVariables();
                    playerController.TotalStamina = 999999999;
                    playerController.CurrentStamina = 999999999;
                }
                if (GUILayout.Button("No Crouch Debuff") && CheckIfRunning())
                {
                    UpdateVariables();
                    playerController.CrouchSpeed = playerController.BaseSpeed;
                }
                if (GUILayout.Button("Normal speed") && CheckIfRunning())
                {
                    UpdateVariables();
                    playerController.BaseSpeed = 4f;
                    playerController.SprintSpeed = 5.5f;
                    playerController.CrouchSpeed = 3f;
                }
                if (GUILayout.Button("High speed") && CheckIfRunning())
                {
                    UpdateVariables();
                    playerController.BaseSpeed = 6.5f;
                    playerController.SprintSpeed = 8;
                    playerController.CrouchSpeed = 5.5f;
                }
                if (GUILayout.Button("Very High speed") && CheckIfRunning())
                {
                    UpdateVariables();
                    playerController.BaseSpeed = 12f;
                    playerController.SprintSpeed = 20;
                    playerController.CrouchSpeed = 10f;
                }
                if (showFun)
                {
                    if (GUILayout.Button("Insane speed") && CheckIfRunning())
                    {
                        UpdateVariables();
                        playerController.BaseSpeed = 20f;
                        playerController.SprintSpeed = 30;
                        playerController.CrouchSpeed = 15f;
                    }
                }
            }

            GUILayout.Space(20);
            GUILayout.Label("Inventory", headerStyle);
            if (GUILayout.Button("Clear Inventory") && CheckIfRunning())
            {
                UpdateVariables();
                playerInventory.ClearInventory();
            }
            GUILayout.Space(15);
            GUILayout.Label("Inventory: Item specific", header2Style);
            selectedItem = EditorGUILayout.Popup(selectedItem, itemNames);
            if (GUILayout.Button("Give item") && CheckIfRunning())
            {
                UpdateVariables();
                PlrRefs.inst.PlayerInventory.PickupItem(items[selectedItem]);
            }
            if (showDetails)
            {
                if (GUILayout.Button("Fill inventory with item") && CheckIfRunning())
                {
                    UpdateVariables();
                    PlrRefs.inst.PlayerInventory.PickupItem(items[selectedItem]);
                    PlrRefs.inst.PlayerInventory.PickupItem(items[selectedItem]);
                    PlrRefs.inst.PlayerInventory.PickupItem(items[selectedItem]);
                    PlrRefs.inst.PlayerInventory.PickupItem(items[selectedItem]);
                    PlrRefs.inst.PlayerInventory.PickupItem(items[selectedItem]);
                }
            }
            if (GUILayout.Button("Remove item") && CheckIfRunning())
            {
                UpdateVariables();
                playerInventory.RemoveSpecificItem(items[selectedItem]);
            }

            GUILayout.Space(20);
            GUILayout.Label("Player Location", headerStyle);
            if (GUILayout.Button("TP to start") && CheckIfRunning())
            {
                UpdateVariables();
                playerController.transform.position = baseGen._initializedRoomGroups[0]._initializedCarriages[0].PlayerSpawnPoint.transform.position;
            }
            if (GUILayout.Button("TP to next room") && CheckIfRunning())
            {
                UpdateVariables();
                playerController.transform.position = baseGen._initializedRoomGroups[0]._initializedCarriages[playerController.CurrentCarriage.roomIndex + 1].EntryPoint.transform.position + new Vector3(0, 1.5f, 0);
            }
            if (GUILayout.Button("TP to previous room") && CheckIfRunning())
            {
                UpdateVariables();
                playerController.transform.position = baseGen._initializedRoomGroups[0]._initializedCarriages[playerController.CurrentCarriage.roomIndex - 1].EntryPoint.transform.position + new Vector3(0, 1.5f, 0);
            }
            if (showDetails)
            {
                if (GUILayout.Button("TP to start of current room") && CheckIfRunning())
                {
                    UpdateVariables();
                    playerController.transform.position = playerController.CurrentCarriage.EntryPoint.transform.position + new Vector3(0, 1.5f, 0);
                }
                if (GUILayout.Button("TP to end of current room") && CheckIfRunning())
                {
                    UpdateVariables();
                    playerController.transform.position = playerController.CurrentCarriage.ExitPoint.transform.position + new Vector3(0, 1.5f, 0);
                }
            }
        }














        Generation baseGen;
        List<CarriageClass> allRooms;
        int selectedEvent = 0;
        int selectedRoomType = 0;
        int selectedRoom = 0;
        public string[] eventNames;
        public string[] roomtypeNames;
        public string[] roomNames;
        bool allItemSpots;
        private void GamePage()
        {
            if (!GameObject.Find("BaseGeneration")) { return; }

            GUILayout.Label("Game", titleStyle);
            showDetails = EditorGUILayout.Toggle("Detailed options", showDetails);
            if (GUILayout.Button("Regen Rooms", importantButtonStyle) && CheckIfRunning())
            {
                UpdateVariables();
                baseGen.RegenerateRooms();
            }

            if (showDetails)
            {
                GUILayout.Space(20);
                GUILayout.Label("DIY Rooms", headerStyle);
                if (GUILayout.Button("SETUP") && CheckIfRunning())
                {
                    UpdateVariables();
                    baseGen.SETUPManualPlacing(roomtypes[selectedRoomType].RoomTypeStartRoom);
                }
                GUILayout.Space(10);
                selectedRoomType = EditorGUILayout.Popup(selectedRoomType, roomtypeNames);
                if (GUILayout.Button("Clear Rooms") && CheckIfRunning())
                {
                    UpdateVariables();
                    baseGen.ClearRooms();
                }

                GUILayout.Space(10);
                selectedRoom = EditorGUILayout.Popup(selectedRoom, roomNames);
                if (GUILayout.Button("Add room") && CheckIfRunning())
                {
                    UpdateVariables();
                    baseGen.AddRoom(rooms[selectedRoom]);
                }
                if (GUILayout.Button("Remove previous room") && CheckIfRunning())
                {
                    UpdateVariables();
                    baseGen.RemoveRoom();
                }
            }
            GUILayout.Space(20);
            GUILayout.Label("Events", headerStyle);

            if (GUILayout.Button("Remove all events", importantButtonStyle) && CheckIfRunning())
            {
                UpdateVariables();
                for (int j = 0; j < allRooms.Count; j++)
                {
                    SerializedObject serRoom = new SerializedObject(allRooms[j]);
                    for (int i = 0; i < allRooms[j].spawnedEventClasses.Count; i++)
                    {
                        if (serRoom.FindProperty("_enterTriggered").boolValue && !serRoom.FindProperty("_exitTriggered").boolValue)
                        {
                            allRooms[j].spawnedEventClasses[i].FirstExit(allRooms[j]);
                        }
                        allRooms[j].spawnedEventClasses[i].CallForDeletion(allRooms[j]);
                    }
                    allRooms[j].spawnedEventClasses = new List<EventClass>(0);
                    serRoom.ApplyModifiedProperties();
                }
            }

            GUILayout.Space(15);
            GUILayout.Label("Events: specific", header2Style);
            selectedEvent = EditorGUILayout.Popup(selectedEvent, eventNames);
            if (GUILayout.Button("Add event to all rooms") && CheckIfRunning())
            {
                UpdateVariables();
                for (int j = 0; j < allRooms.Count; j++)
                {
                    Generation.AddEventToRoom(allRooms[j], events[selectedEvent]);
                }
            }
            if (GUILayout.Button("Add event to current room") && CheckIfRunning())
            {
                UpdateVariables();
                Generation.AddEventToRoom(PlrRefs.inst.PlayerController.CurrentCarriage, events[selectedEvent], true);
            }
            if (showDetails)
            {
                if (GUILayout.Button("Add event to Room 1 specifically") && CheckIfRunning())
                {
                    UpdateVariables();
                    Generation.AddEventToRoom(allRooms[0], events[selectedEvent]);
                }
            }
            if (showFun)
            {
                if (GUILayout.Button("Add event to all rooms 10 times") && CheckIfRunning())
                {
                    UpdateVariables();
                    for (int j = 0; j < allRooms.Count; j++)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            Generation.AddEventToRoom(allRooms[j], events[selectedEvent]);
                        }
                    }
                }
                if (GUILayout.Button("All in one") && CheckIfRunning())
                {
                    UpdateVariables();
                    for (int j = 0; j < allRooms.Count; j++)
                    {
                        for (int i = 0; i < events.Count; i++)
                        {
                            if (events[i].IncludeInAllInOne == false) { continue; }
                            Generation.AddEventToRoom(allRooms[j], events[i]);
                        }
                    }
                }
            }
            if (GUILayout.Button("Remove event from all rooms") && CheckIfRunning())
            {
                UpdateVariables();
                for (int j = 0; j < allRooms.Count; j++)
                {
                    for (int i = 0; i < allRooms[j].spawnedEventClasses.Count; i++)
                    {
                        if (allRooms[j].spawnedEventClasses[i] == events[selectedEvent])
                        {
                            SerializedObject serRoom = new SerializedObject(allRooms[j]);
                            if (serRoom.FindProperty("_enterTriggered").boolValue && !serRoom.FindProperty("_exitTriggered").boolValue)
                            {
                                allRooms[j].spawnedEventClasses[i].FirstExit(allRooms[j]);
                            }
                            allRooms[j].spawnedEventClasses[i].CallForDeletion(allRooms[j]);
                            allRooms[j].spawnedEventClasses.RemoveAt(i);
                        }
                    }
                }
            }
            if (GUILayout.Button("Remove all events except selected",notWorkingButton) && CheckIfRunning())
            {
                UpdateVariables();
                for (int j = 0; j < allRooms.Count; j++)
                {
                    //stupid workaround, list removeat makes things go down so some events woulnd get deleted.
                    SerializedObject serRoom = new SerializedObject(allRooms[j]);
                    for (int i = 0; i < allRooms[j].spawnedEventClasses.Count; i++)
                    {
                        if (allRooms[j].spawnedEventClasses[i] == events[selectedEvent]) { continue; }

                        if (serRoom.FindProperty("_enterTriggered").boolValue && !serRoom.FindProperty("_exitTriggered").boolValue)
                        {
                            allRooms[j].spawnedEventClasses[i].FirstExit(allRooms[j]);
                        }
                        allRooms[j].spawnedEventClasses[i].CallForDeletion(allRooms[j]);
                        allRooms[j].spawnedEventClasses.RemoveAt(i);
                    }
                    for (int i = 0; i < allRooms[j].spawnedEventClasses.Count; i++)
                    {
                        if (allRooms[j].spawnedEventClasses[i] == events[selectedEvent]) { continue; }

                        if (serRoom.FindProperty("_enterTriggered").boolValue && !serRoom.FindProperty("_exitTriggered").boolValue)
                        {
                            allRooms[j].spawnedEventClasses[i].FirstExit(allRooms[j]);
                        }
                        allRooms[j].spawnedEventClasses[i].CallForDeletion(allRooms[j]);
                        allRooms[j].spawnedEventClasses.RemoveAt(i);
                    }
                    for (int i = 0; i < allRooms[j].spawnedEventClasses.Count; i++)
                    {
                        if (allRooms[j].spawnedEventClasses[i] == events[selectedEvent]) { continue; }

                        if (serRoom.FindProperty("_enterTriggered").boolValue && !serRoom.FindProperty("_exitTriggered").boolValue)
                        {
                            allRooms[j].spawnedEventClasses[i].FirstExit(allRooms[j]);
                        }
                        allRooms[j].spawnedEventClasses[i].CallForDeletion(allRooms[j]);
                        allRooms[j].spawnedEventClasses.RemoveAt(i);
                    }
                    serRoom.ApplyModifiedProperties();
                }
            }

            if (showDetails)
            {
                GUILayout.Space(20);
                GUILayout.Label("Visuals", headerStyle);
                if (GUILayout.Button("Remove carriage visual from all rooms") && CheckIfRunning())
                {
                    UpdateVariables();
                    for (int j = 0; j < allRooms.Count; j++)
                    {
                        allRooms[j].transform.Find("Visuals").Find("Carriage")?.gameObject.SetActive(false);
                    }
                }
            }
            GUILayout.Space(20);
            GUILayout.Label("Items", headerStyle);
            allItemSpots = EditorGUILayout.Toggle("AllSpots", allItemSpots);

            if (GUILayout.Button("RespawnRoomItems") && CheckIfRunning())
            {
                UpdateVariables();
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
                UpdateVariables();
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
                UpdateVariables();
                for (int j = 0; j < allRooms.Count; j++)
                {
                    allRooms[j].DespawnItems();
                }
            }
        }









        static bool allSpotsOn = false;
        static bool itemsOn = false;
        static bool boxOn = false;
        static bool nodesOn = false;
        static bool ticketOn = false;
        private void EditorPage()
        {
            GUILayout.Label("Editor", titleStyle);
            showDetails = EditorGUILayout.Toggle("Detailed options", showDetails);

            GUILayout.Label("Room prefabs", headerStyle);
            if (!itemsOn && !ticketOn && !boxOn && !nodesOn)
            {
                allSpotsOn = PrefabVisible.ChangeObjStates(EditorGUILayout.Toggle("Show Everything in Prefab", allSpotsOn));
                if (allSpotsOn) { GUILayout.Space(80); }
            }
            if (!allSpotsOn)
            {
                if (itemsOn || ticketOn || boxOn || nodesOn) { GUILayout.Space(20); }
                itemsOn = PrefabVisible.ChangeObjStatesWName(EditorGUILayout.Toggle("Show Item Spots in Prefab", itemsOn), "Items");
                ticketOn = PrefabVisible.ChangeObjStatesWName(EditorGUILayout.Toggle("Show Ticket Spots in Prefab", ticketOn), "Tickets");
                boxOn = PrefabVisible.ChangeObjStatesWName(EditorGUILayout.Toggle("Show Box Spots in Prefab", boxOn), "Box");
                nodesOn = PrefabVisible.ChangeObjStatesWName(EditorGUILayout.Toggle("Show Nodes in Prefab", nodesOn), "Nodes");
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

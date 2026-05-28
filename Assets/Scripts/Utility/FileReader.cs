using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Gameplay
{
    public class FileReader : MonoBehaviour
    {
        public static FileReader Instance;

        public EventRefs Events;

        private EnemyDeathMessageFile data = new EnemyDeathMessageFile();
        private void Start()
        {
            Instance = this;
            TextAsset json = Resources.Load<TextAsset>("EnemyDeathText");
            JsonUtility.FromJsonOverwrite(json.text, data);
        }

        public static string GetDeathMessage(string enemyType, string deathType)
        {
            string[] messages = Instance.GetMessage(enemyType, deathType);
            int chosenMessage = Random.Range(0, messages.Length);
            return messages[chosenMessage];
        }

        public string[] GetMessage(string enemyType, string deathType)
        {
            if (deathType == "Default")
            {
                deathType = CheckDeathSpecialty(enemyType, out enemyType);
            }

            EnemyDeathInfo enemy = data.AllEnemyDeathInfo.FirstOrDefault(file => file.EnemyType == enemyType);
            if (enemy == null)
            {
                enemy = data.AllEnemyDeathInfo.FirstOrDefault(file => file.EnemyType == "FallBack");
                deathType = "Special1";
            }

            FieldInfo variable = typeof(EnemyDeathMessages).GetField(deathType);
            if(variable == null)
            {
                string[] doesntExist = new string[] { "Wow we are missing death messages","Whoops, How did you....?" };
                return doesntExist;
            }

            return (string[])variable.GetValue(enemy.Messages);
        }

        public string CheckDeathSpecialty(string enemyType, out string newEnemyType)
        {
            newEnemyType = enemyType;
            if (!Events) { return "Default"; }
            CarriageClass room = PlrRefs.inst.PlayerController.CurrentCarriage;
            switch (enemyType)
            {
                case "AllEars":
                    //Allears and Shadowman combo message
                    if (room._selectedEventClasses.Contains(Events.EventClassScriptables[5]))
                    {
                        newEnemyType = "AllEars and Shadowman";
                        return "Special1";
                    //Allears and Broken Windows combo message
                    }else if (room._selectedEventClasses.Contains(Events.EventClassScriptables[9]))
                    {
                        newEnemyType = "AllEars and Windows";
                        return "Special1";
                    }
                    return "Default";


                case "Frostbite: Windows":
                    //Allears and Broken Windows combo message
                    if (room._selectedEventClasses.Contains(Events.EventClassScriptables[0]))
                    {
                        newEnemyType = "AllEars and Windows";
                        return "Special2";
                    }
                    //Broken Windows and Shadowman combo message
                    else if (room._selectedEventClasses.Contains(Events.EventClassScriptables[5]))
                    {
                        newEnemyType = "Shadowman and Windows";
                        return "Special1";
                    }
                    return "Default";


                case "Shadowman":
                    //Allears and Shadowman combo message
                    if (room._selectedEventClasses.Contains(Events.EventClassScriptables[0]))
                    {
                        newEnemyType = "AllEars and Shadowman";
                        return "Special2";
                    }
                    //Broken Windows and Shadowman combo message
                    else if (room._selectedEventClasses.Contains(Events.EventClassScriptables[9]))
                    {
                        newEnemyType = "Shadowman and Windows";
                        return "Special2";
                    }
                    return "Default";


                case "Tickets Please":
                    //Already grabbed ticket special message
                    if (PlrRefs.inst.PlayerMonsterManager.HasFoundTicket)
                    {
                        return "Special1";
                    }
                    return "Default";
                case "Frostbite: Lantern":
                    //Froze self while freezing hot dude
                    if (room._selectedEventClasses.Contains(Events.EventClassScriptables[2]))
                    {
                        return "Special1";
                    }
                    return "Default";
            }
            return "Default";
        }
    }

    [System.Serializable]
    public class EnemyDeathMessageFile
    {
        public EnemyDeathInfo[] AllEnemyDeathInfo;
    }

    [System.Serializable]
    public class EnemyDeathInfo
    {
        public string EnemyType;
        public EnemyDeathMessages Messages;
    }

    [System.Serializable]
    public class EnemyDeathMessages
    {
        public string[] Default;
        public string[] Special1;
        public string[] Special2;
    }
}

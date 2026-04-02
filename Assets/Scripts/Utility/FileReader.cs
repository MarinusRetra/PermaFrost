using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Gameplay
{
    public class FileReader : MonoBehaviour
    {
        public static FileReader Instance;

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
            EnemyDeathInfo enemy = data.AllEnemyDeathInfo.FirstOrDefault(file => file.EnemyType == enemyType);
            if (enemy == null)
            {
                enemy = data.AllEnemyDeathInfo.FirstOrDefault(file => file.EnemyType == "FallBack");
                deathType = "SpecialDeath1";
            }

            FieldInfo variable = typeof(EnemyDeathMessages).GetField(deathType);
            if(variable == null)
            {
                string[] doesntExist = new string[] { "Wow we are missing death messages","Whoops, How did you....?" };
                return doesntExist;
            }

            return (string[])variable.GetValue(enemy.Messages);
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
        public string[] FirstDeath;
        public string[] SecondDeath;
        public string[] DeathLoop;
        public string[] SpecialDeath1;
        public string[] SpecialDeath2;
    }
}

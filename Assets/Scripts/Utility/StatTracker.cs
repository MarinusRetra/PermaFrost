using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(fileName = "StatTracker", menuName = "StatTracker")]
    public class StatTracker : ScriptableObject
    {
        public List<InventoryItem> TrackedItems;

        private void OnEnable()
        {
            LoadStats();
        }

        public void PrintItems()
        {
            foreach (var item in TrackedItems)
            {
                Debug.Log($"Used {item.name} {item.TimesUsed} times.");
            }
        }

        public void SaveStats()
        {
            foreach(var item in TrackedItems)
            {
                PlayerPrefs.SetInt(item.name, item.TimesUsed);
            }
        }

        public void LoadStats()
        {
            foreach(var item in TrackedItems)
            {
                item.TimesUsed = PlayerPrefs.GetInt(item.name, item.TimesUsed);
            }
        }
    }
}

using TMPro;
using UnityEngine;

namespace Gameplay
{
    public class StatsManager : MonoBehaviour
    {
        [SerializeField] private StatTracker _statTrack;
        [SerializeField] private TextMeshProUGUI[] _StatNamesUI;
		[SerializeField] private TextMeshProUGUI[] _StatValuesUI;

        public void SetStatsUIText()
        {
            for (int i = 0; i < _statTrack.TrackedItems.Count; i++)
            {
                if(_statTrack.TrackedItems[i].TimesUsed > 0)
                {
                    _StatNamesUI[i].text = _statTrack.TrackedItems[i].name; 
                    _StatValuesUI[i].text = $"Times Used: {_statTrack.TrackedItems[i].TimesUsed.ToString()}";
                    _StatNamesUI[i].gameObject.SetActive(true);
                    _StatValuesUI[i].gameObject.SetActive(true);
                }
            }
        }
    }
}

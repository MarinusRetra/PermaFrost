using UnityEngine;

namespace Gameplay
{
    [ExecuteAlways]
    public class PrefabObjVisible : MonoBehaviour
    {
        public string typeName;
        public bool IsOn = false;
        public bool TurnOnRendererIfNameFits(bool state, string name)
        {
            if(name == typeName)
            {
                TurnOnRenderers(state);
                return true;
            }
            return false;
        }
        public void TurnOnRenderers(bool state)
        {
            MeshRenderer[] children = transform.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < children.Length; i++)
            {
                children[i].enabled = state;
            }
            IsOn = state;
        }
    }
}

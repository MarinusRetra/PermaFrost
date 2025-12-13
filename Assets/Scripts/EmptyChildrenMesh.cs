using UnityEngine;

namespace Gameplay
{
    public class EmptyChildrenMesh : MonoBehaviour
    {
        void Start()
        {
            MeshRenderer[] children = transform.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < children.Length; i++)
            {
                children[i].enabled = false;
            }
        }
    }
}

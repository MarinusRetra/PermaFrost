using UnityEngine;

namespace Gameplay
{
    public class Node : MonoBehaviour
    {
        public enum NodeType { Transfer,Forcekill }
        public NodeType type;

        public Transform transferNodeHolder;

        [SerializeField] private CarriageClass parent;

        private void Start()
        {
            if (parent?.nextCarriage && type == NodeType.Transfer)
            {
                transferNodeHolder = parent.nextCarriage.NodeHolder;
            }
            else
            {
                type = NodeType.Forcekill;
            }
        }
    }
}

using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class Node : MonoBehaviour
    {
        public enum NodeType { Transfer,Forcekill,Random }
        public NodeType type;
        public bool TransferCounts = true;
        public bool TransferToNextRoom = true;

        public Transform transferNodeHolder;

        public Transform[] RandomNodeHolders;

        [SerializeField] private CarriageClass parent;

        private void Start()
        {
            if (TransferToNextRoom && type == NodeType.Transfer)
            {
                StartCoroutine(CheckCarriageJump());
            }
        }

        private IEnumerator CheckCarriageJump()
        {
            yield return new WaitForSeconds(5);
            if (parent == null)
            {
                if (transform.parent.parent.GetComponent<CarriageClass>() != null)
                {
                    parent = transform.parent.parent.GetComponent<CarriageClass>();
                }
            }
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

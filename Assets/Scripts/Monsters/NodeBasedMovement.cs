using System;
using UnityEngine;

namespace Gameplay
{
    public class NodeBasedMovement : MonoBehaviour
    {
        public float speed = 1;
        public int maxTransfers = 10;
        [SerializeField] private Rigidbody rb;
        private GameObject affectedObject;
        public Action onDeathAction;
        void Start()
        {
            if(rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }
            affectedObject = rb.gameObject;
        }

        public Transform nodes;
        public Transform nextObjective;
        public int currentIndex = 0;
        public void StartMoving(Transform nodeHolder)
        {
            transfers = 0;
            currentIndex = 0;
            Transform nodeOne = nodeHolder.GetChild(0);
            affectedObject.transform.position = nodeOne.position;
            nodes = nodeHolder;
            currentIndex = 1;
            if (NodeEffect(nodeOne)) { return; }
            MoveTowards(nodes.GetChild(currentIndex));
            Transform nextNode = nodes.GetChild(currentIndex);
            if (nextNode)
            {
                nextObjective = nextNode;
            }
        }

        private int transfers = 0;
        private void OnTriggerEnter(Collider other)
        {
            if (!nextObjective || !nodes) { return; }
            if(other.transform == nextObjective)
            {
                Transform oldNode = nextObjective;
                currentIndex++;
                if (oldNode && oldNode.GetComponent<Node>())
                {
                    if (NodeEffect(oldNode)) { return; }
                }
                Transform node = nodes.GetChild(currentIndex);
                if (node)
                {
                    MoveTowards(node);
                    Transform nextNode = nodes.GetChild(currentIndex);
                    if (nextNode)
                    {
                        nextObjective = nextNode;
                    }

                }
            }
        }

        private bool NodeEffect(Transform currentNodeObj)
        {
            if(!currentNodeObj.GetComponent<Node>()) { return false; }
            Node currentNode = currentNodeObj.GetComponent<Node>();
            switch (currentNode.type)
            {
                case Node.NodeType.Forcekill:
                    Obliterate();
                    return true;
                case Node.NodeType.Transfer:
                    if (currentNode.TransferCounts) { transfers++; }
                    if (transfers >= maxTransfers)
                    {
                        Obliterate();
                        return true;
                    }
                    nodes = currentNode.transferNodeHolder;
                    currentIndex = 0;
                    break;
                case Node.NodeType.Random:
                    nodes = currentNode.RandomNodeHolders[UnityEngine.Random.Range(0, currentNode.RandomNodeHolders.Length)];
                    currentIndex = 0;
                    break;
            }
            return false;
        }

        public void MoveTowards(Transform node)
        {
            Vector3 lookPos = node.position - affectedObject.transform.position;
            affectedObject.transform.rotation = Quaternion.LookRotation(lookPos);
            rb.angularVelocity = Vector3.zero;
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(lookPos.normalized * speed, ForceMode.Impulse);

        }
        private void Obliterate()
        {
            onDeathAction?.Invoke();
        }
    }
}

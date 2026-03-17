using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay
{
    public class NodeBasedMovement : MonoBehaviour
    {
        public Transform TempNodeInstant;
        public int speed = 1;
        [SerializeField] private Rigidbody rb;
        private GameObject affectedObject;
        void Start()
        {
            if(rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }
            affectedObject = rb.gameObject;
            StartMoving(TempNodeInstant);
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;
            if (keyboard.lKey.wasPressedThisFrame)
            {
                rb.angularVelocity = Vector3.zero;
                rb.linearVelocity = Vector3.zero;
                StartMoving(TempNodeInstant);
            }
        }

        private Transform nodes;
        public Transform nextObjective;
        public int currentIndex = 0;
        public void StartMoving(Transform nodeHolder)
        {
            currentIndex = 0;
            affectedObject.transform.position = nodeHolder.GetChild(0).position;
            nodes = nodeHolder;
            currentIndex = 1;
            MoveTowards(nodeHolder.GetChild(currentIndex));
            Transform nextNode = nodes.GetChild(currentIndex);
            if (nextNode)
            {
                nextObjective = nextNode;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!nextObjective || !nodes) { return; }
            if(other.transform == nextObjective)
            {
                Transform oldNode = nextObjective;
                currentIndex++;
                if (oldNode && oldNode.GetComponent<Node>())
                {
                    Node currentNode = oldNode.GetComponent<Node>();

                    switch (currentNode.type)
                    {
                        case Node.NodeType.Forcekill:
                            rb.angularVelocity = Vector3.zero;
                            rb.linearVelocity = Vector3.zero;
                            return;
                        case Node.NodeType.Transfer:
                            nodes = currentNode.transferNodeHolder;
                            currentIndex = 0;
                            break;
                    }
                }
                print(oldNode.name);
                Transform node = nodes.GetChild(currentIndex);
                if (node)
                {
                    affectedObject.transform.position = oldNode.position;
                    MoveTowards(node);
                    Transform nextNode = nodes.GetChild(currentIndex);
                    if (nextNode)
                    {
                        nextObjective = nextNode;
                    }

                }
            }
        }

        public void MoveTowards(Transform node)
        {
            Vector3 lookPos = node.position - affectedObject.transform.position;
            //lookPos.y = 0;
            affectedObject.transform.rotation = Quaternion.LookRotation(lookPos);
            rb.angularVelocity = Vector3.zero;
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(lookPos.normalized * speed, ForceMode.Impulse);

            print(currentIndex);

        }
    }
}

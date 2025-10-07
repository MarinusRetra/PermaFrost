using System;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class InteractObject : MonoBehaviour
    {
        private Material[] _materials;
        private bool _isHovered = false;
        [SerializeField] UnityEvent _interactEvent;
        [SerializeField] float _outlineScale;
        [SerializeField] Material _outlineMat;
        void Start()
        {
            _materials = GetComponent<MeshRenderer>().materials;
            if (_materials[^1].color == _outlineMat.color)
            {
                //Zet de laatste material's zijn submesh naar dezelfde submesh als de eerste submesh.
                Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
                mesh.SetSubMesh(mesh.subMeshCount - 1, mesh.GetSubMesh(0));
            }
        }
        public void Hover()
        {
            _isHovered = true;
        }

        void FixedUpdate()
        {
            _materials[^1].SetFloat("_Scale", _isHovered ? _outlineScale : 0f);
            _isHovered = false;
        }

        public void Interact()
        {
            _interactEvent.Invoke();
        }
    }
}
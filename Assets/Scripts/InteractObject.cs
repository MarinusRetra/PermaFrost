using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

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
                Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
                //mesh.SetSubMesh(mesh.subMeshCount - 1, mesh.GetSubMesh(0)); Oude deprecated manier om meshes te wisselen

                //Zet de laatste material's zijn submesh naar dezelfde submesh als de eerste submesh.
                List<SubMeshDescriptor> _descriptors = new();
                for (int i = 0; i < mesh.subMeshCount; i++)
                {
                    _descriptors.Add(mesh.GetSubMesh(i));
                }
                _descriptors[^1] = _descriptors[0];
                mesh.SetSubMeshes(_descriptors);
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
            print("Interacted");
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace Gameplay
{
	public class InteractionManager : MonoBehaviour
	{
		[SerializeField] private float interactDistance = 2f;
		[SerializeField] private LayerMask interactLayer;

		private Camera cam;

		public static Coroutine run;

		private void Awake()
		{
			cam = Camera.main;
		}

		private void Update()
		{
			if (Keyboard.current.eKey.wasPressedThisFrame && run == null)
			{
				TryInteract();
			}
		}

		private void TryInteract()
		{
			Ray ray = new Ray(cam.transform.position, cam.transform.forward);

			if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
			{
				DialogSource dialog = hit.collider.GetComponent<DialogSource>();
				if (dialog != null)
				{
					run = StartCoroutine(dialog.DialogRoutine());
					dialog = null;
				}
			}
		}

		private void OnDrawGizmosSelected()
		{
			if (cam == null)
				cam = Camera.main;

			if (cam == null)
				return;

			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(cam.transform.position, cam.transform.forward * interactDistance);
		}
	}
}

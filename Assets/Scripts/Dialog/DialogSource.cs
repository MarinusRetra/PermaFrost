using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;

namespace Gameplay
{
	public class DialogSource : MonoBehaviour
	{
		[TextArea(5, 10)]
		public List<string> DialogTexts;

		private int _index = 0;

		private GameObject _dialogParent;
		private TextMeshProUGUI _title;
		private TextMeshProUGUI _dialog;

		public static bool IsActive { get; private set; } = false;

		private void Awake()
		{
			_dialogParent = GameObject.Find("@parent");
			if (_dialogParent) print("Found");
		}

		public IEnumerator DialogRoutine()
		{
			GameObject child = _dialogParent.transform.GetChild(0).gameObject;
			child.SetActive(true);

			yield return new WaitUntil(() => _dialogParent == true);
			
			_title = GameObject.Find("@title").GetComponent<TextMeshProUGUI>();
			_dialog = GameObject.Find("@dialog").GetComponent<TextMeshProUGUI>();

			_title.text = gameObject.name;
			_dialog.text = DialogTexts[_index];

			while (true)
			{
				if (Keyboard.current.eKey.wasPressedThisFrame)
				{
					_index++;

					if (_index >= DialogTexts.Count)
					{
						_index = 0;
						child.SetActive(false);
						InteractionManager.run = null;
						yield break;
					}

					_dialog.text = DialogTexts[_index];
				}

				yield return null;
			}
		}
	}
}

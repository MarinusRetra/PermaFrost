using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;

namespace Gameplay
{
	public class DialogSource : MonoBehaviour
	{
		public List<DialogList> DialogTexts;

		private int _index = 0;

		private GameObject _dialogParent;
		private TextMeshProUGUI _title;
		private TextMeshProUGUI _dialog;

		public static bool IsActive { get; private set; } = false;

		private void Awake()
		{
			_dialogParent = GameObject.Find("@parent");
		}

		public IEnumerator DialogRoutine()
		{
			GameObject child = _dialogParent.transform.GetChild(0).gameObject;
			child.SetActive(true);

			yield return new WaitUntil(() => child == true);

			_title = GameObject.Find("@title").GetComponent<TextMeshProUGUI>();
			_dialog = GameObject.Find("@dialog").GetComponent<TextMeshProUGUI>();

			if (DialogTexts[_index].IsPlayer == false) _title.text = gameObject.name + ":".ToString();
			else _title.text = "Me:".ToString();

			_dialog.text = DialogTexts[_index].Text;

			while (true)
			{
				if (Keyboard.current.eKey.wasPressedThisFrame)
				{
					_index++;

					if (_index >= DialogTexts.Count)
					{
						child.SetActive(false);
						_index = 0;
						_title.text = "";
						_dialog.text = "";
						InteractionManager.run = null;
						yield break;
					}

					if (DialogTexts[_index].IsPlayer == false) _title.text = gameObject.name + ":".ToString();
					else _title.text = "Me:".ToString();

					_dialog.text = DialogTexts[_index].Text;
				}

				yield return null;
			}
		}
	}

	[System.Serializable]
	public class DialogList
	{
		[TextArea(5, 10)]
		public string Text;

		public bool IsPlayer;
	}
}

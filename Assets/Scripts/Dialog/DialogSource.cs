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

		private GameObject _child;

		public static bool IsActive { get; private set; } = false;

		private void Awake()
		{
			_dialogParent = GameObject.Find("@parent");
		}

		public IEnumerator DialogRoutine()
		{
			while (true)
			{
				if (_index >= DialogTexts.Count)
				{
					_index = 0;
					_title.text = "";
					_dialog.text = "";
					InteractionManager.run = null;
					yield break;
				}

				yield return StartCoroutine(InitializeDialog());

				_child.SetActive(true);

				yield return new WaitUntil(() => _child.activeSelf);

				yield return new WaitForSeconds(DialogTexts[_index].Timer);

				_child.SetActive(false);

				_index++;

				yield return null;
			}
		}

		private IEnumerator InitializeDialog()
		{
			if (DialogTexts[_index].Chats == DialogList.ChatTypes.Thought)
				_child = _dialogParent.transform.GetChild(1).gameObject;
			else
				_child = _dialogParent.transform.GetChild(0).gameObject;


			_child.SetActive(true);

			_dialog = GameObject.Find("@dialog").GetComponent<TextMeshProUGUI>();

			if (DialogTexts[_index].Chats == DialogList.ChatTypes.Thought)
			{
				_dialog.text = $"\"{DialogTexts[_index].Text}\"";
			}
			else
			{
				_title = GameObject.Find("@title").GetComponent<TextMeshProUGUI>();
				_title.text = DialogTexts[_index].Chats == DialogList.ChatTypes.ChatAsPlayer ? "Me:" : gameObject.name + ":";
				_dialog.text = DialogTexts[_index].Text;
			}

			yield return null;
		}
	}

	[System.Serializable]
	public class DialogList
	{
		[TextArea(5, 10)]
		public string Text;

		public enum ChatTypes{
			ChatAsPlayer,
			ChatAsNpc,
			Thought
		}

		public ChatTypes Chats;

		public float Timer;
	}
}

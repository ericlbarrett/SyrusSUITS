using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionsMenu : AetherMenu {


	public delegate void SelectionEvent(int i);
	public event SelectionEvent OnSelection;

	private Transform content;
	public bool destroyOnSelect = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetTitle(string title) {
		transform.Find("Canvas/TopPanel/TitleText").GetComponent<Text>().text = title;
	}

	public void AddItem(string text, int i) {
		GameObject goButton = (GameObject)Instantiate(Resources.Load("Button"));

		if (content == null) {
			content = transform.Find("Canvas/BottomPanel/Scroll View/Viewport/Content");
		}

		goButton.transform.SetParent(content, false);

		if (content.childCount == 1) {
			defaultObject = goButton;
		}

		goButton.GetComponentInChildren<Text>().text = text;
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.callback.AddListener((data) => {
			defaultObject = goButton;
		});
		entry.eventID = EventTriggerType.Select;

		goButton.AddComponent<EventTrigger>().triggers.Add(entry);
		goButton.GetComponentInChildren<Button>().onClick.AddListener(() => {
			OnSelection(i);
			if (destroyOnSelect)
				Destroy(gameObject);
		});
	}

	public static OptionsMenu Instance(string title, bool _destroyOnSelect) {
		GameObject optionsObj = (GameObject)Instantiate(Resources.Load("Options"));
		OptionsMenu options = optionsObj.GetComponent<OptionsMenu>();
		options.destroyOnSelect = _destroyOnSelect;
		options.SetTitle(title);
		return options;
	}
}

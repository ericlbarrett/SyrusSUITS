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

    //Sets the title of the Options menu
	public void SetTitle(string title) {
		transform.Find("Canvas/TopPanel/TitleText").GetComponent<Text>().text = title;
	}
    
    //Resize Options Menu based off of items inside Content
    public void ResizeOptions( )
    {
        if(content == null) {
            content = transform.Find("Canvas/BottomPanel/Scroll View/Viewport/Content");
        }
        //Debug.Log(content.transform.localScale);
        //RectTransform rt = content.GetComponent<RectTransform>();
        //Debug.Log(rt.rect.height);
        int contentCount = content.transform.childCount;
        float totalHeight = 0;
        float padding = 5;
        float totalPadding = padding * contentCount;
        for (int i = 0; i < contentCount; i++)
        {
            Transform thing = content.transform.GetChild(i);
            float height = thing.GetComponent<RectTransform>().rect.height;
            totalHeight += height;
        }
        totalHeight += totalPadding + 10; // plus 10 for final padding

        RectTransform rt = content.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, totalHeight);

    }

    //Adds an item into the Options Menu ( string , i ) i is the index (need to know for callbacks)
	public void AddItem(string text, int i) {
        GameObject goButton = (GameObject)Instantiate(Resources.Load("Button"));

        if (content == null)
        {
            content = transform.Find("Canvas/BottomPanel/Scroll View/Viewport/Content");
        }

        goButton.transform.SetParent(content, false);

        if (content.childCount == 1)
        {
            defaultObject = goButton;
        }

        goButton.GetComponentInChildren<Text>().text = text;
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.callback.AddListener((data) =>
        {
            defaultObject = goButton;
        });
        entry.eventID = EventTriggerType.Select;

        goButton.AddComponent<EventTrigger>().triggers.Add(entry);
        goButton.GetComponentInChildren<Button>().onClick.AddListener(() =>
        {
            OnSelection(i);
            if (destroyOnSelect)
                Destroy(gameObject);
        });
    }

    //Does something cool
	public static OptionsMenu Instance(string title, bool _destroyOnSelect) {
		GameObject optionsObj = (GameObject)Instantiate(Resources.Load("Options"));
		OptionsMenu options = optionsObj.GetComponent<OptionsMenu>();
		options.destroyOnSelect = _destroyOnSelect;
		options.SetTitle(title);
		return options;
	}
}

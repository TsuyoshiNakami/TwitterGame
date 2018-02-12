using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GachaUIManager : MonoBehaviour {
    [SerializeField] Text followerNameText;
    [SerializeField] Text followerAttributeText;

    GameObject followerImageFrame;
    GameObject followerDataPanel;

    // Use this for initialization
    void Start () {
        followerDataPanel = GameObject.Find("FollowerDataPanel");
        followerImageFrame = GameObject.Find("FollowerImageFrame");
	}
	
	// Update is called once per frame
	public void UpdateText (string name, string attribute) {
        followerNameText.text = name;
        followerAttributeText.text = attribute;
	}

    public void ClearText()
    {
        followerNameText.text = "";
        followerAttributeText.text = "";
    }

    public void DeleteContents()
    {
        ClearText();

        followerImageFrame.transform.Find("Mask").Find("Image").GetComponent<Image>().sprite = null;

        Transform content = followerDataPanel.transform.Find("Content");

           foreach(Transform child in content)
        {
            Destroy(child.gameObject);
        }

        Transform contentValue = followerDataPanel.transform.Find("ContentValue");

        foreach (Transform child in contentValue)
        {
            Destroy(child.gameObject);
        }
    }
}

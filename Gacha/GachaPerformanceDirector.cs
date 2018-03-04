using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaPerformanceDirector : MonoBehaviour
{
    [SerializeField] GameObject followerImageFrame;
    [SerializeField] GameObject callFollowerButton;
    [SerializeField] GameObject followerDataPanel;
    [SerializeField] GameObject followerDataText;
    [SerializeField] GameObject followerNameText;

    GachaUIManager gachaUIManager;
    GachaVideoPlayer videoPlayer;

    // Use this for initialization
    void Start()
    {
        videoPlayer = GameObject.Find("GachaVideoPlayer").GetComponent<GachaVideoPlayer>();
        gachaUIManager = GameObject.Find("GachaUIManager").GetComponent<GachaUIManager>();

        followerDataPanel = GameObject.Find("FollowerDataPanel");
        followerNameText = GameObject.Find("followerNameText");

        followerDataPanel.SetActive(false);
    }

    public void HideUI()
    {
        callFollowerButton.SetActive(false);
    }


    public void ViewFollowerDataPanel(FollowerData followerData)
    {

        followerDataPanel.SetActive(true);

        followerNameText.GetComponent<Text>().text = followerData.name;


        GameObject textLv = Instantiate(followerDataText, followerDataPanel.transform.Find("Content"));
        textLv.GetComponent<Text>().text = "レベル";

        GameObject textLvValue = Instantiate(followerDataText, followerDataPanel.transform.Find("ContentValue"));
        textLvValue.GetComponent<Text>().text = "1";


        GameObject textName = Instantiate(followerDataText, followerDataPanel.transform.Find("Content"));
        textName.GetComponent<Text>().text = "属性";

        GameObject textNameValue = Instantiate(followerDataText, followerDataPanel.transform.Find("ContentValue"));
        textNameValue.GetComponent<Text>().text = AttributeUtil.ToJapanese(followerData.attribute);

        GameObject textHP = Instantiate(followerDataText, followerDataPanel.transform.Find("Content"));
        textHP.GetComponent<Text>().text = "HP";

        GameObject textHPValue = Instantiate(followerDataText, followerDataPanel.transform.Find("ContentValue"));
        textHPValue.GetComponent<Text>().text = followerData.hp.ToString();

        GameObject textAttackForce = Instantiate(followerDataText, followerDataPanel.transform.Find("Content"));
        textAttackForce.GetComponent<Text>().text = "攻撃力";

        GameObject textAttackForceValue = Instantiate(followerDataText, followerDataPanel.transform.Find("ContentValue"));
        textAttackForceValue.GetComponent<Text>().text = followerData.attackForce.ToString();

        GameObject textDefenceForce = Instantiate(followerDataText, followerDataPanel.transform.Find("Content"));
        textDefenceForce.GetComponent<Text>().text = "防御力";

        GameObject textDefenceForceValue = Instantiate(followerDataText, followerDataPanel.transform.Find("ContentValue"));
        textDefenceForceValue.GetComponent<Text>().text = followerData.defenceForce.ToString();

        for (int i = 0; i < 3; i++)
        {
            if (followerData.skillNames[i] != null)
            {
                Skill skill = SkillUtils.GetSkill(followerData.skillNames[i]);
                GameObject textSkill = Instantiate(followerDataText, followerDataPanel.transform.Find("Content"));
                textSkill.GetComponent<Text>().text = skill.name;

                GameObject textSkillValue = Instantiate(followerDataText, followerDataPanel.transform.Find("ContentValue"));
                textSkillValue.GetComponent<Text>().text = skill.description;
            }
        }
    }

    public void OnCloseButtonClicked()
    {
        followerDataPanel.SetActive(false);
        callFollowerButton.SetActive(true);
        gachaUIManager.DeleteContents();

    }
    public void PerformCallFollower()
    {
        HideUI();
        PlayMovie();

       // followerDataPanel.GetComponent<Image>().sprite = null;

        iTween.ShakePosition(followerImageFrame, iTween.Hash("x", 1.5f, "y", 1.5f, "time", 20f));
    }

    void PlayMovie()
    {
        videoPlayer.Play();
    }

    public void InitializeGacha()
    {
       
    }
}

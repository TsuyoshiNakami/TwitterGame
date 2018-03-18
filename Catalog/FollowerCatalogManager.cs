using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using SimpleJSON;



public class FollowerCatalogManager : MonoBehaviour {
	List<string> imageURL;
	[SerializeField] RectTransform viewer;
    [SerializeField] GameObject content;
    [SerializeField] GameObject[] regularFollowerFrame;
    [SerializeField] bool debugCatalog = true;
    FollowerEntity regularFollower;
    GameObject tappedObj1;
    GameObject tappedObj2;
    // Use this for initialization
    void Start () {
        tappedObj1 = null;
        tappedObj2 = null;
		imageURL = new List<string>();
        if (debugCatalog)
        {
            StartCoroutine(GetImageURLsDebug());
        }else { 
            StartCoroutine(GetImageURLs());
        }
	}


    string url = "http://tsuyomilog.php.xdomain.jp/get_follower.php";
    IEnumerator GetImageURLs()
    {
        WWW result = new WWW(url);
        yield return result;
        var resultJson = JSON.Parse(result.text);

        for (int i = 0; i < resultJson.Count; i++)
        {
            var followerData = resultJson[i];


            if (followerData["name"].ToString() == "") { continue; }

            if (followerData["owner_id"] == PlayerPrefs.GetString(TwitterComponentHandler.PLAYER_PREFS_TWITTER_USER_ID))
            {

                imageURL.Add(followerData["image_url"].ToString());

                var newViewer = Instantiate(viewer) as RectTransform;
                newViewer.SetParent(content.transform, false);
                FollowerViewer followerViewer = newViewer.GetComponent<FollowerViewer>();
                yield return followerViewer.GetImage(followerData["image_url"]);
                followerViewer.id_auto = followerData["auto_id"];//.ToString();

                //  TODO ----------------------レギュラーメンバーは枠に入れる。

                if (followerData["is_regular"] != "" && int.Parse(followerData["is_regular"]) > 0)
                {
                    RegistRegularFollower(regularFollowerFrame[int.Parse(followerData["is_regular"]) - 1],
                        followerViewer.gameObject);
                    // followerData["auto_id"].ToString()
                    //↑のところを探し、is_Regular=0にする
                }

                followerViewer.followerData = new FollowerEntity(followerData["name"].ToString(), followerData["id_str"].ToString(), 20, CharacterAttribute.Fire, 1,0, followerData["imageURL"].ToString());
            }
        }
    }

    IEnumerator GetImageURLsDebug()
    {

        string url = "http://tsuyomilog.php.xdomain.jp/get_follower.php";
        WWW result = new WWW(url);
        yield return result;
        var resultJson = JSON.Parse(result.text);


        for (int i = 0; i < resultJson.Count; i++)
        {
            var followerData = resultJson[i];


            if (followerData["name"].ToString() == "") { continue; }
            //  playerIdを挿入できるようになったらコメントアウトを外す

            //if (followerData["playerId"].ToString() == DataCarrier.playerTwitterId)
            {



                var newViewer = Instantiate(viewer) as RectTransform;
                newViewer.SetParent(content.transform, false);
                FollowerViewer followerViewer = newViewer.GetComponent<FollowerViewer>();
                followerViewer.GetImageDebug(followerData["image_url"]);
                yield return null;
                followerViewer.id_auto = followerData["auto_id"];//.ToString();

                CharacterAttribute attribute = (CharacterAttribute)Enum.ToObject(typeof(CharacterAttribute),int.Parse( followerData["attribute"].Value));
                if (followerData["is_regular"] != "" && int.Parse(followerData["is_regular"]) > 0)
                {
                    RegistRegularFollower(regularFollowerFrame[int.Parse(followerData["is_regular"]) - 1],
                        followerViewer.gameObject);

                }
                    followerViewer.followerData = new FollowerEntity(followerData["name"].ToString(), followerData["id_str"].ToString(), 20, attribute, 1, 0, followerData["imageURL"].ToString());

            }
        }
    }


    public void RegistTappedObj(GameObject target)
    {
        if (tappedObj1 == null)
        {
            tappedObj1 = target;
            return;
        } else
        {
            tappedObj2 = target;
        }

        if(tappedObj1.CompareTag("RegularFollowerFrame") && tappedObj2.CompareTag("FollowerCell"))
        {
            RegistRegularFollower(tappedObj1, tappedObj2);
        }
         if(  tappedObj2.CompareTag("RegularFollowerFrame") && tappedObj1.CompareTag("FollowerCell"))
        {
            RegistRegularFollower(tappedObj2, tappedObj1);
        }
        tappedObj1 = null;
        tappedObj2 = null;
    }

    public void RegistRegularFollower(GameObject frame, GameObject cell)
    {
        frame.GetComponent<RegularFollowerFrame>().id_auto = cell.GetComponent<FollowerViewer>().id_auto;

        frame.GetComponent<RegularFollowerFrame>().followerImage.sprite = cell.GetComponent<FollowerViewer>().image.sprite;
    }
}

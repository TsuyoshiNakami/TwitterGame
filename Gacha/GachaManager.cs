using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using SimpleJSON;
using UniRx;

public enum FollowerAttribute
{
    Fire,
    Water,
    Plant,
    Thunder,
    Sky,
    None
}

public class GachaManager : MonoBehaviour
{

    TwitterComponentHandler twitterComponentHandler;

    List<string> mutualFollowerIds = new List<string>();
    List<string> selectedMutualFollowers = new List<string>();
    List<string> randomList;
    GachaPerformanceDirector performaceDirector;
    GachaFollowerCreator gachaFollowerCreator;
    GachaUIManager gachaUIManager;
    GachaDataManager gachaDataManager;

    [SerializeField] Image imageObj;
    [SerializeField] GameObject frame;
    [SerializeField] GameObject callFollowerEffect;

    [SerializeField] bool debugGacha = true;
    [SerializeField] Sprite debugImage;
    public bool debugGetFavorites = true;
    string nowGetFavoritesUserId = "";

    void Start()
    {
        gachaUIManager = GameObject.Find("GachaUIManager").GetComponent<GachaUIManager>();
        performaceDirector = GameObject.Find("GachaPerformanceDirector").GetComponent<GachaPerformanceDirector>();
        gachaFollowerCreator = new GachaFollowerCreator();
        gachaDataManager = new GachaDataManager();

        twitterComponentHandler = GameObject.Find("TwitterComponentHandler").GetComponent<TwitterComponentHandler>();
        SubscribeHandler();
    }

    /// <summary>
    /// TwitterComponentHandlerのIObservableを購読する
    /// </summary>
    void SubscribeHandler()
    {
        twitterComponentHandler.OnGetFriendshipsLookup.Subscribe(param =>
        {
            OnGetFriendshipsLookup(param);
        });
        twitterComponentHandler.OnGetFavoritesList.Subscribe(param =>
        {
            OnGetFavoritesList(param);

        });
        twitterComponentHandler.OnGetUsersLookup.Subscribe(param =>
        {
            OnGetUsersLookup(param);
        });
    }


    /// <summary>
    /// ガチャを引く処理
    /// </summary>
    void CallFollower()
    {

        performaceDirector.PerformCallFollower();

        //      （デバッグ）決められたキャラしか排出されない
        if (debugGacha)
        {
            StartCoroutine(IGetImageDebug());
        }
        else
        {
            if (mutualFollowerIds.Count > 0)
            {
                GetMutualFolloersAtRandom();
            }
            else
            {
                twitterComponentHandler.GetFollowers();
            }

        }
    }

    /// <summary>
    /// DBのfollowerテーブルのカラムを全て削除する
    /// </summary>
    /// <returns>エミュレータ</returns>
    IEnumerator DeleteAllFollowers()
    {
        string url = "http://tsuyomilog.php.xdomain.jp/delete_all_followers.php";
        WWW result = new WWW(url);
        yield return result;
        Debug.Log(result.text);
    }

    /// <summary>
    /// （デバッグ用）ローカルの画像でフォロワーを登録する
    /// </summary>
    /// <returns>エミュレータ</returns>
    IEnumerator IGetImageDebug()
    {
        Sprite[] images = Resources.LoadAll<Sprite>("Sprites/DebugCharacter");

        debugImage = images[UnityEngine.Random.Range(0, images.Length)];
        CharacterAttribute attribute = AttributeUtil.GetCharacterAttributeByTexture(debugImage.texture);
        FollowerData debugFollower = new FollowerData("だいこん！", "12312", 50, attribute, 6, 2, debugImage.name);

        imageObj.GetComponent<Image>().sprite = debugImage;

        iTween.Stop(frame);
        //Instantiate (callFollowerEffect ,imageObj [0].transform.position, imageObj [0].transform.rotation);
        iTween.ShakePosition(frame, iTween.Hash("x", 20f, "y", 20f, "time", 0.5f));


        gachaUIManager.UpdateText(debugFollower.name, AttributeUtil.ToJapanese(debugFollower.attribute));

        //  フォロワーデータを保存
        string playerId = DataCarrier.playerTwitterId;
        GachaDataManager gachaDataManager = new GachaDataManager();
        StartCoroutine(gachaDataManager.SaveFollower(playerId, debugFollower));

        selectedMutualFollowers.Clear();

        yield return null;
    }

    /// <summary>
    /// フォロワーが「いいね」したツイートを取得する
    /// </summary>
    /// <param name="param">GetFavoritesListのレスポンス</param>
    public void OnGetFavoritesList(string param)
    {
        JSONNode jsonParams = JSON.Parse(param);
        foreach (JSONNode jsonParam in jsonParams)
        {
            if (DataCarrier.playerTwitterId == jsonParam["user"]["id_str"])
            {
                selectedMutualFollowers.Add(nowGetFavoritesUserId);
                break;
            }
            else
            {
            }

        }
        if (selectedMutualFollowers.Count >= 1)
        {

            string followerIds = "";
            foreach (string a in selectedMutualFollowers)
            {
                followerIds += a + ",";
            }
            twitterComponentHandler.GetUsersLookup(followerIds);
        }
        else
        {
            //	GetFavoritesCoroutine ();
        }
    }

    /// <summary>
    /// フォロワーとのフォロー関係を取得した時
    /// </summary>
    /// <param name="param">GetFriendshipsLookupのレスポンス</param>
	public void OnGetFriendshipsLookup(string param)
    {
        GetMutualFollower(param);
    }

    /// <summary>
    /// フォロワーの詳細情報を取得した時
    /// </summary>
    /// <param name="param">OnGetUsersLookupのレスポンス</param>
	public void OnGetUsersLookup(string param)
    {
        ViewProfileImage(param);
    }

    /// <summary>
    /// プロフィール画像を表示するコルーチンを呼び出す
    /// </summary>
    /// <param name="param">OnGetUsersLookupのレスポンス</param>
	void ViewProfileImage(string param)
    {

        JSONNode jsonParam = JSON.Parse(param);

        //鍵垢ならもう一回引く
        if (jsonParam[0]["protected"])
        {
            if (mutualFollowerIds.Count > 0)
            {
                GetMutualFolloersAtRandom();
            }
            else
            {
                twitterComponentHandler.GetFollowers();
            }
        }
        else
        {
            //画像の表示
            StartCoroutine(IGetImage(jsonParam));
        }
    }

    /// <summary>
    /// プロフィール画像を取得する
    /// </summary>
    /// <param name="jsonParam">OnGetUsersLookupのレスポンス</param>
    /// <returns>エミュレータ</returns>
	IEnumerator IGetImage(JSONNode jsonParam)
    {
        string imageURL = "";

        Debug.Log(jsonParam);
        foreach (JSONNode json in jsonParam)
        {

            string newStr = json["profile_image_url_https"];

            imageURL = newStr;
            break;
        }

        imageURL = imageURL.Replace("normal.jpg", "400x400.jpg");
        WWW www = new WWW(imageURL);
        yield return www;
        yield return new WaitForSeconds(1);
        CharacterAttribute attribute = AttributeUtil.GetCharacterAttributeByTexture(www.texture);
        imageObj.GetComponent<Image>().sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.zero);

        iTween.Stop(frame);
        //Instantiate (callFollowerEffect ,imageObj [0].transform.position, imageObj [0].transform.rotation);
        iTween.ShakePosition(frame, iTween.Hash("x", 20f, "y", 20f, "time", 0.5f));



        FollowerData followerData = MakeFollowerStatus(jsonParam, attribute, imageURL);

        string playerId = PlayerPrefs.GetString(TwitterComponentHandler.PLAYER_PREFS_TWITTER_USER_ID);

        gachaUIManager.UpdateText(jsonParam[0]["name"], AttributeUtil.ToJapanese(attribute));
        performaceDirector.ViewFollowerDataPanel(followerData);

        StartCoroutine(gachaDataManager.SaveFollower(playerId, followerData));


        selectedMutualFollowers.Clear();
    }

    /// <summary>
    /// 引数からFollowerDataを生成する
    /// </summary>
    /// <param name="jsonParam">OnGetUsersLookupのレスポンス</param>
    /// <param name="attribute">属性</param>
    /// <param name="imageURL">プロフィール画像のURL</param>
    /// <returns></returns>
    public FollowerData MakeFollowerStatus(JSONNode jsonParam, CharacterAttribute attribute, string imageURL)
    {
        int hp = gachaDataManager.DecideHp(attribute);
        int attackForce = gachaDataManager.DecideAttackForce(attribute);
        int defenceForce = gachaDataManager.DecideDefenceForce(attribute);
        string[] skillNames = gachaDataManager.DecideSkills(attribute);
        //  フォロワーデータを保存
        FollowerData followerData = new FollowerData()
        {
            name = jsonParam[0]["name"],
            id_str = jsonParam[0]["id_str"],
            hp = hp,
            attribute = attribute,
            attackForce = attackForce,
            defenceForce = defenceForce,
            skillNames = skillNames,
            image_url = imageURL
        };
        return followerData;
    }

    /// <summary>
    /// ガチャを引くボタンを押した時
    /// </summary>
    public void OnClickCallFollowerButton()
    {
        CallFollower();
    }


    /// <summary>
    /// 相互フォロワーのリストを生成する
    /// </summary>
    /// <param name="param">FriendshipsLookupのレスポンス</param>
	public void GetMutualFollower(string param)
    {
        JSONNode jsonParams = JSON.Parse(param);

        foreach (JSONNode jsonParam in jsonParams)
        {

            foreach (JSONNode connection in jsonParam["connections"])
            {
                if (connection == "followed_by")
                {

                    mutualFollowerIds.Add(jsonParam["id_str"]);
                }
            }
        }
        GetMutualFolloersAtRandom();
    }



    /// <summary>
    /// 取得したフォロワーIDからランダムに1つ取得
    /// </summary>
	public void GetMutualFolloersAtRandom()
    {
        randomList = new List<string>();

        Debug.Log("Get Random Follwers Start");
        if (debugGetFavorites)
        {
            string addId = "";
            for (int n = 0; n < mutualFollowerIds.Count; n++)
            {
                addId = mutualFollowerIds[UnityEngine.Random.Range(0, mutualFollowerIds.Count)];
                randomList.Add(addId);
                mutualFollowerIds.Remove(addId);

            }

            //StartCoroutine (GetFavoritesIenum());
            //GetFavoritesCoroutine ();
        }
        else
        {
            string addId = "";

            for (int n = 0; n < mutualFollowerIds.Count; n++)
            {
                addId = mutualFollowerIds[UnityEngine.Random.Range(0, mutualFollowerIds.Count)];
                mutualFollowerIds.Remove(addId);
            }
            twitterComponentHandler.GetUsersLookup(addId);
        }


    }
    //int num = 0;
    //bool GetFavoritesCoroutine() {
    //	if (num < mutualFollowerIds.Count) {
    //		nowGetFavoritesUserId = randomList [num];
    //		twitterComponentHandler.GetFavorites (randomList [num]);
    //	}
    //	if (selectedMutualFollowers.Count >= 3) {
    //		return true;
    //	} else {
    //		num++;
    //		return false;

    //	}
    //}



}

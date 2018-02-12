using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SimpleJSON;
using Twitter;
using UniRx;




public class TwitterComponentHandler : MonoBehaviour{
    public GameObject pinField;
	public GameObject authButton;

    Subject<string> getFriendshipsLookup = new Subject<string>();
    Subject<string> getFavoritesList = new Subject<string>();
    Subject<string> getUsersLookup = new Subject<string>();
    Subject<Unit> authCompleted = new Subject<Unit>();

    public IObservable<string> OnGetFriendshipsLookup
    {
        get { return getFriendshipsLookup; }
    }
    public IObservable<string> OnGetUsersLookup
    {
        get { return getUsersLookup; }
    }
    public IObservable<string> OnGetFavoritesList
    {
        get { return getFavoritesList; }
    }
    public IObservable<Unit> OnAuthCompleted
    {
        get { return authCompleted; }
    }

    //アプリのコンシューマーキー
    private const string CONSUMER_KEY = "";

    //アプリのコンシューマーシークレット
	private const string CONSUMER_SECRET = "";

	Twitter.RequestTokenResponse m_RequestTokenResponse;
	Twitter.AccessTokenResponse m_AccessTokenResponse;

    //セーブデータのキー
	string playerId = "";
	public const string PLAYER_PREFS_TWITTER_USER_ID    = "TwitterUserID";
	const string PLAYER_PREFS_TWITTER_USER_SCREEN_NAME  = "TwitterUserScreenName";
	const string PLAYER_PREFS_TWITTER_USER_TOKEN        = "TwitterUserToken";
	const string PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET = "TwitterUserTokenSecret";

	const string PLAYER_PREFS_TWITTER_TWEETED_IDS       = "TwitterTweetedIDs";

	List<string> mutualFollowers = new List<string>();
	List<string> mutualFollowerIds = new List<string>();

	[SerializeField] Image[] imageObj;
	[SerializeField] bool playGachaOnStart = false;

	void Start () {
        m_AccessTokenResponse = new Twitter.AccessTokenResponse();

        //  セーブデータの取得
        if(PlayerPrefs.HasKey(PLAYER_PREFS_TWITTER_USER_ID))
        {
            m_AccessTokenResponse.UserId = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_ID);
            m_AccessTokenResponse.ScreenName = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_SCREEN_NAME);
            m_AccessTokenResponse.Token = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_TOKEN);
            m_AccessTokenResponse.TokenSecret = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET);
        }
	}


	public void GetPIN () {
            // PHPで認証頑張りたい時
            //StartCoroutine(GetAccessToken());

            // PINコードで認証するとき
            StartCoroutine(Twitter.API.GetRequestToken(CONSUMER_KEY, CONSUMER_SECRET,
            new Twitter.RequestTokenCallback(this.OnRequestTokenCallback)));
    }

    /// <summary>
    /// テキストフィールドのPINコードを使用してアクセストークンを取得する
    /// </summary>
    public void AuthPIN()
    {
        string pin = pinField.GetComponent<InputField>().text;
        StartCoroutine(Twitter.API.GetAccessToken(CONSUMER_KEY, CONSUMER_SECRET, m_RequestTokenResponse.Token, pin,
            new Twitter.AccessTokenCallback(this.OnAccessTokenCallback)));
    }

    /// <summary>
    /// アクセストークンの取得
    /// </summary>
    /// <returns>エミュレータ</returns>
    IEnumerator GetAccessToken()
    {
        WWWForm form = new WWWForm();
        form.AddField("oauth_callback", UrlConsts.auth);

        WWW result = new WWW(UrlConsts.auth);
        yield return result;
        Debug.Log(result.text);
    }


    /// <summary>
    /// ログアウト時の処理
    /// </summary>
    public void DeletePlayerLoginData ()
    {
        PlayerPrefs.DeleteKey(PLAYER_PREFS_TWITTER_USER_ID);
        PlayerPrefs.DeleteKey(PLAYER_PREFS_TWITTER_USER_SCREEN_NAME);
        PlayerPrefs.DeleteKey(PLAYER_PREFS_TWITTER_USER_TOKEN);
        PlayerPrefs.DeleteKey(PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET);
    }

    /// <summary>
    /// フォローしている人をIDのリストで取得
    /// </summary>
	public void GetFollowers () {
		StartCoroutine(Twitter.API.GetFriendsWithIds(m_AccessTokenResponse.UserId, CONSUMER_KEY, CONSUMER_SECRET, m_AccessTokenResponse,
			new Twitter.GetFriendsWithIdsCallback(this.OnGetFriendsWithIds)));
    }

    /// <summary>
    /// フォロワーの詳細情報を取得
    /// </summary>
    /// <param name="ids">取得したいフォロワーのID。複数の場合はカンマで区切る</param>
    public void GetUsersLookup(string ids)
    {
        StartCoroutine(Twitter.API.GetUsersLookup(m_AccessTokenResponse.UserId, CONSUMER_KEY, CONSUMER_SECRET, m_AccessTokenResponse,
    new Twitter.GetUsersLookupCallback(this.OnTwitterGetUsersLookup), ids));
    }

    /// <summary>
    /// フォロワーが「いいね」したツイートを取得
    /// </summary>
    /// <param name="id"></param>
	public void GetFavorites(string id)
    {
        StartCoroutine(Twitter.API.GetFavoritesList(m_AccessTokenResponse.UserId, CONSUMER_KEY, CONSUMER_SECRET, m_AccessTokenResponse,
            new Twitter.GetFavoritesListCallback(this.OnTwitterGetFavoritesList), id));
    }

    /// <summary>
    /// リクエストトークン取得時
    /// </summary>
    /// <param name="success">成功したか</param>
    /// <param name="response">TwitterAPIからのレスポンス</param>
	void OnRequestTokenCallback(bool success, Twitter.RequestTokenResponse response) {
		if (success) {
			string log = "OnRequestTokenCallback - succeeded";
			log += "\n    Token : " + response.Token;
			log += "\n    TokenSecret : " + response.TokenSecret;
			print(log);

			m_RequestTokenResponse = response;


			Twitter.API.OpenAuthorizationPage(response.Token);
		} else {
			print("OnRequestTokenCallback - failed.");
		}
	}

    /// <summary>
    /// アクセストークン取得時
    /// </summary>
    /// <param name="success">成功したか</param>
    /// <param name="response">TwitterAPIからのレスポンス</param>
    void OnAccessTokenCallback (bool success, Twitter.AccessTokenResponse response) {
		if (success) {
			string log = "OnAccessTokenCallback - succeeded";
			log += "\n    UserId : " + response.UserId;
			log += "\n    ScreenName : " + response.ScreenName;
			log += "\n    Token : " + response.Token;
			log += "\n    TokenSecret : " + response.TokenSecret;
			print(log);

			m_AccessTokenResponse = response;
			DataCarrier.playerTwitterId = m_AccessTokenResponse.UserId;
			PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_ID, response.UserId);
			PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_SCREEN_NAME, response.ScreenName);
			PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_TOKEN, response.Token);
			PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET, response.TokenSecret);
            PlayerPrefs.Save();

            authCompleted.OnNext(Unit.Default);

        } else {
			print("OnAccessTokenCallback - failed.");
		}
	}

    /// <summary>
    /// タイムライン取得時
    /// </summary>
    /// <param name="success">成功したか</param>
    /// <param name="response">TwitterAPIからのレスポンス</param>
	void OnGetUserTimeline(bool success, string response) {
		print ("GetUserTimeline- " + (success ? "succedded." : "failed."));

		if (success) {
			var json = JSON.Parse (response);
			print (json);
		}
	}

    /// <summary>
    /// フォロワーのIDリストを取得した時
    /// </summary>
    /// <param name="success">成功したか</param>
    /// <param name="response">TwitterAPIからのレスポンス</param>
	void OnGetFriendsWithIds(bool success, string response) {
		print ("GetFriendsWithIds- " + (success ? "succedded." : "failed."));

		if (success) {
			var json = JSON.Parse (response);
		//	print (json);
		}
		JSONNode idsJson = JSON.Parse (response);
		string ids = "";
		foreach(JSONNode json in idsJson["ids"]) {
			ids += json + ",";
		}
		//Debug.Log ("On get friends with ids : " + ids);
		StartCoroutine(Twitter.API.GetFriendshipsLookup(m_AccessTokenResponse.UserId, CONSUMER_KEY, CONSUMER_SECRET, m_AccessTokenResponse,
			new Twitter.GetFriendshipsLookupCallback(this.OnTwitterGetFriendshipsLookup), ids));
	}

    /// <summary>
    /// フォロワーとのフォロー関係を取得した時
    /// </summary>
    /// <param name="success">成功したか</param>
    /// <param name="response">TwitterAPIからのレスポンス</param>
	void OnTwitterGetFriendshipsLookup(bool success, string response) {
		print ("GetFriendshipsLookup - " + (success ? "succedded." : "failed."));

		if (success) {
			var json = JSON.Parse (response);
			print (json);
		}
		getFriendshipsLookup.OnNext(response);
	}

    /// <summary>
    /// フォロワーの詳細情報を取得した時
    /// </summary>
    /// <param name="success">成功したか</param>
    /// <param name="response">TwitterAPIからのレスポンス</param>
    void OnTwitterGetUsersLookup(bool success, string response)
    {
        print("GetUsersLookup - " + (success ? "succedded." : "failed."));
        var json = JSON.Parse(response);
        if (success)
        {
            //	print (json);
        }
        var jsonParams = JSON.Parse(response);
        Debug.Log("UsersLookup : " + response);
        foreach (JSONNode jsonParam in json)
        {
            mutualFollowers.Add(jsonParam.ToString());
        }
        getUsersLookup.OnNext(response);

    }

    /// <summary>
    /// フォロワーが「いいね」したツイートの取得した時
    /// </summary>
    /// <param name="success">成功したか</param>
    /// <param name="response">TwitterAPIからのレスポンス</param>
    void OnTwitterGetFavoritesList(bool success, string response) {
		//print ("GetFavoritesList- " + (success ? "succedded." : "failed."));

		if (success) {
			var json = JSON.Parse (response);
			print ("GetFavoritesList- "  + json.ToString());
		}
		getFavoritesList.OnNext (response);
	}
	
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class FriendSpawner : MonoBehaviour {

    GameObject follower;

    GameObject[] followerSpawnPoint;

    Subject<Unit> FriendSpawnSubject = new Subject<Unit>();

    public IObservable<Unit> OnFriendSpawned
    {
        get { return FriendSpawnSubject; }
    }

    public FriendSpawner()
    {
        followerSpawnPoint = new GameObject[3];
        for(int i = 0; i < 3; i++)
        {
            followerSpawnPoint[i] = GameObject.Find("FollowerSpawnPoint" + i);
        }
    }
    public void SpawnFriend(List<FollowerEntity> entities, Texture2D maskTexture)
    {
        follower = Resources.Load<GameObject>("Prefabs/Battle/Friend");

        for (int i = 0; i < 3; i++)
        {
            GameObject newFriend = Instantiate(follower, followerSpawnPoint[i].transform.position, followerSpawnPoint[i].transform.rotation);
            newFriend.name = "Friend" + i;
            //GameObject maskObj = newFriend.transform.Find("Mask").gameObject;
            //maskObj.transform.parent = newFriend.transform;
            //SpriteMask spriteMask = maskObj.AddComponent<SpriteMask>();
            //spriteMask.size = Vector2.one * 1.2f;
            //spriteMask.type = SpriteMask.Type.Texture;
            //spriteMask.texture = maskTexture;

            //maskObj.AddComponent<CircleCollider2D>().isTrigger = true;
            //attachSprite(maskObj, entities[i].sprite, Vector2.zero);
            //spriteMask.updateSprites();
            newFriend.AddComponent<Image>();
            newFriend.GetComponent<Image>().sprite = entities[i].sprite;
            newFriend.transform.parent = GameObject.Find("FollowerUI" + i + "/Profile/Image").transform;
            newFriend.GetComponent<RectTransform>().sizeDelta = new Vector2(27, 27);
            newFriend.transform.localScale = Vector3.one;
            newFriend.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;// = new Vector2(27, 27);
        }
        FriendSpawnSubject.OnNext(Unit.Default);
    }



    private SpriteRenderer attachSprite(GameObject maskObj, Sprite sprite, Vector2 position)
    {
        GameObject spriteObj = new GameObject("Sprite");
        spriteObj.transform.parent = maskObj.transform;
        spriteObj.transform.localPosition = position;
        SpriteRenderer spriteRenderer = spriteObj.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 5;
        spriteObj.AddComponent<FriendCollider>();
        return spriteRenderer;
    }
}

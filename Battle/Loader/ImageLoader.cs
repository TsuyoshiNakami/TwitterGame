using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ImageLoader
{
    Subject<Unit> loadFollowerImagesSubject = new Subject<Unit>();
    Subject<Unit> loadEnemyImagesSubject = new Subject<Unit>();
    Subject<Unit> loadOtherImagesSubject = new Subject<Unit>();
    List<string> followerImageUrls = new List<string>();
    public Texture2D maskTexture = null;
    public Sprite[] followerSprite = new Sprite[3];

    List<string> enemyNames = new List<string>();
    Dictionary<string, string> otherImagePaths = new Dictionary<string, string>();
    public Dictionary<string, Sprite> enemySprites = new Dictionary<string, Sprite>();
    public Dictionary<string, Sprite> otherSpritesDictionary = new Dictionary<string, Sprite>();

    public IObservable<Unit> OnLoadFollowerImages
    {
        get { return loadFollowerImagesSubject; }
    }

    public IObservable<Unit> OnLoadEnemyImages
    {
        get { return loadEnemyImagesSubject; }

    }

    public IObservable<Unit> OnLoadOtherImages
    {
        get { return loadOtherImagesSubject; }
    }

    public void LoadFollowerImages(List<string> urls)
    {
        followerImageUrls = urls;
        Debug.Log("Load Follower Images");
        Observable.FromCoroutine(GetFollowerImages).Subscribe();
    }

    public void LoadEnemyImages(List<string> names)
    {
        enemyNames = names;
        Observable.FromCoroutine(GetEnemyImages).Subscribe();
    }

    public void LoadOtherImages(Dictionary<string, string> paths)
    {
        otherImagePaths = paths;
        GetOtherImages();
    }
    IEnumerator GetFollowerImages()
    {
        bool debug = true;
        if (debug) {
            for (int i = 0; i < 3; i++)
            {

                followerSprite[i] = Resources.Load<Sprite>("Sprites/DebugCharacter/Debug" + i);
                Texture2D texture = followerSprite[i].texture;
                TextureScale.Bilinear(texture, 120, 120);
                followerSprite[i] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                WWW www = new WWW(followerImageUrls[i]);
                yield return www;

                Texture2D newTex = www.texture;
                TextureScale.Bilinear(newTex, 120, 120);
                followerSprite[i] = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), new Vector2(0.5f, 0.5f));
            }
        }
        loadFollowerImagesSubject.OnNext(Unit.Default);
    }

    IEnumerator GetEnemyImages()
    {
        foreach (string name in enemyNames)
        {
            WWW www = new WWW(UrlConsts.enemyImage + name + ".png");
            yield return www;
            enemySprites.Add(name, Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f)));
        }
        loadEnemyImagesSubject.OnNext(Unit.Default);
    }

    void GetOtherImages()
    {
        foreach (KeyValuePair<string, string> pair in otherImagePaths)
        {
            Sprite sprite = Resources.Load<Sprite>(pair.Value);
            otherSpritesDictionary.Add(pair.Key, sprite);
        }
        loadOtherImagesSubject.OnNext(Unit.Default);
    }
}

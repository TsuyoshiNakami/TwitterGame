using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using Zenject;

public class BattleSceneLoader {
    StageDetailLoader stageDetailLoader;
    SkillLoader skillLoader;
    EnemyAbilityLoader enemyAbilityLoader;
    FollowerAbilityLoader followerAbilityLoader;
    ImageLoader imageLoader;
    BattleDataLibrary battleDataLibrary;

    Subject<Unit> allDataLoadedSubject = new Subject<Unit>();
    public IObservable<Unit> OnAllDataLoaded
    {
        get { return allDataLoadedSubject; }
    }

    int stageNo = 0;
    public BattleSceneLoader(BattleDataLibrary battleDataLibrary)
    {
        stageDetailLoader = new StageDetailLoader();
        skillLoader = new SkillLoader();
        enemyAbilityLoader = new EnemyAbilityLoader();
        followerAbilityLoader = new FollowerAbilityLoader();
        imageLoader = new ImageLoader();
        this.battleDataLibrary = battleDataLibrary;
        Debug.Log("BattleSceneLoader : Awake");

    }
    public void Load()
    {
        LoadStageDetails();

        LoadOtherImages();

    }

    void LoadOtherImages()
    {

        Dictionary<string, string> otherImagePaths = new Dictionary<string, string>();
        otherImagePaths.Add(TwitterGameConsts.battleFollowerMask, TwitterGameConsts.battleFollowerMask);
        imageLoader.OnLoadOtherImages.Subscribe(_ =>
        {
            Debug.Log("On Load Other Images");
            battleDataLibrary.otherSpritesDictionary = imageLoader.otherSpritesDictionary;
        });
        imageLoader.LoadOtherImages(otherImagePaths);
    }

    void LoadStageDetails()
    {
        stageDetailLoader.OnLoadStageDetails.Subscribe(_ => {
            Debug.Log("StageDetails Loaded");
            battleDataLibrary.stageDetails = stageDetailLoader.stageDetails;
            LoadSkills();
        });

        stageDetailLoader.LoadStageDetails(stageNo);
    }

    void LoadSkills()
    {
        skillLoader.OnLoadSkills.Subscribe(_ => {
            Debug.Log("Skills Loaded");
            battleDataLibrary.skillEntities = skillLoader.SkillEntities;

            LoadAbilities();
        });

        skillLoader.LoadSkills();
    }

    void LoadAbilities()
    {
        enemyAbilityLoader.OnLoadEnemyAbility.Subscribe(_ =>
        {
            Debug.Log("EnemyAbilities loaded");
            battleDataLibrary.enemyEntityDictionary = enemyAbilityLoader.EnemyAbilityDictionary;


        });
        followerAbilityLoader.OnLoadFollowerAbility.Subscribe(_ =>
        {
            Debug.Log("followerAbilities loaded");
            battleDataLibrary.followerEntities = followerAbilityLoader.FollowerEntities;

        });

        Observable.Zip(enemyAbilityLoader.OnLoadEnemyAbility, followerAbilityLoader.OnLoadFollowerAbility).Subscribe(_ =>
        {
            Debug.Log("Abilities Loaded");

            LoadImages();
        });
        enemyAbilityLoader.LoadAbility();
        followerAbilityLoader.LoadAbility();
    }

    void LoadImages()
    {
        imageLoader.OnLoadEnemyImages
            .Subscribe(_ =>
        {
            battleDataLibrary.SetEnemyImages(imageLoader.enemySprites);
            Debug.Log("OnLoadEnemyImages load");
        });

        imageLoader.OnLoadFollowerImages
            .Subscribe(_ =>
        {
            Debug.Log("OnLoadFollowerImages load");
            for (int i = 0; i < 3; i++)
            {
                battleDataLibrary.followerEntities[i].sprite = imageLoader.followerSprite[i];
            }
        });
        Observable.Zip(imageLoader.OnLoadEnemyImages, imageLoader.OnLoadFollowerImages).Subscribe(_ =>
        {
            Debug.Log("Images Loaded");

            allDataLoadedSubject.OnNext(Unit.Default);
        });

        imageLoader.LoadEnemyImages(battleDataLibrary.GetEnemyNames());
        imageLoader.LoadFollowerImages(battleDataLibrary.GetFollowerImageUrls());
    }
	
}

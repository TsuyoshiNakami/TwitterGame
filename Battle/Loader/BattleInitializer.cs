using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using Zenject;

public class BattleInitializer {

    private FriendSpawner friendSpawner;
    BattleDataLibrary battleDataLibrary;
    BattleUISpawner battleUISpawner;
    HPGaugeController hpGaugeController;
    NameTextController nameTextController;
    Subject<Unit> battleInitializeSubject = new Subject<Unit>();
    public IObservable<Unit> OnBattleInitialized
    {
        get { return battleInitializeSubject; }
    }
    public BattleInitializer(BattleDataLibrary battleDataLibrary, HPGaugeController hpGaugeController)
    {
        friendSpawner = new FriendSpawner();
        battleUISpawner = new BattleUISpawner();

        this.battleDataLibrary = battleDataLibrary;
        this.hpGaugeController = hpGaugeController;
    }

    public void StartInit()
    {
        SpawnFriend();
    }
    void SpawnFriend()
    {
        friendSpawner.OnFriendSpawned.Subscribe(_ =>
        {
            Debug.Log("Spawn Friend");
            SpawnSkillPanel();
        });
        friendSpawner.SpawnFriend(battleDataLibrary.followerEntities, battleDataLibrary.otherSpritesDictionary[TwitterGameConsts.battleFollowerMask].texture);
    }

    void SpawnSkillPanel()
    {
        battleUISpawner.OnBattleUISpawned.Subscribe(msg =>
        {
            if (msg == "skillPanel")
            {
                Debug.Log("Skill Panel Loaded.");
                InitHPText();
                InitNameText();
            }
        });
        battleUISpawner.SpawnSkillPanel();
    }

    void InitHPText()
    {
        hpGaugeController = new HPGaugeController();
        for (int i = 0; i < 3; i++)
        {
            hpGaugeController.SetFollowerHPUI(i, battleDataLibrary.followerEntities[i].hp, battleDataLibrary.followerEntities[i].hp);
        }
        battleInitializeSubject.OnNext(Unit.Default);
    }

    void InitNameText()
    {
        nameTextController = new NameTextController();
        for (int i = 0; i < 3; i++)
        {
            nameTextController.SetFollowerNameText(i, battleDataLibrary.followerEntities[i].name);
        }
    }
}

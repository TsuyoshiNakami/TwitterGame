using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using UniRx;
using SimpleJSON;

public class FollowerAbilityLoader
{
    List<FollowerEntity> followerEntities = new List<FollowerEntity>();
    Subject<Unit> loadFollowerAbilitySubject = new Subject<Unit>();
    public IObservable<Unit> OnLoadFollowerAbility
    {
        get { return loadFollowerAbilitySubject; }
    }

    public List<FollowerEntity> FollowerEntities
    {
        get { return followerEntities; }
    }

    public void LoadAbility()
    {
        for (int i = 0; i < 3; i++)
        {
            followerEntities.Add(new FollowerEntity());
        }
        Observable.FromCoroutine(GetFollowerAbilities).Subscribe();
    }

    IEnumerator GetFollowerAbilities()
    {
        WWWForm form = new WWWForm();
        form.AddField("sql", "SELECT * FROM `followers`;");
        WWW result = new WWW(UrlConsts.execute, form.data);
        yield return result;

        var resultJson = JSON.Parse(result.text);
        for (int i = 0; i < resultJson.Count; i++)
        {
            var regularData = resultJson[i];

            int num = int.Parse(regularData["is_regular"]) - 1;

            if (num < 0) continue;
            followerEntities[num].name = regularData[TwitterGameConsts.ColumnFollowerName];
            followerEntities[num].image_url = regularData["image_url"];
            followerEntities[num].hp = int.Parse(regularData["hp"]);
            followerEntities[num].maxHp = followerEntities[num].hp;
            followerEntities[num].attribute = (CharacterAttribute)Enum.Parse(typeof(CharacterAttribute), regularData["attribute"], true);

            followerEntities[num].attackForce = int.Parse(regularData["attack_force"]);
            followerEntities[num].defenceForce = int.Parse(regularData["defence_force"]);
            followerEntities[num].speed = int.Parse(regularData["speed"]);
            followerEntities[num].type = "Follower";
            followerEntities[num].battleId = num;
            followerEntities[num].obj = GameObject.Find("FollowerUI" + num + "/Profile/Image");
            Debug.Log(followerEntities[num].obj + "うひょー" + i);
            for (int n = 0; n < 3; n++)
            {
                if (regularData["skill" + n] == null)
                {
                    regularData["skill" + n] = "";
                }
                followerEntities[num].skillNames[n] = regularData["skill" + n];

            }
        }

        loadFollowerAbilitySubject.OnNext(Unit.Default);

    }
}

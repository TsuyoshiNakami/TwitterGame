using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using UniRx;
using SimpleJSON;

public class EnemyAbilityLoader
{
    Dictionary<string, EnemyEntity> ememyAbilityDictionary = new Dictionary<string, EnemyEntity>();
    Subject<Unit> loadEnemyAbilitySubject = new Subject<Unit>();

    public Dictionary<string, EnemyEntity> EnemyAbilityDictionary
    {
        get { return ememyAbilityDictionary; }
    }
    public IObservable<Unit> OnLoadEnemyAbility
    {
        get { return loadEnemyAbilitySubject; }
    }

    public void LoadAbility()
    {
        Observable.FromCoroutine(GetEnemyData).Subscribe();
    }

    IEnumerator GetEnemyData()
    {
        WWWForm form = new WWWForm();
        form.AddField("sql", "SELECT * FROM `enemy`;");
        WWW result = new WWW(UrlConsts.execute, form.data);
        yield return result;
        Debug.Log("GetEnemyData" + result.text);
        JSONNode resultJson = JSON.Parse(result.text);
        for (int i = 0; i < resultJson.Count; i++)
        {
            var enemyData = resultJson[i];


            if (enemyData["enemy_name"].ToString() == "") { continue; }
            string[] _skillNames = { enemyData["skill0"], enemyData["skill1"], enemyData["skill2"] };

            foreach (SkillEntity skillEntity in SkillManager.skillList)
            {
                for (int n = 0; n < 3; n++)
                {
                    //  名前が同じ技がリストにあれば
                    if (int.Parse(enemyData["skill" + n]) == skillEntity.skill_no)
                    {
                        _skillNames[n] = skillEntity.skill_name;
                    }

                    if (int.Parse(enemyData["skill" + n]) == 0)
                    {
                        _skillNames[n] = "NormalBullet";
                    }
                }
            }
            
            EnemyEntity entity = new EnemyEntity()
            {
                hp = int.Parse(enemyData["hp"]),
                maxHp = int.Parse(enemyData["hp"]),
                name = enemyData["enemy_name"],
                enemy_no = enemyData["enemy_no"],
                attackForce = int.Parse(enemyData["attack_force"]),
                defenceForce = int.Parse(enemyData["defence_force"]),
                skillNames = _skillNames,
                attribute = (CharacterAttribute)Enum.ToObject(typeof(CharacterAttribute), int.Parse(enemyData["attribute"].Value))
            };
            ememyAbilityDictionary.Add(entity.name, entity);
        }
        loadEnemyAbilitySubject.OnNext(Unit.Default);
    }
}

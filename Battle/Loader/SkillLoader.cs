using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using UniRx;
using SimpleJSON;

public class SkillLoader
{
    List<SkillEntity> skillEntityList = new List<SkillEntity>();

    public List<SkillEntity> SkillEntities
    {
        get
        {
            return skillEntityList;
        }
    }

    Subject<Unit> loadSkillSubject = new Subject<Unit>();
    public IObservable<Unit> OnLoadSkills
    {
        get { return loadSkillSubject; }
    }

    public void LoadSkills()
    {
        Observable.FromCoroutine(GetSkillData).Subscribe();

    }

    IEnumerator GetSkillData()
    {
        string sql = "select * from skill";
        WWWForm form = new WWWForm();
        form.AddField("sql", sql);
        WWW www = new WWW(UrlConsts.execute, form.data);

        yield return www;

        var resultJson = JSON.Parse(www.text);
        if (resultJson != null)
        {
            SetSkillData(resultJson);
        }

        loadSkillSubject.OnNext(Unit.Default);
    }

    void SetSkillData(JSONNode resultJson)
    {

        for (int i = 0; i < resultJson.Count; i++)
        {
            var tempEntity = resultJson[i];


            if (tempEntity["skill_name"].ToString() == "") { continue; }
            SkillEntity entity = new SkillEntity()
            {
                skill_name = tempEntity["skill_name"],
                description = tempEntity["description"],
                skill_no = tempEntity["skill_no"],
                power = int.Parse(tempEntity["power"]),
                attribute = (CharacterAttribute)Enum.ToObject(typeof(CharacterAttribute), int.Parse(tempEntity["attribute"].Value)),
                charge_time = float.Parse(tempEntity["charge_time"]),
                after_attack_time = float.Parse(tempEntity["after_attack_time"])

            };
            skillEntityList.Add(entity);
        }
    }
}

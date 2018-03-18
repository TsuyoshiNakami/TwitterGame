using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.Globalization;
using System;
using UnityEngine;

public class GachaFollowerEntity{
    public CharacterAttribute attribute;
    public int followersCount;
    public int friendsCount;
    public int favoritesCount;
    public int tweetsCount;
    //Twitterを始めてから今までの合計日数
    public int totalDays;
    //1日当たりのツイート数
    public float tweetPerDay;
    //
    public float followerRatio;
    public DateTime createdDate;
}

public class FollowerAbilityMaker{

    private GachaFollowerEntity gachaFollowerEntity;

    public void SetGachaFollowerEntityFromJson(JSONNode json, CharacterAttribute attribute)
    {

        DateTime now = DateTime.Now;
        DateTime createdDate = GetCreatedAtDate(json);
        int totalDays = (int)(now - createdDate).TotalDays;
        gachaFollowerEntity = new GachaFollowerEntity
        {
            followersCount = int.Parse(json[0]["followers_count"]),
            friendsCount = int.Parse(json[0]["friends_count"]),
            favoritesCount = int.Parse(json[0]["favourites_count"]),
            tweetsCount = int.Parse(json[0]["statuses_count"]),
            attribute = attribute,
            createdDate  = createdDate,
            //Twitterを始めてから今までの合計日数
            totalDays = totalDays,
            tweetPerDay = int.Parse(json[0]["statuses_count"]) / totalDays,
            followerRatio = int.Parse(json[0]["followers_count"]) / int.Parse(json[0]["friends_count"])
        };
    }

    DateTime GetCreatedAtDate(JSONNode json)
    {
        string jsonStr = json[0]["created_at"];
        string[] createdDateStr = jsonStr.Split(' ');
        DateTimeFormatInfo createdDateFormat = new DateTimeFormatInfo();

        int year = int.Parse(createdDateStr[5]);
        int month = Array.IndexOf(createdDateFormat.AbbreviatedMonthNames, createdDateStr[1]) + 1;
        int day = int.Parse(createdDateStr[2]);

        return new DateTime(year, month, day);
    }
    public int DecideHp()
    {
        int hp = 0;
        //最大でこの値がHPにプラスされる
        int maxHpRange = 10;
        if (Math.Abs(gachaFollowerEntity.followerRatio) < 1.5f) {
            hp += (15 - (int)Math.Abs(gachaFollowerEntity.followerRatio)) * maxHpRange;
        }

        switch (gachaFollowerEntity.attribute)
        {
            case CharacterAttribute.Fire:
                hp = 20;
                break;
            case CharacterAttribute.Water:
                hp = 50;
                break;
            case CharacterAttribute.Magic:
                hp = 18;
                break;
            case CharacterAttribute.Normal:
                hp = 38;
                break;
            case CharacterAttribute.Sky:
                hp = 29;
                break;
            case CharacterAttribute.Thunder:
                hp = 22;
                break;
            case CharacterAttribute.Plant:
                hp = 26;
                break;
            default:
                hp = 1;
                break;
        }

        return hp;
    }
    public int DecideAttackForce()
    {

        int attackForce = 0;
        int forceRange = 0;
        if(5 <= gachaFollowerEntity.tweetPerDay)
        {
            //ツイートが多いほど攻撃力が高くなる
            gachaFollowerEntity.tweetPerDay = (int)Mathf.Clamp(gachaFollowerEntity.tweetPerDay, 1, 10);
            forceRange = (int)UnityEngine.Random.Range(gachaFollowerEntity.tweetPerDay, gachaFollowerEntity.tweetPerDay + 2);
        }

        if (gachaFollowerEntity.tweetPerDay <= 2)
        {
            forceRange = (int)UnityEngine.Random.Range(-1, -3);
        }

        attackForce += forceRange;

        switch (gachaFollowerEntity.attribute)
        {
            case CharacterAttribute.Fire:
                attackForce += 12;
                break;
            case CharacterAttribute.Water:
                attackForce += 5;
                break;
            case CharacterAttribute.Magic:
                attackForce += 5;
                break;
            case CharacterAttribute.Normal:
                attackForce += 8;
                break;
            case CharacterAttribute.Sky:
                attackForce += 10;
                break;
            case CharacterAttribute.Thunder:
                attackForce += 9;
                break;
            case CharacterAttribute.Plant:
                attackForce += 7;
                break;
            default:
                attackForce += 1;
                break;
        }

        return attackForce;
    }

    public int DecideDefenceForce()
    {
        int defenceForce = 0;
        int defenceRange = 0;
        if (5 <= gachaFollowerEntity.tweetPerDay)
        {
            defenceRange = (int)UnityEngine.Random.Range(-1, -3);

        }

        if (gachaFollowerEntity.tweetPerDay <= 2)
        {
            //ツイートが少ないほど防御力が高くなる
            defenceRange = (int)UnityEngine.Random.Range(3 - gachaFollowerEntity.tweetPerDay, 6 - gachaFollowerEntity.tweetPerDay);
        }

        defenceForce += defenceRange;
        switch (gachaFollowerEntity.attribute)
        {
            case CharacterAttribute.Fire:
                defenceForce = 3;
                break;
            case CharacterAttribute.Water:
                defenceForce = 8;
                break;
            case CharacterAttribute.Magic:
                defenceForce = 1;
                break;
            case CharacterAttribute.Normal:
                defenceForce = 3;
                break;
            case CharacterAttribute.Sky:
                defenceForce = 6;
                break;
            case CharacterAttribute.Thunder:
                defenceForce = 1;
                break;
            case CharacterAttribute.Plant:
                defenceForce = 2;
                break;
            default:
                defenceForce = 1;
                break;
        }

        return defenceForce;
    }

    public string[] DecideSkills()
    {
        string[] skills = new string[3];
        switch (gachaFollowerEntity.attribute)
        {
            case CharacterAttribute.Fire:
                skills[0] = "FlameRadiation";
                break;
            case CharacterAttribute.Water:
                skills[0] = "WaterPistol";
                break;
            case CharacterAttribute.Magic:
                skills[0] = "BodyBlow";
                break;
            case CharacterAttribute.Normal:
                skills[0] = "BodyBlow";
                break;
            case CharacterAttribute.Sky:
                skills[0] = "BodyBlow";
                break;
            case CharacterAttribute.Thunder:
                skills[0] = "BodyBlow";
                break;
            case CharacterAttribute.Plant:
                skills[0] = "BodyBlow";
                break;
            default:
                break;
        }

        return skills;
    }
}

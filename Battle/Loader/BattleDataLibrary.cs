using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDataLibrary {

    public List<string> stageDetails = new List<string>();
    public List<SkillEntity> skillEntities = new List<SkillEntity>();
    public Dictionary<string, EnemyEntity> enemyEntityDictionary = new Dictionary<string, EnemyEntity>();
    public List<FollowerEntity> followerEntities = new List<FollowerEntity>();
    public Dictionary<string, Sprite> otherSpritesDictionary = new Dictionary<string, Sprite>();

    public  void SetEnemyEntityDictionary(Dictionary<string, EnemyEntity> param)
    {
        enemyEntityDictionary = param;
    }

    public void SetEnemyImages(Dictionary<string, Sprite> param)
    {
        foreach(KeyValuePair<string, Sprite> pair in param)
        {
            enemyEntityDictionary[pair.Key].sprite = pair.Value;
        }
    }

    public List<string> GetEnemyNames()
    {
        List<string> enemyNames = new List<string>();
        foreach(string name in enemyEntityDictionary.Keys)
        {
            enemyNames.Add(name);
        }
        return enemyNames;
    }

    public List<FollowerEntity> GetFollowerEntities()
    {
        return followerEntities;
    }

    public List<string> GetFollowerImageUrls()
    {
        List<string> followerNames = new List<string>();
        foreach (FollowerEntity entity in followerEntities)
        {
            followerNames.Add(entity.image_url);
        }
        return followerNames;
    }

    public EnemyEntity GetEnemyEntity(string name)
    {
        return enemyEntityDictionary[name];
    }
    public EnemyEntity GetEnemyClone(string name)
    {
        return (EnemyEntity)GetEnemyEntity(name).GetClone();
    }
}

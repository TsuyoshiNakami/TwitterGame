using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;

public class EnemySpawner : MonoBehaviour
{

    GameObject emptyEnemy;

    Subject<Unit> EnemySpawnSubject = new Subject<Unit>();

    HPGaugeController hpGaugeController;

    public IObservable<Unit> OnEnemySpawned
    {
        get { return EnemySpawnSubject; }
    }

    private Subject<GameObject> enemyClickedSubject = new Subject<GameObject>();
    public IObservable<GameObject> OnEnemyClicked
    {
        get { return enemyClickedSubject; }
    }

    private Subject<int> enemyDeadSubject = new Subject<int>();
    public IObservable<int> OnEnemyDead
    {
        get { return enemyDeadSubject; }
    }
    public EnemySpawner(HPGaugeController hpGaugeController)
    {
        emptyEnemy = Resources.Load<GameObject>(TwitterGameConsts.emptyEnemy);
        this.hpGaugeController = hpGaugeController;
    }


    public EnemyController2 SpawnEnemy(int pos, EnemyEntity entity)
    {
        GameObject respawnObj = GameObject.Find("EnemyRespawnPoint" + pos);

        GameObject newEnemy;

        Debug.Log("Spawn Enemy");
        newEnemy = Instantiate(emptyEnemy, respawnObj.transform.position, respawnObj.transform.rotation);
        
        if (newEnemy == null)
        {
            return null;
        }
        newEnemy.name = "Enemy" + pos;
        Debug.Log("Spawn Enemy2");
        newEnemy.GetComponentInChildren<SpriteRenderer>().sprite = entity.sprite;
        EnemyController2 newController = newEnemy.GetComponent<EnemyController2>();

        SetEnemyData(newController, entity);
        Debug.Log("entity" + entity.hp);
        hpGaugeController.SetEnemyHPUI(pos, entity.hp, entity.hp);
        //newController.SetSkill();


        //       GameObject.Find("HPGaugeE" + pos).GetComponent<HPGauge>().SetParent(newController);
        newController.GetComponent<EnemyController2>().OnClicked.Subscribe(obj =>
        {
            Destroy(newEnemy);
            /**************あとで消す************************************/
            enemyDeadSubject.OnNext(pos);
            /***************************************************/
            enemyClickedSubject.OnNext(obj);
        });

        newController.GetComponent<EnemyController2>().OnDead.Subscribe(enemyController =>
        {
            Debug.Log("EnemyDead");
            enemyDeadSubject.OnNext(pos);
        });

        return newController;
    }

    private SpriteRenderer attachSprite(GameObject maskObj, Sprite sprite, Vector2 position)
    {
        GameObject spriteObj = new GameObject("Sprite");
        spriteObj.transform.parent = maskObj.transform;
        spriteObj.transform.localPosition = position;
        SpriteRenderer spriteRenderer = spriteObj.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 5;
        return spriteRenderer;
    }

    private void SetEnemyData(EnemyController2 enemyController, EnemyEntity entity)
    {
        enemyController.hp = entity.hp;
        enemyController.attackForce = entity.attackForce;
        enemyController.defenceForce = entity.defenceForce;
        enemyController.attribute = entity.attribute;
        enemyController.skillNames = entity.skillNames;
        enemyController.transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite = entity.sprite;
    }
}

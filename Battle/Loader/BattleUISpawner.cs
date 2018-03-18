using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;

public class BattleUISpawner : MonoBehaviour
{
    Subject<string> battleUISpawnSubject = new Subject<string>();
    GameObject skillPanelManagerObj;


    public IObservable<string> OnBattleUISpawned
    {
        get { return battleUISpawnSubject; }
    }

    public BattleUISpawner()
    {
        skillPanelManagerObj = Resources.Load<GameObject>(TwitterGameConsts.skillPanelManager);

    }

    public void SpawnSkillPanel()
    {
        GameObject skillPanel = Instantiate(skillPanelManagerObj);
        battleUISpawnSubject.OnNext("skillPanel");
    }


}

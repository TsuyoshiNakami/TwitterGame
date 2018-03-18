using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using SimpleJSON;

public class StageDetailLoader {
    Subject<Unit> loadStageDetailSubject = new Subject<Unit>();
    int stageNo;
    List<string> stageData = new List<string>();

    public List<string> stageDetails
    {
        get { return stageData; }
    }

    public IObservable<Unit> OnLoadStageDetails
    {
        get { return loadStageDetailSubject; }
    }

    public void LoadStageDetails(int stageNo)
    {
        this.stageNo = stageNo;
        Observable.FromCoroutine(GetStageDetailData).Subscribe();

    }
    IEnumerator GetStageDetailData()
    {

        string sql = "select * from stage_detail " +
            "where stage_id = " + stageNo + " " +
            "order by detail_no";
        WWWForm form = new WWWForm();
        form.AddField("sql", sql);
        WWW www = new WWW(UrlConsts.execute, form.data);

        yield return www;

        var resultJson = JSON.Parse(www.text);


        if (resultJson != null)
        {
            for (int i = 0; i < resultJson.Count; i++)
            {
                var stageDetailData = resultJson[i];

                stageData.Add(stageDetailData["value"]);

            }
        }
        else
        {
            Debug.LogError("Jsonが空です。");
        }
        loadStageDetailSubject.OnNext(Unit.Default);

    }
}

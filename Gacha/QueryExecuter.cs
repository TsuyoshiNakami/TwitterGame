using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UniRx;

public class QueryExecuter : EditorWindow
{

    static QueryExecuter queryExecuter;
    string sql;
    [MenuItem("Window/TwitterGame/QueryExecuter")]
    static void Open()
    {
        if(queryExecuter == null)
        {
            queryExecuter = CreateInstance<QueryExecuter>();
        }
        queryExecuter.ShowUtility();
    }

    void OnGUI()
    {
        sql = EditorGUILayout.TextArea(sql, GUILayout.ExpandHeight(true));
       if( GUILayout.Button("実行")) {
            {
                Observable.FromCoroutine(Execute).Subscribe(_ => {
                });
            }
        }
    }

    IEnumerator Execute()  
    {
        WWWForm form = new WWWForm();
        form.AddField("sql", sql);
        WWW www = new WWW(UrlConsts.execute, form.data);

        yield return www;
        Debug.Log(www.text);

    }
}

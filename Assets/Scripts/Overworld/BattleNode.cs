using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleNode : MapNode
{
    public override void enterNode()
    {
        base.enterNode();
        {
            GameManager.Instance.Battle(this);
            
            //StartCoroutine(sceneLoadCoroutine());
        }
    }

    IEnumerator sceneLoadCoroutine()
    {
        //SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
        string sceneName = "BattleScene";
        var loading = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return loading;
        var scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(scene);
    }
}

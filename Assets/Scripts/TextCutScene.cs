using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextCutScene : MonoBehaviour
{
    private string _sceneID;

    public TextCutScene(string scene_id)
    {
        _sceneID = scene_id;
    }

    public string SceneID
    {
        get { return _sceneID; }
        set { _sceneID = value; }
    }
}

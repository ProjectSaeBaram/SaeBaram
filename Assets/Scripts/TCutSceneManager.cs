using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class TCutSceneManager : MonoBehaviour
{
    private Task fetchDataTask;
    private CharacterScript[] characterScripts;
    private Coroutine coroutineHandler;
    private bool isSpacePressed;
    private TextMeshProUGUI displayText;

    [SerializeField] private CinemachineBlendController _cinemachineBlendController;
    [SerializeField] private BossRoom _bossRoom;
    
    public enum Speaker {
        곰,
        가온,
    }
    
    [System.Serializable]
    public class CharacterScript
    {
        public int index;
        public string name;
        public string script;
        public string emotion;
    }
    
    void Start()
    {
        Debug.Log("TCutSceneManager Initiated");
    }
    
    void Update()
    {
        // 비동기 작업 완료 확인
        if (fetchDataTask != null && fetchDataTask.IsCompleted)
        {
            Debug.Log("Data fetch completed");
            fetchDataTask = null; // 완료된 후 다시 확인하지 않도록 설정
        }
        
    }

    public async void TriggerCutScene()
    {
        // Time.timeScale = 0;
        
        // 비동기 작업 시작
        await FetchDataAsync();

        Debug.Log("Executed");

        StartCoroutine(ScriptCoroutine());
    }

    IEnumerator ScriptCoroutine()
    {
        foreach (var script in characterScripts)
        {
            Debug.Log($"Index: {script.index}, Name: {script.name}, Script: {script.script}, Emotion : {script.emotion}");
            //displayText.text = $"Index: {script.index}, Name: {script.name}, Script: {script.script}";
            coroutineHandler = StartCoroutine(DelayCoroutine());
            
            // 카메라 조작
            if (Enum.TryParse(script.name, out Speaker speaker))
            {
                switch (speaker)
                {
                    case Speaker.곰:
                        _cinemachineBlendController.LookBear();
                        Managers.ScriptDialogue.GetTalk(script.index, script.name, script.script, script.emotion);
                        break;
                    case Speaker.가온:
                        _cinemachineBlendController.LookPlayer();
                        Managers.ScriptDialogue.GetTalk(script.index, script.name, script.script, script.emotion);
                        break;
                    default:
                        DebugEx.LogWarning("Speaker Unidentified!");
                        break;
                }
            }

            // 사용자가 스페이스 키를 눌렀다면 바로 다음 스크립트로 넘어갑s니다.
            while (coroutineHandler != null && !isSpacePressed)
            {
                yield return null;
            }

            // 스페이스 키를 감지하면 다음 스크립트로 넘어가도록 플래그를 초기화합니다.
            isSpacePressed = false;
            
        }
        _bossRoom.EngageStart();
    }

    IEnumerator DelayCoroutine()
    {
        float timer = 0f;
        while (timer < 5f && !isSpacePressed)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        coroutineHandler = null; // DelayCoroutine이 완료되면 null로 설정합니다.
    }

    private async Task FetchDataAsync()
    {
        TextCutScene scene_1 = new TextCutScene("scene_1");
        HTTPSystem httpSystem = new HTTPSystem(scene_1);
        try
        {
            // String 형으로된 Json 가져오기
            string data = await httpSystem.GetSheetDataAsync();

            // JSON 데이터 파싱
            characterScripts = JsonUtility.FromJson<Wrapper>("{\"characters\":" + data + "}").characters;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    [System.Serializable]
    private class Wrapper
    {
        public CharacterScript[] characters;
    }
}

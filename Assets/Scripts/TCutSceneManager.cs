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
    
    enum Speaker {
        곰,
        가온,
    }
    
    [System.Serializable]
    public class CharacterScript
    {
        public int index;
        public string name;
        public string script;
    }
    
    void Start()
    {
        Debug.Log("TCutSceneManager Initiated");
        
        TMP_FontAsset sam3KRFont = Resources.Load<TMP_FontAsset>("Fonts/Sam3KRFont SDF");

        GameObject canvasObject = new GameObject("Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        // TextMeshProUGUI 표시를 위한 컴포넌트 추가
        GameObject textObject = new GameObject("DisplayText");
        textObject.transform.SetParent(canvas.transform);
        displayText = textObject.AddComponent<TextMeshProUGUI>();
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(800, 200);
        textRect.anchoredPosition = new Vector2(0, -100);
        displayText.fontSize = 24;
        displayText.alignment = TextAlignmentOptions.Center;
        displayText.color = Color.black;
        displayText.font = sam3KRFont;
    }
    
    void Update()
    {
        // 비동기 작업 완료 확인
        if (fetchDataTask != null && fetchDataTask.IsCompleted)
        {
            Debug.Log("Data fetch completed");
            fetchDataTask = null; // 완료된 후 다시 확인하지 않도록 설정
        }

        // 키보드 입력 감지
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     Debug.Log("Space key pressed");
        //     isSpacePressed = true;
        // }
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
            Debug.Log($"Index: {script.index}, Name: {script.name}, Script: {script.script}");
            displayText.text = $"Index: {script.index}, Name: {script.name}, Script: {script.script}";
            coroutineHandler = StartCoroutine(DelayCoroutine());
            
            // 카메라 조작
            if (Enum.TryParse(script.name, out Speaker speaker))
            {
                switch (speaker)
                {
                    case Speaker.곰:
                        _cinemachineBlendController.LookBear();
                        break;
                    case Speaker.가온:
                        _cinemachineBlendController.LookPlayer();
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

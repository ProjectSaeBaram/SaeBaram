using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TCutSceneManager : MonoBehaviour
{
    private Task fetchDataTask;
    private CharacterScript[] characterScripts;
    private Coroutine coroutineHandler;
    public event Action<KeyCode> OnKeyPressed;

    [System.Serializable]
    public class CharacterScript
    {
        public int index;
        public string name;
        public string script;
    }

    // Start is called before the first frame update
    void Start()
    {
        OnKeyPressed += HandleKeyPressed;

        GameObject canvasObject = new GameObject("Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject buttonObject = new GameObject("FetchDataButton");
        Button fetchDataButton = buttonObject.AddComponent<Button>();
        RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
        rectTransform.SetParent(canvas.transform);
        rectTransform.sizeDelta = new Vector2(200, 50);
        rectTransform.anchoredPosition = new Vector2(0, 0);
        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = Color.white;

        fetchDataButton.onClick.AddListener(OnFetchDataButtonClicked);

        Debug.Log("Start method completed");
    }

    // Update is called once per frame
    void Update()
    {
        // 비동기 작업 완료 확인
        if (fetchDataTask != null && fetchDataTask.IsCompleted)
        {
            Debug.Log("Data fetch completed");
            fetchDataTask = null; // 완료된 후 다시 확인하지 않도록 설정
        }

        // 키보드 입력 감지
        if (Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    Debug.Log($"Key pressed: {keyCode}");
                    OnKeyPressed?.Invoke(keyCode);
                }
            }
        }
    }

    private async void OnFetchDataButtonClicked()
    {
        // 비동기 작업 시작
        await FetchDataAsync();

        Debug.Log("Executed");

        coroutineHandler = StartCoroutine(ScriptCoroutine());
    }

    IEnumerator ScriptCoroutine()
    {
        foreach (var script in characterScripts)
        {
            Debug.Log($"Index: {script.index}, Name: {script.name}, Script: {script.script}");
            yield return new WaitForSeconds(1f);
        }
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

    // 키 입력 이벤트 핸들러
    void HandleKeyPressed(KeyCode keyCode)
    {
        Debug.Log($"HandleKeyPressed invoked with key: {keyCode}");
        if (coroutineHandler != null)
        {
            StopCoroutine(coroutineHandler);
            coroutineHandler = null;
            Debug.Log("Coroutine stopped");
        }
    }

    [System.Serializable]
    private class Wrapper
    {
        public CharacterScript[] characters;
    }
}

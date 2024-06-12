using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class TCutSceneManager : MonoBehaviour
{
    private Task fetchDataTask;
    CharacterScript[] characterScripts;

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
        GameObject canvasObject = new GameObject("Canvas");
        Canvas canvas= canvasObject.AddComponent<Canvas>();
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
    }

    private async void OnFetchDataButtonClicked()
    {
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
        catch (System.Exception ex)
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

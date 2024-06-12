using System.Collections;
using UnityEngine;

public class CoroutineHelper : MonoBehaviour
{
    private static MonoBehaviour monoInstance;
    
    [RuntimeInitializeOnLoadMethod]
    private static void Initializer()
    {
        monoInstance = new GameObject($"[{nameof(CoroutineHelper)}]").AddComponent<CoroutineHelper>();
        DontDestroyOnLoad(monoInstance.gameObject);
    }
    
    public static new Coroutine StartCoroutine(IEnumerator coroutine)
    {
        return monoInstance.StartCoroutine(coroutine);
    }

    public static new void StopCoroutine(Coroutine coroutine)
    {
        monoInstance.StopCoroutine(coroutine);
    }
}
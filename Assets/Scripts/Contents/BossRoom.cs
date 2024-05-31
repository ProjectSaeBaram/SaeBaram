using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Player가 BossRoom에 진입함을 감지하는 객체
/// </summary>
public class BossRoom : MonoBehaviour
{
    /// <summary>
    /// Player가 BossRoom에 진입할 때 Invoke시킬 UnityEvents
    /// </summary>
    [Tooltip("Player가 BossRoom에 진입할 때 Invoke시킬 UnityEvents")]
    [SerializeField] private UnityEvent _onPlayerEntered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            DebugEx.Log("Entered BossRoom!");
            UI_Scene sceneUI = Managers.UI.GetCurrentSceneUI();

            Managers.UI.MakeSubItem<UI_BossHPBar>(sceneUI.transform);
            _onPlayerEntered.Invoke();
        }
    }
}

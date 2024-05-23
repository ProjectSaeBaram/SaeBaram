using UnityEngine;

public class Handled_Item : MonoBehaviour
{
    [SerializeField] private ToolStatsDatabase _toolStatsDatabase;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private UI_Inven_Item _uiInvenItem;
    
    private void OnEnable()
    {
        _spriteRenderer.sprite = _uiInvenItem?.image.sprite;
    }

    private void OnDisable()
    {
        _spriteRenderer.sprite = null;
    }

    public void ItemUIReferenceSetter(UI_Inven_Item item)
    {
        _uiInvenItem = item;
    }
}

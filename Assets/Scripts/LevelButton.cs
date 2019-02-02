using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public static LevelButton SelectedButton;
    
    public Text levelNameLaber;
    public Image background;

    public Color selectedColor;
    public Sprite selectedSprite;

    public Color normalColor;
    public Sprite normalSprite;

    private Button myButton;
    private bool isSelected;
    
    public string LevelName { get; private set; }

    public bool IsSelected
    {
        get { return isSelected; }
        set
        {
            isSelected = value;
            if (value)
            {
                if (SelectedButton != null)
                {
                    SelectedButton.IsSelected = false;
                }
                SelectedButton = this;
            }
            Refresh();
        }
    }

    private void Start()
    {
        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        myButton.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        IsSelected = true;
    }

    public void SetLevelName(string levelName, bool selected = false)
    {
        LevelName = levelName;
        IsSelected = selected;
        
        Refresh();
    }

    private void Refresh()
    {
        levelNameLaber.text = LevelName.ToUpper();

        background.color = IsSelected ? selectedColor : normalColor;
        background.sprite = IsSelected ? selectedSprite : normalSprite;
    }
    
}
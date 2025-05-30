using UnityEngine;

public class ButtonLogic : MonoBehaviour
{
    public bool disabled = false;

    [SerializeField] private Transform actualButton;
    [SerializeField] private Transform content;
    [SerializeField] private SpriteRenderer bgSprite;

    private SpriteRenderer contentSprite;
    private TextMesh textMesh;

    [SerializeField] private Color normalColor;
    [SerializeField] private Color hoverColor;
    [SerializeField] private Color pressedColor;
    [SerializeField] private Color disabledColor;
    [SerializeField] private Color disabledTextColor;

    [SerializeField] private float pressedScale;

    [SerializeField] private float hoverTextSize;

    private bool mouseOver = false;
    private bool mouseDown = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        contentSprite = content.GetComponent<SpriteRenderer>();
        textMesh = content.GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        if (disabled)
        {
            bgSprite.color = disabledColor;
            if (textMesh)
            {
                textMesh.color = disabledTextColor;
            }
            if (contentSprite)
            {
                contentSprite.color = disabledTextColor;
            }
            actualButton.localScale = Vector3.one;
            content.localScale = Vector3.one;
            return;
        }

        if (mouseOver)
        {
            bgSprite.color = hoverColor;
            content.localScale = hoverTextSize * Vector3.one;
        }
        else
        {
            bgSprite.color = normalColor;
            content.localScale = Vector3.one;
        }

        if (mouseDown)
        {
            bgSprite.color = pressedColor;
            actualButton.localScale = pressedScale * Vector3.one;
        }
        else
        {
            actualButton.localScale = Vector3.one;
        }
    }

    void OnMouseEnter()
    {
        mouseOver = true;
    }

    void OnMouseExit()
    {
        mouseOver = false;
    }

    void OnMouseDown()
    {
        mouseDown = true;
    }

    void OnMouseUp()
    {
        mouseDown = false;
    }
}

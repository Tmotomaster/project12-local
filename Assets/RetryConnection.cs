using UnityEngine;

public class RetryConnection : MonoBehaviour
{
    [SerializeField] private GameMaster gameMaster;

    private bool mouseOver = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseEnter()
    {
        mouseOver = true;
    }

    void OnMouseExit()
    {
        mouseOver = false;
    }
    
    void OnMouseUp()
    {
        if (mouseOver)
        {
            gameMaster.retries = 0;
        }
    }
}

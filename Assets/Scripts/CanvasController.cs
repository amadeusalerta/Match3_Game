using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private GameObject startPanel, finishPanel;

    [Header("Panels")]
    [SerializeField] private GameObject bluePanel;
    [SerializeField] private GameObject yellowPanel;
    [SerializeField] private GameObject redPanel;
    [SerializeField] private GameObject greenPanel;

    [Header("Values & Texts")]
    [SerializeField] private int blueValue;
    [SerializeField] private int yellowValue;
    [SerializeField] private int redValue;
    [SerializeField] private int greenValue;
    public TMP_Text blueText;
    public TMP_Text yellowText;
    public TMP_Text redText;
    public TMP_Text greenText;

    [Header("Level Size")]
    [SerializeField] private int width;
    [SerializeField] private int height;

    public GridManager grid { get; private set; }

    private void Awake()
    {
        grid = FindObjectOfType<GridManager>();
        if (grid != null)
        {
            grid.width = width;
            grid.height = height;
        }
    }

    private void Start()
    {
        startPanel.SetActive(true);
        finishPanel.SetActive(false);

        InitializePanel(bluePanel, blueValue, blueText);
        InitializePanel(yellowPanel, yellowValue, yellowText);
        InitializePanel(redPanel, redValue, redText);
        InitializePanel(greenPanel, greenValue, greenText);
    }

    private void InitializePanel(GameObject panel, int value, TMP_Text text)
    {
        bool isActive = value > 0;
        panel.SetActive(isActive);
        if (isActive)
        {
            text.text = value.ToString();
        }
    }

    public void StartGame()
    {
        startPanel.SetActive(false);
        grid?.Initialize();
    }

    public void UpdateTask(int taskID)
    {
        switch (taskID)
        {
            case 0: DecreaseValue(ref blueValue, blueText); break;
            case 1: DecreaseValue(ref greenValue, greenText); break;
            case 2: DecreaseValue(ref redValue, redText); break;
            case 3: DecreaseValue(ref yellowValue, yellowText); break;
        }

        if (blueValue == 0 && greenValue == 0 && redValue == 0 && yellowValue == 0)
        {
            finishPanel.SetActive(true);
            if (grid != null)
            {
                grid.gameStatus = GameStatus.over;
            }
        }
    }

    private void DecreaseValue(ref int value, TMP_Text text)
    {
        if (value > 0)
        {
            value--;
            text.text = value.ToString();
        }
    }

    public void SceneLoader(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
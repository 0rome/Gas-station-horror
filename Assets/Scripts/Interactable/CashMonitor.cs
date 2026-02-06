using UnityEngine;
using TMPro;
using Unity.AppUI.UI;
using System.Collections;
using System.Linq;

public class CashMonitor : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform cameraPoint;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI productNamesText;
    [SerializeField] private TextMeshProUGUI productPricesText;
    [SerializeField] private TextMeshProUGUI incomingAmountText;
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private TMP_InputField changeInputField;

    [Header("Audio")]
    [SerializeField] private SoundPlayer soundPlayer;

    private NpcController currentNpcController;
    private int[] prices;
    private int incomingMoney;

    private Collider monitorCollider;
    private MiniMenu miniMenu;
    private bool isActive = false;
    private bool haveClient = false;
    private Camera mainCamera;
    void Start()
    {
        miniMenu = FindFirstObjectByType<MiniMenu>();
        monitorCollider = GetComponent<Collider>();
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && isActive)
        {
            ExitMonitor();
        }
    }

    public void DoSomething()
    {
        if (isActive) return;
        isActive = true;
        miniMenu.blockMenu = true;
        playerController.DeactivePlayer();
        playerController.LockCamera();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        mainCamera.transform.SetParent(cameraPoint);
        mainCamera.transform.localPosition = Vector3.zero;
        mainCamera.transform.localRotation = Quaternion.identity;
        monitorCollider.enabled = false;
    }

    private void ExitMonitor()
    {
        isActive = false;
        playerController.UnlockCamera();
        playerController.ActivePlayer();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        monitorCollider.enabled = true;
        miniMenu.blockMenu = false;
    }



    public void SetMonitorData(NpcController npcController)
    {
        currentNpcController = npcController;

        if (currentNpcController == null || currentNpcController.allNeedItems == null)
            return;

        haveClient = true;

        var items = currentNpcController.allNeedItems;

        productNamesText.text = "Product names: ";
        productPricesText.text = "Prices: ";
        incomingAmountText.text = "Incoming money: ";

        prices = new int[items.Length];


        for (int i = 0; i < items.Length; i++)
        {
            productNamesText.text += items[i].name + ", ";

            prices[i] = Random.Range(10, 100) * 10;

            productPricesText.text += prices[i] + ", ";
        }

        int bonus = Random.Range(10, 100) * 10;
        incomingMoney = prices.Sum() + bonus;

        incomingAmountText.text += incomingMoney.ToString();
    }
    public void SetChange()
    {
        if (!haveClient) { warningText.text = "No data"; soundPlayer.PlaySound(1); return; }

        warningText.text = "";

        if (!int.TryParse(changeInputField.text, out int enteredChange))
        {
            soundPlayer.PlaySound(1);
            warningText.text = "Not a number!";
            warningText.color = Color.red;
            return;
        }

        if (currentNpcController == null)
        {
            soundPlayer.PlaySound(1);
            warningText.text = "No data";
            return;
        }

        if (enteredChange == incomingMoney - prices.Sum())
        {
            soundPlayer.PlaySound(0);
            currentNpcController.GetChange();
            currentNpcController = null;
            ResetData();
        }
        else
        {
            soundPlayer.PlaySound(0);
            CallSystem.instance.AddValue(50);
            currentNpcController.GetChange();
            currentNpcController = null;
            ResetData();
        }
    }
    public void ResetData()
    {
        productNamesText.text = "Product names: ";
        productPricesText.text = "Prices: ";
        incomingAmountText.text = "Incoming money: ";
        warningText.text = "";
        changeInputField.text = "";
        haveClient = false;
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarSelection1 : MonoBehaviour
{
    [Header("Car Change Button")]
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;

    private int currentCar;

    [Header("Play/Buy/Price")]
    [SerializeField] private Button Play;
    [SerializeField] private Button Buy;
    [SerializeField] private TMP_Text priceText;
    [Header("CarName")]
    [SerializeField] private TMP_Text CarSpeedText;
    [SerializeField] private TMP_Text CarTorqueText;

    [Header("Car Price")]
    [SerializeField] private int[] CarPrice;
    [SerializeField] private int[] CarSpeed;
    [SerializeField] private int[] CarTorque;
    [SerializeField] private Slider CarSpeedSlider;
    [SerializeField] private Slider CarTorqueSlider;
    private void Start()
    {
        currentCar = SaveManager.instance.currentCar;
        SelectCar(currentCar);

        // SelectCar(currentCar);
    }

    private void SelectCar(int _index)
    {
        previousButton.interactable = (_index != 0);
        nextButton.interactable = (_index != transform.childCount - 1);
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(i == _index);

        UpdateUi();
        CarSpeedSlider.value = CarSpeed[_index];
        CarTorqueSlider.value = CarTorque[_index];


        Play.interactable = (SaveManager.instance.currentCar != _index);
    }


    private void UpdateUi()
    {





        if (SaveManager.instance.CarUnlock[currentCar])
        {
            Play.gameObject.SetActive(true);
            Buy.gameObject.SetActive(false);
            priceText.enabled = false;
        }
        else
        {
            Play.gameObject.SetActive(false);
            Buy.gameObject.SetActive(true);
            priceText.text = CarPrice[currentCar] + "";
            priceText.enabled = true;


        }
        if (SaveManager.instance.CarUnlock[currentCar])
        {
            Play.gameObject.SetActive(true);
            Buy.gameObject.SetActive(false);
            CarSpeedText.enabled = true;
            CarSpeedText.text = CarSpeed[currentCar] + "";
        }
        else
        {
            Play.gameObject.SetActive(false);
            Buy.gameObject.SetActive(true);
            CarSpeedText.text = CarSpeed[currentCar] + "";
            CarSpeedText.enabled = true;


        }
        if (SaveManager.instance.CarUnlock[currentCar])
        {
            Play.gameObject.SetActive(true);
            Buy.gameObject.SetActive(false);
            CarTorqueText.enabled = true;
            CarTorqueText.text = CarTorque[currentCar] + "";
        }
        else
        {
            Play.gameObject.SetActive(false);
            Buy.gameObject.SetActive(true);
            CarTorqueText.text = CarTorque[currentCar] + "";
            CarTorqueText.enabled = true;


        }


    }

    private void Update()
    {
        if (Buy.gameObject.activeInHierarchy)
        {
            Buy.interactable = (SaveManager.instance.MoneyCoin >= CarPrice[currentCar]);
            // Buy.interactable = (SaveManager.instance.MoneyCoin >= CarName[currentCar]);
        }

    }

    public void ChangeCar(int _change)
    {
        currentCar += _change;
        SelectCar(currentCar);

        if (currentCar > transform.childCount - 1)
            currentCar = 0;
        else if (currentCar < 0) { currentCar = transform.childCount - 1; }




    }

    public void CarBuy()
    {
        SaveManager.instance.MoneyCoin -= CarPrice[currentCar];
        SaveManager.instance.CarUnlock[currentCar] = true;
        SaveManager.instance.Save();
        UpdateUi();
    }

    public void selected()
    {
        SaveManager.instance.currentCar = currentCar;
        SaveManager.instance.Save();
        //  int index = SaveManager.instance.currentCar;

        SelectCar(SaveManager.instance.currentCar);
    }

}
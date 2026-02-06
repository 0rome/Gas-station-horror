using UnityEngine;

public class Item : MonoBehaviour
{
    private string itemName = "Item";
    private int itemPrice;

    private void Start()
    {
        itemName = RemoveNonLetters(name);
        itemPrice = itemPrice = Random.Range(10,100) * 5; 
    }

    private string RemoveNonLetters(string text)
    {
        // Удаляет всё, кроме букв (включая русские) и пробелов
        return System.Text.RegularExpressions.Regex.Replace(text, @"[^a-zA-Zа-яА-ЯёЁ\s]", "");
    }

    public string GetName() { return itemName; }
    public int GetPrice() { return itemPrice; }
}

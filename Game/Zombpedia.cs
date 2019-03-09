using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombpedia : MonoBehaviour
{
    // For displaying and hiding the 'Zombpedia' menu

    public GameObject zombpediaMenu;


    public void ShowPediaMenu()
    {
        zombpediaMenu.SetActive(true);
    }

    public void HidePediaMenu()
    {
        zombpediaMenu.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifiedDR : MonoBehaviour
{

    public static float walkerDR;
    public static float slugDR;
    public static float juggernautDR;
    public static float MutantDR;
    public static float insectDR;
    public static float reptileDR;
    public static float arachnidDR;

    public GameObject[] enemyList;

    private static bool defaultsRegistered = false;

    private ZombieAI component;

    private void Awake()
    {
        if (!gameObject.tag.Equals("Zevolution"))
        {
            component = GetComponent<ZombieAI>();
            
        }

        if (!defaultsRegistered)
        {
            DefaultDR();
            defaultsRegistered = true;
        }
        
    }


    public void UpdateDR()
    {
        if (GameObject.FindWithTag("Zevolution").GetComponent<Zevolution>().IsActive())
        {
            tag = gameObject.tag;
            switch (tag)
            {
                case "Walker":
                    component.DR = walkerDR;
                    break;
                case "Slug":
                    component.DR = slugDR;
                    break;
                case "Juggernaut":
                    component.DR = juggernautDR;
                    break;
                case "Mutant":
                    component.DR = MutantDR;
                    break;
                case "Insect":
                    component.DR = insectDR;
                    break;
                case "Reptile":
                    component.DR = reptileDR;
                    break;
                case "Arachnid":
                    component.DR = arachnidDR;
                    break;
                default:
                    break;
                
            }
        } 
        
    }
    
    public void AddDR(string tag, float amount)
    {
        if (GameObject.FindWithTag("Zevolution").GetComponent<Zevolution>().IsActive())
        {
            switch (tag)
            {
                case "Walker":
                    walkerDR += amount;
                    break;
                case "Slug":
                    slugDR += amount;
                    break;
                case "Juggernaut":
                    juggernautDR += amount;
                    break;
                case "Mutant":
                    MutantDR += amount;
                    break;
                case "Poisoned":
                case "poison":
                    insectDR += amount;
                    break;
                case "Reptile":
                    reptileDR += amount;
                    break;
                case "Baby Arachnid":
                case "Arachnid":
                    arachnidDR += amount;
                    break;
                default:
                    break;
                
            }
            Debug.Log("Increased DR for: " + tag);
        } 
        
    }

    public void DefaultDR()
    {
        
        walkerDR = enemyList[0].GetComponent<ZombieAI>().DR;
        slugDR = enemyList[1].GetComponent<ZombieAI>().DR;
        juggernautDR = enemyList[2].GetComponent<ZombieAI>().DR;
        MutantDR = enemyList[3].GetComponent<ZombieAI>().DR;
        insectDR = enemyList[4].GetComponent<ZombieAI>().DR;
        reptileDR = enemyList[5].GetComponent<ZombieAI>().DR;
        arachnidDR = enemyList[6].GetComponent<ZombieAI>().DR;
                
            
        
        
    }
    
    public float GetDR(string tag)
    {
        if (GameObject.FindWithTag("Zevolution").GetComponent<Zevolution>().IsActive())
        {
            switch (tag)
            {
                case "Walker":
                    return walkerDR;
                    break;
                case "Slug":
                    return slugDR;
                    break;
                case "Juggernaut":
                    return juggernautDR;
                    break;
                case "Mutant":
                    return MutantDR;
                    break;
                case "Insect":
                    return insectDR;
                    break;
                case "Reptile":
                    return reptileDR;
                    break;
                case "Arachnid":
                    return arachnidDR;
                    break;
                default:
                    return 0;
                    break;
                
            }
        }

        return 0;

    }
}

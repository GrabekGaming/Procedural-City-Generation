using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Rules")] // ability to create the asset in the folder menu
public class Rule : ScriptableObject
{
    /****************************************/
    ///////////////Variables ////////////////
    public string let;

    [SerializeField]
    private string[] results = null;
    [SerializeField]
    private bool randomResults = false;//
    /****************************************/

    public string GetResult()
    {
        //random generation of given rules
        if(randomResults)
        {
            int randomIndex = UnityEngine.Random.Range(0, results.Length);
            return results[randomIndex];
        }
        return results[0];
    }
}

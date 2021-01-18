using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ProceduralGenerator : MonoBehaviour
{
    /****************************************/
    ///////////////Variables ////////////////
    public Rule[] rules;
    public string axiom; //axiom sentence

    [Range(0, 10)]
    public int iterationLimit = 1; // limit of the iteration generated

    public bool randomIgnoreRule = true;
    [Range(0, 1)]
    public float chanceToIgnore = 0.3f;
    /****************************************/

    public void Start()
    {
        Debug.Log(sequenceGenerator()); // just for checking the sentence // delete before handling the assessment!
    }
    public string sequenceGenerator(string sentence = null)
    {
        if (sentence == null)
        {
            sentence = axiom;
        }
        return recursionGenerator(sentence);
    }

    private string recursionGenerator(string sentence, int index = 0)
    {
        if (index >= iterationLimit)
        {
            return sentence;
        }
        StringBuilder sB = new StringBuilder();

        foreach (var s in sentence) // check if char is good as well // for now give var
        {
            sB.Append(s); // append a character to the input field
            rulesRecursion(sB, s, index);
        }
        return sB.ToString();
    }

    private void rulesRecursion(StringBuilder sB, char s, int index)
    {
        foreach (var rule in rules)
        {
            if (rule.let == s.ToString())
            {
                if (randomIgnoreRule && index > 1)
                {
                    if (UnityEngine.Random.value < chanceToIgnore)
                    {
                        return;
                    }
                }
                sB.Append(recursionGenerator(rule.GetResult(), index + 1));
            }
        }
    }
}

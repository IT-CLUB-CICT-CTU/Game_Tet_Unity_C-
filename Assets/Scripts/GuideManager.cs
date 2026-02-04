using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideManager : MonoBehaviour
{
    public int stepIndex = 0;
    public GameObject[] guideSteps;

    void Start()
    {
        // Load step index from PlayerPrefs
        stepIndex = PlayerPrefs.GetInt("GuideStepIndex", 0);
        if (stepIndex > 0 && stepIndex < guideSteps.Length)
        {
            stepIndex = 1;
        }
            ShowStep(stepIndex);
    }

    public void ShowStep(int index)
    {
        for (int i = 0; i < guideSteps.Length; i++)
        {
            guideSteps[i].SetActive(i == index);
        }
    }

    public void NextStep()
    {
        if (stepIndex > guideSteps.Length)
            return;

        stepIndex++;
        // Save step index to PlayerPrefs
        PlayerPrefs.SetInt("GuideStepIndex", stepIndex);

        if (stepIndex < guideSteps.Length)
        {
            ShowStep(stepIndex);
        }
        else
        {
            foreach (GameObject step in guideSteps)
            {
                step.SetActive(false);
            }
        }
    }
}

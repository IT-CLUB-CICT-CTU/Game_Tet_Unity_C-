using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonChange : MonoBehaviour
{
    public Animator[] animators;
    public GameObject[] button;
    public OpenButton openButton;

    void OnEnable()
    {
        StartCoroutine(DisableNextFrame());
    }

    IEnumerator DisableNextFrame()
    {
        yield return null; // đợi 1 frame
        DisableAllAnimations();
    }

    void OnDisable()
    {
        DisableAllAnimations();
    }

    public void DisableAllAnimations()
    {
        foreach (Animator animator in animators)
        {
            animator.SetBool("IsActive", false);
        }
    }

    public void ChangeAnimation(int index)
    {
        DisableAllAnimations();
        if (index >= 0 && index < animators.Length)
        {
            animators[index].SetBool("IsActive", true);
        }
    }

    public void ResetButtons()
    {
        StartCoroutine(WaitClosed());
    }

    IEnumerator WaitClosed()
    {
        foreach (GameObject btn in button)
        {
            btn.SetActive(false);
        }
        DisableAllAnimations();
        yield return new WaitForSeconds(0.1f);
        if (openButton != null)
        {
            openButton.Closed();
        }
    }
}

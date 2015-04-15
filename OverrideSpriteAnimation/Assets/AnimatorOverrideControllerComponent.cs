using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class AnimatorOverrideControllerComponent : MonoBehaviour
{
    [SerializeField]
    private AnimatorOverrideController overrideController;

    void Awake()
    {
        if (overrideController != null)
            SetOverrideController();
    }

    void OnValidate()
    {
        if (overrideController != null && Application.isPlaying)
            SetOverrideController();
    }


    void SetOverrideController()
    {
        GetComponent<Animator>().runtimeAnimatorController = overrideController;
    }
}

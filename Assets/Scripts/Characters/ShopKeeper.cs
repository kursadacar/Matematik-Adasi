using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    [SerializeField] Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Shopping", true);
    }
}

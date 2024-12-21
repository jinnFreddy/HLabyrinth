using TMPro;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private Vector3 _interactionBoxSize;
    [SerializeField] private LayerMask _interactableMask;

    private readonly Collider[] _colliders = new Collider[3];
    [SerializeField] private int _numFound;

    //[SerializeField] GameObject _avisoBackground;
    //public GameObject aviso;
    //public TMP_Text aviso_text;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        _numFound = Physics.OverlapBoxNonAlloc(_interactionPoint.position, _interactionBoxSize / 2, _colliders, Quaternion.identity, _interactableMask);
        
        //aviso.SetActive(false);
        //_avisoBackground.SetActive(false);

        if (_numFound > 0)
        {
            
            //aviso.SetActive(true);
            //aviso_text.text = _colliders[0].name;
            var interactable = _colliders[0].GetComponent<IInteractable>();
            //_avisoBackground.SetActive(true);
            if (interactable != null && Input.GetKeyDown(KeyCode.E))
            {
                interactable.Interact(this);
                //aviso.SetActive(false);
                
                //anim.SetBool("IsInteracting", true);
                //_avisoBackground.SetActive(false);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_interactionPoint.position, _interactionBoxSize);
    }
}

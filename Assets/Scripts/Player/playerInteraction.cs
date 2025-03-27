
using UnityEngine;


public class playerInteraction : MonoBehaviour
{
    [Header("References")]
    public GameObject primary_weapon;
    private PrimaryWeapon wb;
    private Take_Drop_Stuff tds;
    private PlayerInventory pi;
    private GameObject[] invArray;
    private Transform p_camera;
    private LayerMask collidingLayerMask;


    [Header("Keybinds")]
    public KeyCode dropKey = KeyCode.G;
    public KeyCode interactKey = KeyCode.E;
    private float scrollDelay = 10f;

    // Start is called before the first frame update
    void Start()
    {
        collidingLayerMask = LayerMask.GetMask("Default");

        pi = gameObject.GetComponent<PlayerInventory>();

        invArray = pi.Get_playerInvArray();
        primary_weapon = invArray[0].transform.GetChild(0).gameObject;
        wb = primary_weapon.GetComponent<PrimaryWeapon>();


        tds = gameObject.GetComponent<Take_Drop_Stuff>();

        p_camera = GameObject.Find("PlayerCamera").transform;

    }


    // Update is called once per frame
    void Update()
    {

        if (Input.mouseScrollDelta.y > 0)
        {
            scrollDelay += Time.deltaTime;
            pi.EquipPrecedentObj();
            Debug.Log("Scroll Up");
        }
            
        if (Input.mouseScrollDelta.y < 0)
        {
            scrollDelay += Time.deltaTime;
            pi.EquipNextObj();
            Debug.Log("Scroll Down");
        }
            


        if (Input.GetKeyDown(dropKey))
            if (invArray[pi.Get_currentSelected()].transform.childCount != 0)
                tds.DropCurrentObj();

        if (Input.GetKeyDown(interactKey))
        {
            if (invArray[pi.Get_currentSelected()].transform.childCount == 0)
                tds.TakeObj();

            RaycastHit hit;
            if (Physics.Raycast(p_camera.position, p_camera.forward, out hit, 5f, ~collidingLayerMask))
            {
                if (hit.transform.tag == "Interactable")
                {
                    Debug.Log(hit.transform.tag);
                    hit.transform.parent.GetComponent<Door>().hasPlayerInteracted = true;
                    hit.transform.parent.GetComponent<Door>().PlayDoorSound();


                }
                    
            }

        }
            

    }
}

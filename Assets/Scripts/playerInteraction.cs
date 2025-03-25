
using UnityEngine;


public class playerInteraction : MonoBehaviour
{
    [Header("References")]
    public GameObject weapon;
    private PrimaryWeapon wb;
    private Take_Drop_Stuff tds;
    private PlayerInventory pi;

    [Header("Keybinds")]
    public KeyCode reloadKey = KeyCode.R;
    public KeyCode dropKey = KeyCode.G;
    public KeyCode interactKey = KeyCode.E;

    private float scrollDelay = 10f;

    // Start is called before the first frame update
    void Start()
    {
        wb = weapon.GetComponent<PrimaryWeapon>();
        tds = gameObject.GetComponent<Take_Drop_Stuff>();
        pi = gameObject.GetComponent<PlayerInventory>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButton(0) && wb.hasAmmo)
            wb.Shoot();
        if (Input.GetKeyDown(reloadKey) && wb.isNotReloading && wb.getAmmo() < wb.maxAmmo)
            StartCoroutine(wb.WeaponReload());


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
            tds.DropCurrentObj();
        if (Input.GetKeyDown(interactKey))
            tds.TakeObj();

    }
}

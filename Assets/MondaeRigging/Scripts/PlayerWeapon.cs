using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using JetBrains.Annotations;

public class PlayerWeapon : MonoBehaviour
{
    public GameObject player;
    public Transform[] spawnPoint;
    public float fireSpeed = 20;
    public GameObject bullet;
    public Rotator rotatorScript;
    public GameObject explosionObject;

    public int maxAmmo;
    public int ammoLeft;
    public int durability;

    public GameObject reloadingScreen;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI durabilityText;

    public AudioSource audioSource;
    public AudioClip weaponFire;
    public AudioClip weaponReload;
    public AudioClip weaponBreak;

    public bool reloadingWeapon = false;

    // Start is called before the first frame update
    void Start()
    {
        rotatorScript = GetComponent<Rotator>();
        reloadingScreen.SetActive(false);
        ammoLeft = maxAmmo;
        ammoText.text = ammoLeft.ToString();
        XRGrabNetworkInteractable grabbable = GetComponent<XRGrabNetworkInteractable>();
        grabbable.activated.AddListener(FireBullet);
    }

    // Update is called once per frame
    void Update()
    {
        ammoText.text = ammoLeft.ToString();
        durabilityText.text = durability.ToString();  
        
        if(durability <= 0)
        {
            audioSource.PlayOneShot(weaponBreak);
            GetComponent<XRGrabNetworkInteractable>().enabled = false;
            StartCoroutine(DestroyWeapon());
        }
    }

    public void FireBullet(ActivateEventArgs arg)
    {
        if (ammoLeft >= 1)
        {
            foreach (Transform t in spawnPoint)
            {
                audioSource.PlayOneShot(weaponFire);
                GameObject spawnedBullet = PhotonNetwork.Instantiate(bullet.name, t.position, Quaternion.identity);
                spawnedBullet.GetComponent<Bullet>().bulletModifier = player.GetComponentInParent<PlayerHealth>().bulletModifier;
                spawnedBullet.gameObject.GetComponent<Bullet>().bulletOwner = player.gameObject;
                spawnedBullet.gameObject.GetComponent<Bullet>().playerBullet = true;
                spawnedBullet.GetComponent<Rigidbody>().velocity = t.right * fireSpeed;
                ammoLeft -= 1;
            }
        }

        if (ammoLeft <= 0 && reloadingWeapon == false)
        {
            reloadingWeapon = true;
            StartCoroutine(ReloadWeapon());
        }
    }

    IEnumerator ReloadWeapon()
    {
        yield return new WaitForSeconds(0);
        reloadingScreen.SetActive(true);
        audioSource.PlayOneShot(weaponReload);
        durability--;
        yield return new WaitForSeconds(2);
        ammoLeft = maxAmmo;
        reloadingScreen.SetActive(false);
        reloadingWeapon = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            player = other.transform.parent.gameObject;
            var newMaxAmmo = player.GetComponentInParent<PlayerHealth>().maxAmmo + maxAmmo;
            maxAmmo = newMaxAmmo;
            GetComponent<Rigidbody>().isKinematic = false;
            rotatorScript.enabled = false;
        }
    }

    IEnumerator DestroyWeapon()
    {
        yield return new WaitForSeconds(0.5f);
        explosionObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        PhotonNetwork.Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Photon.Pun;

public class WeaponCrate : MonoBehaviour
{
    [SerializeField]
    private VisualEffect _visualEffect;

    public string[] powerups;
    public string[] weapons;
    public Transform spawn1;
    public Transform spawn2;
    public Transform spawn3;

    private Animator _animator;
    private BoxCollider _collider;

    public bool cacheActive;
    public GameObject cacheBase;


    void Start()
    {
        cacheActive = true;
        _animator = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand") || other.CompareTag("Player") && cacheActive == true)
        {
            _collider.enabled = false;
            _animator.SetBool("Open", true);
            OnLidLifted();
            cacheActive = false;
            StartCoroutine(WeaponCache());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _animator.SetBool("Open", false);
    }

    private void OnLidLifted()
    {
        _visualEffect.SendEvent("OnPlay");
    }

    IEnumerator WeaponCache()
    {
        yield return new WaitForSeconds(1);
        PhotonNetwork.Instantiate(weapons[Random.Range(0, weapons.Length)], spawn1.position, spawn1.rotation);
        PhotonNetwork.Instantiate(weapons[Random.Range(0, weapons.Length)], spawn3.position, spawn3.rotation);
        PhotonNetwork.Instantiate(powerups[Random.Range(0, powerups.Length)], spawn2.position, spawn2.rotation);
        cacheBase.SetActive(false);
        StartCoroutine(CacheRespawn());
    }

    IEnumerator CacheRespawn()
    {
        yield return new WaitForSeconds(30);
        cacheBase.SetActive(true);
        _animator.SetBool("Open", false);
        cacheActive = true;
    }
}

using LootLocker.Requests;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class StingerBulletNet : MonoBehaviourPunCallbacks
{
    [Header("Bullet Behavior ---------------------------------------------------")]
    public GameObject smallBulletPrefab;
    public GameObject explosionPrefab;
    public Transform spawnTransform;

    private Rigidbody rb;
    private bool hasExploded = false;

    public GameObject bulletOwner;
    public PlayerHealth playerHealth;
    public bool playerBullet = false;
    public int bulletModifier;
    public MeshCollider colliderBullet;

    [Header("Bullet Effects ---------------------------------------------------")]
    public float energyPulseRadius = 5.0f;
    public int numSmallBullets = 100;
    public float smallBulletTargetRadius = 10.0f;
    public float smallBulletLifetime = 5.0f;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(ExplodeBullets());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Bullet"))
        {
            if (!hasExploded)
            {
                hasExploded = true;
                Explode();
            }
        }
    }

    [System.Obsolete]
    private void OnTriggerEnter(Collider other)
    {
        if (playerBullet == true)
        {
            playerHealth = bulletOwner.GetComponent<PlayerHealth>();
        }
        else
        {
            playerHealth = null;
        }

        if (other.CompareTag("Enemy") || other.CompareTag("BossEnemy"))
        {
            FollowAI enemyDamageCrit = other.GetComponent<FollowAI>();
            if (enemyDamageCrit.Health <= (20 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
            {
                playerHealth.EnemyKilled();
                enemyDamageCrit.TakeDamage((20 * bulletModifier));
            }
            else if (enemyDamageCrit.Health > (20 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
            {
                enemyDamageCrit.TakeDamage((20 * bulletModifier));
            }
            Explode();
        }
        else
        {
            FollowAI enemyDamage = other.GetComponent<FollowAI>();
            if (enemyDamage.Health <= (10 * bulletModifier) && enemyDamage.alive == true && playerHealth != null)
            {
                playerHealth.EnemyKilled();
                enemyDamage.TakeDamage((10 * bulletModifier));
            }
            else if (enemyDamage.Health > (10 * bulletModifier) && enemyDamage.alive == true && playerHealth != null)
            {
                enemyDamage.TakeDamage((10 * bulletModifier));
            }
            Explode();
        }

        if (other.CompareTag("Security"))
        {
            float criticalChance = 30f;

            //cal it at random probability
            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                DroneHealth enemyDamageCrit = other.GetComponent<DroneHealth>();
                enemyDamageCrit.TakeDamage((30 * bulletModifier));
                Explode();
            }

            else
            {
                DroneHealth enemyDamage = other.GetComponent<DroneHealth>();
                enemyDamage.TakeDamage((20 * bulletModifier));
                Explode();
            }
        }

        if (other.CompareTag("Player"))
        {
            float criticalChance = 10f;

            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                PlayerHealth playerDamageCrit = other.GetComponent<PlayerHealth>();
                if (playerDamageCrit.Health <= (10 * bulletModifier) && playerDamageCrit.alive == true && playerHealth != null)
                {
                    playerHealth.PlayersKilled();
                }
                playerDamageCrit.TakeDamage((10 * bulletModifier));
                Explode();
            }

            else
            {
                PlayerHealth playerDamage = other.GetComponent<PlayerHealth>();
                if (playerDamage.Health <= (5 * bulletModifier) && playerDamage.alive == true && playerHealth != null)
                {
                    playerHealth.PlayersKilled();
                }
                playerDamage.TakeDamage((5 * bulletModifier));
                Explode();
            }
        }
    }

    IEnumerator ExplodeBullets()
    {
        yield return new WaitForSeconds(3);
        if (!hasExploded)
        {
            hasExploded = true;
            Explode();
        }
    }

    private void Explode()
    {
        // Create an energy pulse that pushes enemies away
        Collider[] colliders = Physics.OverlapSphere(transform.position, energyPulseRadius);
        foreach (Collider collider in colliders)
        {
            if (!collider.CompareTag("Player"))
            {
                Rigidbody targetRb = collider.GetComponent<Rigidbody>();
                if (targetRb != null)
                {
                    Vector3 direction = targetRb.transform.position - transform.position;
                    direction.Normalize();
                    targetRb.AddForce(direction * 30.0f, ForceMode.Impulse);
                }
            }
        }

        // Create smaller bullets and target nearby enemies
        for (int i = 0; i < numSmallBullets; i++)
        {
            GameObject smallBullet = PhotonNetwork.InstantiateRoomObject(smallBulletPrefab.name, transform.position, Quaternion.identity, 0, null);
            smallBullet.transform.forward = Random.insideUnitSphere;
            smallBullet.gameObject.GetComponent<Bullet>().bulletOwner = bulletOwner.gameObject;
            smallBullet.gameObject.GetComponent<Bullet>().playerBullet = true;
            Collider[] targets = Physics.OverlapSphere(transform.position, smallBulletTargetRadius);
            if (targets.Length > 0)
            {
                Transform target = targets[Random.Range(0, targets.Length)].transform;
                smallBullet.GetComponent<SmallBullet>().SetTarget(target, smallBulletLifetime);
            }
        }

        // Create explosion effect and destroy bullet
        PhotonNetwork.InstantiateRoomObject(explosionPrefab.name, transform.position, Quaternion.identity, 0, null);
        PhotonNetwork.Destroy(gameObject);
    }
}

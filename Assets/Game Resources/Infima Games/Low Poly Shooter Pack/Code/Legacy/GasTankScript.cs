﻿//Copyright 2022, Infima Games. All Rights Reserved.

using UnityEngine;
using System.Collections;
using Photon.Pun;
using Unity.VisualScripting;

namespace InfimaGames.LowPolyShooterPack.Legacy
{
    public class GasTankScript : MonoBehaviourPunCallbacks
    {

        float randomRotationValue;
        float randomValue;

        bool routineStarted = false;

        //Used to check if the gas tank 
        //has been hit
        public bool isHit = false;

        [Header("Prefabs")]
        //Explosion prefab
        public GameObject explosionPrefab;

        //The destroyed gas tank prefab
        public GameObject destroyedGasTankPrefab;

        [Header("Customizable Options")]
        //Time before the gas tank explodes, 
        //after being hit
        public float explosionTimer;

        //How fast the gas tank rotates
        public float rotationSpeed;

        //The maximum rotation speed of the
        //gast tank
        public float maxRotationSpeed;

        //How fast the gast tank moves
        public float moveSpeed;

        //How fast the audio pitch should increase
        public float audioPitchIncrease = 0.5f;

        [Header("Explosion Options")]
        //How far the explosion will reach
        public float explosionRadius = 12.5f;

        //How powerful the explosion is
        public float explosionForce = 250f;

        [Header("Light")]
        public Light lightObject;

        [Header("Particle Systems")]
        public ParticleSystem flameParticles;

        public ParticleSystem smokeParticles;

        [Header("Audio")]
        public AudioSource flameSound;

        public AudioSource impactSound;

        public int Health = 50;

        //Used to check if the audio has played
        bool audioHasPlayed = false;

        private void Start()
        {
            photonView.RPC("RPC_Start", RpcTarget.AllBuffered);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("EnemyBullet"))
            {
                PhotonNetwork.Destroy(other.gameObject);
                photonView.RPC("RPC_TakeDamage", RpcTarget.AllBuffered);
            }
        }

        private void Update()
        {
            //If the gas tank is hit
            if (isHit == true)
            {
                //Start increasing the rotation speed over time
                randomRotationValue += 1.0f * Time.deltaTime;

                //If the random rotation is higher than the maximum rotation, 
                //set it to the max rotation value
                if (randomRotationValue > maxRotationSpeed)
                {
                    randomRotationValue = maxRotationSpeed;
                }

                //Add force to the gas tank 
                gameObject.GetComponent<Rigidbody>().AddRelativeForce
                    (Vector3.down * moveSpeed * 50 * Time.deltaTime);

                //Rotate the gas tank, based on the random rotation values
                transform.Rotate(randomRotationValue, 0, randomValue *
                                                         rotationSpeed * Time.deltaTime);

                //Play the flame particles
                flameParticles.Play();
                //Play the smoke particles
                smokeParticles.Play();

                //Increase the flame sound pitch over time
                flameSound.pitch += audioPitchIncrease * Time.deltaTime;

                //If the audio has not played, play it
                if (!audioHasPlayed)
                {
                    flameSound.Play();
                    //Audio has played
                    audioHasPlayed = true;
                }

                if (routineStarted == false)
                {
                    //Start the explode coroutine
                    StartCoroutine(Explode());
                    routineStarted = true;
                    //Set the light intensity to 3
                    lightObject.intensity = 3;
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            photonView.RPC("RPC_PlaySound", RpcTarget.AllBuffered);
        }

        private IEnumerator Explode()
        {
            //Wait for set amount of time
            yield return new WaitForSeconds(explosionTimer);

            //Spawn the destroyed gas tank prefab
            Instantiate(destroyedGasTankPrefab, transform.position,
                transform.rotation);

            //Explosion force
            Vector3 explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                //Add force to nearby rigidbodies
                if (rb != null)
                    rb.AddExplosionForce(explosionForce * 50, explosionPos, explosionRadius);

                //If the gas tank explosion hits other gas tanks with the tag "GasTank"
                if (hit.transform.tag == "GasTank")
                {

                    //Toggle the isHit bool on the gas tank object
                    hit.transform.gameObject.GetComponent<GasTankScript>().isHit = true;
                }

                //If the gas tank explosion hits any explosive barrel
                if (hit.transform.tag == "ExplosiveBarrel")
                {
                    //Toggle explode bool on explosive barrel object
                    hit.transform.gameObject.GetComponent<ExplosiveBarrelScript>().explode = true;
                }

                //If the explosion hit the tag "Target"
                //Toggle the isHit bool on the target object
                if (hit.gameObject.CompareTag("Enemy") || hit.gameObject.CompareTag("BossEnemy"))
                {
                    FollowAI enemyHealth = hit.gameObject.GetComponent<FollowAI>();
                    {
                        if (enemyHealth != null)
                            enemyHealth.TakeDamage(25);
                    }
                }

                if (hit.gameObject.CompareTag("Security"))
                {
                    DroneHealth droneEnemyHealth = hit.gameObject.GetComponent<DroneHealth>();
                    {
                        if (droneEnemyHealth != null)
                            droneEnemyHealth.TakeDamage(30);
                    }
                }

                if (hit.gameObject.CompareTag("Player"))
                {
                    PlayerHealth playerhealth = hit.gameObject.GetComponentInChildren<PlayerHealth>();
                    {
                        if (playerhealth != null)
                            playerhealth.TakeDamage(15);
                    }
                }
            }

            //Spawn the explosion prefab
            PhotonNetwork.Instantiate(explosionPrefab.name, transform.position,
                transform.rotation);

            //Destroy the current gas tank object
            PhotonNetwork.Destroy(gameObject);
        }

        [PunRPC]
        void RPC_TakeDamage()
        {
            Health -= 25;

            if (Health <= 0)
            {
                isHit = true;
            }
        }

        [PunRPC]
        void RPC_Start()
        {
            //Make sure the light is off at start
            lightObject.intensity = 0;
            //Get a random value for the rotation
            randomValue = Random.Range(-50, 50);
        }

        [PunRPC]
        void RPC_PlaySound()
        {
            impactSound.Play();
        }
    }
}
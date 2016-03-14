using UnityEngine;
using System.Collections;

public class Factory : MonoBehaviour {

    [Header("Particle")]
    public GameObject bloodParticle;
    public GameObject speedParticle;

    [Header("Barel")]
    public GameObject barelExplosion;
    public GameObject smokeParticle;
    public GameObject barelAcid;

    public GameObject bolt;
    public GameObject boltNova;

    public static Factory instance;

    void Awake()
    {
        if (instance ==null)
        {
            instance = this;
        }
        
        //TODO CREATE POOL HERE

    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour {

    //var de velocidade
	float speed = 20.0F;
    //velocidade de rotacao
    float rotationSpeed = 120.0F;
    //var para prefab da bala
    public GameObject bulletPrefab;
    //var para locar de spawn da bala
    public Transform bulletSpawn;

    void Update() {
        //movimento vertical multiplicado pela velocidade
        float translation = Input.GetAxis("Vertical") * speed;
        //movimento de rotacao multiplicado pela velocidade
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        //multiplicando a translation por delta time
        translation *= Time.deltaTime;
        //multiplicando a rotation por delta time
        rotation *= Time.deltaTime;
        //movimento vertical
        transform.Translate(0, 0, translation);
        //rotacionando na horizontal
        transform.Rotate(0, rotation, 0);
        
        //atira o projetil 
        if(Input.GetKeyDown("space"))
        {
            //instancia a bala
            GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            //adiciona força no rigidbody
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward*2000);
        }
    }
}

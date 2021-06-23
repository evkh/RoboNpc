using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Panda;
using Unity.Collections.LowLevel.Unsafe;

namespace Panda.Examples.Shooter
{
    public class AI : MonoBehaviour
    {
        //var do transform do player
        public Transform player; 
        //locar que vai spawnar as balas
        public Transform bulletSpawn; 
        //slider da barra de vida
        public Slider healthBar;  
        //prefab dos projeteis
        public GameObject bulletPrefab; 

        //var para pegar o navmeshagent
        NavMeshAgent agent;
        // var do local de destino
        public Vector3 destination; 
        // local onde vai mirar
        public Vector3 target;     
        //vida do objeto
        float health = 100.0f; 
        //velocidade de rotação
        float rotSpeed = 5.0f; 

        //distancia da visao
        float visibleRange = 80.0f;
        //distancia que o tiro vai percorrer
        float shotRange = 40.0f;

        void Start()
        {
            //colocando o component na var
            agent = this.GetComponent<NavMeshAgent>(); 
            agent.stoppingDistance = shotRange - 5; //for a little buffer
            //invoca repetidamente o metodo que recupera vida
            InvokeRepeating("UpdateHealth", 5, 0.5f);
        }

        void Update()
        {
            //barra de vida acompanha a camera 
            Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);
            //faz a health bar value ficar igual a vida do player
            healthBar.value = (int)health;
            //posicao da barra de vida
            healthBar.transform.position = healthBarPos + new Vector3(0, 60, 0); 
        }

        void UpdateHealth()
        {
            //recupera 1 de vida caso a vida fique menos de 100
            if (health < 100)
                health++;
        }

        void OnCollisionEnter(Collision col)
        {
            //perde 10 de vida se colidir com uma bala
            if (col.gameObject.tag == "bullet")
            {
                health -= 10;
            }
        }
        [Task]
        public void PickRandomDestination()
        {
            //var dest recebe uma vector 3 random em x e z que varia de -100 ate 100
            Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
            //faz o movimento ate o dest
            agent.SetDestination(dest);
            //da a task como sucesso
            Task.current.Succeed();
        }
        [Task]
        public void MoveToDestination()
        {
            //tempo que esta sendo executado a task
            if (Task.isInspected) Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);
            //se a distancia que resta for menor que a distancia dele parar, da a task como bem sucedida
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                Task.current.Succeed(); 
            }
        }
        [Task]
        public void PickDestination(int x, int z)
        {
            //seta o x e o y do destino
            Vector3 dest = new Vector3(x, 0, z);
            //seta o destino
            agent.SetDestination(dest);
            //da a task como sucesso
            Task.current.Succeed();

        }
        [Task]
        public void TargetPlayer()
        {
            //seta a target para o transform position do player
            target = player.transform.position;
            //da a task como sucesso
            Task.current.Succeed();
        }
        [Task]
        public bool Fire()
        {
            //define o instantiade da bala como um gameobject chamado bullet
            GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);

            //da um addforce no rigidbody
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000);

            return true;
        }
        [Task]
        public void LookAtTarget()
        {

            //define a direcao = target - transform.position
            Vector3 direction = target - this.transform.position;

            //faz a rotacao
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);


            if (Task.isInspected)
                Task.current.debugInfo = string.Format("angle={0}", Vector3.Angle(this.transform.forward, direction));

            //analisa o angulo e se for <5
            if (Vector3.Angle(this.transform.forward, direction) < 5.0f)
            {
                //da a task como sucesso
                Task.current.Succeed();
            }    
        }
        [Task]
        bool SeePlayer()
        {

            Vector3 distance = player.transform.position - this.transform.position;
            //cria o raycast
            RaycastHit hit;
            //define seewall false
            bool seeWall = false;
            //define os parametros do drawray
            Debug.DrawRay(this.transform.position, distance, Color.red);
            //se o raycast colidir
            if(Physics.Raycast(this.transform.position, distance, out hit))
            {
                //se colidir com algo com a tag wall
                if (hit.collider.gameObject.tag == "wall")
                {
                    //define seewall true
                    seeWall = true;
                }
            }

            if(Task.isInspected)
                Task.current.debugInfo = string.Format("wall={0}", seeWall);

            if (distance.magnitude < visibleRange && !seeWall)
                return true;
            else
                return false;
        }
        [Task]
        bool Turn(float angle)
        {
            var p = this.transform.position + Quaternion.AngleAxis(angle, Vector3.up) * this.transform.forward;
            target = p;
            return true;
        }

        [Task]                                                 
        public bool IsHealthLessThan(float health)             
        {
            //detecta se a vida esta menor que a propria vida pra dar seguimento a outro processo
            return this.health < health;                      
        }

        [Task]                
        //destruir o gameobject
        public bool Explode()                  
        {
            //destroi a barra de vida
            Destroy(healthBar.gameObject);     
            //destroi o proprio gameobject
            Destroy(this.gameObject);          
            //retorna verdadeiro
            return true;                       
        }
    }
}

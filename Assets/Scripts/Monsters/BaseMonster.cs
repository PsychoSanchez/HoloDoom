using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Monsters
{
    public class BaseMonster : MonoBehaviour
    {
        public AudioClip SpawnSound;
        public AudioClip TakeDamageSound;
        public AudioClip AttackSound;
        public AudioClip DeathSound;

        protected int _health;
        protected int _armor;
        protected Weapon _weapon;
        protected bool _dead = false;
        protected Animator _animator;
        protected bool _playerFound;
        protected Vector3 _playerPosition;

        AudioSource audioSource;

        public virtual void Spawn()
        {

        }

        public virtual bool IsAlive()
        {
            return !_dead;
        }

        public virtual void GetHit(int amount)
        {
            _health -= amount;
        }

        protected virtual void Die()
        {
            audioSource.PlayOneShot(DeathSound, 0.7F);
            _dead = true;
        }

        public virtual void Shoot()
        {

        }

        // Use this for initialization
        protected virtual void Start()
        {
            audioSource = GetComponent<AudioSource>();
            _health = 100;
            _armor = 0;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            this.GetComponent<SpriteRenderer>().transform.forward = Camera.main.transform.forward;
            // This method will be called all the time, while _health <= 0
            //if (_health <= 0)
            //{
            //    Die();
            //}
        }

        public virtual void FindPlayer(Vector3 playerPosition)
        {
            _playerFound = true;
            _playerPosition = playerPosition;

        }
    }
}

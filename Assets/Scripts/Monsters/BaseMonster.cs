using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Monsters
{
    public class BaseMonster : MonoBehaviour
    {
        protected int _health;
        protected int _armor;
        protected Weapon _weapon;
        protected bool _dead = false;
        protected Animator _animator;
        protected bool _playerFound;
        protected Vector3 _playerPosition;

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
          
            _dead = true;
        }

        public virtual void Shoot()
        {

        }

        // Use this for initialization
        protected virtual void Start()
        {
            _health = 100;
            _armor = 0;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            transform.forward = Camera.main.transform.forward;
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

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
            if (_health <= 0)
            {
                Die();
            }
        }
    }
}

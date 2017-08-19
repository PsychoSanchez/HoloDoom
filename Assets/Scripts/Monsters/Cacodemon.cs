﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Monsters
{
    public class Cacodemon : BaseMonster
    {
        public float ShootDelay = 0.15f;
        public float Range = 2f;
        public int Damage = 5;
        int shootableMask;
        bool canShoot = true;
        float lastShot;
        RaycastHit shotHit;

        protected override void Start()
        {
            _animator = this.GetComponent<Animator>();
            base.Start();
        }

        public override void GetHit(int amount)
        {
            base.GetHit(amount);
            if (_health <= 0)
            {
                Die();
                return;
            }
            this._animator.SetTrigger("Hit");
            this._animator.SetBool("Attack", true);
        }

        public override void FindPlayer(Vector3 playerPosition)
        {
            base.FindPlayer(playerPosition);
            this._animator.SetBool("PlayerFound", true);
        }

        protected override void Die()
        {
            base.Die();
            //foreach (Collider c in GetComponents<Collider>())
            //{
            //    c.enabled = false;
            //}
            this._animator.SetBool("Dead", true);
        }

        protected override void Update()
        {
            lastShot += Time.deltaTime;
            if (lastShot >= ShootDelay)
            {
                canShoot = true;
            }
            if (_playerFound)
            {
                Shoot();
            }
            
            base.Update();
        }

        public override void Shoot()
        {
            RaycastHit hit;
            if (canShoot && _playerFound)
            {
                this._animator.SetBool("Shoot", true);
                base.Shoot();
            } else
            {
                if (!Physics.Raycast(this.transform.position, _playerPosition, out hit, Range))
                {
                    if(hit.collider.tag == "Player")
                    {
                        hit.collider.GetComponent<PlayerHealth>()?.TakeDamage(Damage);
                    }
                }
            }
        }
    }
}

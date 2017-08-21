using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Monsters
{
    public class Cacodemon : BaseMonster
    {
        public float ShootDelay = 1f;
        public float Range = 20f;
        public int Damage = 5;
        int shootableMask;
        bool canShoot = true;
        float lastShot;
        RaycastHit shotHit;
        private bool isDead;

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
            // base.FindPlayer(playerPosition);
            // this._animator.SetBool("PlayerFound", true);
        }

        protected override void Die()
        {
            base.Die();
            //foreach (Collider c in GetComponents<Collider>())
            //{
            //    c.enabled = false;
            //}
            StartCoroutine(MyCoroutine());

            this._animator.SetBool("Dead", true);
        }
        IEnumerator MyCoroutine()
        {
            yield return new WaitForSeconds(0.3f);

            var rigidBody = this.GetComponent<Rigidbody>();
            if (rigidBody != null)
            {
                rigidBody.useGravity = true;
            }
        }

        protected override void Update()
        {
            lastShot += Time.deltaTime;
            if (lastShot >= ShootDelay)
            {
                lastShot = 0;
                canShoot = true;
            }
            if (_playerFound)
            {
                Shoot();
            }

            base.Update();
            canShoot = false;
        }

        public override void Shoot()
        {
            if (!canShoot || !_playerFound) return;
            //this._animator.SetBool("Shoot", true);
            DoShootPlayer();
            base.Shoot();
        }

        private void DoShootPlayer()
        {
            RaycastHit hit;
            Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
            if (!Physics.Raycast(this.transform.position, forward, out hit, Range)) return;
            if (hit.collider == null) return;
            if (hit.collider.tag != "Player") return;
            var player = hit.collider.GetComponent<PlayerHealth>();
            if (player == null) return;
            player.TakeDamage(Damage);
        }
    }
}

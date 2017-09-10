using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Monsters
{
    public class Cacodemon : BaseMonster
    {
        public float ShootDelay = 1f;
        public float Range = 20f;
        public int Damage = 5;
        private float rotatingSpeed = 0.1f;
        int shootableMask;
        bool canShoot = true;
        float lastShot;
        RaycastHit shotHit;
        private bool isDead;
        public GameObject shootPrefab;
        ///TODD: Remove it from here and create new object
        int projectileSpeed = 2;
        float projectileLifeTime = 5.0f;


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

        public override void FindPlayer(Transform playerTransfome)
        {
            base.FindPlayer(playerTransfome);
            this._animator.SetBool("PlayerFound", true);
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
            if (_playerFound && canShoot)
            {
                Shoot();
            }

            base.Update();
            canShoot = false;
        }

        public override void Shoot()
        {
            //this._animator.SetBool("Shoot", true);
            DoShootPlayer();
            base.Shoot();
        }

        private void DoShootPlayer()
        {
            //Rotate prefab to player
            //var rotationAngle = Quaternion.LookRotation(_playerTransform.position - transform.position);
            //transform.rotation = Quaternion.Lerp(transform.rotation, rotationAngle, Time.deltaTime * rotatingSpeed);

            //if (!CheckIfPlayeReachable()) return;
            ThrowProjectile();
            //player.TakeDamage(Damage);
        }

        private bool CheckIfPlayeReachable()
        {
            RaycastHit hit;
            Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
            if (!Physics.Raycast(this.transform.position, forward, out hit, Range)) return false;
            if (hit.collider == null) return false;
            if (hit.collider.tag != "Player") return false;
            var player = hit.collider.GetComponent<PlayerHealth>();
            if (player == null) return false;
            return true;
        }

        private void ThrowProjectile()
        {
            if (shootPrefab == null) return;
            GameObject projectile = Instantiate(shootPrefab, this.transform.position, Quaternion.LookRotation(this.transform.position - _playerTransform.position)) as GameObject;
            if (projectile == null) return;
            //projectile.transform.forward =  projectile.transform.position - _playerTransform.position;
            //projectile.transform.forward = this.transform.forward;
            //Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
            //if (projectileRigidbody == null) return;
            //projectileRigidbody.velocity = (_playerTransform.position - transform.position).normalized * projectileSpeed; //transform.TransformDirection(Vector3.forward) * projectileSpeed;
            Destroy(projectile, projectileLifeTime);
        }
    }
}

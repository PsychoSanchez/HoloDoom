using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Monsters
{
    public class Cacodemon : BaseMonster
    {
        public float ShootDelay = 1f;
        public float Range = 20f;
        public int Damage = 5;
        // private float rotatingSpeed = 0.1f;
        bool canShoot = true;
        float lastShot;
        // RaycastHit shotHit;
        public GameObject shootPrefab;
        ///TODD: Remove it from here and create new object
        // int projectileSpeed = 2;
        float projectileLifeTime = 5.0f;
        Rigidbody _rigidBody = null;
        float fallTime;
        private bool resourcesCleared = false;

        protected override void Start()
        {
            _animator = this.GetComponent<Animator>();
            _rigidBody = this.GetComponent<Rigidbody>();
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
            // this._animator.SetTrigger("Hit");
            // this._animator.SetBool("Attack", true);
        }

        public override void FindPlayer(Transform playerTransform)
        {
            base.FindPlayer(playerTransform);
            // this._animator.SetBool("PlayerFound", true);
        }

        protected override void Die()
        {
            base.Die();
        }

        /**
        Disables physics and prevents update
         */
        private void ClearResources()
        {
            _rigidBody.useGravity = false;
            this.GetComponent<BoxCollider>().enabled = false;
            Debug.Log("Physics disabled");

            resourcesCleared = true;
        }

        protected override void Update()
        {
            base.Update();
            if (resourcesCleared)
            {
                return;
            }
            if (AppStateManager.Instance.GetCurrentAppState() != AppState.Ready)
            {
                return;
            }

            if (_dead)
            {
                if (_rigidBody != null && _dieAnim.Frame == 2)
                {
                    _rigidBody.useGravity = true;
                    Debug.Log("rigid body enabled");
                }
                Fall();
                return;
            }

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

            canShoot = false;
        }

        private void Fall()
        {
            fallTime += Time.deltaTime;

            if (fallTime > 5.0f)
            {
                ClearResources();
            }
            else if (fallTime > 1.0f)
            {
                var speed = _rigidBody.velocity.magnitude;
                if (speed != 0)
                {
                    return;
                }
                ClearResources();
            }
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

            if (!Physics.Raycast(this.transform.position, forward, out hit, Range) ||
                hit.collider == null ||
                hit.collider.tag != "Player")
            {
                return false;
            }

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

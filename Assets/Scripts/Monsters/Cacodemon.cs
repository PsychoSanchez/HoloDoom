using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Monsters
{
    public class Cacodemon : BaseMonster
    {
        public float ShootDelay = 1f;
        public float Range = 20f;
        public int Damage = 5;
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

            if (AppStateManager.Instance.HeadUserID != CustomMessages.Instance.localUserID)
            {
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
            ThrowProjectile();
            base.Shoot();
        }
        public void Shoot(Vector3 position, Quaternion rotation)
        {
            this.transform.position = position;
            this.transform.rotation = rotation;
            ThrowProjectile(position, rotation);
            base.Shoot();
        }


        private bool CheckIfPlayeReachable()
        {
            RaycastHit hit;
            Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;

            if (IsNotPlayer(out hit, forward))
            {
                return false;
            }

            var player = hit.collider.GetComponent<PlayerHealth>();
            if (player == null) return false;
            return true;
        }

        private bool IsNotPlayer(out RaycastHit hit, Vector3 forward)
        {
            return !Physics.Raycast(this.transform.position, forward, out hit, Range) ||
                            hit.collider == null ||
                            hit.collider.tag != "Player";
        }

        private void ThrowProjectile()
        {
            if (shootPrefab == null) return;
            var position = this.transform.position;
            var rotation = Quaternion.LookRotation(this.transform.position - _playerTransform.position);
            GameObject projectile = Instantiate(shootPrefab, position, rotation) as GameObject;
            if (projectile == null) return;
            CustomMessages.Instance.SendShootProjectile(position, rotation);

            Destroy(projectile, projectileLifeTime);
        }

        private void ThrowProjectile(Vector3 position, Quaternion rotation)
        {
            if (shootPrefab == null) return;
            GameObject projectile = Instantiate(shootPrefab, position, rotation) as GameObject;
            if (projectile == null) return;

            Destroy(projectile, projectileLifeTime);
        }
    }
}

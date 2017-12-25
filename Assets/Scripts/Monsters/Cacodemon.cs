using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Monsters
{

    public class Cacodemon : BaseMonster
    {

        // Fly state
        float flyDistance = 2;
        float flyTimer = 0;

        Vector3 startPosition;
        Vector3 endPosition;

        // Change state
        float chargeTimer = 0;

        Rigidbody rigidbodyComponent;

        protected override void updateAIState()
        {
            switch (this.aiState)
            {
                case MonsterAIState.BeforeMoving:
                    // play audio

                    this.playIdleSound();

                    this.startPosition = GameManager.Instance.GetSpawnPosition(5, 2).position;

                    this.flyTimer = 0;
                    this.flyDistance = Vector3.Distance(startPosition, endPosition);
                    this.flyDistance = flyDistance != 0 ? flyDistance : 2;
                    this.aiState = MonsterAIState.Moving;
                    break;
                case MonsterAIState.Moving:
                    flyTimer += Time.deltaTime;

                    // Debug
                    flyDistance = 1;

                    Vector3 position = Vector3.Slerp(startPosition, endPosition, flyTimer / flyDistance);
                    transform.rotation = Quaternion.LookRotation(startPosition - endPosition);

                    this._rigidBody.velocity = Vector3.Normalize(transform.position - position);


                    if (flyTimer / flyDistance > 1)
                    {
                        chargeTimer = 0;

                        // transform.rotation = Quaternion.LookRotation(transform.position - _playerTransform.position);
                        this.LookAtMainCamera();
                        aiState = MonsterAIState.Charging;

                        waitForMsec(1000);
                    }
                    break;
                case MonsterAIState.Charging:
                    this.playIdleSound();
                    break;

                case MonsterAIState.Fire:
                    // create fireball towards the player

                    if (bPlayerFound)
                    {
                        Shoot();
                    }

                    aiState = MonsterAIState.Chilling;
                    break;
                case MonsterAIState.Chilling:
                    waitForMsec(1000);
                    // keep last firing animation frame for a second
                    aiState = MonsterAIState.BeforeMoving;
                    break;

                case MonsterAIState.WaitingForTeleport:
                    break;
                case MonsterAIState.Teleported:
                    break;
                case MonsterAIState.ChillingAfterTeleport:
                    break;
                /* 
                    1) шипит
                    2) начинает перемещаться в сторону игрока по диагонали туда-сюда
                    3) время от времени создаёт фаербол и кидает в игрока
                    4) после фаербола ждёт секунду ничего не делая и не уворачиваясь
                    5) если не видит игрока больше 10 секунд, телепортируется к игроку (позиция вычисляется рейкастом от игрока)
                    6) если игрок наводит ружьё на какодемона, тот детает резкий скачок в сторону (редко)
                */
                case MonsterAIState.Teleporting:
                    /*
                        1) тупит 2 секунды
                        2) пердит 2 секунды (плазмой)
                        3) перемещается
                        4) тупит ещё 3 секунды
                    */
                    break;
            }
        }

        public float ShootDelay = 1f;
        public float Range = 20f;
        public int Damage = 5;
        bool canShoot = true;
        float lastShot;
        public GameObject shootPrefab;
        float projectileLifeTime = 5.0f;
        Rigidbody _rigidBody = null;
        float fallTime;
        private bool resourcesCleared = false;

        protected override void Start()
        {
            _rigidBody = this.GetComponent<Rigidbody>();
            startPosition = transform.position;
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
            this.transform.Find("Sprite").GetComponent<Billboard>().enabled = true;
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
            updateAIState();
            if (resourcesCleared)
            {
                return;
            }
            if (AppStateManager.Instance.GetAppState() != AppState.Playing)
            {
                return;
            }

            if (bDead)
            {
                if (_rigidBody != null && _dieAnim.Frame == 2)
                {
                    _rigidBody.useGravity = true;
                }
                Fall();
                return;
            }

            if (AppStateManager.Instance.HeadUserID != CustomMessages.Instance.localUserID)
            {
                return;
            }

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
        public override void Shoot(long id, Vector3 position, Quaternion rotation)
        {
            this.transform.position = position;
            this.transform.rotation = rotation;
            ThrowProjectile(id, position, rotation);
            base.Shoot();
        }


        private bool CheckIfPlayerReachable()
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
            var projScript = projectile.GetComponent<Projectile>();
            projScript.SetId(EnemyManager.Instance.GenerateEnemyId());
            CustomMessages.Instance.SendShootProjectile(this.Id, projScript.Id, position, rotation);
            EnemyManager.Instance.AddProjectile(projScript.Id, projectile);

            Destroy(projectile, projectileLifeTime);
        }

        private void ThrowProjectile(long projId, Vector3 position, Quaternion rotation)
        {
            if (shootPrefab == null) return;
            GameObject projectile = Instantiate(shootPrefab, position, rotation) as GameObject;
            if (projectile == null) return;
            projectile.GetComponent<Projectile>().SetId(projId);
            EnemyManager.Instance.AddProjectile(projId, projectile);

            Destroy(projectile, projectileLifeTime);
        }
    }
}

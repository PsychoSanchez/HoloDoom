using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Sharing.Tests;
using UnityEngine;

namespace Assets.Scripts.Monsters
{

    public abstract class BaseMonster : OverridableMonoBehaviour
    {
        public AudioClip soundSpawn;
        public AudioClip soundTakeDamage;
        public AudioClip soundAttack;
        public AudioClip soundDeath;
        public AudioClip soundIdle;

        private float takeDamageLastTime = 0;

        public Sprite[] EnemySprites;
        public Sprite[] SpawnAnimationSprites;
        public Sprite[] ShootAnimationSprites;
        public Sprite[] DeathAnimationSprites;
        private long id;
        public long Id
        {
            get { return id; }
            set { id = value; }
        }

        private long chasedPlayer;
        public long ChasedPlayer
        {
            get { return chasedPlayer; }
            private set { chasedPlayer = value; }
        }

        protected int _health;
        protected int _armor;
        protected Weapon _weapon;
        protected bool bDead = false;

        protected bool bPlayerFound;
        protected Transform _playerTransform;
        protected bool bSpawning = true;
        protected CustomAnimation _spawnAnim;
        protected CustomAnimation _shootAnim;
        protected CustomAnimation _dieAnim;

        private SpriteRenderer _sprite;
        private float _renderUpdate;

        AudioSource _audioSource;

        protected CustomAnimator animator;

        protected MonsterAIState aiState = MonsterAIState.Spawning;
        public enum MonsterAIState
        {
            Spawning, // когда появляется 
            BeforeMoving, // перед перемещением
            Moving,
            Charging,
            Fire,
            Chilling,
            WaitingForTeleport,
            Teleporting,
            Teleported,
            ChillingAfterTeleport,
            Death,
            Deceased
        }

        float aiWaitTimer = 0;
        protected void waitForMsec(float msec)
        {
            this.aiWaitTimer = msec * 1000;
        }

        protected virtual void updateAIState()
        {
            // to be overridden by children
        }

        private void updateAIStateBase()
        {
            switch (aiState)
            {
                case MonsterAIState.Spawning:
                    _audioSource.PlayOneShot(this.soundSpawn, 0.7F);
                    animator.Play(_spawnAnim);
                    aiState = MonsterAIState.Spawning;
                    // ждать пару секунд после спавна
                    waitForMsec(2);
                    aiState = MonsterAIState.BeforeMoving;
                    break;
                case MonsterAIState.Death:
                    animator.Play(_dieAnim);
                    _audioSource.PlayOneShot(soundDeath, 0.7F);
                    waitForMsec(animator.animation.duration);
                    aiState = MonsterAIState.Deceased;
                    break;
            }
        }

        public virtual bool IsAlive()
        {
            return !bDead;
        }

        public virtual void GetHit(int amount)
        {
            if (Time.time - takeDamageLastTime > 1.0f)
            {
                takeDamageLastTime = Time.time;
                _audioSource.PlayOneShot(this.soundTakeDamage, 0.7F);
            }
            _health -= amount;
        }

        protected virtual void Die()
        {
            bDead = true;
            GameManager.Instance.EnemyKilled();
        }

        public virtual void Shoot()
        {
            _audioSource.PlayOneShot(this.soundAttack, 0.7F);
        }

        public abstract void Shoot(long id, Vector3 position, Quaternion rotation); // to be overridden

        protected IEnumerator playIdleSound()
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(1, 3));
            _audioSource.PlayOneShot(soundDeath, 0.7F);
        }

        // Use this for initialization
        protected virtual void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();

            _spawnAnim = new CustomAnimation(SpawnAnimationSprites, 12, false);
            _shootAnim = new CustomAnimation(ShootAnimationSprites, 12, false);
            _dieAnim = new CustomAnimation(DeathAnimationSprites, 6, false);

            _health = 100;
            _armor = 0;

            animator = new CustomAnimator(_sprite);

            this.aiState = MonsterAIState.Spawning;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (_sprite == null) return;
            _renderUpdate += Time.deltaTime;

            if (_renderUpdate < 0.08f)
            {
                return;
            }

            animator.Update(_renderUpdate);

            aiWaitTimer -= Time.deltaTime;
            if (aiWaitTimer <= 0)
            {
                this.updateAIStateBase();
                this.updateAIState();
            }


            _renderUpdate = 0f;
        }


        private void MoveToPlayer() // TODO: прихуярить куда-нибудь
        {
            if (_playerTransform == null)
            {
                return;
            }
            gameObject.transform.forward = _playerTransform.position - gameObject.transform.position;
            // RotateSprite();
        }


        // private void RotateMonster(float rotation)
        // {
        //     transform.Rotate(0, -rotation, 0);
        // }
        public static bool IsRightSide(Vector3 right, Vector3 to)
        {
            return Vector3.Angle(right, to) > 90f;
        }
        protected void LookAtMainCamera()
        {
            var target = Camera.main.transform;
            var vec1 = target.position - gameObject.transform.position;
            var vec2 = gameObject.transform.forward;
            var angle = Vector3.Angle(vec1, vec2);
            int index = GetSpriteIndexFromAngle(angle);
            _sprite.flipX = IsRightSide(gameObject.transform.right, vec1);
            _sprite.sprite = EnemySprites[index];
            _sprite.transform.forward = vec1;
        }

        private static int GetSpriteIndexFromAngle(float angle)
        {
            var index = 0;
            if (angle < 20)
            {
                index = 0;
            }
            else if (angle < 60)
            {
                index = 1;
            }
            else if (angle < 100)
            {
                index = 2;
            }
            else if (angle < 140)
            {
                index = 3;
            }
            else
            {
                index = 4;
            }

            return index;
        }

        public virtual void FindPlayer()
        {
            if (bPlayerFound)
            {
                return;
            }


            bPlayerFound = true;
            _playerTransform = Camera.main.transform;
            CustomMessages.Instance.MonsterFoundPlayer(this.id);
        }
        public virtual void FindPlayer(long chasedPlayerId)
        {
            if (bPlayerFound)
            {
                return;
            }

            bPlayerFound = true;
            chasedPlayer = chasedPlayerId;
            var remoteHead = RemoteHeadManager.Instance.TryGetRemoteHeadInfo(chasedPlayer);
            if (remoteHead == null)
            {
                Debug.Log("User head not found. id " + chasedPlayerId);
                return;
            }
            _playerTransform = remoteHead.HeadObject.transform;
        }



    }
}

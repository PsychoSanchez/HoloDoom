using System;
using System.Collections;
using System.Collections.Generic;
using HoloDoom.Animation;
using HoloToolkit.Sharing.Tests;
using UnityEngine;

namespace Assets.Scripts.Monsters
{
    public class BaseMonster : OverridableMonoBehaviour
    {
        public AudioClip SpawnSound;
        public AudioClip TakeDamageSound;
        public AudioClip AttackSound;
        public AudioClip DeathSound;
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
        protected CustomAnimator _animator;

        private float _renderUpdate;

        AudioSource _audioSource;
        private SpriteRenderer _renderer;

        public virtual void Spawn()
        {
            _animator.PlayOnce("spawn");
            bSpawning = true;
        }

        public virtual bool IsAlive()
        {
            return !bDead;
        }

        public virtual void GetHit(int amount)
        {
            _health -= amount;
        }

        protected virtual void Die()
        {
            _audioSource.PlayOneShot(DeathSound, 0.7F);
            _animator.PlayOnce("die");
            bDead = true;
            GameManager.Instance.EnemyKilled();
        }

        public virtual void Shoot()
        {
            _animator.PlayOnce("shoot");
        }
        public virtual void Shoot(long id, Vector3 position, Quaternion rotation)
        {
        }
        // Use this for initialization
        protected virtual void Start()
        {
            // GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            _audioSource = GetComponent<AudioSource>();
            _renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            _animator = new CustomAnimator(12, _renderer);
            _animator.AddAnimationSequence("spawn", SpawnAnimationSprites);
            _animator.AddAnimationSequence("shoot", ShootAnimationSprites);
            _animator.AddAnimationSequence("die", DeathAnimationSprites, 6);
            _health = 100;
            _armor = 0;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            _animator.Update(Time.deltaTime);

            if (bDead)
            {
                return;
            }

            _renderUpdate += Time.deltaTime;

            if (_renderUpdate < 0.08f)
            {
                return;
            }

            if (bPlayerFound)
            {
                MoveToPlayer();
                return;
            }
            RotateMonster(15);
            RotateSprite();
            _renderUpdate = 0f;
        }

        private void MoveToPlayer()
        {
            if (_playerTransform == null)
            {
                return;
            }
            gameObject.transform.forward = _playerTransform.position - gameObject.transform.position;
            RotateSprite();
        }

        private void RotateMonster(float rotation)
        {
            transform.Rotate(0, -rotation, 0);
        }
        public static bool IsRightSide(Vector3 right, Vector3 to)
        {
            return Vector3.Angle(right, to) > 90f;
        }
        private void RotateSprite()
        {
            var target = Camera.main.transform;
            var vec1 = target.position - gameObject.transform.position;
            var vec2 = gameObject.transform.forward;
            var angle = Vector3.Angle(vec1, vec2);
            int index = GetSpriteIndexFromAngle(angle);
            _renderer.flipX = IsRightSide(gameObject.transform.right, vec1);
            _renderer.sprite = EnemySprites[index];
            _renderer.transform.forward = vec1;
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

using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Sharing.Tests;
using UnityEngine;

namespace Assets.Scripts.Monsters
{
    public class CustomAnimation
    {
        Sprite[] _timeline;
        public int Frame { get; private set; }
        public bool Finished { get; private set; }

        private DateTime _lastFrame = DateTime.Now;
        private float _delay = 0f;
        public CustomAnimation(Sprite[] timeline, float frameRate)
        {
            _timeline = timeline;
            _delay = 1000 / frameRate;
            Frame = 0;
        }

        public Sprite GetNextFrame()
        {
            if (_timeline == null)
            {
                return null;
            }

            float timePassed = (DateTime.Now - _lastFrame).Milliseconds;
            if (timePassed > _delay)
            {
                Frame++;
                _lastFrame = DateTime.Now;
            }

            if (Frame >= _timeline.Length)
            {
                Frame = 0;
                Finished = true;
                return null;
            }

            return _timeline[Frame];
        }

        public void Reset()
        {
            Frame = 0;
            Finished = false;
        }
    }
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
        protected bool _dead = false;
        protected Animator _animator;
        protected bool _playerFound;
        protected Transform _playerTransform;
        protected bool _spawning = true;
        protected CustomAnimation _spawnAnim;
        protected CustomAnimation _shootAnim;
        protected CustomAnimation _dieAnim;

        private SpriteRenderer _sprite;
        private float _renderUpdate;
        private Vector3 _direction;

        AudioSource _audioSource;

        public virtual void Spawn()
        {
            _spawning = true;
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
            _audioSource.PlayOneShot(DeathSound, 0.7F);
            _dead = true;
            GameManager.Instance.EnemyKilled();
        }

        public virtual void Shoot()
        {
            PlayShootAnimation();
        }
        public virtual void Shoot(long id, Vector3 position, Quaternion rotation)
        {
        }

        private void PlayShootAnimation()
        {

        }

        // Use this for initialization
        protected virtual void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
            _direction = transform.forward;
            _spawnAnim = new CustomAnimation(SpawnAnimationSprites, 12);
            _shootAnim = new CustomAnimation(ShootAnimationSprites, 12);
            _dieAnim = new CustomAnimation(DeathAnimationSprites, 6);
            _health = 100;
            _armor = 0;
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

            if (_spawning)
            {
                PlaySpawnAnimation();
                return;
            }

            if (_dead)
            {
                PlayDeathAnimation();
                return;
            }

            if (_playerFound)
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

        private void PlayDeathAnimation()
        {
            if (_dieAnim.Finished)
            {
                return;
            }
            var sprite = _dieAnim.GetNextFrame();
            if (sprite != null)
            {
                _sprite.sprite = sprite;
            }
        }

        private void PlaySpawnAnimation()
        {
            var sprite = _spawnAnim.GetNextFrame();
            if (sprite != null)
            {
                _sprite.sprite = sprite;
            }
            else
            {
                _spawning = false;
            }
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
            if (_playerFound)
            {
                return;
            }


            _playerFound = true;
            _playerTransform = Camera.main.transform;
            CustomMessages.Instance.MonsterFoundPlayer(this.id);
        }
        public virtual void FindPlayer(long chasedPlayerId)
        {
            if (_playerFound)
            {
                return;
            }


            _playerFound = true;
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

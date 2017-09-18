using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Monsters
{
    public class BaseMonster : OverridableMonoBehaviour
    {
        public AudioClip SpawnSound;
        public AudioClip TakeDamageSound;
        public AudioClip AttackSound;
        public AudioClip DeathSound;

        protected int _health;
        protected int _armor;
        protected Weapon _weapon;
        protected bool _dead = false;
        protected Animator _animator;
        protected bool _playerFound;
        protected Transform _playerTransform;

        private SpriteRenderer _sprite;
        private float _renderUpdate;

        AudioSource _audioSource;

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
            _audioSource.PlayOneShot(DeathSound, 0.7F);
            _dead = true;
        }

        public virtual void Shoot()
        {

        }

        // Use this for initialization
        protected virtual void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _sprite = GetComponent<SpriteRenderer>();
            _health = 100;
            _armor = 0;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (_sprite == null) return;
            _renderUpdate += Time.deltaTime;
            if (_renderUpdate >= 0.32f)
            {
                _sprite.transform.forward = -Camera.main.transform.forward;
                _renderUpdate = 0f;
            }
            // This method will be called all the time, while _health <= 0
            //if (_health <= 0)
            //{
            //    Die();
            //}
        }

        public virtual void FindPlayer(Transform playerTransform)
        {
            _playerFound = true;
            _playerTransform = playerTransform;
        }
    }
}

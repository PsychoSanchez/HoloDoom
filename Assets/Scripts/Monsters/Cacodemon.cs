using UnityEngine;

namespace Assets.Scripts.Monsters
{
    public class Cacodemon : BaseMonster
    {
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
    }
}

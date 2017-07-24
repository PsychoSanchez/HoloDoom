using System.Collections;
using System.Collections.Generic;
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

        protected override void Die()
        {
            this._animator.SetBool("Dead", true);
        }
    }
}

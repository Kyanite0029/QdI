using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Spine;
using Spine.Unity;

namespace Qdi_MOD_Creatures
{
    internal class ScribbledLifeAnim : CreatureAnimScript
    {
        private ScribbledLife script;

        private new SkeletonAnimation animator;

        public void SetScript(ScribbledLife sl) 
        {
            this.script = sl;
            this.animator = base.gameObject.GetComponent<SkeletonAnimation>();
            this.animator.transform.localScale *= 0.5f;

            this.Default();

        }

        public void Default()
        {
            this.animator.AnimationState.SetAnimation(0, "default", true);
        }

        public void Default(TrackEntry entry)
        {
            this.animator.AnimationState.SetAnimation(0, "default", true);
        }

        public void StartWork()
        { 
            this.animator.AnimationState.SetAnimation(0, "start_work", true).Complete += this.Default;
        }

        public void Skill()
        {
            this.animator.AnimationState.SetAnimation(0, "skill", false).Complete += this.Default;
        }

        public void Attack()
        {
            this.animator.AnimationState.SetAnimation(0, "attack", false).Complete += this.Default;
        }
        
        public void Escape()
        {
            this.animator.AnimationState.SetAnimation(0, "move", true);
        }

        public void Dead()
        {
            this.animator.AnimationState.SetAnimation(0, "dead", false);
        }

    }
}

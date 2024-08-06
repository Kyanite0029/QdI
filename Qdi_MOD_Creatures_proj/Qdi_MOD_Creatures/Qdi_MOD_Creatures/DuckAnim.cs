using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Spine;
using Spine.Unity;


namespace Qdi_MOD_Creatures
{
    internal class DuckAnim : CreatureAnimScript
    {
        private Duck script;

        private new SkeletonAnimation animator;

        public void SetScript(Duck duck)
        {
            this.script = duck;
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

        public void Quack()
        {
            this.animator.AnimationState.SetAnimation(0, "quack", false).Complete += this.Default;
        }
    }
}

using System;
using System.Runtime.CompilerServices;


namespace Qdi_MOD_Creatures
{
    public class TurquoiseDebuf : UnitBuf
    {
        public TurquoiseDebuf(float dmg)
        {
            this.duplicateType = BufDuplicateType.UNLIMIT;
            if (dmg > 50.0f)
            {
                _tickTime = 0.5f;
            }
            else
            {
                _tickTime = 1.0f;
            }
            this.tickTimer.StartTimer(this._tickTime);
            this.tdmg = dmg;

        }

        public override void Init(UnitModel model)
        {
            base.Init(model);
            this.remainTime = tdmg;

        }

        public override void FixedUpdate()
        {
            float weakres = -10;
            int tickdamage = (int)Math.Sqrt(this.tdmg);
            base.FixedUpdate();
            if (this.model.hp <= 0f)
            {
                return;
            }
            if (this.tickTimer.RunTimer())
            {
                weakres = this.model.defense.GetMultiplier(RwbpType.R);
                this._dmgType = RwbpType.R;
                if (weakres < this.model.defense.GetMultiplier(RwbpType.W))
                {
                    weakres = this.model.defense.GetMultiplier(RwbpType.W);
                    this._dmgType = RwbpType.W;
                }
                if (weakres < this.model.defense.GetMultiplier(RwbpType.B))
                {
                    weakres = this.model.defense.GetMultiplier(RwbpType.B);
                    this._dmgType = RwbpType.B;
                }
                if (weakres < this.model.defense.GetMultiplier(RwbpType.P))
                {
                    weakres = this.model.defense.GetMultiplier(RwbpType.P);
                    this._dmgType = RwbpType.P;
                }
                this.model.TakeDamage(new DamageInfo(this._dmgType, (float)tickdamage));
                this.tickTimer.StartTimer(1f);
                global::DamageParticleEffect damageParticleEffect = global::DamageParticleEffect.Invoker(this.model, this._dmgType, this.model.defense);
            }
        }

        public override void OnUnitDie()
        {
            base.OnUnitDie();
            this.Destroy();
        }

        private RwbpType _dmgType;

        private float _tickTime;

        private float tdmg;

        private float _dmg;

        private Timer tickTimer = new Timer();
    }
}






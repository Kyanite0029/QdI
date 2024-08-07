using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Qdi_MOD_Creatures
{
    public class WendigoArmor : EquipmentScriptBase
    {
        private void PrintLog(string s)
        {
            if (this._LOG_STATE)
            {
                Debug.LogError(s);
            }
        }
        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            List<global::WorkerModel> deads = this.GetDeads();
            foreach (global::WorkerModel workerModel in deads)
            {
                if (this.IsInRange(workerModel, 1f))
                {
                    this.Heal(workerModel);
                }
            }
        }

        private void Heal(global::WorkerModel dead)
        {
            GameObject gameObject = dead.GetWorkerUnit().gameObject;
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            (base.model.owner as global::WorkerModel).RecoverMental((float)base.model.owner.maxHp * 0.05f);
            gameObject.SetActive(false);
            global::ExplodeGutEffect explodeGutEffect = null;
            if (global::ExplodeGutManager.instance.MakeEffects(dead.GetCurrentViewPosition(), ref explodeGutEffect))
            {
                explodeGutEffect.particleCount = UnityEngine.Random.Range(3, 9);
                explodeGutEffect.ground = dead.GetMovableNode().GetCurrentViewPosition().y;
                explodeGutEffect.SetEffectSize(0.5f);
                explodeGutEffect.Shoot(global::ExplodeGutEffect.Directional.CENTRAL, null);
            }
        }

        private List<global::WorkerModel> GetDeads()
        {
            global::PassageObjectModel passage = base.model.owner.GetMovableNode().GetPassage();
            List<global::WorkerModel> list = new List<global::WorkerModel>();
            if (passage == null)
            {
                return list;
            }
            global::MovableObjectNode[] deletedUnits = passage.GetDeletedUnits();
            foreach (global::MovableObjectNode movableObjectNode in deletedUnits)
            {
                global::WorkerModel workerModel = movableObjectNode.GetUnit() as global::WorkerModel;
                if (workerModel != null && workerModel.GetWorkerUnit().gameObject.activeInHierarchy)
                {
                    list.Add(workerModel);
                }
            }
            return list;
        }

        private bool IsInRange(global::UnitModel target, float range)
        {
            float x = target.GetCurrentViewPosition().x;
            float x2 = base.model.owner.GetCurrentViewPosition().x;
            if (base.model.owner.GetMovableNode().GetDirection() == global::UnitDirection.LEFT)
            {
                if (x > x2)
                {
                    return false;
                }
            }
            else if (x < x2)
            {
                return false;
            }
            return this.GetDistance(target) <= range;
        }

        private float GetDistance(global::UnitModel target)
        {
            float x = target.GetCurrentViewPosition().x;
            float x2 = base.model.owner.GetMovableNode().GetCurrentViewPosition().x;
            float num = 1f;
            if (base.model.owner.GetMovableNode().currentPassage != null)
            {
                num = base.model.owner.GetMovableNode().currentPassage.scaleFactor;
            }
            return Math.Abs(x - x2) / num;
        }

        private readonly bool _LOG_STATE;

        private const float _lootRange = 1f;

    }
}

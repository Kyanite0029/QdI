﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Qdi_MOD_Creatures
{
    public  class JackTheRipper : CreatureBase
    {

        public override void OnStageStart()
        {
            base.OnStageStart();
            //게임 시작할때 모든 직원들에게 혼자 있는지 체크하는 숨겨진 상태이상 부여
            List<global::AgentModel> list = new List<global::AgentModel>();
            foreach (global::AgentModel agentModel in global::AgentManager.instance.GetAgentList())
            {
                list.Add(agentModel);
            }
            foreach (global::AgentModel agentModel2 in list)
            {
                agentModel2.AddUnitBuf(new JackBuf());
            }
            //클리포트 카운터 감소 쿨다운
            this.qccd = 0f;

        }

        public override void OnReleaseWork(UseSkill skill)
        {
            base.OnReleaseWork(skill);
            //나쁨 뜨면 클카 감소
            if (this.model.feelingState == CreatureFeelingState.BAD)
            {
                this.model.SubQliphothCounter();
            }
            //좋음 뜨면 클카 증가
            else if (this.model.feelingState == CreatureFeelingState.GOOD)
            {
                this.model.AddQliphothCounter();
            }
        }

        public override void OnFixedUpdate(CreatureModel creature)
        {
            //쿨타임 체크
            base.OnFixedUpdate(creature);
            qccd += Time.deltaTime;

        }

        private void QliSub()
        {
            //쿨다운이 충분히 지나야만 클리포트 카운터 감소
            if (this.qccd > 30f)
            {
                this.model.SubQliphothCounter();
                this.qccd = 0f;
            }
            
        }

        private bool breached;

        private float qccd;

        private class JackBuf : UnitBuf
        {

            public JackBuf()
            {
                this.type = (UnitBufType)270398;
                this.duplicateType = BufDuplicateType.ONLY_ONE;
            }

            public void SetAbno(JackTheRipper ab)
            {
                this.abno = ab;
            }

            public override void Init(UnitModel model)
            {
                base.Init(model);
                bool flag = !(model is WorkerModel);
                if (flag)
                {
                    this.Destroy();
                }
            }

            public override void FixedUpdate()
            {
                bool flag = !(this.model is WorkerModel) || ((WorkerModel)this.model).IsDead();
                if (!flag)
                {
                    try
                    {
                        bool flag2 = !this.abno.breached && !((WorkerModel)this.model).IsDead() && !((WorkerModel)this.model).IsStunned() && !Array.Exists<MovableObjectNode>(this.model.GetMovableNode().currentPassage.GetEnteredTargetsAsArray(), (MovableObjectNode x) => (x.GetUnit() is WorkerModel || x.GetUnit() is CreatureModel) && x.GetUnit() != this.model);
                        if (flag2)
                        {
                            this.lonely += Time.deltaTime;
                        }
                        else
                        {
                            this.lonely = 0f;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                    //200초 이상 혼자 있었는가?
                    bool flag3 = this.lonely > 200f;
                    if (flag3)
                    {
                        bool breached = this.abno.breached;
                        if (breached)
                        {
                            this.lonely = 0f;
                        }
                        else
                        {
                            this.lonely = 0f;
                            bool flag4 = (model is AgentModel);
                            if (flag4)
                            {
                                //클카 감소 시도
                                this.abno.QliSub();
                            }
                        }
                    }
                    this.time += Time.deltaTime;


                }
            }

            public override void Destroy()
            {
                base.Destroy();
            }

            public override void OnUnitDie()
            {
                base.OnUnitDie();
                this.Destroy();
            }

            public override void OnStageRelease()
            {
                base.OnStageRelease();
                this.Destroy();
            }

            private GameObject _effect;

            private JackTheRipper abno;

            private float lonely = -10f;

            private float time;

        }

    }
}

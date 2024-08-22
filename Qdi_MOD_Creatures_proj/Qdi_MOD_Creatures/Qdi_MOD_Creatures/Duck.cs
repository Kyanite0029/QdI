using System;
using System.Collections.Generic;

using UnityEngine;


namespace Qdi_MOD_Creatures
{
    public class Duck : CreatureBase
    {
        private DuckAnim _animScript; // 아직 미구현
        public bool IsQuack = false;

        public override void OnViewInit(CreatureUnit unit)
        {
            base.OnViewInit(unit);
            this._animScript = (DuckAnim)unit.animTarget; // 아직 미구현
            this.ParamInit();
        }

        public override void ParamInit()
        {
            base.ParamInit();
            this.IsQuack = false;
        }

        public override void OnStageStart()
        {
            base.OnStageStart();
            this.ParamInit();
        }

        public override void OnStageRelease()
        {
            base.OnStageRelease();
            this.ParamInit();
        }

        public override void OnStageEnd()
        {
            base.OnStageEnd();
            this.ParamInit();
        }

        public void ActiveSkill(AgentModel agent, RwbpType workType)
        {
            if (agent.IsDead() || agent.IsPanic())
            {
                return;
            }
            else
            {
                // agent의 체력이 30% 이하일 경우 높은 폭으로 체력과 정신력 모두 회복
                if (agent.hp <= agent.maxHp * 0.3f)
                {
                    agent.RecoverHP(agent.maxHp*0.3f);
                    agent.RecoverMental(agent.maxMental*0.35f);
                }
                else
                {
                    float recoverAmount = UnityEngine.Random.Range(0.01f, 0.05f);
                    //  workType이 "W"일 경우 체력을, "B"일 경우 정신력을 낮은 량 회복
                    if (workType == RwbpType.W)
                    {
                        agent.RecoverHP(agent.maxHp * recoverAmount);
                    }
                    else if (workType == RwbpType.B)
                    {
                        agent.RecoverMental(agent.maxMental * recoverAmount);
                    }
                }
            }
            // 50% 확률로 꽥꽥거리기
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                this.IsQuack = true;
            }

            if (this.IsQuack)
            {
                //꽥꽥거리는 애니메이션 재생
                //체력, 정신력 추가 회복
                agent.RecoverHP(agent.maxHp * 0.05f);
                agent.RecoverMental(agent.maxMental * 0.05f);
                this._animScript.Quack(); // 아직 미구현
            }
        }

        public override void OnReleaseWork(UseSkill skill)
        {
            base.OnReleaseWork(skill);
            AgentModel agent = skill.agent;

            if (agent != null)
            {
                Debug.Log("Duck ACTIVATED");
                if (skill.skillTypeInfo.id == 2L || skill.skillTypeInfo.id == 3L)
                {
                    Debug.Log("yes skill");
                    // 작업 결과가 좋음일 경우 스킬 발동
                    if (this.model.feelingState == CreatureFeelingState.GOOD) this.ActiveSkill(agent, skill.skillTypeInfo.rwbpType);
                }
                else
                {
                    Debug.Log("no skill");
                    return;
                }
            }
        }
    }
}
//
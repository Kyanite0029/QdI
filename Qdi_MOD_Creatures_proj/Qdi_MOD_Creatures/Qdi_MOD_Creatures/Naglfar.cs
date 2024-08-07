using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qdi_MOD_Creatures
{
    public class Naglfar : CreatureBase, IObserver
    {
        public Naglfar()
        {

        }

        public override void OnStageStart()
        {
            base.OnStageStart();
            this.model.ResetQliphothCounter();
            this._currentAgentDead = 5;
            this.ParamInit();
            this.deadList.Clear();
            global::Notice.instance.Observe(global::NoticeName.OnAgentDead, this);
            global::Notice.instance.Observe(global::NoticeName.OnOfficerDie, this);
        }

        public void OnNotice(string notice, params object[] param)
        {
            if (notice == global::NoticeName.OnAgentDead)
            {
                global::AgentModel agentModel = param[0] as global::AgentModel;
                if (agentModel == null)
                {
                    return;
                }
                if (agentModel.DeadType == global::DeadType.EXECUTION)
                {
                    return;
                }
                if (this.deadList.Contains(agentModel))
                {
                    return;
                }
                this.deadList.Add(agentModel);
                if (!this.model.IsEscaped())
                {
                    this.ReduceAliveAgent();
                }
            }
            if (notice == global::NoticeName.OnOfficerDie)
            {
                global::OfficerModel officerModel = param[0] as global::OfficerModel;
                if (officerModel == null)
                {
                    return;
                }
                if (officerModel.DeadType == global::DeadType.EXECUTION)
                {
                    return;
                }
                if (this.deadList.Contains(officerModel))
                {
                    return;
                }
                this.deadList.Add(officerModel);
                if (!this.model.IsEscaped())
                {
                    this.ReduceAliveAgent();
                }
            }
        }


        private void ReduceAliveAgent()
        {
            this._currentAgentDead--;
            if (this._currentAgentDead == 0)
            {
                this.model.SubQliphothCounter();
                this._currentAgentDead = 5;
            }
        }

        public override void OnFinishWork(UseSkill skill)
        {
            base.OnFinishWork(skill);
            global::AgentModel agent = skill.agent;
            if (skill.skillTypeInfo.id == 1L)
            {
                this.model.SubQliphothCounter();
            }
            else if (skill.skillTypeInfo.id == 2L)
            {
                this.model.AddQliphothCounter();
            }
            else if (skill.skillTypeInfo.id == 4L)
            {
                this.model.ResetQliphothCounter();
            }
        }


        private List<global::WorkerModel> deadList = new List<global::WorkerModel>();

        private int _currentAgentDead;
    }
}

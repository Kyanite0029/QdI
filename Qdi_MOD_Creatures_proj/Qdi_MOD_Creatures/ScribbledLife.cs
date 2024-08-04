using System;
using System.Collections.Generic;

using UnityEngine;

namespace Qdi_MOD_Creatures
{
    public class ScribbledLife : CreatureBase, IObserver
    {
        private ScribbledLifeAnim _animScript;

        private bool _attacking;

        private Timer _attackTimer = new Timer();

        private static DamageInfo damageInfo = new DamageInfo(RwbpType.B, 5, 15);

        private MapNode _currentDestNode;

        private int goodCounter;
        
        public List<String> workedAgents = new List<String>();

        public override void OnViewInit(CreatureUnit unit)
        {
            base.OnViewInit(unit);
            this._animScript = (ScribbledLifeAnim)unit.animTarget;
            this.ParamInit();
        }


        public override void ParamInit()
        {
            base.ParamInit();
            this._attacking = false;
            this._attackTimer.StopTimer();
            this._currentDestNode = null;
            this.goodCounter = 0;
            this.workedAgents.Clear();
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

        public override void OnSuppressed()
        {
            base.OnSuppressed();
            this.movable.StopMoving();
        }

        public void ActiveSkill(Sefira sefira)
        {
            foreach (AgentModel agent in sefira.agentList)
            {
                agent.RecoverMental(goodCounter * 5f);
                if (workedAgents.Contains(agent.name))
                {
                    agent.RecoverMental(goodCounter * 10f);
                }
            }
        }

        public override void ActivateQliphothCounter()
        {
            base.ActivateQliphothCounter();
            this.goodCounter = 0;
            this.workedAgents.Clear();
            this.Escape();
        }

        public void SubQliphothCounter()
        {
            if (this.model.qliphothCounter > 0)
            {
                this.model.SubQliphothCounter();
            }
        }

        public void AddQliphothCounter()
        {
            if (this.model.qliphothCounter >= this.GetQliphothCounterMax())
            {
                return;
            }
            if (this.model.IsEscaped())
            {
                return;
            }
            this.model.AddQliphothCounter();
        }
            
        public override void OnEnterRoom(UseSkill skill)
        {
            base.OnEnterRoom(skill);
            int agentLevel = skill.agent.level;

            skill.agent.RecoverMental((float)(0.5f * skill.agent.maxMental * (agentLevel * workedAgents.Count))*0.125f);

            if (skill.agent.HasEquipment(434281))
            {
                skill.agent.RecoverMental((float)(0.25f * skill.agent.maxMental));
            }
            this._animScript.StartWork();
        }

        public override void OnReleaseWork(UseSkill skill)
        {
            base.OnReleaseWork(skill);
            if (this.model.feelingState == CreatureFeelingState.GOOD)
            {
                if (!skill.agent.IsPanic() && !skill.agent.IsDead())
                {
                    if (!this.workedAgents.Contains(skill.agent.name))
                    {
                        this.workedAgents.Add(skill.agent.name);
                    }
                    if (this.goodCounter >= 3)
                    {
                        this.workedAgents.Remove(skill.agent.name);
                        skill.agent.Die();
                        this._animScript.Skill();
                        this.ActivateQliphothCounter();
                    }
                    else
                    {
                        this.goodCounter++;
                    }
                    this.ActiveSkill(skill.agent.GetCurrentSefira());
                }
            }
            else if (this.model.feelingState != CreatureFeelingState.GOOD)
            {
                if (this.model.feelingState == CreatureFeelingState.BAD)
                {
                    this.SubQliphothCounter();
                }
                this.goodCounter = 0;
            }
        }

        public override void Escape()
        {
            base.Escape();
            this._attacking = false;
            this._animScript.Escape();
            this.model.Escape();
            this._attackTimer.StartTimer(0.5f);
        }

        public override void UniqueEscape()
        {
            base.UniqueEscape();
            bool dead = this.model.hp <= 0f;

            if (!dead)
            {
                bool attacking = this._attackTimer.RunTimer();
                if (attacking)
                {
                    this._attacking = true;
                }

                if (this._attacking)
                {
                    this._animScript.Attack();
                    this.InvokeDamage(this.GetNearUnits());
                    this._attacking = false;
                    this._attackTimer.StartTimer(0.5f);
                }
                bool flag = this._currentDestNode != null && this._currentDestNode == this.movable.currentNode;
                if (flag)
                {
                    this._currentDestNode = null;
                }
                this.MakeMovement();
            }
            else 
            {
                this._animScript.Dead();
                return;
            }

        }

        private MapNode GetRandomNode(bool removeSefira = false) 
        {
            MapNode mapNode;

            if (removeSefira) 
            {
                int num = 10;
                do {
                    num--;
                    mapNode = MapGraph.instance.GetRoamingNodeByRandom(this.model.sefira.indexString);
                    bool flag = mapNode.GetAttachedPassage().type > PassageType.SEFIRA;
                    if (flag)
                    {
                        break;
                    }
                } while (num != 0);
            }
            else 
            {
                mapNode = MapGraph.instance.GetRoamingNodeByRandom();
            }

            return mapNode;
        }

        public void MakeMovement()
        {
            bool flag = this._currentDestNode == null;
            if (flag) 
            {
                this._currentDestNode = this.GetRandomNode(false);
            }
            bool flag2 = !this.movable.IsMoving();
            if (flag2)
                
            {
                this.movable.MoveToNode(this._currentDestNode);
            }
        }

        private void InvokeDamage(List<UnitModel> unit) 
        {
            foreach (UnitModel unitModel in unit) 
            {
                bool isWorker = unitModel is WorkerModel;
                DamageInfo damageInfo = ScribbledLife.damageInfo * (isWorker ? 1 : 2);
                unitModel.TakeDamage(this.model, damageInfo);
                DamageParticleEffect damageParticleEffect = DamageParticleEffect.Invoker(unitModel, ScribbledLife.damageInfo.type, this.model);
            }
        }

        private List<UnitModel> GetNearUnits() 
        { 
            List<UnitModel> units = new List<UnitModel>();
            bool flag = this.currentPassage == null;
            List<UnitModel> units2;
            if (flag)
            {
                units2 = units;
            }
            else 
            {
                try
                {
                    MovableObjectNode[] enteredTargets = this.currentPassage.GetEnteredTargets(this.movable);
                    foreach (MovableObjectNode movableObjectNode in enteredTargets)
                    {
                        UnitModel unit = movableObjectNode.GetUnit();
                        bool flag2 = !units.Contains(unit);
                        if (flag2) 
                        { 
                            bool flag3 = unit.IsAttackTargetable();
                            if (flag3) 
                            { 
                                bool flag4 = this.movable != movableObjectNode;
                                if (flag4) 
                                { 
                                    bool flag5 = this.movable.GetDistanceDouble(movableObjectNode) <= 5f;
                                    if (flag5) 
                                    { 
                                        units.Add(unit);
                                    }
                                }
                            }
                        }
                    }
                } 
                catch (Exception e) 
                {
                    Debug.LogError("An Error occured in QdI Creatures : ScribbledLife // Detail : " + e);
                }
                units2 = units;
            }
            return units2;
        }


        public void OnNotice(string notice, params object[] param)
        {
            // This method is empty but it's required to implement IObserver
            // Observe 할게 없기 때문에 역할 없이 return
            return;
        }
    }
}

using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Modules;
using DoAnTotNghiep.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.WebSockets;
using static QRCoder.PayloadGenerator;

namespace DoAnTotNghiep.Service
{
    public class RuleService : IRuleService
    {
        private DoAnTotNghiepContext _context;
        public RuleService(DoAnTotNghiepContext context)
        {
            this._context = context;
        }
        public bool SaveRule(Rules rule)
        {
            try
            {
                this._context.Rules.Add(rule);
                this._context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                FileSystem.WriteExceptionFile(ex.ToString(), "SaveRule_IdUser_" + rule.Content);
            }
            return false;
        }
        public bool UpdateRule(Rules rule)
        {
            try
            {
                this._context.Rules.Update(rule);
                this._context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                FileSystem.WriteExceptionFile(ex.ToString(), "UpdateRule_IdUser_" + rule.Content);
            }
            return false;
        }
        public List<Rules> All()
        {
            return this._context.Rules.ToList();
        }
        public void AddRuleForHouse(House house, List<int> currentRule)
        {
            List<RulesInHouse> rules = new List<RulesInHouse>();
            foreach (var item in currentRule)
            {
                RulesInHouse rulesInHouse = new RulesInHouse()
                {
                    IdRules = item,
                    IdHouse = house.Id,
                    Status = true
                };
                rules.Add(rulesInHouse);
            }
            this._context.RulesInHouses.AddRange(rules);
            this._context.SaveChanges();
        }
        public void UpdateRuleForHouse(House house, List<int> currentRule)
        {
            if (house.RulesInHouses == null)
            {
                this.AddRuleForHouse(house, currentRule);
            }
            else{
                List<RulesInHouse> ruleUpdateFalse = house.RulesInHouses
                                                            .Where(m => !currentRule.Contains(m.IdRules))
                                                            .ToList();
                foreach (var item in ruleUpdateFalse)
                {
                    item.Status = false;
                }
                this._context.RulesInHouses.UpdateRange(ruleUpdateFalse);

                List<RulesInHouse> ruleUpdateTrue = house.RulesInHouses
                                                        .Where(m => currentRule.Contains(m.IdRules))
                                                        .ToList();
                foreach (var item in ruleUpdateTrue)
                {
                    item.Status = true;
                }
                this._context.RulesInHouses.UpdateRange(ruleUpdateTrue);

                List<int> id = ruleUpdateTrue
                                    .Select(m => m.IdRules)
                                    .ToList();

                List<RulesInHouse> rules = new List<RulesInHouse>();
                foreach (var item in currentRule)
                {
                    if (!id.Contains(item))
                    {
                        RulesInHouse rulesInHouse = new RulesInHouse()
                        {
                            IdRules = item,
                            IdHouse = house.Id,
                            Status = true
                        };
                        rules.Add(rulesInHouse);
                    }
                }
                this._context.RulesInHouses.AddRange(rules);
                this._context.SaveChanges();
            }
        }
    }

    public interface IRuleService
    {
        public bool SaveRule(Rules rule);
        public bool UpdateRule(Rules rule);
        public List<Rules> All();
        public void AddRuleForHouse(House house, List<int> currentRule);
        public void UpdateRuleForHouse(House house, List<int> currentRule);
    }
}

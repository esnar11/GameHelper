using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GameHelper.Interfaces;

namespace GameHelper.Albion
{
    public class SkillRepository: IRepository<SkillInfo>
    {
        private static readonly SkillInfo[] EffectInfos = {
            new SkillInfo { Id = -1, Name = "Автоатака", IsNegative = true },
            new SkillInfo { Id = 1353, Name = "Ураган", IsNegative = true },
            new SkillInfo { Id = 1334, Name = "Смертельный раскол", IsNegative = true },
            new SkillInfo { Id = 1320, Name = "Разрывающий удар", IsNegative = true },
            new SkillInfo { Id = 1332, Name = "Потеря крови", IsNegative = true },
            new SkillInfo { Id = 1951, Name = "Электрическое поле", IsNegative = true },
            new SkillInfo { Id = 1699, Name = "Оборонительный удар", IsNegative = true },
            new SkillInfo { Id = 1714, Name = "Дальний прыжок", IsNegative = true },
            new SkillInfo { Id = 1720, Name = "Оборонительный удар", IsNegative = true },
            new SkillInfo { Id = 1727, Name = "Крушитель земли", IsNegative = true },
            new SkillInfo { Id = 1744, Name = "Проклятье уменьшения", IsNegative = true },
            new SkillInfo { Id = 1723, Name = "Сокрушающая угроза", IsNegative = true },
            new SkillInfo { Id = 4572, Name = "Взрыв бочки", IsNegative = true },
            new SkillInfo { Id = 2113, Name = "Омолаживающая пробежка", IsNegative = false }
        };

        private static readonly ICollection<SkillInfo> _unknowns = new List<SkillInfo>();

        public SkillInfo GetById(int skillId)
        {
            var info = EffectInfos.FirstOrDefault(e => e.Id == skillId);
            if (info != null)
                return info;

            info = _unknowns.FirstOrDefault(e => e.Id == skillId);
            if (info != null)
                return info;

            Debug.WriteLine($"---!!!--- Unknown skill {skillId} ---!!!---");
            var newSkillInfo = new SkillInfo
            {
                Id = skillId,
                Name = skillId.ToString()
            };
            _unknowns.Add(newSkillInfo);
            return newSkillInfo;
        }
    }
}

using System.Diagnostics;
using System.Linq;
using GameHelper.Interfaces;

namespace GameHelper.Albion
{
    internal class BuffRepository: IRepository<BuffInfo>
    {
        private static readonly BuffInfo[] Buffs = {
            new BuffInfo { Id = 3, Name = "Обычные удары" },

            new BuffInfo { Id = 467, IsInvisible = true },
            new BuffInfo { Id = 3072, IsInvisible = true },

            new BuffInfo { Id = 1721, Name = "Сопротивление увеличено" }, 
            new BuffInfo { Id = 1999, Name = "Аура бессилия" }, 
            new BuffInfo { Id = 2058, Name = "Каменная кожа" }, 
            new BuffInfo { Id = 2113, Name = "Омолаживающая пробежка" }, 
            new BuffInfo { Id = 3130, Name = "Оглушение", IsNegative = true }, 
            new BuffInfo { Id = 3118, Name = "Вы горите", IsNegative = true }, 
        };

        public BuffInfo GetById(int buffId)
        {
            var buffInfo = Buffs.FirstOrDefault(b => b.Id == buffId);
            if (buffInfo != null)
                return buffInfo;

            Debug.WriteLine($"Неизвестный баф {buffId}");
            return new BuffInfo
            {
                Name = "Баф " + buffId,
                Id = buffId
            };
        }
    }
}

using GameCreator.Runtime.Common;
using GameCreator.Runtime.Stats;
using GameCreator.Runtime.Variables;
using System.Linq;
using UnityEngine;

namespace Johnny.SimDungeon
{
    [Title("Find by Name")]
    [Category("Game Objects/Find by Name222")]
    public class GetLocalListTest : PropertyTypeGetGameObject
    {
        [SerializeField] protected CollectorListVariable m_LocalList = new CollectorListVariable();
        [SerializeField] private PropertyGetStat m_Stat = new PropertyGetStat();

        public override string String => $"[] Last Pick";

        public override GameObject Get(Args args)
        {
            Stat stat = this.m_Stat.Get(args);
            var source = m_LocalList.Get(args);

            GameObject best = null;
            var max = double.MinValue;
            foreach (var obj in source.OfType<GameObject>())
            {
                var traits = obj.Get<Traits>();
                if (traits == null) continue;

                var value = traits.RuntimeStats.Get(stat.ID).Value;
                if (value > max)
                {
                    max = value;
                    best = obj;
                }
            }
            return best;
        }
    }
}

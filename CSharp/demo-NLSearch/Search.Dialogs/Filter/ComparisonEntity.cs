using Microsoft.Bot.Builder.Luis.Models;

namespace Search.Dialogs.Filter
{
    class ComparisonEntity
    {
        public EntityRecommendation Entity;
        public EntityRecommendation Operator;
        public EntityRecommendation Lower;
        public EntityRecommendation Upper;
        public EntityRecommendation Property;

        public ComparisonEntity(EntityRecommendation comparison)
        {
            Entity = comparison;
        }

        public void AddEntity(EntityRecommendation entity)
        {
            if (entity.Type != "Comparison" && entity.StartIndex >= Entity.StartIndex && entity.EndIndex <= Entity.EndIndex)
            {
                switch (entity.Type)
                {
                    case "builtin.number": AddNumber(entity); break;
                    case "builtin.currency": AddNumber(entity); break;
                    case "Value": AddNumber(entity); break;
                    case "Dimension": AddNumber(entity); break;
                    case "Operators":
                        if (Operator == null || entity.Contains(Operator))
                        {
                            Operator = entity;
                        }
                        break;
                    case "Properties": Property = entity; break;
                }
            }
        }

        private void AddNumber(EntityRecommendation entity)
        {
            if (Lower == null)
            {
                Lower = entity;
            }
            else if (entity.Congruent(Lower))
            {
                if (entity.Type.StartsWith("builtin."))
                {
                    Lower = entity;
                }
            }
            else if (entity.Contains(Lower))
            {
                Lower = entity;
            }
            else if (Upper != null)
            {
                if (entity.Congruent(Upper))
                {
                    if (entity.Type.StartsWith("builtin."))
                    {
                        Upper = entity;
                    }
                }
                else if (entity.Contains(Upper))
                {
                    Upper = entity;
                }
            }
            else if (!Lower.Overlaps(entity))
            {
                Upper = entity;
            }
            if (Lower != null && Upper != null && Upper.StartIndex < Lower.StartIndex)
            {
                var lower = Lower;
                Lower = Upper;
                Upper = lower;
            }
        }
    }
}
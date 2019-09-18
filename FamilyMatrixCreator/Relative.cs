using System.Collections.Generic;

namespace FamilyMatrixCreator
{
    public class Relative
    {
        public int RelativeNumber { get; set; }
        public RelationshipDegree RelationshipDegree { get; set; }
        public List<Relative> ParentsList { get; set; }
        public List<Relative> DescendantsList { get; set; }

        public Relative(int relativeNumber, RelationshipDegree relationshipDegree, 
            List<Relative> parentsList, List<Relative> descendantsList)
        {
            RelativeNumber = relativeNumber;
            RelationshipDegree = relationshipDegree;
            ParentsList = parentsList;
            DescendantsList = descendantsList;
        }
    }
}

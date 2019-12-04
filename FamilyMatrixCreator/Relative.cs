using System.Collections.Generic;

namespace FamilyMatrixCreator
{
    public class Relative
    {
        public int RelativeNumber { get; set; }
        public RelationshipDegree RelationshipDegree { get; set; }
        public List<Relative> ParentsList { get; set; }
        public List<Relative> ChildsList { get; set; }

        public Relative(int relativeNumber, RelationshipDegree relationshipDegree, 
            List<Relative> parentsList, List<Relative> childsList)
        {
            RelativeNumber = relativeNumber;
            RelationshipDegree = relationshipDegree;
            ParentsList = parentsList;
            ChildsList = childsList;
        }
    }
}

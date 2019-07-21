using System.Collections.Generic;

namespace FamilyMatrixCreator
{
    class Relationship
    {
        public int RelationshipNumber { get; set; }
        public int CoordY { get; set; }
        public double CommonCm { get; set; }
        public bool IsAncestorOfProband { get; set; }
        public bool IsSiblindantOfProband { get; set; }
        public int RelationshipMaxCount { get; set; }
        public Dictionary<int, List<string>> PossibleRelationships { get; set; }

        public Relationship(int relationshipNumber, double commonCm, bool isAncestorOfProband, bool isSiblindantOfProband, int relationshipMaxCount)
        {
            RelationshipNumber = relationshipNumber;
            CommonCm = commonCm;
            IsAncestorOfProband = isAncestorOfProband;
            IsSiblindantOfProband = isSiblindantOfProband;
            RelationshipMaxCount = relationshipMaxCount;
        }
    }
}

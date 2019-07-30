using System.Collections.Generic;

namespace FamilyMatrixCreator
{
    public class Relationship
    {
        public int RelationshipNumber { get; set; }
        public int CoordY { get; set; }
        public float CommonCm { get; set; }
        public bool IsAncestorOfProband { get; set; }
        public bool IsSiblindantOfProband { get; set; }
        public int RelationshipMaxCount { get; set; }
        public Dictionary<int, List<string>> PossibleRelationships { get; set; }

        public Relationship(int relationshipNumber, float commonCm, int coordY, bool isAncestorOfProband, bool isSiblindantOfProband, int relationshipMaxCount)
        {
            RelationshipNumber = relationshipNumber;
            CommonCm = commonCm;
            CoordY = coordY;
            IsAncestorOfProband = isAncestorOfProband;
            IsSiblindantOfProband = isSiblindantOfProband;
            RelationshipMaxCount = relationshipMaxCount;
        }
    }
}

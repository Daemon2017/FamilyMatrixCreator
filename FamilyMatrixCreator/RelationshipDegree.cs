using System.Collections.Generic;

namespace FamilyMatrixCreator
{
    public class RelationshipDegree
    {
        public int RelationshipNumber { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public float CommonCm { get; set; }
        public bool IsAncestorOfProband { get; set; }
        public bool IsSiblindantOfProband { get; set; }
        public int RelationshipMaxCount { get; set; }
        public Dictionary<int, List<string>> PossibleRelationships { get; set; }

        public RelationshipDegree(int relationshipNumber,
            float commonCm,
            int coordX, int coordY,
            bool isAncestorOfProband, bool isSiblindantOfProband,
            int relationshipMaxCount)
        {
            RelationshipNumber = relationshipNumber;
            CommonCm = commonCm;
            X = coordX;
            Y = coordY;
            IsAncestorOfProband = isAncestorOfProband;
            IsSiblindantOfProband = isSiblindantOfProband;
            RelationshipMaxCount = relationshipMaxCount;
        }
    }
}

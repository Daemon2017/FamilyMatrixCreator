using System.Collections.Generic;

namespace FamilyMatrixCreator
{
    public class RelationshipDegree
    {
        public int RelationshipDegreeNumber { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public float CommonCm { get; set; }
        public bool IsAncestorOfProband { get; set; }
        public bool IsDescendantOfProband { get; set; }
        public int RelationshipMaxCount { get; set; }
        public Dictionary<int, List<string>> PossibleRelationships { get; set; }

        public RelationshipDegree(int relationshipDegreeNumber,
            float commonCm,
            int X, int Y,
            bool isAncestorOfProband, bool isSiblindantOfProband,
            int relationshipMaxCount)
        {
            RelationshipDegreeNumber = relationshipDegreeNumber;
            CommonCm = commonCm;
            this.X = X;
            this.Y = Y;
            IsAncestorOfProband = isAncestorOfProband;
            IsDescendantOfProband = isSiblindantOfProband;
            RelationshipMaxCount = relationshipMaxCount;
        }
    }
}

using System.Collections.Generic;

namespace FamilyMatrixCreator
{
    public class RelationshipDegree
    {
        public int RelationshipNumber { get; set; }
        public int CoordX { get; set; }
        public int CoordY { get; set; }
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
            CoordX = coordX;
            CoordY = coordY;
            IsAncestorOfProband = isAncestorOfProband;
            IsSiblindantOfProband = isSiblindantOfProband;
            RelationshipMaxCount = relationshipMaxCount;
        }
    }
}

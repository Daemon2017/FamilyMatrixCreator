using System.Collections.Generic;
using System.Linq;

namespace FamilyMatrixCreator
{
    public class Integrations
    {
        Modules modules = new Modules();

        /*
         * Исключение невозможных видов родства.
         */
        public int[] RemoveImpossibleRelationships(float[][] generatedOutputMatrix, List<int> persons, int person, List<int> relatives, int relative, int[] allPossibleRelationships, int[,][] relationshipsMatrix, int numberOfProband)
        {
            for (int previousPerson = 0; previousPerson < person; previousPerson++)
            {
                int numberOfI = 0,
                    numberOfJ = 0;

                /*
                 * Среди возможных видов родства пробанда ищутся порядковые номера тех, что содержат выбранные виды родства.
                 */
                for (int number = 0; number < relationshipsMatrix.GetLength(1); number++)
                {
                    if (relationshipsMatrix[numberOfProband, number][0] == generatedOutputMatrix[persons[previousPerson]][persons[person]])
                    {
                        numberOfI = number;
                    }

                    if (relationshipsMatrix[numberOfProband, number][0] == generatedOutputMatrix[persons[previousPerson]][relatives[relative]])
                    {
                        numberOfJ = number;
                    }
                }

                if (0 == persons[previousPerson])
                {
                    allPossibleRelationships = relationshipsMatrix[numberOfI, numberOfJ].Where(val => val != 1).ToArray();
                    int[] currentPossibleRelationships = Enumerable.Range(0, relationshipsMatrix.GetLength(1))
                                                                   .Select(j => relationshipsMatrix[numberOfProband, j][0]).ToArray();

                    allPossibleRelationships = modules.RemoveImpossibleRelations(allPossibleRelationships, currentPossibleRelationships);
                }
                else
                {
                    //int[] currentPossibleRelationships = relationshipsMatrix[numberOfI, numberOfJ].Where(val => val != 1).ToArray();

                    //allPossibleRelationships = modules.RemoveImpossibleRelations(allPossibleRelationships, currentPossibleRelationships);
                }
            }

            return allPossibleRelationships;
        }
    }
}

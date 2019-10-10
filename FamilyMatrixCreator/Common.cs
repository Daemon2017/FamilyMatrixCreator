using System;
using System.Collections.Generic;
using System.Linq;

namespace FamilyMatrixCreator
{
    public static partial class Form1
    {
        public static int GetYOfMRCA(int startX, int startY, int endX, int endY)
        {
            int yOfMRCA = 0;

            /*
             * Определение количества поколений до БОП.
             */
            if (startX > endX)
            {
                if (0 == endX)
                {
                    if (0 < startX && startX < endY)
                    {
                        yOfMRCA = endY;
                    }
                    else
                    {
                        yOfMRCA = startX;
                    }
                }
                else
                {
                    yOfMRCA = startX;
                }
            }
            else if (startX == endX)
            {
                yOfMRCA = startY >= endY ? startY : endY;
            }
            else if (startX < endX)
            {
                if (0 == startX)
                {
                    if (0 < endX && endX < startY)
                    {
                        yOfMRCA = startY;
                    }
                    else
                    {
                        yOfMRCA = endX;
                    }
                }
                else
                {
                    yOfMRCA = endX;
                }
            }

            return yOfMRCA;
        }

        public static RelationshipDegree GetRelationship(int distanceBetweenMrcaAndZeroPerson, int distanceBetweenMrcaAndFirstPerson,
    List<RelationshipDegree> relatives)
        {
            /*
             * Определение степени родства между парой персон по данным о расстоянии от каждого из них до БОП.
             */
            if (0 == distanceBetweenMrcaAndFirstPerson)
            {
                return relatives.Where(rel =>
                rel.X == 0 &&
                rel.Y == distanceBetweenMrcaAndZeroPerson).Single();
            }
            else
            {
                return relatives.Where(rel =>
                rel.X == distanceBetweenMrcaAndZeroPerson &&
                rel.Y == distanceBetweenMrcaAndZeroPerson - distanceBetweenMrcaAndFirstPerson).Single();
            }
        }

        public static List<RelationshipDegree> GetPossibleRelationshipsList(int yOfMrca,
            int numberOfGenerationsBetweenMrcaAndZeroRelative, int numberOfGenerationsBetweenMrcaAndFirstRelative,
            RelationshipDegree _zeroRelative, RelationshipDegree _firstRelative,
            List<RelationshipDegree> _relativesList)
        {
            /*
             * Определение основной степени родства.
             */
            List<RelationshipDegree> possibleRelationshipsList = new List<RelationshipDegree>
            {
                GetRelationship(
                    numberOfGenerationsBetweenMrcaAndZeroRelative,
                    numberOfGenerationsBetweenMrcaAndFirstRelative,
                    _relativesList)
            };

            /*
             * Определение дополнительных степеней родства, которые могут возникать от того, что 1-я и 2-я личности
             * находятся в одной вертикали.
             */
            if (_zeroRelative.X == _firstRelative.X &&
                !((_zeroRelative.X == 0 && _zeroRelative.Y >= 0) || (_firstRelative.X == 0 && _firstRelative.Y >= 0)))
            {
                int y0New = _zeroRelative.Y;
                int y1New = _firstRelative.Y;

                while (y0New < _zeroRelative.X && y1New < _firstRelative.X)
                {
                    try
                    {
                        yOfMrca = GetYOfMRCA(_zeroRelative.X, ++y0New, _firstRelative.X, ++y1New);
                        possibleRelationshipsList.Add(GetRelationship(
                            yOfMrca - _zeroRelative.Y,
                            yOfMrca - _firstRelative.Y,
                            _relativesList));
                    }
                    catch (InvalidOperationException)
                    {

                    }
                }
            }

            /*
             * Определение возможности отсутствия родства между 1-й и 2-й личностями. 
             */
            if (((_zeroRelative.X > 1) && (_firstRelative.X > 1)) ||
                ((_zeroRelative.Y > 0) && (_firstRelative.Y > 0)) ||
                ((_zeroRelative.Y > 0) && (_firstRelative.X > 1) || (_firstRelative.Y > 0) && (_zeroRelative.X > 1)))
            {
                possibleRelationshipsList.Add(_relativesList.Where(rel => rel.X == -1 && rel.Y == -1).Single());
            }

            return possibleRelationshipsList;
        }
    }
}

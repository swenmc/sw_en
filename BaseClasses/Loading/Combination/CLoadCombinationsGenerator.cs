using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseClasses
{
    [Serializable]
    public class CLoadCombinationsGenerator
    {
        private CLoadCaseGroup[] m_arrLoadCaseGroups;
        private List<CLoadCombination> combinations;

        List<List<CLoadCase>> combinationsLoadCases;
        List<List<CLoadCase>> permutationsLoadCases;
        
        public CLoadCombinationsGenerator(CLoadCaseGroup[] arrLoadCaseGroups)
        {
            m_arrLoadCaseGroups = arrLoadCaseGroups;
            combinations = new List<CLoadCombination>();
            combinationsLoadCases = new List<List<CLoadCase>>();
            permutationsLoadCases = new List<List<CLoadCase>>();

            //List<List<int>> combos = GetAllCombos(new int[] { 1, 2, 3, 4, 5, 6 }.ToList());
        }
        
        public void Generate()
        {
            combinationsLoadCases.Add(new List<CLoadCase>());

            foreach (CLoadCaseGroup loadCaseGr in m_arrLoadCaseGroups)
            {
                if (loadCaseGr.MType == ELCGType.eTogether)
                {
                    foreach (List<CLoadCase> list in combinationsLoadCases)
                    {
                        list.AddRange(loadCaseGr.MLoadCasesList);
                    }                    
                }
                else if (loadCaseGr.MType == ELCGType.eExclusive)
                {
                    List<List<CLoadCase>> newCombinationsLoadCases = new List<List<CLoadCase>>();

                    for (int i = 0; i < loadCaseGr.MLoadCasesList.Count; i++)
                    {
                        if (i == 0)  //first element add to all lists
                        {
                            foreach (List<CLoadCase> list in combinationsLoadCases)
                            {
                                list.Add(loadCaseGr.MLoadCasesList[i]);
                            }
                        }
                        else //for all other elements create new list and add list to all combinations
                        {
                            List<CLoadCase> newList = new List<CLoadCase>();                            
                            foreach (List<CLoadCase> list in combinationsLoadCases)
                            {
                                newList = list.GetRange(0, list.Count - 1);  //get copy without last element
                                newList.Add(loadCaseGr.MLoadCasesList[i]);   //put element to newList
                                newCombinationsLoadCases.Add(newList);
                            }
                        }                        
                    }
                    combinationsLoadCases.AddRange(newCombinationsLoadCases);
                }
            }

            //we have all full combinations, we need to make Permutations for each list from combinationsLoadCases
            //foreach (List<CLoadCase> l in combinationsLoadCases)
            //{
            //    permutationsLoadCases.AddRange(GetAllCombos(l));
            //}
        }

        public void WriteCombinations()
        {
            int count = 0;
            foreach (List<CLoadCase> l in combinationsLoadCases)
            {
                count++;
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0}. ", count);
                foreach (CLoadCase lc in l)
                {
                    sb.AppendFormat("{0},", lc.ID);
                }
                System.Diagnostics.Trace.WriteLine(sb.ToString());
            }
        }

        // Recursive
        public static List<List<T>> GetAllCombos<T>(List<T> list)
        {
            List<List<T>> result = new List<List<T>>();
            // head
            result.Add(new List<T>());
            result.Last().Add(list[0]);
            if (list.Count == 1)
                return result;
            // tail
            List<List<T>> tailCombos = GetAllCombos(list.Skip(1).ToList());
            tailCombos.ForEach(combo =>
            {
                result.Add(new List<T>(combo));
                combo.Add(list[0]);
                result.Add(new List<T>(combo));
            });
            return result;
        }

        // Iterative, using 'i' as bitmask to choose each combo members
        //public static List<List<T>> GetAllCombos<T>(List<T> list)
        //{
        //    int comboCount = (int)Math.Pow(2, list.Count) - 1;
        //    List<List<T>> result = new List<List<T>>();
        //    for (int i = 1; i < comboCount + 1; i++)
        //    {
        //        // make each combo here
        //        result.Add(new List<T>());
        //        for (int j = 0; j < list.Count; j++)
        //        {
        //            if ((i >> j) % 2 != 0)
        //                result.Last().Add(list[j]);
        //        }
        //    }
        //    return result;
        //}

        // Example usage
        

    }
}

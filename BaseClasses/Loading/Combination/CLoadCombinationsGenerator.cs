using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseClasses
{
    [Serializable]
    public class CLoadCombinationsGenerator
    {
        private List<CLoadCombination> MCombinations;

        private CLoadCaseGroup[] m_arrLoadCaseGroups;
        List<List<CLoadCase>> combinationsLoadCases;
        List<List<CLoadCase>> permutationsLoadCases;

        public List<CLoadCombination> Combinations
        {
            get { return MCombinations; }
            set { MCombinations = value; }
        }

        //temp for testing
        private List<CLoadCombination> MSameCombinations;
        //temp for testing
        public List<CLoadCombination> SameCombinations
        {
            get { return MSameCombinations; }
            set { MSameCombinations = value; }
        }

        public CLoadCombinationsGenerator(CLoadCaseGroup[] arrLoadCaseGroups)
        {
            m_arrLoadCaseGroups = arrLoadCaseGroups;
            MCombinations = new List<CLoadCombination>();
            MSameCombinations = new List<CLoadCombination>();
            combinationsLoadCases = new List<List<CLoadCase>>();
            permutationsLoadCases = new List<List<CLoadCase>>();

            //List<List<int>> combos = GetAllCombos(new int[] { 1, 2, 3, 4, 5, 6 }.ToList());
        }

        public void GenerateAll()
        {
            combinationsLoadCases = new List<List<CLoadCase>>();
            permutationsLoadCases = new List<List<CLoadCase>>();

            List<CLoadCaseGroup> loadCaseGroups = new List<CLoadCaseGroup>();

            // Types of load cases (G - permanent, Q - imposed, W - wind, S - snow, EQ - earthquake)
            // ULS
            // 1 - [0.9 * m_arrLoadCaseGroups[0]][0.9 G]
            SetFactorForLoadCases(m_arrLoadCaseGroups[0].MLoadCasesList, 0.9f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[0]);
            Generate(loadCaseGroups, "[0.9 G]", ELSType.eLS_ULS);
            // 2 - [1.2 * m_arrLoadCaseGroups[0], 1.5 * m_arrLoadCaseGroups[1]][1.2G, 1.5Q]
            loadCaseGroups.Clear();
            SetFactorForLoadCases(m_arrLoadCaseGroups[0].MLoadCasesList, 1.2f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[0]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[1].MLoadCasesList, 1.5f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[1]);
            Generate(loadCaseGroups, "[1.2G, 1.5Q]", ELSType.eLS_ULS);
            // 3 - [0.9 * m_arrLoadCaseGroups[0], 1.0 * m_arrLoadCaseGroups[3], 1.0 * m_arrLoadCaseGroups[4], ψc * m_arrLoadCaseGroups[1]] [0.9G, Wu_i, Wu_e_min, ψc Q]
            loadCaseGroups.Clear();
            SetFactorForLoadCases(m_arrLoadCaseGroups[0].MLoadCasesList, 0.9f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[0]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[3].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[3]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[4].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[4]);
            Generate(loadCaseGroups, "[0.9G, Wu_i, Wu_e_min, ψc Q]", ELSType.eLS_ULS);
            // 4 - [0.9 * m_arrLoadCaseGroups[0], 1.0 * m_arrLoadCaseGroups[3], 1.0 * m_arrLoadCaseGroups[5], ψc * m_arrLoadCaseGroups[1]] [0.9G, Wu_i, Wu_e_max, ψc Q] ψc = 0
            loadCaseGroups.Clear();
            SetFactorForLoadCases(m_arrLoadCaseGroups[0].MLoadCasesList, 0.9f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[0]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[3].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[3]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[5].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[5]);
            Generate(loadCaseGroups, "[0.9G, Wu_i, Wu_e_max, ψc Q] ψc = 0", ELSType.eLS_ULS);
            // 5 - [1.2 * m_arrLoadCaseGroups[0], 1.0 * m_arrLoadCaseGroups[3], 1.0 * m_arrLoadCaseGroups[4], ψc * m_arrLoadCaseGroups[1]] [1.2G, Wu_i, Wu_e_min, ψc Q]
            loadCaseGroups.Clear();
            SetFactorForLoadCases(m_arrLoadCaseGroups[0].MLoadCasesList, 1.2f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[0]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[3].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[3]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[4].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[4]);
            Generate(loadCaseGroups, "[1.2G, Wu_i, Wu_e_min, ψc Q]", ELSType.eLS_ULS);
            // 6 - [1.2 * m_arrLoadCaseGroups[0], 1.0 * m_arrLoadCaseGroups[3], 1.0 * m_arrLoadCaseGroups[5], ψc * m_arrLoadCaseGroups[1]] [1.2G, Wu_i, Wu_e_max, ψc Q]
            loadCaseGroups.Clear();
            SetFactorForLoadCases(m_arrLoadCaseGroups[0].MLoadCasesList, 1.2f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[0]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[3].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[3]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[5].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[5]);
            Generate(loadCaseGroups, "[1.2G, Wu_i, Wu_e_max, ψc Q]", ELSType.eLS_ULS);
            // 7 - [1.0 * m_arrLoadCaseGroups[0], 1.0 * m_arrLoadCaseGroups[6], ψE * m_arrLoadCaseGroups[1]][G, Eu, ψE Q] ψE = 0;
            loadCaseGroups.Clear();
            SetFactorForLoadCases(m_arrLoadCaseGroups[0].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[0]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[6].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[6]);
            Generate(loadCaseGroups, "[G, Eu, ψE Q] ψE = 0;", ELSType.eLS_ULS);
            // 8 - [1.20 * m_arrLoadCaseGroups[0], 1.0 * m_arrLoadCaseGroups[2], ψc * m_arrLoadCaseGroups[1]][1.2G, Su, ψc Q]
            loadCaseGroups.Clear();
            SetFactorForLoadCases(m_arrLoadCaseGroups[0].MLoadCasesList, 1.2f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[0]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[2].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[2]);
            Generate(loadCaseGroups, "[1.2G, Su, ψc Q]", ELSType.eLS_ULS);
            // 9 - [1.35 * m_arrLoadCaseGroups[0]][1.35 G]
            loadCaseGroups.Clear();
            SetFactorForLoadCases(m_arrLoadCaseGroups[0].MLoadCasesList, 1.35f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[0]);
            Generate(loadCaseGroups, "[1.35 G]", ELSType.eLS_ULS);
            //10 - [0.9 * m_arrLoadCaseGroups[0], 1.0 * m_arrLoadCaseGroups[3], 1.0 * m_arrLoadCaseGroups[4]][0.9G, Wu_i, Wu_e_min]
            loadCaseGroups.Clear();
            SetFactorForLoadCases(m_arrLoadCaseGroups[0].MLoadCasesList, 0.9f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[0]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[3].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[3]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[4].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[4]);
            Generate(loadCaseGroups, "[0.9G, Wu_i, Wu_e_min]", ELSType.eLS_ULS);
            //11 - [0.9 * m_arrLoadCaseGroups[0], 1.0 * m_arrLoadCaseGroups[3], 1.0 * m_arrLoadCaseGroups[5]][0.9G, Wu_i, Wu_e_max]
            loadCaseGroups.Clear();
            SetFactorForLoadCases(m_arrLoadCaseGroups[0].MLoadCasesList, 0.9f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[0]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[3].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[3]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[5].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[5]);
            Generate(loadCaseGroups, "[0.9G, Wu_i, Wu_e_max]", ELSType.eLS_ULS);

            // SLS
            // 1 - [1.0 * m_arrLoadCaseGroups[0]][1.0 G]
            loadCaseGroups.Clear();
            SetFactorForLoadCases(m_arrLoadCaseGroups[0].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[0]);
            Generate(loadCaseGroups, "[1.0 G]", ELSType.eLS_SLS);
            // 2 - [1.0 * m_arrLoadCaseGroups[0] x ψs * m_arrLoadCaseGroups[1]][G, ψs Q] ψs = 0.7;
            loadCaseGroups.Clear();
            SetFactorForLoadCases(m_arrLoadCaseGroups[0].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[0]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[1].MLoadCasesList, 0.7f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[1]);
            Generate(loadCaseGroups, "[G, ψs Q] ψs = 0.7;", ELSType.eLS_SLS);
            // 3 - [1.0 * m_arrLoadCaseGroups[0], 1.0 * m_arrLoadCaseGroups[8], 1.0 * m_arrLoadCaseGroups[9], ψs * m_arrLoadCaseGroups[1]][G, Ws_i, Ws_e_min, Q] ψs = 0.7;
            loadCaseGroups.Clear();
            SetFactorForLoadCases(m_arrLoadCaseGroups[0].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[0]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[8].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[8]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[9].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[9]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[1].MLoadCasesList, 0.7f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[1]);
            Generate(loadCaseGroups, "[G, Ws_i, Ws_e_min, Q] ψs = 0.7;", ELSType.eLS_SLS);
            // 4 - [1.0 * m_arrLoadCaseGroups[0], 1.0 * m_arrLoadCaseGroups[8], 1.0 * m_arrLoadCaseGroups[10], ψs * m_arrLoadCaseGroups[1]][G, Ws_i, Ws_e_max, Q] ψs = 0.7;
            loadCaseGroups.Clear();
            SetFactorForLoadCases(m_arrLoadCaseGroups[0].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[0]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[8].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[8]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[10].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[10]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[1].MLoadCasesList, 0.7f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[1]);
            Generate(loadCaseGroups, "[G, Ws_i, Ws_e_max, Q] ψs = 0.7;", ELSType.eLS_SLS);
            // 5 - [1.0 * m_arrLoadCaseGroups[0], 1.0 * m_arrLoadCaseGroups[8], 1.0 * m_arrLoadCaseGroups[9]][G, Ws_i, Ws_e_min]
            loadCaseGroups.Clear();
            SetFactorForLoadCases(m_arrLoadCaseGroups[0].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[0]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[8].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[8]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[9].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[9]);
            Generate(loadCaseGroups, "[G, Ws_i, Ws_e_min]", ELSType.eLS_SLS);
            // 6 - [1.0 * m_arrLoadCaseGroups[0], 1.0 * m_arrLoadCaseGroups[8], 1.0 * m_arrLoadCaseGroups[10]][G, Ws_i, Ws_e_max]
            loadCaseGroups.Clear();
            SetFactorForLoadCases(m_arrLoadCaseGroups[0].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[0]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[8].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[8]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[10].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[10]);
            Generate(loadCaseGroups, "[G, Ws_i, Ws_e_max]", ELSType.eLS_SLS);
            // 7 - [1.0 * m_arrLoadCaseGroups[0], 1.0 * m_arrLoadCaseGroups[7], ψs * m_arrLoadCaseGroups[1]][G, Ss, ψs Q] ψs = 0.7;
            loadCaseGroups.Clear();
            SetFactorForLoadCases(m_arrLoadCaseGroups[0].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[0]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[7].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[7]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[1].MLoadCasesList, 0.7f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[1]);
            Generate(loadCaseGroups, "[G, Ss, ψs Q] ψs = 0.7;", ELSType.eLS_SLS);
            // 8 - [1.0 * m_arrLoadCaseGroups[0], 1.0 * m_arrLoadCaseGroups[11], ψE * m_arrLoadCaseGroups[1]][G, Eu, ψE Q] ψE = 0;
            loadCaseGroups.Clear();
            SetFactorForLoadCases(m_arrLoadCaseGroups[0].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[0]);
            SetFactorForLoadCases(m_arrLoadCaseGroups[11].MLoadCasesList, 1.0f);
            loadCaseGroups.Add(m_arrLoadCaseGroups[11]);
            Generate(loadCaseGroups, "[G, Eu, ψE Q] ψE = 0;", ELSType.eLS_SLS);


            FilterCombinationsByWindDirection();
            SetIDandNameForCombinations();
            //CreateCombinations();
        }

        private void CreateCombinations()
        {
            Combinations.Clear();
            CLoadCombination combination;

            int i = 1; // Combination ID

            foreach (List<CLoadCase> l in permutationsLoadCases)
            {
                combination = new CLoadCombination();
                List<float> factors = new List<float>();
                foreach (CLoadCase lc in l)
                {
                    factors.Add(lc.Factor);
                }
                combination.LoadCasesFactorsList = factors;
                combination.LoadCasesList = l;
                combination.ID = i;
                combination.Name = "CO " + i.ToString();

                if (!Combinations.Contains(combination))
                {
                    Combinations.Add(combination);
                    i++; // Increase ID for next combination
                }
                else SameCombinations.Add(combination);
            }
        }
        private void AddNewCombinations(string combinationKey, ELSType lsType)
        {
            CLoadCombination combination;

            foreach (List<CLoadCase> l in combinationsLoadCases)
            {
                combination = new CLoadCombination();
                List<float> factors = new List<float>();
                foreach (CLoadCase lc in l)
                {
                    factors.Add(lc.Factor);
                }
                combination.LoadCasesFactorsList = factors;
                combination.LoadCasesList = l;
                combination.CombinationKey = combinationKey;
                combination.eLComType = lsType;

                if (!Combinations.Contains(combination))
                {
                    Combinations.Add(combination);                    
                }
                else SameCombinations.Add(combination);
            }
        }

        private void FilterCombinationsByWindDirection()
        {
            List<CLoadCombination> combinationsToRemove = new List<CLoadCombination>();
            foreach (CLoadCombination combination in Combinations)
            {
                if (HasSameWindDirection(combination.LoadCasesList)) combinationsToRemove.Add(combination);
            }
            foreach (CLoadCombination combToRemove in combinationsToRemove)
            {
                Combinations.Remove(combToRemove);
            }
        }

        private bool HasSameWindDirection(List<CLoadCase> loadcases)
        {
            ELCMainDirection foundWindDirection = ELCMainDirection.eUndefined;
            foreach (CLoadCase lc in loadcases)
            {
                if (lc.Type == ELCType.eWind)
                {
                    if (foundWindDirection == ELCMainDirection.eUndefined) foundWindDirection = lc.MainDirection;
                    else
                    {
                        //we found different wind directions
                        if (foundWindDirection != lc.MainDirection) return false;
                    }
                } 
            }

            return true;
        }

        private void SetIDandNameForCombinations()
        {
            for(int i = 0; i < Combinations.Count; i++)
            {
                Combinations[i].ID = i + 1;
                Combinations[i].Name = string.Format("CO {0}", (i+1));                
            }
        }

        private void SetFactorForLoadCases(List<CLoadCase> loadCases, float factor)
        {
            foreach (CLoadCase loadCase in loadCases)
            {
                loadCase.Factor = factor;
            }
        }

        private void Generate(List<CLoadCaseGroup> loadCaseGroups, string combinationKey, ELSType lsType)
        {
            combinationsLoadCases.Clear();
            combinationsLoadCases.Add(new List<CLoadCase>());

            foreach (CLoadCaseGroup loadCaseGr in loadCaseGroups)
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

            AddNewCombinations(combinationKey, lsType);
            //we have all full combinations, we need to make Permutations for each list from combinationsLoadCases
            //foreach (List<CLoadCase> l in combinationsLoadCases)
            //{
            //    permutationsLoadCases.AddRange(GetAllCombos(l));
            //}
        }

        public void GenerateULS()
        {
            combinationsLoadCases.Add(new List<CLoadCase>());

            foreach (CLoadCaseGroup loadCaseGr in m_arrLoadCaseGroups)
            {
                if (loadCaseGr.MType_LS == ELCGTypeForLimitState.eULSOnly || loadCaseGr.MType_LS == ELCGTypeForLimitState.eUniversal)
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
            }

            //we have all full combinations, we need to make Permutations for each list from combinationsLoadCases
            foreach (List<CLoadCase> l in combinationsLoadCases)
            {
                permutationsLoadCases.AddRange(GetAllCombos(l));
            }
        }

        public void GenerateSLS()
        {
            combinationsLoadCases.Add(new List<CLoadCase>());

            foreach (CLoadCaseGroup loadCaseGr in m_arrLoadCaseGroups)
            {
                if (loadCaseGr.MType_LS == ELCGTypeForLimitState.eSLSOnly || loadCaseGr.MType_LS == ELCGTypeForLimitState.eUniversal)
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

            }

            //we have all full combinations, we need to make Permutations for each list from combinationsLoadCases
            foreach (List<CLoadCase> l in combinationsLoadCases)
            {
                permutationsLoadCases.AddRange(GetAllCombos(l));
            }
        }

        public void WriteCombinations()
        {
            int count = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("\nCombinations: \n\n");
            foreach (CLoadCombination lc in MCombinations)
            {
                count++;
                sb.AppendFormat("{0}. ", count);
                for (int i = 0; i < lc.LoadCasesList.Count; i++)
                {
                    sb.AppendFormat("[ID:{0} Factor:{1}], ", lc.LoadCasesList[i].ID, lc.LoadCasesFactorsList[i]);
                }
                sb.Append(Environment.NewLine);
            }
            System.Diagnostics.Trace.WriteLine(sb.ToString());
        }
        public void WriteCombinationsLoadCases()
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

        public void WritePermutations()
        {
            int count = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("\nPermutations:\n\n");
            foreach (List<CLoadCase> l in permutationsLoadCases)
            {
                count++;
                sb.AppendFormat("{0}. ", count);
                foreach (CLoadCase lc in l)
                {
                    sb.AppendFormat("{0},", lc.ID);
                }
                sb.Append(Environment.NewLine);
            }
            System.Diagnostics.Trace.WriteLine(sb.ToString());
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
    }
}

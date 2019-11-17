//else
//                        {
//                            //Переход в текущую область происходит если после скобки есть звездочка (*) Данный кусок практически не отличается
//                            //от части где нет *, за исключением одного добавленного куска кода в конце.
//                            Dictionary<string, int> pairs = new Dictionary<string, int>();
//var splitByVB = SplitByVerticalBar(splitByBreckets[i]);//делим содержимое текущей скобки на части между  |
//                            for (int k = 0; k<splitByBreckets.Count; k++)//цикл проходит по содержимому скобок
//                            {
//                                while (tempStates.Count != 0)
//                                {
//                                    var ts = tempStates.Dequeue();
//curStateStorage.Add(ts);
//                                    System.Console.WriteLine("Достали из очереди в лист {0}", ts._stateID);
//                                    counter++;
//                                }
//                                foreach (var state2 in curStateStorage)
//                                {
//                                    if (splitByBreckets[k] != "*")//проверяем не явл ли текущая часть *
//                                    {
//                                        curStartState = state._stateID;
//                                        if (!iterationsPos.Contains(k + 1))//проверяем не явл след часть *
//                                        {
//                                            var splitByVBar = SplitByVerticalBar(splitByBreckets[k]);//делим содержимое текущей скобки на части между  |
//                                            for (int j = 0; j<splitByVB.Count; j++)//цикл проходит по частям между | и делит их на управляющие символы
//                                            {
//                                                curStartState = state._stateID;
//                                                System.Console.WriteLine("Для {0} CSS = {1}", splitByVBar[j], curStartState);
//                                                var controlSybols = GetControlCharsFromCurPart(splitByVBar[j]);//получены символы для текущей части до |
//int count = controlSybols.Count;
//                                                for (int x = 0; x<count; x++)//цикл проходит по текущим управляющим символам и добавляет в матрицу переходов соотв данные
//                                                {
//                                                    var curSymbol = controlSybols[x];
//                                                    if (k == splitByBreckets.Count - 1)
//                                                    {
//                                                        if (x<count - 1)
//                                                        {
//                                                            var newState = new State(stateCounter, 1, $"S{stateCounter}");
//states.Add(newState);
//                                                            stateCounter++;
//                                                            curFinsihState = newState._stateID;
//                                                            scount++;
//                                                        }
//                                                        else
//                                                        {
//                                                            var newState = new State(stateCounter, 2, $"S{stateCounter}");
//states.Add(newState);
//                                                            stateCounter++;
//                                                            curFinsihState = newState._stateID;
//                                                            scount++;
//                                                        }
//                                                    }
//                                                    else
//                                                    {
//                                                        var newState = new State(stateCounter, 1, $"S{stateCounter}");
//states.Add(newState);
//                                                        stateCounter++;
//                                                        curFinsihState = newState._stateID;
//                                                        scount++;
//                                                    }
//                                                    var curSignals = _specialSymbols[$"\\{curSymbol}"];
//                                                    foreach (var item in curSignals)
//                                                    {
//                                                        int curCharID = GetIdByChar(item, sigma);
//delta[curStartState, curCharID] = curFinsihState;
//                                                    }
//                                                    pairs.Add(curSymbol.ToString(), curFinsihState);
//                                                    var tmpState = GetStateByID(states, curFinsihState);
//tempStates.Enqueue(tmpState);
//                                                    System.Console.WriteLine("помещаем {0}", tmpState);
//                                                    System.Console.WriteLine("для {0} CFS = {1}", curSymbol, curFinsihState);
//                                                    curFinsihState++;
//                                                    foreach (var c in pairs)// в двойном цикле проходим по элементам словаря и добавляем переходы между состояними по их управляющим символам.
//                                                    {                       // проще говоря соединяем между собой состояния соотв сигналами , а также добавляем "петли".
//                                                        foreach (var z in pairs)
//                                                        {
//                                                            var curentSignals = _specialSymbols[$"\\{z.Key}"];
//                                                            foreach (var item in curSignals)
//                                                            {
//                                                                int curCharID = GetIdByChar(item, sigma);
//delta[c.Value, curCharID] = z.Value;
//                                                            }
//                                                        }
//                                                    }
//                                                }
//                                            }
//                                        }
//                                    }
//                                }
//                            }
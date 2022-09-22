using System.Collections.Generic;

using UnityEngine;

namespace GameDevTV.Utils
{
    [System.Serializable]
    public class Condition
    {
        [SerializeField]
        Disjunction[] and;

        public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
        {
            if(and == null || and.Length == 0)
                return true;

            foreach(Disjunction disjunction in and)
            {
                if(!disjunction.Check(evaluators))
                    return false;
            }

            return true;
        }

        [System.Serializable]
        class Disjunction
        {
            [SerializeField]
            Predicate[] or;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach(Predicate predicate in or)
                {
                    if(predicate.Check(evaluators))
                        return true;
                }

                return false;
            }
        }

        [System.Serializable]
        class Predicate
        {
            [SerializeField]
            private string _predicate;
            [SerializeField]
            private string[] _parameters;
            [SerializeField]
            private bool _negate = false;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach (IPredicateEvaluator evaluator in evaluators)
                {
                    bool? result = evaluator.Evaluate(_predicate, _parameters);
                    if(result == null)
                        continue;
                    
                    if(result == _negate)
                        return false;
                }

                return true;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReGoap.Unity.FSM;

namespace WorldBuilder.AI.FSM
{
    public class StateIdle : SmState
    {
        public override string ToString()
        {
            return "GoapState('Idle')";
        }
    }
}
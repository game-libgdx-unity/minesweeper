using System;
using System.Collections;
using System.Collections.Generic;

public interface IGameSolver
{
    IEnumerator Solve(float waitForNextStep);
}
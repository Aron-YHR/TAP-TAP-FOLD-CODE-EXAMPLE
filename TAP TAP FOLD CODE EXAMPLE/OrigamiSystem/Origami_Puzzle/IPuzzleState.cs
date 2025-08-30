using UnityEngine;

public interface IPuzzleState
{
    public void StartPuzzle();

    public void OnPuzzleAction();

    public void CompletePuzzle(); // DETECT right or wrong

    public void ExitPuzzle();
}

using UnityEngine;

public abstract class BasePlotAction : MonoBehaviour, IPlotAction
{
    protected bool actionCompleted = false;

    public bool isEnd() => actionCompleted;

    public abstract void Action();

    protected virtual void CompleteAction()
    {
        actionCompleted = true;
    }

    // Сброс состояния для переиспользования
    public virtual void ResetAction()
    {
        actionCompleted = false;
    }
}
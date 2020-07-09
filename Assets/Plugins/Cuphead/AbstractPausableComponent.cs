// AbstractPausableComponent
using System;
using System.Collections;

public class AbstractPausableComponent : AbstractMonoBehaviour
{
    [NonSerialized]
    public bool preEnabled;

    protected override void Awake()
    {
        base.Awake();
        PauseManager.AddChild(this);
        this.preEnabled = base.enabled;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        PauseManager.RemoveChild(this);
    }

    public virtual void OnPause()
    {
    }

    public virtual void OnUnpause()
    {
    }

    protected IEnumerator WaitForPause_CR()
    {
        while (PauseManager.state == PauseManager.State.Paused)
        {
            yield return null;
        }
    }

    public virtual void OnLevelEnd()
    {
        if (this != null)
        {
            this.StopAllCoroutines();
            base.enabled = false;
        }
    }
}

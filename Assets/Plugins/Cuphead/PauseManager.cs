using System.Collections.Generic;
using UnityEngine;

public static class PauseManager
{
    public enum State
    {
        Unpaused,
        Paused
    }

    public static State state;

	public static bool Paused => state == State.Paused;


    private static float oldSpeed;
    private static float oldTimeScale;

    private static List<AbstractPausableComponent> children = new List<AbstractPausableComponent>();

    public static void Reset()
    {
        PauseManager.state = State.Unpaused;
    }

    public static void AddChild(AbstractPausableComponent child)
    {
        PauseManager.children.Add(child);
    }

    public static void RemoveChild(AbstractPausableComponent child)
    {
        PauseManager.children.Remove(child);
    }

    public static void Pause()
    {
        if (PauseManager.state != State.Paused)
        {
            PauseManager.state = State.Paused;
			//AudioListener.pause = true;

			PauseManager.oldSpeed = CupheadTime.GlobalSpeed;
			CupheadTime.GlobalSpeed = 0f;
			foreach ( AbstractPausableComponent child in PauseManager.children ) {
				child.OnPause ();
			}

			PauseManager.SetChildren ( false );
			PauseManager.oldTimeScale = Time.timeScale;
			Time.timeScale = 0f;
		}
    }

    public static void Unpause()
    {
        if (PauseManager.state != 0)
        {
            PauseManager.state = State.Unpaused;
            AudioListener.pause = false;

            CupheadTime.GlobalSpeed = PauseManager.oldSpeed;
			foreach ( AbstractPausableComponent child in PauseManager.children ) {
				child.OnUnpause ();
			}

			PauseManager.SetChildren(true);

			Time.timeScale = PauseManager.oldTimeScale;
		}
    }

    public static void Toggle()
    {
        if (PauseManager.state == State.Paused)
        {
            PauseManager.Unpause();
        }
        else
        {
            PauseManager.Pause();
        }
    }

    private static void SetChildren(bool enabled)
    {
        for (int i = 0; i < PauseManager.children.Count; i++)
        {
            AbstractPausableComponent abstractPausableComponent = PauseManager.children[i];
            if (abstractPausableComponent == null)
            {
                PauseManager.children.Remove(abstractPausableComponent);
                i--;
            }
            else if (enabled)
            {
                abstractPausableComponent.enabled = abstractPausableComponent.preEnabled;
            }
            else
            {
                abstractPausableComponent.preEnabled = abstractPausableComponent.enabled;
                abstractPausableComponent.enabled = false;
            }
        }
    }
}

using System;
using UnityEngine;


public static class ComponentExtensions {
	public static T GetOrAddComponent< T > ( this Component c ) where T : Component {
		T t = c.GetComponent< T > ();
		if ( !t ) {
			t = c.gameObject.AddComponent< T > ();
		}

		return t;
	}

	public static T GetOrAddComponent< T > ( this GameObject c ) where T : Component {
		T t = c.GetComponent< T > ();
		if ( !t ) {
			t = c.AddComponent< T > ();
		}

		return t;
	}

	public static Component GetOrAddComponent ( this GameObject c, Type t ) {
		Component component = c.GetComponent ( t );
		if ( !component ) {
			component = c.AddComponent ( t );
		}

		return component;
	}

	public static Component GetOrAddComponent ( this Component c, Type t ) {
		return c.GetComponent ( t ) ?? c.gameObject.AddComponent ( t );
	}

	public static bool IsPrefab ( this Component c ) {
		return c.gameObject.scene.name == null;
	}

	public static bool IsPrefab ( this GameObject c ) {
		return c.scene.name == null;
	}

	public static ValueTuple< T1, T2 > GetComponents< T1, T2 > ( this Component c )
		where T1 : Component where T2 : Component {
		return new ValueTuple< T1, T2 > ( c.GetComponent< T1 > (), c.GetComponent< T2 > () );
	}

	public static ValueTuple< T1, T2, T3 > GetComponents< T1, T2, T3 > ( this Component c )
		where T1 : Component where T2 : Component where T3 : Component {
		return new ValueTuple< T1, T2, T3 > ( c.GetComponent< T1 > (), c.GetComponent< T2 > (),
			c.GetComponent< T3 > () );
	}

	public static ValueTuple< T1, T2, T3, T4 > GetComponents< T1, T2, T3, T4 > ( this Component c ) where T1 : Component
		where T2 : Component
		where T3 : Component
		where T4 : Component {
		return new ValueTuple< T1, T2, T3, T4 > ( c.GetComponent< T1 > (), c.GetComponent< T2 > (),
			c.GetComponent< T3 > (), c.GetComponent< T4 > () );
	}

	public static ValueTuple< T1, T2 > GetOrAddComponents< T1, T2 > ( this Component c )
		where T1 : Component where T2 : Component {
		return new ValueTuple< T1, T2 > ( c.GetOrAddComponent< T1 > (), c.GetOrAddComponent< T2 > () );
	}

	public static ValueTuple< T1, T2, T3 > GetOrAddComponents< T1, T2, T3 > ( this Component c )
		where T1 : Component where T2 : Component where T3 : Component {
		return new ValueTuple< T1, T2, T3 > ( c.GetOrAddComponent< T1 > (), c.GetOrAddComponent< T2 > (),
			c.GetOrAddComponent< T3 > () );
	}

	public static ValueTuple< T1, T2, T3, T4 > GetOrAddComponents< T1, T2, T3, T4 > ( this Component c )
		where T1 : Component where T2 : Component where T3 : Component where T4 : Component {
		return new ValueTuple< T1, T2, T3, T4 > ( c.GetOrAddComponent< T1 > (), c.GetOrAddComponent< T2 > (),
			c.GetOrAddComponent< T3 > (), c.GetOrAddComponent< T4 > () );
	}

	public static ValueTuple< T1, T2 > GetComponents< T1, T2 > ( this GameObject c )
		where T1 : Component where T2 : Component {
		return new ValueTuple< T1, T2 > ( c.GetComponent< T1 > (), c.GetComponent< T2 > () );
	}

	public static ValueTuple< T1, T2, T3 > GetComponents< T1, T2, T3 > ( this GameObject c )
		where T1 : Component where T2 : Component where T3 : Component {
		return new ValueTuple< T1, T2, T3 > ( c.GetComponent< T1 > (), c.GetComponent< T2 > (),
			c.GetComponent< T3 > () );
	}

	public static ValueTuple< T1, T2, T3, T4 > GetComponents< T1, T2, T3, T4 > ( this GameObject c )
		where T1 : Component where T2 : Component where T3 : Component where T4 : Component {
		return new ValueTuple< T1, T2, T3, T4 > ( c.GetComponent< T1 > (), c.GetComponent< T2 > (),
			c.GetComponent< T3 > (), c.GetComponent< T4 > () );
	}

	public static ValueTuple< T1, T2 > GetOrAddComponents< T1, T2 > ( this GameObject c )
		where T1 : Component where T2 : Component {
		return new ValueTuple< T1, T2 > ( c.GetOrAddComponent< T1 > (), c.GetOrAddComponent< T2 > () );
	}

	public static ValueTuple< T1, T2, T3 > GetOrAddComponents< T1, T2, T3 > ( this GameObject c )
		where T1 : Component where T2 : Component where T3 : Component {
		return new ValueTuple< T1, T2, T3 > ( c.GetOrAddComponent< T1 > (), c.GetOrAddComponent< T2 > (),
			c.GetOrAddComponent< T3 > () );
	}

	public static ValueTuple< T1, T2, T3, T4 > GetOrAddComponents< T1, T2, T3, T4 > ( this GameObject c )
		where T1 : Component where T2 : Component where T3 : Component where T4 : Component {
		return new ValueTuple< T1, T2, T3, T4 > ( c.GetOrAddComponent< T1 > (), c.GetOrAddComponent< T2 > (),
			c.GetOrAddComponent< T3 > (), c.GetOrAddComponent< T4 > () );
	}

	public static bool DestroyComponent< T > ( this Component c ) where T : Component {
		T component = c.GetComponent< T > ();
		if ( !component ) {
			return false;
		}

		UnityEngine.Object.Destroy ( component );
		return true;
	}

	// Token: 0x06000153 RID: 339 RVA: 0x00006453 File Offset: 0x00004653
	public static bool TryGetComponent< T > ( this Component c, out T result ) where T : Component {
		result = c.GetComponent< T > ();
		return result;
	}

	// Token: 0x06000154 RID: 340 RVA: 0x00006471 File Offset: 0x00004671
	public static bool TryGetComponents< T > ( this Component c, out T[] result ) where T : Component {
		result = c.GetComponents< T > ();
		return result.Length != 0;
	}

	// Token: 0x06000155 RID: 341 RVA: 0x00006481 File Offset: 0x00004681
	public static bool TryGetComponentInParent< T > ( this Component c, out T result ) where T : Component {
		result = c.GetComponentInParent< T > ();
		return result;
	}

	// Token: 0x06000156 RID: 342 RVA: 0x0000649F File Offset: 0x0000469F
	public static bool TryGetComponentInChildren< T > ( this Component c, out T result, bool IncInactive = false )
		where T : Component {
		result = c.GetComponentInChildren< T > ( IncInactive );
		return result;
	}

	// Token: 0x06000157 RID: 343 RVA: 0x000064BE File Offset: 0x000046BE
	public static bool TryGetComponentsInChildren< T > ( this Component c, out T[] result, bool IncInactive = false )
		where T : Component {
		result = c.GetComponentsInChildren< T > ( IncInactive );
		return result.Length != 0;
	}

	// Token: 0x06000158 RID: 344 RVA: 0x000064CF File Offset: 0x000046CF
	public static bool TryGetComponentsInParent< T > ( this Component c, out T[] result, bool IncInactive = false )
		where T : Component {
		result = c.GetComponentsInParent< T > ( IncInactive );
		return result.Length != 0;
	}
}

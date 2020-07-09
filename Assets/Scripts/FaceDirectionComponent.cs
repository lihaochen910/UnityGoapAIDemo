using UnityEngine;

/// <summary>
/// 2D平台面朝方向控制组件
/// </summary>
public class FaceDirectionComponent : MonoBehaviour {

	public Transform transformComponent;

	public int FaceDirection {
		get => GetrRelativeHorizontalFaceDirection ();
	}
	
	public void SetFaceDirection ( Vector2 absoluteDirectionUp, int relativeDir ) {
		
		// 最常用的情况，以世界方向上作为目标的上方向
		if ( absoluteDirectionUp == Vector2.up ) {
			if ( relativeDir == 1 ) {
				transformComponent.localRotation = Quaternion.identity;
			}
			else if ( relativeDir == -1 ) {
				transformComponent.localEulerAngles = new Vector3(180, 0, 180);
			}
		}
		// 以世界方向下作为目标的上方向
		else if ( absoluteDirectionUp == Vector2.down ) {
			if ( relativeDir == 1 ) {
				transformComponent.localEulerAngles = new Vector3(0, 0, 180);
			}
			else if ( relativeDir == -1 ) {
				transformComponent.localEulerAngles = new Vector3(0, 180, 180);
			}
		}
		// 以世界方向左作为目标的上方向
		else if ( absoluteDirectionUp == Vector2.left ) {
			if ( relativeDir == 1 ) {
				transformComponent.localEulerAngles = new Vector3(0, 0, 90);
			}
			else if ( relativeDir == -1 ) {
				transformComponent.localEulerAngles = new Vector3(180, 0, 90);
			}
		}
		// 以世界方向右作为目标的上方向
		else if ( absoluteDirectionUp == Vector2.right ) {
			if ( relativeDir == 1 ) {
				transformComponent.localEulerAngles = new Vector3(0, 0, -90);
			}
			else if ( relativeDir == -1 ) {
				transformComponent.localEulerAngles = new Vector3(180, 0, -90);
			}
		}
	}

	public void SetFaceDirection ( int dir ) {
		SetFaceDirection ( Vector2.up, dir );
	}
	
	public void SetFaceDirection ( float dir ) {
		SetFaceDirection ( Vector2.up, (int)dir );
	}
	
	public int GetrRelativeHorizontalFaceDirection () {
//		Debug.Log ( $"localEulerAngles: {transformComponent.localEulerAngles}" );
		if ( transformComponent.localEulerAngles == new Vector3 ( 180, 0, 180 ) ||
		     transformComponent.localEulerAngles == new Vector3 ( 0, 180, 180 ) ||
		     transformComponent.localEulerAngles == new Vector3 ( 180, 0, 90 ) ||
		     transformComponent.localEulerAngles == new Vector3 ( 180, 0, -90 ) ||
		     transformComponent.localEulerAngles == new Vector3 ( 0, 180, 0 ) ) {
			return -1;
		}
		
		return 1;
	}
}

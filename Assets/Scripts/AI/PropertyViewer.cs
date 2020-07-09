#if UNITY_EDITOR
using UnityEngine;
using RPGStatSystem;

public class PropertyViewer : MonoBehaviour {

    public Color lookRangeColor = Color.blue;
    public Color chaseRangeColor = Color.green;
    public Color meleeAttackRangeColor = Color.red;
    public Color jumpAttackRangeColor = new Color ( 128, 0, 128 );
    public Color remoteAttackRangeColor = Color.cyan;
    public Color safeRangeColor = Color.gray;
    
    public RPGStatCollection collection { get; private set; }
    public RPGStatCollectionDefineLoader defineLoader { get; private set; }
    
    void Start()
    {
        collection = GetComponent<RPGStatCollection>();
    }
    
    void OnDrawGizmos()
    {
        if ( Application.isPlaying ) {
            
            if ( collection == null ) {
                collection = GetComponentInParent< RPGStatCollection > ();
            }
        
            Vector3 centerOffset = Vector3.zero;
            if ( collection.GetStat ( GlobalSymbol.CENTER_OFFSET_X ) != null ) {
                centerOffset.x = collection.GetStatValue ( GlobalSymbol.CENTER_OFFSET_X );
            }
            if ( collection.GetStat ( GlobalSymbol.CENTER_OFFSET_Y ) != null ) {
                centerOffset.y = collection.GetStatValue ( GlobalSymbol.CENTER_OFFSET_Y );
            }

            Gizmos.color = lookRangeColor;
            Gizmos.DrawWireCube(transform.position + centerOffset, new Vector3(collection.GetStatValue(GlobalSymbol.LOOK_RANGE_X) * 2, collection.GetStatValue(GlobalSymbol.LOOK_RANGE_Y), 0));
            Gizmos.color = chaseRangeColor;
            Gizmos.DrawWireCube(transform.position + centerOffset, new Vector3(collection.GetStatValue(GlobalSymbol.CHASE_RANGE_X) * 2, collection.GetStatValue(GlobalSymbol.CHASE_RANGE_Y), 0));
            Gizmos.color = meleeAttackRangeColor;
            Gizmos.DrawWireCube(transform.position + centerOffset, new Vector3(collection.GetStatValue(GlobalSymbol.MELEE_ATTACK_RANGE_X) * 2, collection.GetStatValue(GlobalSymbol.MELEE_ATTACK_RANGE_Y), 0));
            Gizmos.color = jumpAttackRangeColor;
            Gizmos.DrawWireSphere(transform.position + centerOffset, collection.GetStatValue(GlobalSymbol.JUMP_ATTACK_RANGE));
            Gizmos.color = remoteAttackRangeColor;
            Gizmos.DrawWireCube(transform.position + centerOffset, new Vector3(collection.GetStatValue(GlobalSymbol.REMOTE_ATTACK_RANGE_X) * 2, collection.GetStatValue(GlobalSymbol.REMOTE_ATTACK_RANGE_Y), 0));
            Gizmos.color = safeRangeColor;
            Gizmos.DrawLine(new Vector3(transform.position.x + centerOffset.x - collection.GetStatValue(GlobalSymbol.SAFE_RANGE_X), transform.position.y + centerOffset.y),
                new Vector3(transform.position.x + centerOffset.x + collection.GetStatValue(GlobalSymbol.SAFE_RANGE_X), transform.position.y + centerOffset.y));
        }
        else {
            
            if ( defineLoader == null ) {
                defineLoader = GetComponentInParent< RPGStatCollectionDefineLoader > ();
            }
        
            Vector3 centerOffset = Vector3.zero;
            if ( defineLoader.HasStat ( GlobalSymbol.CENTER_OFFSET_X ) ) {
                centerOffset.x = defineLoader.GetStatValue ( GlobalSymbol.CENTER_OFFSET_X );
            }
            if ( defineLoader.HasStat ( GlobalSymbol.CENTER_OFFSET_Y ) ) {
                centerOffset.y = defineLoader.GetStatValue ( GlobalSymbol.CENTER_OFFSET_Y );
            }
            
            if ( defineLoader.HasStat ( GlobalSymbol.LOOK_RANGE_X ) && defineLoader.HasStat ( GlobalSymbol.LOOK_RANGE_Y ) ) {
                Gizmos.color = lookRangeColor;
                Gizmos.DrawWireCube(transform.position + centerOffset, new Vector3(defineLoader.GetStatValue(GlobalSymbol.LOOK_RANGE_X) * 2, defineLoader.GetStatValue(GlobalSymbol.LOOK_RANGE_Y), 0));
            }

            if ( defineLoader.HasStat ( GlobalSymbol.CHASE_RANGE_X ) &&
                 defineLoader.HasStat ( GlobalSymbol.CHASE_RANGE_Y ) ) {
                Gizmos.color = chaseRangeColor;
                Gizmos.DrawWireCube(transform.position + centerOffset, new Vector3(defineLoader.GetStatValue(GlobalSymbol.CHASE_RANGE_X) * 2, defineLoader.GetStatValue(GlobalSymbol.CHASE_RANGE_Y), 0));
            }
            
            if ( defineLoader.HasStat ( GlobalSymbol.MELEE_ATTACK_RANGE_X ) &&
                 defineLoader.HasStat ( GlobalSymbol.MELEE_ATTACK_RANGE_Y ) ) {
                Gizmos.color = meleeAttackRangeColor;
                Gizmos.DrawWireCube ( transform.position + centerOffset,
                    new Vector3 ( defineLoader.GetStatValue ( GlobalSymbol.MELEE_ATTACK_RANGE_X ) * 2,
                        defineLoader.GetStatValue ( GlobalSymbol.MELEE_ATTACK_RANGE_Y ), 0 ) );
            }
            
            if ( defineLoader.HasStat ( GlobalSymbol.JUMP_ATTACK_RANGE ) ) {
                Gizmos.color = jumpAttackRangeColor;
                Gizmos.DrawWireSphere ( transform.position + centerOffset,
                    defineLoader.GetStatValue ( GlobalSymbol.JUMP_ATTACK_RANGE ) );
            }
            
            if ( defineLoader.HasStat ( GlobalSymbol.REMOTE_ATTACK_RANGE_X ) &&
                 defineLoader.HasStat ( GlobalSymbol.REMOTE_ATTACK_RANGE_Y ) ) {
                Gizmos.color = remoteAttackRangeColor;
                Gizmos.DrawWireCube ( transform.position + centerOffset,
                    new Vector3 ( defineLoader.GetStatValue ( GlobalSymbol.REMOTE_ATTACK_RANGE_X ) * 2,
                        defineLoader.GetStatValue ( GlobalSymbol.REMOTE_ATTACK_RANGE_Y ), 0 ) );

            }
            
            if ( defineLoader.HasStat ( GlobalSymbol.SAFE_RANGE_X ) ) {
                Gizmos.color = safeRangeColor;
                Gizmos.DrawLine(new Vector3(transform.position.x + centerOffset.x - defineLoader.GetStatValue(GlobalSymbol.SAFE_RANGE_X), transform.position.y + centerOffset.y),
                    new Vector3(transform.position.x + centerOffset.x + defineLoader.GetStatValue(GlobalSymbol.SAFE_RANGE_X), transform.position.y + centerOffset.y));
            }
        }
    }
}
#endif

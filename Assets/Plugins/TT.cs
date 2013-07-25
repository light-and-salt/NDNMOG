using UnityEngine;
using System.Collections;

public class TT : MonoBehaviour {

	public enum ccn_tt {
		CCN_EXT,        /**< starts composite extension - numval is subtype */
    	CCN_TAG,        /**< starts composite - numval is tagnamelen-1 */ 
    	CCN_DTAG,       /**< starts composite - numval is tagdict index (enum ccn_dtag) */
    	CCN_ATTR,       /**< attribute - numval is attrnamelen-1, value follows */
    	CCN_DATTR,      /**< attribute numval is attrdict index */
    	CCN_BLOB,       /**< opaque binary data - numval is byte count */
    	CCN_UDATA,      /**< UTF-8 encoded character data - numval is byte count */
    	CCN_NO_TOKEN    /**< should not occur in encoding */
	};
}

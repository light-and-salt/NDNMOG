using UnityEngine;
using System.Collections;

public class PCO : MonoBehaviour {

	/* Analogous to enum ccn_parsed_interest_offsetid, but for content */
public enum ccn_parsed_content_object_offsetid : long {
    CCN_PCO_B_Signature,
    CCN_PCO_B_DigestAlgorithm,
    CCN_PCO_E_DigestAlgorithm,
    CCN_PCO_B_Witness,
    CCN_PCO_E_Witness,
    CCN_PCO_B_SignatureBits,
    CCN_PCO_E_SignatureBits,
    CCN_PCO_E_Signature,
    CCN_PCO_B_Name,
    CCN_PCO_B_Component0,
    CCN_PCO_E_ComponentN,
    CCN_PCO_E_ComponentLast = CCN_PCO_E_ComponentN,
    CCN_PCO_E_Name,
    CCN_PCO_B_SignedInfo,
    CCN_PCO_B_PublisherPublicKeyDigest,
    CCN_PCO_E_PublisherPublicKeyDigest,
    CCN_PCO_B_Timestamp,
    CCN_PCO_E_Timestamp,
    CCN_PCO_B_Type,
    CCN_PCO_E_Type,
    CCN_PCO_B_FreshnessSeconds,
    CCN_PCO_E_FreshnessSeconds,
    CCN_PCO_B_FinalBlockID,
    CCN_PCO_E_FinalBlockID,
    CCN_PCO_B_KeyLocator,
    /* Exactly one of Key, Certificate, or KeyName will be present */
    CCN_PCO_B_Key_Certificate_KeyName,
    CCN_PCO_B_KeyName_Name,
    CCN_PCO_E_KeyName_Name,
    CCN_PCO_B_KeyName_Pub,
    CCN_PCO_E_KeyName_Pub,
    CCN_PCO_E_Key_Certificate_KeyName,
    CCN_PCO_E_KeyLocator,
    CCN_PCO_E_SignedInfo,
    CCN_PCO_B_Content,
    CCN_PCO_E_Content,
    CCN_PCO_E
};

}

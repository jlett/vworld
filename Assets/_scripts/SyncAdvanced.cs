using UnityEngine;
using System.Collections;

[RequireComponent (typeof(PhotonView))]
public class SyncAdvanced : MonoBehaviour {
	
	public bool doSyncPosition, doSyncRotation;
	public bool useInterpolation, usePrediction;
	private bool doSyncVelocity;//true if syncPosition && usePrediction
	
	private bool useRigidbody;
	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;
	private Quaternion syncStartRotation = Quaternion.identity;
	private Quaternion syncEndRotation = Quaternion.identity;
	
	void Start() {
		useRigidbody = (GetComponent<Rigidbody>() != null);
		doSyncVelocity = (doSyncPosition && usePrediction && useRigidbody);
	}
	
	void Update() {
		if(!PhotonView.Get(gameObject).isMine) {
			if(useInterpolation) {
				syncTime += Time.deltaTime;
				if(useRigidbody) {
					if(doSyncPosition)
						GetComponent<Rigidbody>().position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
					if(doSyncRotation)
						GetComponent<Rigidbody>().rotation = Quaternion.Lerp(syncStartRotation, syncEndRotation, syncTime / syncDelay);
				} else {
					if(doSyncPosition)
						transform.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
					if(doSyncRotation)
						transform.rotation = Quaternion.Lerp(syncStartRotation, syncEndRotation, syncTime / syncDelay);
				}
			} else {
				if(useRigidbody) {
					if(doSyncPosition)
						GetComponent<Rigidbody>().position = syncEndPosition;
					if(doSyncRotation)
						GetComponent<Rigidbody>().rotation = syncEndRotation;
				} else {
					if(doSyncPosition)
						transform.position = syncEndPosition;
					if(doSyncRotation)
						transform.rotation = syncEndRotation;
				}
			}
		}
	}
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		Vector3 syncPosition = Vector3.zero;
		Vector3 syncVelocity = Vector3.zero;
		Quaternion syncRotation = Quaternion.identity;
		if(stream.isWriting) {//aka we are the one sending data
			if(doSyncPosition) {
				if(useRigidbody)
					syncPosition = GetComponent<Rigidbody>().position;
				else
					syncPosition = transform.position;
				stream.Serialize(ref syncPosition);
			}
			
			if(doSyncVelocity) {
				syncVelocity = GetComponent<Rigidbody>().velocity;
				stream.Serialize(ref syncVelocity);
			}
			
			if(doSyncRotation) {
				if(useRigidbody)
					syncRotation = GetComponent<Rigidbody>().rotation;
				else
					syncRotation = transform.rotation;
				stream.Serialize(ref syncRotation);
			}
		} else {//aka we are receiving data
			if(doSyncPosition)
				stream.Serialize(ref syncPosition);
			if(doSyncVelocity)
				stream.Serialize(ref syncVelocity);
			if(doSyncRotation)
				stream.Serialize(ref syncRotation);

 
			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;
			if(doSyncPosition) {
				if(usePrediction)
					syncEndPosition = syncPosition + syncVelocity * syncDelay;
				else
					syncEndPosition = syncPosition;
				if(useRigidbody)
					syncStartPosition = GetComponent<Rigidbody>().position;
				else
					syncStartPosition = transform.position;
			}
			
			if(doSyncRotation) {
				if(useRigidbody)
					syncStartRotation = GetComponent<Rigidbody>().rotation;
				else
					syncStartRotation = transform.rotation;
				syncEndRotation = syncRotation;
			}
		}
	}
}

using System.Collections;
using UnityEngine;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.Utils;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using Improbable.Core;
using Improbable.Worker;

[WorkerType(WorkerPlatform.UnityClient)]
public class PlayerAnim : MonoBehaviour
{
	[SerializeField] private Animator anim;

	private Vector3 lastPosition;


	private bool IsNotAnAuthoritativePlayer()
	{
		return gameObject.GetAuthority(ClientAuthorityCheck.ComponentId) == Authority.NotAuthoritative;
	}

	private void Update()
	{
		if(IsNotAnAuthoritativePlayer())
		{
			float movementTargetDistance = (lastPosition - transform.position).magnitude;
			float animSpeed = Mathf.Min(1, movementTargetDistance / 0.005f);
			anim.SetFloat("ForwardSpeed", animSpeed);
			lastPosition = transform.position;
		}
	}
}

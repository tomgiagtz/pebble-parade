using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public static class AdvancedUtil
{

	public static Vector3 ResetX(this Vector3 v)
	{
		v.x = 0f;
		return v;
	}

	public static Vector3 ResetY(this Vector3 v)
	{
		v.y = 0f;
		return v;
	}

	public static Vector3 ResetZ(this Vector3 v)
	{
		v.z = 0f;
		return v;
	}

	private static float LogarithmicValue(float distance, float minDistance, float rolloffScale)
	{
		if ((double)distance > (double)minDistance && (double)rolloffScale != 1.0)
		{
			distance -= minDistance;
			distance *= rolloffScale;
			distance += minDistance;
		}
		if ((double)distance < 9.99999997475243E-07)
		{
			distance = 1E-06f;
		}
		return minDistance / distance;
	}

	public static float VolumeLinearToDb(float linear)
	{
		float result;
		if (linear != 0f)
		{
			result = 20f * Mathf.Log10(linear);
		}
		else
		{
			result = -144f;
		}
		return result;
	}

	public static float VolumeDbToLinear(float dB)
	{
		return Mathf.Pow(10f, dB / 20f);
	}

	public static void MoveX(this AnimationCurve curve, float x)
	{
		for (int i = curve.keys.Length - 1; i >= 0; i--)
		{
			Keyframe keyframe = curve.keys[i];
			float inTangent = keyframe.inTangent;
			float outTangent = keyframe.outTangent;
			keyframe.time += x;
			keyframe.inTangent = inTangent;
			keyframe.outTangent = outTangent;
		}
	}

	public static AnimationCurve Logarithmic(float timeStart, float timeEnd, float logBase)
	{
		List<Keyframe> list = new List<Keyframe>();
		float num = 2f;
		timeStart = Mathf.Max(timeStart, 0.0001f);
		float num2 = timeStart;
		while ((double)num2 < (double)timeEnd)
		{
			float num3 = AdvancedUtil.LogarithmicValue(num2, timeStart, logBase);
			float num4 = num2 / 50f;
			float num5 = (float)(((double)AdvancedUtil.LogarithmicValue(num2 + num4, timeStart, logBase) - (double)AdvancedUtil.LogarithmicValue(num2 - num4, timeStart, logBase)) / ((double)num4 * 2.0));
			list.Add(new Keyframe(num2, num3 - num3 * ((num2 - timeStart) / (timeEnd - timeStart)), num5, num5));
			num2 *= num;
		}
		AdvancedUtil.LogarithmicValue(timeEnd, timeStart, logBase);
		float num6 = timeEnd / 50f;
		float num7 = (float)(((double)AdvancedUtil.LogarithmicValue(timeEnd + num6, timeStart, logBase) - (double)AdvancedUtil.LogarithmicValue(timeEnd - num6, timeStart, logBase)) / ((double)num6 * 2.0));
		list.Add(new Keyframe(timeEnd, 0f, num7, num7));
		return new AnimationCurve(list.ToArray());
	}

	public static IEnumerator WaitForRealSeconds(float time)
	{
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < start + time)
		{
			yield return null;
		}
	}

	public static void PlayAnimation(Animator animator, string anim, int layer = -1, float transitionTime = 0f)
	{
		int hash = Animator.StringToHash(anim);
		animator.Play(hash, -1, transitionTime);
	}

	public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
	{
		return Quaternion.Euler(angles) * (point - pivot) + pivot;
	}
}
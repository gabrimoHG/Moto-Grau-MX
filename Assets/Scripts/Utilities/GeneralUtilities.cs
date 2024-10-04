using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
using TMPro;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.UIElements;

namespace HistoricGamesUtils
{
    public static class GeneralUtilities
    {

        public static bool HasInvalidValues(Vector3 vector, float value = 1)
        {
            return vector.x < value || vector.y < value || vector.z < value;
        }

        public static float ConvertCurrencyToFloat(string currencyString,
            string exclusionString)
        {
            string numericString = currencyString.Replace(exclusionString, "").Trim();
            numericString = numericString.Replace(",", ".");

            if (float.TryParse(numericString, NumberStyles.Any, CultureInfo.InvariantCulture, out float result))
                return result;
            else
                throw new FormatException("The provided string could not be converted to a float.");
        }

        public static bool IsEven(int number)
        {
            if (number % 2 == 0)
                return true;
            return false;
        }
        public static bool IsEven(float number)
        {
            if (number % 2 == 0)
                return true;
            return false;
        }

        public static float CurrencyStringToFloat(string currencyString,
            string removableTerm)
        {
            string cleanedString = currencyString.Replace(removableTerm, "").Trim();
            cleanedString = cleanedString.Replace(",", ".");
            if (float.TryParse(cleanedString, NumberStyles.Any,
                CultureInfo.InvariantCulture, out float result))
                return result;
            else
                throw new FormatException("Formato de string incorreto!");
        }

        public static T GetRandomItem<T>(List<T> list)
        {
            int randomIndex = UnityEngine.Random.Range(0, list.Count);
            return list[randomIndex];
        }

        public static IEnumerator CheckIfAnimationFinished(Animator animator, int layerIndex,
            float specificWait, Action onComplete = null)
        {

            AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(layerIndex);
            //Debug.Log("Playing " + animator.GetCurrentAnimatorClipInfo(layerIndex)[0].clip.name);
            //Debug.Log("Animation normalized time: " + animationState.normalizedTime);
            //while (animationState.normalizedTime>1)
            //    yield return null;

            while (animationState.normalizedTime < 1.0f || animator.IsInTransition(layerIndex))
            {
                //Debug.Log("Animation normalized time: " + animationState.normalizedTime);
                if (specificWait > 0)
                {
                    yield return new WaitForSecondsRealtime(1);
                    break;
                }
                else
                    yield return null;
            }
            //Debug.Log("Finished!");
            onComplete?.Invoke();
        }

        public static KeyValuePair<TKey, TValue> GetRandomElement<
            TKey, TValue>(Dictionary<TKey, TValue> dict)
        {
            int index = UnityEngine.Random.Range(0, dict.Count);
            foreach (var kvp in dict)
            {
                if (index == 0)
                    return kvp;
                index--;
            }
            throw new InvalidOperationException("The dictionary is empty.");
        }

        public static bool VectorsAreTheSameOnInt(Vector3 a, Vector3 b)
        {
            return Mathf.FloorToInt(a.x) == Mathf.FloorToInt(b.x) &&
                   Mathf.FloorToInt(a.y) == Mathf.FloorToInt(b.y) &&
                   Mathf.FloorToInt(a.z) == Mathf.FloorToInt(b.z);
        }

        public static Vector3 RandomPositionWithinColliderBoundaries(BoxCollider boxCollider)
        {
            Vector3 boxSize = boxCollider.size;
            Vector3 boxCenter = boxCollider.center;

            float randomX = UnityEngine.Random.Range(-boxSize.x / 2, boxSize.x / 2);
            float randomY = UnityEngine.Random.Range(-boxSize.y / 2, boxSize.y / 2);
            float randomZ = UnityEngine.Random.Range(-boxSize.z / 2, boxSize.z / 2);


            Vector3 localRandomPos = new Vector3(randomX, randomY, randomZ);

            Vector3 worldRandomPos = boxCollider.transform.TransformPoint(localRandomPos + boxCenter);

            return worldRandomPos;
        }
        public static int GetSubscriberCount(Action eventAction)
        {
            if (eventAction == null)
                return 0;

            return eventAction.GetInvocationList().Length;
        }

        public static float ConvertAngleToFirstHalfStandard(float angle)
        {
            //Debug.Log(angle + " % 360 = ");
            angle = angle % 360;
            //Debug.Log(angle);

            if (angle < 0)
            {
                //Debug.Log(angle + " += 360 = ");
                angle += 360;
                //Debug.Log(angle);
            }

            if (angle > 180)
            {
                //Debug.Log(angle + " -= 360");
                angle -= 360;
                //Debug.Log(angle);
            }
            angle = Mathf.Abs(angle);
            //Debug.Log("--------------------------");

            return angle;
        }
    }
}

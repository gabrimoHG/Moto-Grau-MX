using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace HistoricGamesUtils
{
    public static class InterpolationUtilities
    {
        public static IEnumerator MoveToTargetLinearly(Transform rootObject, Vector3 startingPos,
                 Vector3 targetPos, float duration, Action onComplete = null, bool isLocal = false)
        {
            var elapsedTime = 0f;
            //Debug.Log("Starting pos " + startingPos);
            //Debug.Log("Target pos " + targetPos);
            while (elapsedTime < duration)
            {
                if (!isLocal)
                    rootObject.position = Vector3.Lerp(startingPos, targetPos, elapsedTime / duration);
                else
                    rootObject.localPosition = Vector3.Lerp(startingPos, targetPos, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }

            if (!isLocal)
                rootObject.position = targetPos;
            else
                rootObject.localPosition = targetPos;

            onComplete?.Invoke();
        }

        public static IEnumerator RotateToTargetLinearly(Transform rootObject, Quaternion startRot,
            Quaternion targetRot, float duration, Action onComplete = null, bool isLocal = false)
        {
            var elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;

                if (isLocal)
                    rootObject.localRotation = Quaternion.Slerp(startRot, targetRot, t);
                else
                    rootObject.rotation = Quaternion.Slerp(startRot, targetRot, t);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (isLocal)
                rootObject.localRotation = targetRot;
            else
                rootObject.rotation = targetRot;

            onComplete?.Invoke();
        }

        public static IEnumerator ScaleToTargetLinearly(Transform rootObject, Vector3 targetScale,
            float duration, Action onComplete = null)
        {
            Vector3 originalScale = rootObject.localScale;
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                rootObject.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            rootObject.localScale = targetScale;
            onComplete?.Invoke();
        }
        public static IEnumerator MoveToTargetLinearly(RectTransform rootObject, Vector3 startingPos,
                Vector3 targetPos, float duration, Action onComplete = null, bool isLocal = false)
        {
            var elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                if (!isLocal)
                    rootObject.position = Vector3.Lerp(startingPos, targetPos, elapsedTime / duration);
                else
                    rootObject.localPosition = Vector3.Lerp(startingPos, targetPos, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }

            if (!isLocal)
                rootObject.position = targetPos;
            else
                rootObject.localPosition = targetPos;

            onComplete?.Invoke();
        }

        public static IEnumerator RotateToTargetLinearly(RectTransform rootObject, Quaternion startRot,
            Quaternion targetRot, float duration, Action onComplete = null, bool isLocal = false)
        {
            var elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                if (isLocal)
                    rootObject.localRotation = Quaternion.Lerp(rootObject.localRotation,
                        targetRot, elapsedTime / duration);
                else
                    rootObject.rotation = Quaternion.Lerp(rootObject.localRotation,
                        targetRot, elapsedTime / duration);

                elapsedTime += Time.deltaTime;
                yield return null;
            }
            if (isLocal)
                rootObject.localRotation = targetRot;
            else
                rootObject.rotation = targetRot;

            onComplete?.Invoke();
        }

        public static IEnumerator RotateInZLinearly(Transform rootObject, float targetDegree, float duration,
            Action onComplete = null, bool isLocal = false)
        {
            Quaternion startRotation = Quaternion.identity;
            if (isLocal)
                startRotation = rootObject.localRotation;
            else
                startRotation = rootObject.rotation;
            Vector3 startEuler = startRotation.eulerAngles;

            Vector3 targetEuler = new Vector3(startEuler.x, startEuler.y, targetDegree);
            Quaternion endRotation = Quaternion.Euler(targetEuler);

            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                float newZ = Mathf.LerpAngle(startEuler.z, targetDegree, elapsedTime / duration);
                rootObject.localRotation = Quaternion.Euler(startEuler.x, startEuler.y, newZ);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (isLocal)
                rootObject.localRotation = Quaternion.Euler(startEuler.x, startEuler.y, targetDegree);
            else
                rootObject.rotation = Quaternion.Euler(startEuler.x, startEuler.y, targetDegree);
            onComplete?.Invoke();
        }

        public static IEnumerator RotateInXLinearly(Transform rootObject, float targetDegree, float duration,
            Action onComplete = null, bool isLocal = false)
        {

            Quaternion startRotation = isLocal ? rootObject.localRotation : rootObject.rotation;
            Vector3 startEuler = startRotation.eulerAngles;

            // Normalize angles to the range -180 to 180
            float startAngle = startEuler.x > 180 ? startEuler.x - 360 : startEuler.x;
            float targetAngle = targetDegree > 180 ? targetDegree - 360 : targetDegree;

            float elapsedTime = 0;
            while (elapsedTime < duration)
            {
                float newX = Mathf.Lerp(startAngle, targetAngle, elapsedTime / duration);
                Vector3 newEuler = new Vector3(newX, startEuler.y, startEuler.z);

                if (isLocal)
                    rootObject.localRotation = Quaternion.Euler(newEuler);
                else
                    rootObject.rotation = Quaternion.Euler(newEuler);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the final rotation is set precisely
            if (isLocal)
                rootObject.localRotation = Quaternion.Euler(targetDegree, startEuler.y, startEuler.z);
            else
                rootObject.rotation = Quaternion.Euler(targetDegree, startEuler.y, startEuler.z);

            onComplete?.Invoke();

        }

        public static IEnumerator RotateInYLinearly(Transform rootObject, float targetDegree, float duration,
            Action onComplete = null, bool isLocal = false)
        {
            Quaternion startRotation = Quaternion.identity;
            if (isLocal)
                startRotation = rootObject.localRotation;
            else
                startRotation = rootObject.rotation;
            Vector3 startEuler = startRotation.eulerAngles;

            Vector3 targetEuler = new Vector3(startEuler.x, targetDegree, startEuler.z);
            Quaternion endRotation = Quaternion.Euler(targetEuler);

            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                float newY = Mathf.LerpAngle(startEuler.y, targetDegree, elapsedTime / duration);
                if (isLocal)
                    rootObject.localRotation = Quaternion.Euler(startEuler.x, newY, startEuler.z);
                else
                    rootObject.rotation = Quaternion.Euler(startEuler.x, newY, startEuler.z);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (isLocal)
                rootObject.localRotation = Quaternion.Euler(startEuler.x, targetDegree, startEuler.z);
            else
                rootObject.rotation = Quaternion.Euler(startEuler.x, targetDegree, startEuler.z);
            onComplete?.Invoke();
        }

        public static IEnumerator AlphaToTargetLinearly(Image image,
            float targetAlpha, float duration, Action onComplete = null)
        {
            var color = image.color;
            float startAlpha = color.a;
            var elapsedTime = 0.0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
                color.a = newAlpha;
                image.color = color;

                yield return null;
            }
            color.a = targetAlpha;
            image.color = color;

            onComplete?.Invoke();
        }

        public static IEnumerator AlphaToTargetLinearly(TextMeshProUGUI textMeshProUGUI,
            float targetAlpha, float duration, Action onComplete = null)
        {
            var color = textMeshProUGUI.color;
            float startAlpha = color.a;
            var elapsedTime = 0.0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
                color.a = newAlpha;
                textMeshProUGUI.color = color;

                yield return null;
            }
            color.a = targetAlpha;
            textMeshProUGUI.color = color;

            onComplete?.Invoke();
        }


        public static IEnumerator MoveToTargetLinearly(Transform rootObject, Vector3 startPos,
            Vector3 targetPos, float duration, float radius, bool isGoing,
            Action onComplete = null, bool isLocal = false)
        {
            Vector3 center = (startPos + targetPos) / 2;
            Vector3 direction = (targetPos - startPos).normalized;
            Vector3 referenceDirection = Vector3.forward;
            Vector3 perpendicularDirection = Vector3.zero;
            if (isGoing)
            {
                perpendicularDirection = Vector3.Cross(direction, Vector3.forward).normalized;
                if (perpendicularDirection.y < 0)
                {
                    perpendicularDirection = -perpendicularDirection;
                }
            }
            else
            {
                perpendicularDirection = Vector3.Cross(direction, Vector3.back).normalized;
                if (perpendicularDirection.y > 0)
                {
                    perpendicularDirection = -perpendicularDirection;
                }
            }

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                float angle = Mathf.Lerp(0, Mathf.PI, t);

                Vector3 position = center +
                                   Mathf.Cos(angle) * -direction * (Vector3.Distance(startPos, targetPos) / 2) +
                                   Mathf.Sin(angle) * perpendicularDirection * radius;
                if (isLocal)
                    rootObject.localPosition = position;
                else
                    rootObject.position = position;


                yield return null;
            }
            if (isLocal)
                rootObject.localPosition = targetPos;
            else
                rootObject.position = targetPos;

            onComplete?.Invoke();
        }

        public static IEnumerator RotateInYLinearly(Transform rootObject, Transform targetObject, Vector3 targetAxis,
            float angleThreshold, float rotationSpeed, bool isLocal = false, Action onComplete = null)
        {
            while (true)
            {
                var rootForwardLocalAxis = rootObject.InverseTransformDirection(rootObject.forward);
                Vector3 targetDirection = targetObject.TransformDirection(targetAxis).normalized;
                Vector3 currentForward = rootObject.TransformDirection(rootForwardLocalAxis).normalized;
                float dotProduct = Vector3.Dot(currentForward, targetDirection);
                float angleDifference = Mathf.Acos(dotProduct);

                //Debug.Log("Angle difference " + angleDifference);
                if (angleDifference < angleThreshold)
                    break;

                // Calculate the new forward direction
                Vector3 newForward = Vector3.RotateTowards(currentForward,
                    targetDirection, rotationSpeed * Time.deltaTime, 0.0f);
                Quaternion targetRotation = Quaternion.LookRotation(newForward, rootObject.up);

                if (!isLocal)
                    rootObject.rotation = Quaternion.Slerp(rootObject.rotation,
                        targetRotation, rotationSpeed * Time.deltaTime);
                else
                    rootObject.localRotation = Quaternion.Slerp(rootObject.localRotation,
                        targetRotation, rotationSpeed * Time.deltaTime);

                yield return null;
            }
            onComplete?.Invoke();
        }
    }
}

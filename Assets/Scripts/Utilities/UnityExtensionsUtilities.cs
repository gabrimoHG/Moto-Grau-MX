using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HistoricGamesUtils {
    public static class UnityExtensionsUtilities
    {
        public static T FindObjectOfTypeIncludingInactive<T>() where T : Component
        {
            var activeScene = SceneManager.GetActiveScene();
            foreach (T obj in Resources.FindObjectsOfTypeAll<T>())
            {
                if (obj.gameObject.scene == activeScene && 
                    obj.gameObject.hideFlags == HideFlags.None)
                {
                    return obj;
                }
            }
            return null;
        }
        public static List<T> FindObjectsOfTypeIncludingInactive<T>() where T : Component
        {
            var list = new List<T>();
            var activeScene = SceneManager.GetActiveScene();
            foreach (T obj in Resources.FindObjectsOfTypeAll<T>())
            {
                if (obj.gameObject.scene == activeScene && 
                    obj.gameObject.hideFlags == HideFlags.None)
                {
                    list.Add(obj);
                }
            }
            return list;
        }

        public static T GetComponentInChildrenIncludingInactive<T>(
            this GameObject parent, int childIndex) where T : Component
        {
            if (parent == null)
                return null;

            Transform child = parent.transform.GetChild(childIndex);
            if (child != null)
                return GetComponentIncludingInactive<T>(child.gameObject);

            return null;
        }

        public static T GetComponentInChildrenIncludingInactive<T>(
            Transform parent, int childIndex) where T : Component
        {
            if (parent == null)
                return null;

            var child = parent.GetChild(childIndex);
            if (child != null)
                return GetComponentIncludingInactive<T>(child.gameObject);

            return null;
        }

        public static T GetComponentIncludingInactive<T>(
            this GameObject obj) where T : Component
        {
            T component = obj.GetComponent<T>();
            if (component != null)
                return component;

            return null;
        }


        public static T GetComponentIncludingInactive<T>(
            Transform transform) where T : Component
        {
            T component = transform.GetComponent<T>();
            if (component != null)
                return component;

            return null;
        }

        public static List<T> GetComponentsInChildrenIncludingInactive<T>(
            this GameObject parent) where T : Component
        {
            if (parent == null)
                return null;
            var components = new List<T>();
            foreach (Transform child in parent.transform)
                components.Add(child.GetComponent<T>());
            return components;
        }

        public static List<T> GetComponentsInChildrenIncludingInactive<T>(
            Transform transform) where T : Component
        {
            if (transform == null)
                return null;

            var components = new List<T>();
            foreach (Transform child in transform)
                components.Add(child.GetComponent<T>());

            return components;
        }
    }
}

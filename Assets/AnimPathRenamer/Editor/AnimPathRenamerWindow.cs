using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace AnimPathRenamer
{
    public class AnimPathRenamerWindow : EditorWindow
    {
        bool _active = true;
        string _selectPath = "";
        int _selectID = 0;
        GameObject _Animator;
        AnimationClip[] _clips;


        [MenuItem("Window/AnimPathMonitor")]
        static void ShowWindow()
        {
            var window = (AnimPathRenamerWindow)EditorWindow.GetWindow(typeof(AnimPathRenamerWindow), true, "AnimPathMonitor");
            window.ShowUtility();
        }

        protected void OnGUI()
        {
            _active = EditorGUILayout.Toggle("Active", _active);
            EditorGUI.BeginChangeCheck();
            _Animator = EditorGUILayout.ObjectField("Animator", _Animator, typeof(GameObject), true) as GameObject;
            if (EditorGUI.EndChangeCheck())
            {
                _clips = AnimationUtility.GetAnimationClips(_Animator);
            }

            EditorGUILayout.Space();

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.LabelField("Path:", _selectPath);
                if (_clips != null)
                {
                    foreach (var clip in _clips)
                    {
                        EditorGUILayout.ObjectField(clip, typeof(AnimationClip), false);
                    }
                }
            }
        }

        void Update()
        {
            if (!_active) return;
            if (_Animator == null) return;
            var obj = EditorUtility.InstanceIDToObject(_selectID);

            //前情報が足りない場合は取得だけして戻る
            if (obj == null || _selectPath == "")
            {
                if (Selection.activeTransform == null) return;
                var path = AnimationUtility.CalculateTransformPath(Selection.activeTransform, _Animator.transform);
                if (path == "") return;
                _selectID = Selection.activeTransform.GetInstanceID();
                _selectPath = path;
                Repaint();
                return;
            }

            //保持してるIDのPathが変わったかチェック
            var selectPath = AnimationUtility.CalculateTransformPath((Transform)obj, _Animator.transform);
            if (selectPath != _selectPath)
            {
                //変更後のpathが重複してないかチェック
                var isDuplication = false;
                foreach (var clip in _clips)
                {
                    var so = new SerializedObject(clip);
                    var pos_prop = so.FindProperty("m_PositionCurves");
                    for (int i = 0; i < pos_prop.arraySize; i++)
                    {
                        var curve = pos_prop.GetArrayElementAtIndex(i);
                        var path = curve.FindPropertyRelative("path");
                        if (path.stringValue.StartsWith(selectPath)) isDuplication = true;
                    }
                    var scale_prop = so.FindProperty("m_ScaleCurves");
                    for (int i = 0; i < scale_prop.arraySize; i++)
                    {
                        var curve = scale_prop.GetArrayElementAtIndex(i);
                        var path = curve.FindPropertyRelative("path");
                        if (path.stringValue.StartsWith(selectPath)) isDuplication = true;
                    }
                    var euler_prop = so.FindProperty("m_EulerCurves");
                    for (int i = 0; i < euler_prop.arraySize; i++)
                    {
                        var curve = euler_prop.GetArrayElementAtIndex(i);
                        var path = curve.FindPropertyRelative("path");
                        if (path.stringValue.StartsWith(selectPath)) isDuplication = true;
                    }
                    var float_prop = so.FindProperty("m_FloatCurves");
                    for (int i = 0; i < float_prop.arraySize; i++)
                    {
                        var curve = float_prop.GetArrayElementAtIndex(i);
                        var path = curve.FindPropertyRelative("path");
                        if (path.stringValue.StartsWith(selectPath)) isDuplication = true;
                    }
                    var pprt_prop = so.FindProperty("m_PPtrCurves");
                    for (int i = 0; i < pprt_prop.arraySize; i++)
                    {
                        var curve = pprt_prop.GetArrayElementAtIndex(i);
                        var path = curve.FindPropertyRelative("path");
                        if (path.stringValue.StartsWith(selectPath)) isDuplication = true;
                    }
                    var edit_prop = so.FindProperty("m_EditorCurves");
                    for (int i = 0; i < edit_prop.arraySize; i++)
                    {
                        var curve = edit_prop.GetArrayElementAtIndex(i);
                        var path = curve.FindPropertyRelative("path");
                        if (path.stringValue.StartsWith(selectPath)) isDuplication = true;
                    }
                }
                if (isDuplication)
                {
                    _selectPath = selectPath;
                    return;
                }
                //animationClipのpathを書き換え
                foreach (var clip in _clips)
                {
                    var so = new SerializedObject(clip);
                    var pos_prop = so.FindProperty("m_PositionCurves");
                    for (int i = 0; i < pos_prop.arraySize; i++)
                    {
                        var curve = pos_prop.GetArrayElementAtIndex(i);
                        var path = curve.FindPropertyRelative("path");
                        if (path.stringValue.StartsWith(_selectPath)) path.stringValue = selectPath + path.stringValue.Remove(0, _selectPath.Length);
                    }
                    var scale_prop = so.FindProperty("m_ScaleCurves");
                    for (int i = 0; i < scale_prop.arraySize; i++)
                    {
                        var curve = scale_prop.GetArrayElementAtIndex(i);
                        var path = curve.FindPropertyRelative("path");
                        if (path.stringValue.StartsWith(_selectPath)) path.stringValue = selectPath + path.stringValue.Remove(0, _selectPath.Length);
                    }
                    var euler_prop = so.FindProperty("m_EulerCurves");
                    for (int i = 0; i < euler_prop.arraySize; i++)
                    {
                        var curve = euler_prop.GetArrayElementAtIndex(i);
                        var path = curve.FindPropertyRelative("path");
                        if (path.stringValue.StartsWith(_selectPath)) path.stringValue = selectPath + path.stringValue.Remove(0, _selectPath.Length);
                    }
                    var float_prop = so.FindProperty("m_FloatCurves");
                    for (int i = 0; i < float_prop.arraySize; i++)
                    {
                        var curve = float_prop.GetArrayElementAtIndex(i);
                        var path = curve.FindPropertyRelative("path");
                        if (path.stringValue.StartsWith(_selectPath)) path.stringValue = selectPath + path.stringValue.Remove(0, _selectPath.Length);
                    }
                    var pprt_prop = so.FindProperty("m_PPtrCurves");
                    for (int i = 0; i < pprt_prop.arraySize; i++)
                    {
                        var curve = pprt_prop.GetArrayElementAtIndex(i);
                        var path = curve.FindPropertyRelative("path");
                        if (path.stringValue.StartsWith(_selectPath)) path.stringValue = selectPath + path.stringValue.Remove(0, _selectPath.Length);
                    }
                    var edit_prop = so.FindProperty("m_EditorCurves");
                    if (edit_prop != null)
                    {
                        for (int i = 0; i < edit_prop.arraySize; i++)
                        {
                            var curve = edit_prop.GetArrayElementAtIndex(i);
                            var path = curve.FindPropertyRelative("path");
                            if (path.stringValue.StartsWith(_selectPath)) path.stringValue = selectPath + path.stringValue.Remove(0, _selectPath.Length);
                        }
                    }
                    so.ApplyModifiedProperties();
                }
                _selectPath = selectPath;
                Repaint();
            }

            if (Selection.activeTransform == null) return;

            //選択オブジェクトが変わってたらIDとPathを更新
            var selectID = Selection.activeTransform.GetInstanceID();
            if (selectID != _selectID)
            {
                _selectID = selectID;
                _selectPath = AnimationUtility.CalculateTransformPath(Selection.activeTransform, _Animator.transform);
                Repaint();
                return;
            }

        }
    }
}

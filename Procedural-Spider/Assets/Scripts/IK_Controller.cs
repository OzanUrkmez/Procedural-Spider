using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class IK_Controller : MonoBehaviour
{

    [SerializeField]
    private int _chainLength;

    [SerializeField]
    private Transform _target;




    protected float[] _bonesLength;
    protected float _completeLength;
    protected Transform[] _bones;
    protected Vector3[] _positions;


    private void Awake()
    {
        Init();
    }


    private void Init()
    {
        //initialize our arrays
        _bones = new Transform[_chainLength + 1];
        _positions = new Vector3[_chainLength + 1];
        _bonesLength = new float[_chainLength];

        _completeLength = 0;

        //the lowest in the hierarchy is this one.
        var current = transform;

        for(int i = _bones.Length - 1; i >= 0; i--)
        {
            _bones[i] = current;

            if(i == _bones.Length - 1)
            {
                //bone lowest in hierarchy. This has no length.

            }
            else
            {
                //all other bones
                _bonesLength[i] = (_bones[i + 1].position - current.position).magnitude; //distance between us and next lower in hierarchy. 
                _completeLength += _bonesLength[i];
            }

            current = current.parent;
        }
    }

    private void LateUpdate()
    {
        ResolveIK();
    }

    private void ResolveIK()
    {
        if (_target == null)
            return;

        if (_bonesLength.Length != _chainLength)
            Init();

        //get positions

        for(int i = 0; i < _bones.Length; i++)
        {
            _positions[i] = _bones[i].position;
        }

        //fun part!

        //sqr magnitude is faster since no square roots are calculated
        if((_target.position - _bones[0].position).sqrMagnitude >= _completeLength * _completeLength)
        {
            //extend towards the target in a line. "Reach" for a far target.
            var direction = (_target.position - _positions[0]).normalized;

            for(int i = 1; i < _positions.Length; i++)
            {
                _positions[i] = _positions[i - 1] + direction * _bonesLength[i - 1];
            }
        }


        //set positions
        for (int i = 0; i < _bones.Length; i++)
        {
            _bones[i].position = _positions[i];  
        }
    }

    #region Visuals


    [SerializeField]
    private bool _drawGizmos;

    private void OnDrawGizmos()
    {
        if (_drawGizmos)
        {
            var current = this.transform;
            for(int i = 0; i < _chainLength && current != null && current.parent != null;i++)
            {
                var scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
                Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position),
                    new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));

                Handles.color = Color.green;
                Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
                current = current.parent;
            }
        }    
    }

    #endregion

}

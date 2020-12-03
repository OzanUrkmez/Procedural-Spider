using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class IK_Controller : MonoBehaviour
{

    [SerializeField]
    private Transform lowestTransform;

    [SerializeField]
    private int _chainLength;

    [SerializeField]
    private Transform _target;
    [SerializeField]
    private Transform _pole;

    [SerializeField]
    private int _iterations;
    [SerializeField]
    private float stopDelta;

    //bone positioning
    protected float[] _bonesLength;
    protected float _completeLength;
    protected Transform[] _bones;
    protected Vector3[] _positions;

    //bone rotations
    protected Vector3[] _startDirSuccessive;
    protected Quaternion[] _startRotationBone;
    protected Quaternion _startRotationTarget;
    protected Quaternion _startRotationRoot;


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

        _startDirSuccessive = new Vector3[_chainLength + 1];
        _startRotationBone = new Quaternion[_chainLength + 1];
        _startRotationTarget = _target.rotation;

        _completeLength = 0;

        //the lowest in the hierarchy is this one.
        var current = lowestTransform;

        for(int i = _bones.Length - 1; i >= 0; i--)
        {
            _bones[i] = current;
            _startRotationBone[i] = current.rotation;

            if(i == _bones.Length - 1)
            {
                //bone lowest in hierarchy. This has no length.

                //this bone is called "leaf" formally

                _startDirSuccessive[i] = _target.position - current.position;
            }
            else
            {
                //all other bones
                _bonesLength[i] = (_bones[i + 1].position - current.position).magnitude; //distance between us and next lower in hierarchy. 
                _completeLength += _bonesLength[i];

                _startDirSuccessive[i] = _bones[i + 1].position - current.position;
            }

            current = current.parent;
        }

        _startRotationRoot = (_bones[0].parent != null) ? _bones[0].parent.rotation : Quaternion.identity;
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

        var rootRot = (_bones[0].parent != null) ? _bones[0].parent.rotation : Quaternion.identity;
        var rootRotDiff = rootRot * Quaternion.Inverse(_startRotationRoot);

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
        else //Reach for the target, bending other bones accordingly.
        {
            for(int iteration = 0; iteration < _iterations; iteration++)
            {
                //back
                for(int i = _positions.Length - 1; i > 0; i--)
                {
                    if (i == _positions.Length - 1)
                        _positions[i] = _target.position; //the last bone should be at target.
                    else
                        CorrectPositionForward(i);
                }

                //forward
                for(int i = 1; i < _positions.Length; i++)
                {
                    CorrectPositionBackward(i);
                }

                //close enough
                if ((_positions[_positions.Length - 1] - _target.position).sqrMagnitude < stopDelta * stopDelta)
                    break;
            }
        }


        //movement towards pole
        if(_pole != null)
        {
            for(int i = 1; i < _positions.Length - 1; i++)
            {
                CorrectTowardsPole(i);
            }
        }


        //set positions
        for (int i = 0; i < _bones.Length; i++)
        {
            CorrectBoneRotation(i);
            _bones[i].position = _positions[i];  
        }
    }

    #region Utilities

    private void CorrectPositionForward(int positionIndex)
    {
        _positions[positionIndex] = 
            _positions[positionIndex + 1] + (_positions[positionIndex] - _positions[positionIndex + 1]).normalized * _bonesLength[positionIndex];
    }

    private void CorrectPositionBackward(int positionIndex)
    {
        _positions[positionIndex] = 
            _positions[positionIndex - 1] + (_positions[positionIndex] - _positions[positionIndex - 1]).normalized * _bonesLength[positionIndex - 1];
    }

    private void CorrectTowardsPole(int positionIndex)
    {
        Vector3 planeCenter = _positions[positionIndex - 1];

        Plane plane = new Plane(_positions[positionIndex + 1] - planeCenter, planeCenter);

        var projectedPole = plane.ClosestPointOnPlane(_pole.position);
        var projectedBone = plane.ClosestPointOnPlane(_positions[positionIndex]);

        var angle = Vector3.SignedAngle(projectedBone - planeCenter, projectedPole - planeCenter, plane.normal);
        _positions[positionIndex] = Quaternion.AngleAxis(angle, plane.normal) * (_positions[positionIndex] - planeCenter) + planeCenter;
    }

    private void CorrectBoneRotation(int positionIndex)
    {
        if (positionIndex == _positions.Length - 1)
        {
            _bones[positionIndex].rotation = _target.rotation * Quaternion.Inverse(_startRotationTarget) * _startRotationBone[positionIndex];
        }
        else
        {
            _bones[positionIndex].rotation =
                Quaternion.FromToRotation(_startDirSuccessive[positionIndex], _positions[positionIndex + 1] - _positions[positionIndex]) * _startRotationBone[positionIndex];
        }
    }

    #endregion

    #region Visuals


    [SerializeField]
    private bool _drawGizmos;

    private void OnDrawGizmos()
    {
        if (_drawGizmos)
        {
            var current = lowestTransform;
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

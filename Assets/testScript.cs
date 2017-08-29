using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class testScript : MonoBehaviour {

	public float t;
	public Vector3 testPoint;
	public float resolution = 0.5f;
	public int lineSegments = 100;
	public CubicBezierSpline curve = new CubicBezierSpline();
	public float totalLenght = 0;
	public List<float> lenghts = new List<float>();

	private bool curveUpdated = false;
	private CubicBezierSpline _curve = new CubicBezierSpline();
	public List<Vector3> curvePoints = new List<Vector3>();

	private Vector3 midPoint;


	// Use this for initialization
	void Start () {
		_curve.points.Clear();
		lenghts.Clear();
		for(int i=0; i<curve.points.Count; i++){
			_curve.points.Add(new BezierSplinePoint(curve.points[i].point, curve.points[i].leftTangent, curve.points[i].rightTangent, curve.points[i].breakTangent));
			if(i<curve.points.Count-1){	lenghts.Add(curve.GetCurveLenght(i));	}
		}
		totalLenght = curve.totalLenght;
	}

	void OnValidate(){
		_curve.wrapMode = curve.wrapMode;

		while(_curve.points.Count<curve.points.Count){	_curve.points.Add(new BezierSplinePoint());	}
		while(_curve.points.Count>curve.points.Count){	_curve.points.RemoveAt(_curve.points.Count-1);	}

		int lenghtsLimit = (_curve.wrapMode == CubicBezierSpline.WrapMode.Loop? _curve.points.Count: _curve.points.Count-1);
		while(lenghts.Count<lenghtsLimit){	lenghts.Add(0);	}
		while(lenghts.Count>lenghtsLimit){	lenghts.RemoveAt(lenghts.Count-1);	}

		for(int i=0; i<curve.points.Count; i++){
			if(_curve.points[i].point != curve.points[i].point){	_curve.points[i].point = curve.points[i].point;	}
			if(_curve.points[i].leftTangent != curve.points[i].leftTangent)		{
				_curve.points[i].leftTangent = curve.points[i].leftTangent;
				curve.points[i].rightTangent = _curve.points[i].rightTangent;
			}
			if(_curve.points[i].rightTangent != curve.points[i].rightTangent)	{	
				_curve.points[i].rightTangent = curve.points[i].rightTangent;
				curve.points[i].leftTangent = _curve.points[i].leftTangent;
			}
			if(_curve.points[i].breakTangent != curve.points[i].breakTangent)	{	_curve.points[i].breakTangent = curve.points[i].breakTangent;	}

			if(i<lenghtsLimit){	lenghts[i] = curve.GetCurveLenght(i);	}
		}
		
		totalLenght = curve.totalLenght;
		testPoint = _curve.Evaluate(t);

		curveUpdated = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(_curve.points.Count > 1 && resolution > 0 && curveUpdated){

			curvePoints.Clear();

			midPoint = _curve.Evaluate(0.5f);


			curvePoints = _curve.GetCurvePoints(resolution);

			curveUpdated = false;
		}

		Debug.DrawLine(transform.TransformPoint(midPoint+(Vector3.left)*0.1f),transform.TransformPoint(midPoint+(Vector3.right)*0.1f),Color.blue,0);
		Debug.DrawLine(transform.TransformPoint(midPoint+(Vector3.up)*0.1f),transform.TransformPoint(midPoint+(Vector3.down)*0.1f),Color.blue,0);

		for(int i=0; i<=_curve.points.Count-2;i++){
			Debug.DrawLine(transform.TransformPoint(_curve.points[i].point+(Vector3.down+Vector3.left)*0.1f)
				,transform.TransformPoint(_curve.points[i].point+(Vector3.up+Vector3.right)*0.1f),Color.red,0);
			Debug.DrawLine(transform.TransformPoint(_curve.points[i].point+(Vector3.up+Vector3.left)*0.1f)
				,transform.TransformPoint(_curve.points[i].point+(Vector3.down+Vector3.right)*0.1f),Color.red,0);

			Debug.DrawLine(transform.TransformPoint(_curve.points[i].worldRightTangent+(Vector3.down+Vector3.left)*0.1f)
				,transform.TransformPoint(_curve.points[i].worldRightTangent+(Vector3.up+Vector3.right)*0.1f),Color.white,0);
			Debug.DrawLine(transform.TransformPoint(_curve.points[i].worldRightTangent+(Vector3.up+Vector3.left)*0.1f)
				,transform.TransformPoint(_curve.points[i].worldRightTangent+(Vector3.down+Vector3.right)*0.1f),Color.white,0);

			Debug.DrawLine(transform.TransformPoint(_curve.points[i+1].worldLeftTangent+(Vector3.down+Vector3.left)*0.1f)
				,transform.TransformPoint(_curve.points[i+1].worldLeftTangent+(Vector3.up+Vector3.right)*0.1f),Color.white,0);
			Debug.DrawLine(transform.TransformPoint(_curve.points[i+1].worldLeftTangent+(Vector3.up+Vector3.left)*0.1f)
				,transform.TransformPoint(_curve.points[i+1].worldLeftTangent+(Vector3.down+Vector3.right)*0.1f),Color.white,0);
		}

		if(curve.wrapMode == CubicBezierSpline.WrapMode.Loop){
			Debug.DrawLine(transform.TransformPoint(_curve.points[0].worldLeftTangent+(Vector3.down+Vector3.left)*0.1f)
				,transform.TransformPoint(_curve.points[0].worldLeftTangent+(Vector3.up+Vector3.right)*0.1f),Color.white,0);
			Debug.DrawLine(transform.TransformPoint(_curve.points[0].worldLeftTangent+(Vector3.up+Vector3.left)*0.1f)
				,transform.TransformPoint(_curve.points[0].worldLeftTangent+(Vector3.down+Vector3.right)*0.1f),Color.white,0);

			Debug.DrawLine(transform.TransformPoint(_curve.points[curve.points.Count-1].worldRightTangent+(Vector3.down+Vector3.left)*0.1f)
				,transform.TransformPoint(_curve.points[curve.points.Count-1].worldRightTangent+(Vector3.up+Vector3.right)*0.1f),Color.white,0);
			Debug.DrawLine(transform.TransformPoint(_curve.points[curve.points.Count-1].worldRightTangent+(Vector3.up+Vector3.left)*0.1f)
				,transform.TransformPoint(_curve.points[curve.points.Count-1].worldRightTangent+(Vector3.down+Vector3.right)*0.1f),Color.white,0);
		}

		Debug.DrawLine(transform.TransformPoint(_curve.points[_curve.points.Count-1].point+(Vector3.down+Vector3.left)*0.1f)
			,transform.TransformPoint(_curve.points[_curve.points.Count-1].point+(Vector3.up+Vector3.right)*0.1f),Color.red,0);
		Debug.DrawLine(transform.TransformPoint(_curve.points[_curve.points.Count-1].point+(Vector3.up+Vector3.left)*0.1f)
			,transform.TransformPoint(_curve.points[_curve.points.Count-1].point+(Vector3.down+Vector3.right)*0.1f),Color.red,0);

		Debug.DrawLine(transform.TransformPoint(testPoint+(Vector3.left)*0.1f)
			,transform.TransformPoint(testPoint+(Vector3.right)*0.1f),Color.black,0);
		Debug.DrawLine(transform.TransformPoint(testPoint+(Vector3.up)*0.1f)
			,transform.TransformPoint(testPoint+(Vector3.down)*0.1f),Color.black,0);

		for(int i=0; i<curvePoints.Count-1; i++){	Debug.DrawLine(transform.TransformPoint(curvePoints[i])
			,transform.TransformPoint(curvePoints[i+1]),Color.green,0,false);	}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Utils {
	/// <summary>
	/// Gets the first set bit from the least to most significant.
	/// </summary>
	/// <returns>The set bit number.</returns>
	/// <param name="value">Value.</param>
	static public int GetSetBit(int value){
		int setBit = 0;
		for(int i=0; i<32;i++){	if((value & 1 << i) != 0){	setBit = i;	break;	}	}
		return setBit;
	}
}

[System.Serializable]
public class CubicBezierSpline{

	private readonly static int lenghtSegmentCount = 10;

	public enum WrapMode {	Clamp, Repeat, Loop	};

	/// <summary>
	/// The curve control points and tangents.
	/// </summary>
	public WrapMode wrapMode;
	public List<BezierSplinePoint> points = new List<BezierSplinePoint>();
	private List<float> lenghts = new List<float>();
	private float _totalLenght = 0;

	/// <summary>
	/// Gets the n curve lenght.
	/// </summary>
	/// <returns>The curve lenght in float.</returns>
	/// <param name="index">Index.</param>
	public float GetCurveLenght(int n){	CalculateLenght(lenghtSegmentCount);	return lenghts[n];	}

	/// <summary>
	/// Gets the total lenght of the curves.
	/// </summary>
	/// <value>The total lenght.</value>
	public float totalLenght{	get{	return _totalLenght;	}	}

	/// <summary>
	/// Evaluates the curve on the specified t. Takes into account curve lenght.
	/// </summary>
	/// <returns>The evaluated Vector3.</returns>
	/// <param name="t">T.</param>
	public Vector3 Evaluate(float t){
		if(wrapMode == WrapMode.Clamp){	
			if(t<0){	return points[0].point;	}
			if(t>1){	return points[points.Count-1].point;	}
		}
		if((wrapMode == WrapMode.Repeat || wrapMode == WrapMode.Loop) && (t < 0 || t > 1)){
			if(t<0){	t = 1-(t-Mathf.Ceil(t))*(-1);	}
			if(t>1){	t = t-Mathf.Floor(t);	}
		}

		CalculateLenght(lenghtSegmentCount);

		float lenghtT = t*_totalLenght;
		float remainingLenghtT = lenghtT;
		float sumLenght = 0;
		float correctedT = 0;

		int pointIndex = 0;
		int index2 = 0;
		for(int i=0; i<(wrapMode == WrapMode.Loop? points.Count:points.Count-1); i++){
			float newLenght = sumLenght + lenghts[i];
			if(remainingLenghtT - newLenght < 0){
				remainingLenghtT = lenghtT - sumLenght;
				correctedT = remainingLenghtT/lenghts[i];
				pointIndex = i;
				index2 = (i+1 == points.Count? 0:i+1);
				//Debug.Log(sumLenght+remainingLenghtT);
				break;
			}else{
				sumLenght = newLenght;
			}
		}

		//Debug.Log(pointIndex+"|"+index2);

		return GetPoint(points[pointIndex].point, points[pointIndex].worldRightTangent,points[index2].worldLeftTangent,points[index2].point,correctedT);

	}

	/// <summary>
	/// Evaluates the curve on the specified t, does not accounts for curve lenght.
	/// </summary>
	/// <returns>The raw evaluated Vector3.</returns>
	/// <param name="t">T.</param>
	public Vector3 EvaluateRaw(float t){
		if(wrapMode == WrapMode.Clamp){	
			if(t<=0){	return points[0].point;	}
			if(t>=1){	return points[points.Count-1].point;	}
		}
		if((wrapMode == WrapMode.Repeat || wrapMode == WrapMode.Loop) && (t < 0 || t > 1)){
			if(t<0){	t = 1-(t-Mathf.Ceil(t))*(-1);	}
			if(t>1){	t = t-Mathf.Floor(t);	}
		}

		float correctedT = t*((float)(wrapMode == WrapMode.Loop? points.Count : points.Count-1));
		int pointIndex = Mathf.FloorToInt(correctedT);
		float newT = correctedT-pointIndex;

//		Debug.Log("originalT:"+t+"|CorrectedT:"+correctedT+"|point index:"+pointIndex+"|netT:"+newT);


//		Debug.Log("Evaluating Curve at "+pointIndex+"-"+(pointIndex+1)+"| t = "+t);

		if(newT == 0){	return points[pointIndex].point;	}
		if(pointIndex == (wrapMode == WrapMode.Loop? points.Count : points.Count-1)){	return points[(wrapMode == WrapMode.Loop? 0: pointIndex)].point;	}
//		if(t == 1){	return points[1].point;	}
		int index2 = (pointIndex == points.Count? 0: pointIndex+1);

		Vector3 result = GetPoint(points[pointIndex].point, points[pointIndex].worldRightTangent,points[index2].worldLeftTangent,points[index2].point,newT);

		return result;
	}

	/// <summary>
	/// Gets the curve points with the specified resolution.
	/// </summary>
	/// <returns>Vector3 list with the evaluated points.</returns>
	/// <param name="resolution">Resolution.</param>
	public List<Vector3> GetCurvePoints(float resolution){
		CalculateLenght(lenghtSegmentCount);
		List<int> segmentsByCurve = new List<int>();
		for(int i=0; i<(wrapMode == WrapMode.Loop? points.Count: points.Count-1); i++){	segmentsByCurve.Add(Mathf.FloorToInt(lenghts[i]/resolution));	/*Debug.Log(segmentsByCurve[segmentsByCurve.Count-1]);*/	}
		//int failsafe = 0;
		List<Vector3> curvePoints = new List<Vector3>();
		for(int i=0; i<(wrapMode == WrapMode.Loop? points.Count: points.Count-1); i++){
			int index2 = (i == points.Count-1? 0: i+1);
			float tIncrement = 1.0f/segmentsByCurve[i];
			for(float t=0; t<1; t+=tIncrement){
				//Debug.Log("tIncrement:"+tIncrement+"|t:"+t);
				curvePoints.Add(GetPoint(points[i].point,points[i].worldRightTangent,points[index2].worldLeftTangent,points[index2].point,t));
				//if(failsafe < 1000){	failsafe++;	}else{	break;	}
			}
		}
		curvePoints.Add(points[(wrapMode == WrapMode.Loop? 0: points.Count-1)].point);

		return curvePoints;
	}

	/// <summary>
	/// Gets the curve points interval. (ESSA BOSTA NÃO TA IMPLEMENTADA AINDA!!!ONE!!!!!SHIFT!!!!)
	/// </summary>
	/// <returns>The curve points interval.</returns>
	/// <param name="resolution">Resolution.</param>
	/// <param name="t0">T0.</param>
	/// <param name="t1">T1.</param>
	public List<Vector3> GetCurvePointsInterval(float resolution, float t0, float t1){
		CalculateLenght(lenghtSegmentCount);
		List<int> segmentsByCurve = new List<int>();
		for(int i=0; i<(wrapMode == WrapMode.Loop? points.Count: points.Count-1); i++){	segmentsByCurve.Add(Mathf.FloorToInt(lenghts[i]/resolution));	/*Debug.Log(segmentsByCurve[segmentsByCurve.Count-1]);*/	}
		//int failsafe = 0;
		List<Vector3> curvePoints = new List<Vector3>();
		for(int i=0; i<(wrapMode == WrapMode.Loop? points.Count: points.Count-1); i++){
			int index2 = (i == points.Count-1? 0: i+1);
			float tIncrement = 1.0f/segmentsByCurve[i];
			for(float t=0; t<1; t+=tIncrement){
				//Debug.Log("tIncrement:"+tIncrement+"|t:"+t);
				curvePoints.Add(GetPoint(points[i].point,points[i].worldRightTangent,points[index2].worldLeftTangent,points[index2].point,t));
				//if(failsafe < 1000){	failsafe++;	}else{	break;	}
			}
		}
		curvePoints.Add(points[(wrapMode == WrapMode.Loop? 0: points.Count-1)].point);

		return curvePoints;
	}

	/// <summary>
	/// Gets the interpolated Vector3 at t(0~1) given four points.
	/// </summary>
	/// <returns>The evaluated Vector3.</returns>
	/// <param name="term1">Term1.</param>
	/// <param name="term2">Term2.</param>
	/// <param name="term3">Term3.</param>
	/// <param name="term4">Term4.</param>
	/// <param name="t">T.</param>
	public static Vector3 GetPoint(Vector3 term1, Vector3 term2, Vector3 term3, Vector3 term4, float t){
		float tCube = t*t*t;
		float tCube3 = tCube*3;
		float tSquare3 = t*t*3;
		Vector3 result = (-tCube+tSquare3-3*t+1)*term1+(tCube3-2*tSquare3+3*t)*term2+(-tCube3+tSquare3)*term3+(tCube)*term4;

		return result;
	}

	void CalculateLenght(int resolution){
		float tIncrement = 1.0f/resolution;
		float totalLenght = 0;
		Vector3 lastPoint;

		lenghts.Clear();

		for(int i=0; i<(wrapMode == WrapMode.Loop? points.Count: points.Count-1);i++){
			int index2 = (i+1==points.Count?0:i+1);
			float curveLength = 0;
			lastPoint = points[i].point;
			for(float t=tIncrement; t<=1; t+=tIncrement){
				Vector3 currentPoint = GetPoint(points[i].point,points[i].worldRightTangent,points[index2].worldLeftTangent,points[index2].point,t);
				curveLength += Vector3.Distance(lastPoint,currentPoint);
			}
			lenghts.Add(curveLength);
			totalLenght += curveLength;
		}
		this._totalLenght = totalLenght;
	}
}

[System.Serializable]
public class BezierSplinePoint{
	public Vector3 point;
	[SerializeField]
	private Vector3 _leftTangent;
	[SerializeField]
	private Vector3 _rightTangent;
	public bool breakTangent;

	public BezierSplinePoint(){
		this.point = Vector3.zero;
		this._leftTangent = Vector3.zero;
		this._rightTangent = Vector3.zero;
		this.breakTangent = false;
	}

	public BezierSplinePoint(Vector3 point, Vector3 leftTangent, Vector3 rightTangent, bool breakTangent){
		this.point = point;
		this._leftTangent = leftTangent;
		this._rightTangent = rightTangent;
		this.breakTangent = breakTangent;
	}
	
	public Vector3 rightTangent{
		get{	return _rightTangent;	}
		set{	_rightTangent = value;	UpdateTangent(true);	}
	}
	
	public Vector3 leftTangent{
		get{	return _leftTangent;	}
		set{	_leftTangent = value;	UpdateTangent(false);	}
	}

	public Vector3 worldRightTangent{	get{	return _rightTangent+point;	}	}
	public Vector3 worldLeftTangent{	get{	return _leftTangent+point;	}	}

	void UpdateTangent(bool isLeftTangent){
		if(breakTangent){	return;	}
		if(isLeftTangent){	_leftTangent = _rightTangent*(-1);	}
		else{	_rightTangent = _leftTangent*(-1);	}
	}
}


//var fractureToPoint = false;
//var totalMaxFractures = 3;
//var forcePerDivision = 20.0;
//var minBreakingForce = 0.0;
//var maxFracturesPerCall = 3;
//var randomOffset = 0.0;
//var minFractureSize : Vector3 = Vector3.zero;
//var grain : Vector3 = Vector3.one;
//var useCollisionDirection = 0.0;
//var fractureAtCenter = false;
//var smartJoints = false;
//var destroyAllAfterTime = 0.0;
//var destroySmallAfterTime = 0.0;
//var instantiateOnBreak : GameObject;
//var totalMassIfStatic = 1.0;
//private var joints : Joint[];
////-------------------------------------------------------------------
//function Start () {
//	if (GetComponent.<Rigidbody>()) {
//		temp = new Array();
//		for(j in FindObjectsOfType(Joint)) {
//			if (j.connectedBody == GetComponent.<Rigidbody>()) {
//				temp.Add(j);
//				joints  = temp.ToBuiltin(Joint);
//			}
//		}
//	}
//}
////-------------------------------------------------------------------
//function OnCollisionEnter (collision : Collision) {
//	point=collision.contacts[0].point;
//	vec = collision.relativeVelocity*UsedMass(collision);
//	FractureAtPoint(point,vec);
//}
////-------------------------------------------------------------------
//function FractureAtPoint (hit : Vector3, force : Vector3) {
//	if (force.magnitude < Mathf.Max(minBreakingForce,forcePerDivision)) {return;}
//	iterations =  Mathf.Min(Mathf.RoundToInt(force.magnitude/forcePerDivision),Mathf.Min(maxFracturesPerCall,totalMaxFractures));
//	point = transform.worldToLocalMatrix.MultiplyPoint(hit);
//	Fracture(point,force,iterations);
//}
////-------------------------------------------------------------------
//function Fracture (point : Vector3, force : Vector3, iterations : float) {
//	if (instantiateOnBreak && force.magnitude >= Mathf.Max(minBreakingForce,forcePerDivision)) {
//		Instantiate(instantiateOnBreak,transform.position,transform.rotation);
//		instantiateOnBreak = null;
//	}
//	while (iterations > 0) {
//		// if we are smaller than our minimum fracture size in any dimension, no more divisions.
//		if (totalMaxFractures == 0 || Vector3.Min(gameObject.GetComponent(MeshFilter).mesh.bounds.size,minFractureSize) != minFractureSize) {
//			if (destroySmallAfterTime >= 1) {
//				Destroy(GetComponent(MeshCollider),destroySmallAfterTime-1);
//				Destroy(gameObject,destroySmallAfterTime);
//			}
//			totalMaxFractures = 0;
//			return;
//		}
//		totalMaxFractures -= 1;
//		iterations -= 1;
//		// define the splitting plane by the user settings.
//		if(fractureAtCenter){point=GetComponent(MeshFilter).mesh.bounds.center;}
//		vec = Vector3.Scale(grain,Random.insideUnitSphere).normalized;
//		sub = transform.worldToLocalMatrix.MultiplyVector(force.normalized)*useCollisionDirection*Vector3.Dot(transform.worldToLocalMatrix.MultiplyVector(force.normalized),vec);
//		plane = Plane(vec-sub,Vector3.Scale(Random.insideUnitSphere,GetComponent(MeshFilter).mesh.bounds.size)*randomOffset+point);
//		// create the clone
//		newObject = Instantiate(gameObject,transform.position,transform.rotation);
//		if (GetComponent.<Rigidbody>()) {newObject.GetComponent.<Rigidbody>().velocity = GetComponent.<Rigidbody>().velocity;}
//		// arrays of the verts
//		var vertsA =  gameObject.GetComponent(MeshFilter).mesh.vertices;
//		var vertsB =  newObject.GetComponent(MeshFilter).mesh.vertices;
//		average = Vector3.zero;
//		for (i in vertsA) {
//			average += i;
//		}
//		average /= gameObject.GetComponent(MeshFilter).mesh.vertexCount;
//		average -= plane.GetDistanceToPoint(average)*plane.normal;
//		//-------------------------------------------------------------------
//		broken = 0;
//		// split geometry along plane
//		if (fractureToPoint) {
//			for (i=0;i<gameObject.GetComponent(MeshFilter).mesh.vertexCount;i++) {
//				if (plane.GetSide(vertsA[i])) {
//					vertsA[i] = average;
//					broken += 1;
//				}
//				else {
//					vertsB[i] = average;
//				}
//			}
//		}
//		else {
//			for (i=0;i<gameObject.GetComponent(MeshFilter).mesh.vertexCount;i++) {
//				if (plane.GetSide(vertsA[i])) {
//					vertsA[i] -= plane.GetDistanceToPoint(vertsA[i])*plane.normal;
//					broken += 1;
//				}
//				else {
//					vertsB[i] -= plane.GetDistanceToPoint(vertsB[i])*plane.normal;
//				}
//			}
//		}
//		// IMPORTANT: redo if we have a problem splitting; without this, we will get a lot of non-manifold meshes, convexhull errors and maybe even crash the game.
//		if (broken == 0 || broken == gameObject.GetComponent(MeshFilter).mesh.vertexCount) {
//			totalMaxFractures += 1;
//			iterations += 1;
//			Destroy(newObject);
//			// this yield is here JUST so that when a large amount of objects are being broken, the screen doesn't freeze for a long time. It allows the screen to refresh before we're finnished, but if you don't, it might slow the script down trying to break a loop of bad planes.
//			yield;
//		}
//		// if all's fine, apply the changes to each mesh
//		else {
//			gameObject.GetComponent(MeshFilter).mesh.vertices = vertsA;
//			newObject.GetComponent(MeshFilter).mesh.vertices = vertsB;
//			gameObject.GetComponent(MeshFilter).mesh.RecalculateNormals();
//			newObject.GetComponent(MeshFilter).mesh.RecalculateNormals();
//			gameObject.GetComponent(MeshFilter).mesh.RecalculateBounds();
//			newObject.GetComponent(MeshFilter).mesh.RecalculateBounds();
//			if (gameObject.GetComponent(MeshCollider)) {
//				gameObject.GetComponent(MeshCollider).sharedMesh = gameObject.GetComponent(MeshFilter).mesh;
//				newObject.GetComponent(MeshCollider).sharedMesh = newObject.GetComponent(MeshFilter).mesh;
//			}
//			// if we weren't using a convexhull, the pieces colliders won't work right. It's best for everyone if we just remove them.
//			else {
//				Destroy(GetComponent.<Collider>());
//				Destroy(gameObject,1);
//			}
//			// smartjoints will allow joints to function properly.
//			if (smartJoints) {
//				jointsb = GetComponents(Joint);
//				if (jointsb){
//					// Basically, it goes through each joint and sees if the object A or B are closer to the connected body. Whichever is closer keeps the joint.
//					for (i=0;i<jointsb.length;i++){
//						if (jointsb[i].connectedBody != null && plane.GetSide(transform.worldToLocalMatrix.MultiplyPoint(jointsb[i].connectedBody.transform.position))) {
//							if (jointsb[i].gameObject.GetComponent(SimpleFracture).joints) {
//								// If we're attached to a fracture object and the new object is closer, switch the connected object's joint variable at the correct index.
//								for (c in jointsb[i].gameObject.GetComponent(SimpleFracture).joints) {
//									if (c == jointsb[i]) {c = newObject.GetComponents(Joint)[i];}
//								}
//							}
//							Destroy(jointsb[i]);
//						}
//						else {
//							Destroy(newObject.GetComponents(Joint)[i]);
//						}
//					}
//				}
//				// joints contains all joints this object is attached to. It checks if the joint still exists, and if the new object is closer. If so, changes the connection. It then removes the joint from the joints variable at the correct index.
//				if (joints){
//					for (i=0;i<joints.length;i++){
//						if (joints[i] && plane.GetSide(transform.worldToLocalMatrix.MultiplyPoint(joints[i].transform.position))) {
//							joints[i].connectedBody = newObject.GetComponent.<Rigidbody>();
//							temp = new Array(joints);
//							temp.RemoveAt(i);
//							joints = temp.ToBuiltin(Joint);
//						}
//						else {
//							temp = new Array(joints);
//							temp.RemoveAt(i);
//							newObject.GetComponent("SimpleFracture").joints = temp.ToBuiltin(Joint);
//						}
//					}
//				}
//			}
//			// if we don't have smartJoints, the code is much shorter. destroy all joints.
//			else {
//				if (GetComponent(Joint)) {
//					for (i=0;i<GetComponents(Joint).length;i++){
//						Destroy(GetComponents(Joint)[i]);
//						Destroy(newObject.GetComponents(Joint)[i]);
//					}
//				}
//				if (joints) {
//					for (i=0;i<joints.length;i++){
//						Destroy(joints[i]);
//					}
//					joints = null;
//				}
//			}
//			// if the script is attached to a static object, make it dynamic. If not, divide the mass up.
//			if (!GetComponent.<Rigidbody>()) {
//				gameObject.AddComponent(Rigidbody);
//				newObject.AddComponent(Rigidbody);
//				GetComponent.<Rigidbody>().mass = totalMassIfStatic;
//				newObject.GetComponent.<Rigidbody>().mass = totalMassIfStatic;
//			}
//			gameObject.GetComponent.<Rigidbody>().mass *= 0.5;
//			newObject.GetComponent.<Rigidbody>().mass *= 0.5;
//			gameObject.GetComponent.<Rigidbody>().centerOfMass = transform.worldToLocalMatrix.MultiplyPoint3x4(gameObject.GetComponent.<Collider>().bounds.center);
//			newObject.GetComponent.<Rigidbody>().centerOfMass = transform.worldToLocalMatrix.MultiplyPoint3x4(newObject.GetComponent.<Collider>().bounds.center);
//			
//			newObject.GetComponent("SimpleFracture").Fracture(point,force,iterations);
//			
//			if (destroyAllAfterTime >= 1) {
//				Destroy(newObject.GetComponent(MeshCollider),destroyAllAfterTime-1);
//				Destroy(GetComponent(MeshCollider),destroyAllAfterTime-1);
//				Destroy(newObject,destroyAllAfterTime);
//				Destroy(gameObject,destroyAllAfterTime);
//			}
//			// this yield is here JUST so that when a large amount of objects are being broken, the screen doesn't freeze for a while.
//			yield;
//		}// if not broken end
//	}// while itterations end
//	if (totalMaxFractures == 0 || Vector3.Min(gameObject.GetComponent(MeshFilter).mesh.bounds.size,minFractureSize) != minFractureSize) {
//		if (destroySmallAfterTime >= 1) {
//			Destroy(GetComponent(MeshCollider),destroySmallAfterTime-1);
//			Destroy(gameObject,destroySmallAfterTime);
//		}
//		totalMaxFractures = 0;
//	}
//}
////--------------------------------------------------------------
//function UsedMass (collision : Collision) {
//	if (collision.rigidbody) {
//		if (GetComponent.<Rigidbody>()) {
//			if (collision.rigidbody.mass < GetComponent.<Rigidbody>().mass) {
//				return (collision.rigidbody.mass);
//			}
//			else {
//				return (GetComponent.<Rigidbody>().mass);
//			}
//		}
//		else {
//			return (collision.rigidbody.mass);
//		}
//	}
//	else if (GetComponent.<Rigidbody>()) {
//		return (GetComponent.<Rigidbody>().mass);
//	}
//	else {return (1);}
//}

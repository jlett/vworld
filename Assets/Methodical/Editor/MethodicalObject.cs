//-------------------------------
//          Methodical
// Copyright © 2014 Linkage Games
//-------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Linkage.Methodical{
	public class MethodicalObject : IEnumerable<MethodicalObject> {
		public enum ObjectTypes {variableType,intType,DoubleType,floatType,boolType,stringType,arrayType,hashMapType,methodType,equalityType};
		public ObjectTypes objectType;//Type of the object
		public MethodicalObject member;
		public List<MethodicalObject> arguments = new List<MethodicalObject>();//Method args
		public string objectName= "";
		public object value = null;
		public MethodicalObject arrayDictionaryIndex;

		// For equalities only
		public MethodicalObject leftEquality;
		public MethodicalObject rightEquality;
		public string leftEqualityString;
		public string rightEqualityString;

		/**!
		 * Does the object contained within pass by value or reference?
		 * */
		public bool passesByValue {
			get{
				if (!objectType.Equals(ObjectTypes.variableType)
					&&!objectType.Equals(ObjectTypes.arrayType)
					&&!objectType.Equals(ObjectTypes.hashMapType)
					&&!objectType.Equals(ObjectTypes.methodType)
						){
					return true;
				}
				else{
					return false;
				}

			}
		}

		public MethodicalObject(){
			
		}

		public MethodicalObject(ObjectTypes objectType, MethodicalObject member, string objectName){
			this.objectType = objectType;
			this.member = member;
			this.objectName = objectName;
		}

		/*!
		 * This method goes down the length of the chain and adds mo to the last node
		 * */
		public void AppendToChain(MethodicalObject mo){
			MethodicalObject currentObject = this;
			while (currentObject!=null && currentObject.member!=null){
				currentObject = currentObject.member;
			}
			currentObject.member = mo;
		}

		


		public override string ToString ()
		{
			//if (arrayDictionaryIndex != null){
			//	displayIndex = "" + member.arrayDictionaryIndex.objectName;
			//}
			return "name: " + objectName + " Value: " + value + " type: " + objectType.ToString () + " arguments: " + arguments.Count + " member: " + member ;
		}

		#region Foreach Stuff
		public IEnumerator<MethodicalObject> GetEnumerator()
		{
			MethodicalObject currentObject = this;
			while (currentObject!= null){

				yield return currentObject;
				currentObject = currentObject.member;

			}
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion
	}
}
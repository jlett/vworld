//-------------------------------
//          Methodical
// Copyright © 2014 Linkage Games
//-------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Linkage.Methodical{

	//Abandon hope, all ye who enter...

	public class MethodicalExecutor {

		public static bool isStatic = false;
		public static string staticRoot = "";
		public static MonoBehaviour root;
		public static object execute( MethodicalObject chain){
			object newValue = null;
			if (chain!=null && chain.objectType.Equals(MethodicalObject.ObjectTypes.equalityType)) {
				newValue = execute(chain.rightEquality);
				return execute(chain.leftEquality, newValue);
			}
			else { 
				MethodicalObject currentLink = chain;
				object lastReturn = root;

				if (isStatic) {
					if (currentLink.passesByValue){
						lastReturn = currentLink.value;
					}
					else {
						lastReturn = GetFromClass(root, staticRoot, lastReturn, currentLink);
						currentLink = currentLink.member;
					}
				}
				while (currentLink!=null && lastReturn!=null){
					if (currentLink.passesByValue){
						lastReturn = currentLink.value;
					
					}else if (ExistsInClass(lastReturn,currentLink.objectName))
						lastReturn = GetFromClass(root, staticRoot, lastReturn, currentLink);
					else
						return new MethodicalErrorObject(currentLink.objectName,MethodicalErrorObject.ERRORTYPES.DoesNotExist);
					currentLink = currentLink.member;
				}
				return lastReturn;
			}
			
		}

		private static object execute( MethodicalObject chain, object assignValue) {
			MethodicalObject currentLink = chain;
			object lastReturn = root;
			while (currentLink != null && lastReturn != null) {
				if (ExistsInClass(lastReturn, currentLink.objectName)) {
					lastReturn = SetFromClass(root, staticRoot, lastReturn, currentLink, assignValue);
					if (lastReturn == null || !lastReturn.GetType().Equals(typeof(MethodicalErrorObject))) {
						lastReturn = assignValue;// just going to print out the assigned value
					}
				}
				else
					return new MethodicalErrorObject(currentLink.objectName, MethodicalErrorObject.ERRORTYPES.DoesNotExist);
				currentLink = currentLink.member;
			}
			return lastReturn;
		}

		private static void GetAllMembers(){
			//TODO unstub GetAllMembers()
		}

		private static void GetAllMethods(){
			//TODO unstub GetAllMethods(){

		
		}

		private static bool ExistsInClass(object parentObject,string memberName){
			FieldInfo field = null;
			PropertyInfo property = null;
			MethodInfo method = null;
			if (parentObject == null) { //Then static
				System.Type type = GetTypeByName(staticRoot);
				field = type.GetField(memberName);
				property = type.GetProperty(memberName);
				method = type.GetMethod(memberName);
			}
			else {
				field = MatchField(parentObject.GetType(), memberName);
				property = MatchProperty(parentObject.GetType(), memberName);
				method = MatchMethod(parentObject.GetType(), memberName);
			}
			if (field!=null){//Ugly and pointlessly verbose but readable
				return true;
			}else if (property!=null){
				return true;
			}else if(method != null){
				return true;
			}
			return false;
		}

		public static System.Type GetTypeFromClass(object parentObject,string objectName){
			FieldInfo field = null;
			PropertyInfo property = null;
			MethodInfo method = null;
			if (isStatic) { //Then static
				System.Type type = GetTypeByName(staticRoot);
				field = type.GetField(objectName);
				property = type.GetProperty(objectName);
				method = type.GetMethod(objectName);
			}
			else {
				field = MatchField(parentObject.GetType(), objectName);
				property = MatchProperty(parentObject.GetType(), objectName);
				method = MatchMethod(parentObject.GetType(), objectName);
			}
			if (field != null) {//Ugly and pointlessly verbose but readable
				return field.FieldType;
			} else if (property != null) {
				return property.PropertyType;
			} else if (method != null) {
				return method.ReturnType;
			} else {
				return null;
			}
		}

		/*!
		 * Get the member object from the given class in the given context(root) and evaluates 
		 * */
		private static object GetFromClass(MonoBehaviour root, string staticRoot, object parentObject, MethodicalObject member) {
			//Going to try to get each as a field, a property, or method and see what sticks
			FieldInfo field = null;
			PropertyInfo property = null;
			MethodInfo method = null;
			if (isStatic && parentObject == null) { //Then static
				System.Type type = GetTypeByName(staticRoot);
				field = type.GetField(member.objectName);
				property = type.GetProperty(member.objectName);
				method = type.GetMethod(member.objectName);
			}
			else {
				field = MatchField(parentObject.GetType(), member.objectName);
				property = MatchProperty(parentObject.GetType(), member.objectName);
				method = MatchMethod(parentObject.GetType(), member.objectName);
			}
			

			// Doing fields now
			if (field!=null){//Ugly and pointlessly verbose but readable

				// Array Field
				if (member.objectType.Equals(MethodicalObject.ObjectTypes.arrayType)){//If array
					//Getting the array
					object[] array = (object[])field.GetValue(parentObject);
					int index = (int)execute(member.arrayDictionaryIndex);
					//Checking bounds
					if (index < 0 || index >= array.Length){
						return new MethodicalErrorObject(member.objectName,MethodicalErrorObject.ERRORTYPES.OutOfBounds);
					}

					//Getting the array value
					return array.GetValue(index);

					//Hashmap Field
				}else if (member.objectType.Equals(MethodicalObject.ObjectTypes.hashMapType)){
					object index = (object)execute(member.arrayDictionaryIndex);
					if (index.GetType().Equals(typeof(MethodicalErrorObject))){//escaping of index variable doesn't exist
						return index;
					}


					//Checking the index type. Messy, but it works I guess
					if (!field.FieldType.ToString().Contains("[" + index.GetType().ToString())
						&& field.FieldType.ToString().Contains("System.Collections.Generic.Dictionary")) {
						return new MethodicalErrorObject(member.objectName, MethodicalErrorObject.ERRORTYPES.TypeMismatch);
					}
					object value = field.FieldType .GetMethod("get_Item")
						.Invoke(field.GetValue(parentObject), new object[] {index });
					//Debug.Log("return value: " + value);

					return value;

					//Just a normal field
				}else{
					return field.GetValue(parentObject);
				}

				//Getting props
			}else if (property!=null){

				// Getting array members
				if (member.objectType.Equals(MethodicalObject.ObjectTypes.arrayType)){//If array
					//Getting the array
					object[] array = (object[])property.GetValue(parentObject,null);
					int index = (int)execute(member.arrayDictionaryIndex);
					//Checking bounds
					if (index < 0 || index >= array.Length){
						return new MethodicalErrorObject(member.objectName,MethodicalErrorObject.ERRORTYPES.OutOfBounds);
					}

					//Getting the array value
					return array.GetValue(index);

					// Getting hashmap member
				}else if (member.objectType.Equals(MethodicalObject.ObjectTypes.hashMapType)){
					object index = (object)execute(member.arrayDictionaryIndex);
					if (index.GetType().Equals(typeof(MethodicalErrorObject))){//escaping of index variable doesn't exist
						return index;
					}

					//Checking the index type. Messy, but it works I guess
					if (!field.FieldType.ToString().Contains("[" + index.GetType().ToString()) 
						&& field.FieldType.ToString().Contains("System.Collections.Generic.Dictionary")) {
						return new MethodicalErrorObject(member.objectName, MethodicalErrorObject.ERRORTYPES.TypeMismatch);
					}
					object value = property.PropertyType .GetMethod("get_Item")
						.Invoke(property.GetValue(parentObject,null), new object[] {index });
					//Debug.Log("return value: " + value);
					
					return value;

					//Just a regular member
				}else{
					return property.GetValue(parentObject,null);
				}
				
				// It's a method!
			}else if(method != null){
				List<object> parameters = new List<object>();

				//Evaluating each argument and turn them into something usable for reflection
				foreach(MethodicalObject mo in member.arguments){
					object param = execute(mo);
					parameters.Add(param);
				}
				
				if (parameters.Count != method.GetParameters().Length){
					return new MethodicalErrorObject(method.Name,MethodicalErrorObject.ERRORTYPES.BadArguments);
				}else{//Then both parameter counts should be equal, no need to check
					for (int i = 0; i < parameters.Count; i++){
						if(parameters[i] != null && !method.GetParameters()[i].ParameterType.IsAssignableFrom(parameters[i].GetType())){
							return new MethodicalErrorObject(method.Name,MethodicalErrorObject.ERRORTYPES.BadArguments);
						}
					}
				}
				return method.Invoke(parentObject,parameters.ToArray());

				//Well no clue what it is then...
			}else{
				return null;
			}
		}

		private static MethodicalErrorObject SetFromClass(MonoBehaviour root, string staticRoot, object parentObject, MethodicalObject member, object newValue) {
			//Going to try to get each as a field, a property, or method and see what sticks
			FieldInfo field = null;
			PropertyInfo property = null;
			//MethodInfo method = null;
			if (isStatic && parentObject == null) { //Then static
				System.Type type = GetTypeByName(staticRoot);
				field = type.GetField(member.objectName);
				property = type.GetProperty(member.objectName);
				//method = type.GetMethod(member.objectName);
			}
			else {
				field = MatchField(parentObject.GetType(), member.objectName);
				property = MatchProperty(parentObject.GetType(), member.objectName);
				//method = MatchMethod(parentObject.GetType(), member.objectName);
			}


			if (field != null) {//Ugly and pointlessly verbose but readable

				if (member.objectType.Equals(MethodicalObject.ObjectTypes.arrayType)) {//If array
					//Getting the array
					object[] array = (object[])field.GetValue(parentObject);
					int index = (int)execute(member.arrayDictionaryIndex);
					//Checking bounds
					if (index < 0 || index >= array.Length) {
						return new MethodicalErrorObject(member.objectName, MethodicalErrorObject.ERRORTYPES.OutOfBounds);
					}
					//Making sure the types match
					if (!array.GetType().ToString().Equals(newValue.GetType().ToString() + "[]")) {
						return new MethodicalErrorObject(member.objectName, MethodicalErrorObject.ERRORTYPES.TypeMismatch);
					}

					//Getting the array value
					array.SetValue(newValue, index);
				}
				else if (member.objectType.Equals(MethodicalObject.ObjectTypes.hashMapType)) {
					object index = (object)execute(member.arrayDictionaryIndex);
					if (index.GetType().Equals(typeof(MethodicalErrorObject))) {//escaping of index variable doesn't exist
						return (MethodicalErrorObject)index;
					}
					/*object value = field.FieldType.GetMethod("get_Item")
						.Invoke(field.GetValue(parentObject), new object[] { index });*/
					//Debug.Log("return value: " + value);
					Debug.Log("How did I get here?");
					//field.SetValue(index, newValue);//TODO figure out if this works?!?!
				}
				else {
					field.SetValue(root,newValue);//TODO figure out what to do here
				}
			}
			else if (property != null) {
				if (member.objectType.Equals(MethodicalObject.ObjectTypes.arrayType)) {//If array
					//Getting the array
					object[] array = (object[])property.GetValue(parentObject, null);
					int index = (int)execute(member.arrayDictionaryIndex);
					//Checking bounds
					if (index < 0 || index >= array.Length) {
						return new MethodicalErrorObject(member.objectName, MethodicalErrorObject.ERRORTYPES.OutOfBounds);
					}
					//Making sure the types match
					if (!array.GetType().ToString().Equals(newValue.GetType().ToString() + "[]")) {
						return new MethodicalErrorObject(member.objectName, MethodicalErrorObject.ERRORTYPES.TypeMismatch);
					}
					//Getting the array value
					array.SetValue(newValue, index) ;
				}
				else if (member.objectType.Equals(MethodicalObject.ObjectTypes.hashMapType)) {
					object index = (object)execute(member.arrayDictionaryIndex);
					if (index.GetType().Equals(typeof(MethodicalErrorObject))) {//escaping of index variable doesn't exist
						return (MethodicalErrorObject)index;
					}
					property.PropertyType.GetMethod("get_Item")
						.Invoke(property.GetValue(parentObject, null), new object[] { index });
					//Debug.Log("return value: " + value);
					Debug.Log("How did I get here?");
					//return value;
				}
				else {
					//return property.GetValue(parentObject, null);
					property.SetValue(parentObject, newValue, null);
				}
				
			}
			
				return null;
			
		}

		//Thank you LightStriker http://forum.unity3d.com/threads/using-type-gettype-with-unity-objects.136580/
		private static System.Type GetTypeByName(string name) {
			foreach (Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies()) {
				foreach (System.Type type in assembly.GetTypes()) {
					if (type.Name == name)
						return type;
				}
			}

			return null;
		}

		#region Match Methods
		//next two methods adapted from http://stackoverflow.com/questions/994698/ambiguousmatchexception-type-getproperty-c-sharp-reflection
		private static MethodInfo MatchMethod(System.Type type, string name)
		{
			while (type != null)
			{	
				MethodInfo method = type.GetMethod(name);
				if (method != null)
				{
					return method;
				}
				type = type.BaseType;
			}
			return null;
		}
		
		private static FieldInfo MatchField(System.Type type, string name)
		{
			while (type != null)
			{	
				FieldInfo field = type.GetField(name,BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
				if (field != null)
				{
					return field;
				}
				type = type.BaseType;
			}
			return null;
		}

		private static PropertyInfo MatchProperty(System.Type type, string name)
		{
			while (type != null)
			{	
				
				PropertyInfo prop = type.GetProperty(name,BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
				
				if (prop != null)
				{
					return prop;
				}
				type = type.BaseType;
			}
			return null;
		}

		#endregion


	}


}
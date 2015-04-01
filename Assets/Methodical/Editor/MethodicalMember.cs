//-------------------------------
//          Methodical
// Copyright © 2014 Linkage Games
//-------------------------------

using UnityEngine;
using System.Collections;
using System.Reflection;

namespace Linkage.Methodical{
	public class MethodicalMember {
		public enum MemberTypes{method,property,field};
		public MemberTypes memberType;
		public string type = "";
		public FieldInfo fieldInfo;
		public MethodInfo methodInfo;
		public PropertyInfo propertyInfo;


	}
}
